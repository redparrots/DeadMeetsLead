using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Multimedia;
using SlimDX.XAudio2;

namespace JMOD
{
    public enum StopReason
    {
        Ended,
        Stolen,
        Stopped
    }

    public interface ISourceVoiceSlot : IDisposable
    {
        ISourceVoice CurrentSourceVoice { get; }
        void StopSourceVoice(StopReason stopReason);
        void Update();
        void TemporaryShutdown();
        void ShutdownRevival();
    }

    public class DummySourceVoiceSlot : ISourceVoiceSlot
    {
        public DummySourceVoiceSlot(StreamSourceVoice streamSourceVoice)
        {
            CurrentSourceVoice = streamSourceVoice;
        }

        public void Update() { }
        public void Dispose() { }
        public void StopSourceVoice(StopReason stopReason) { }
        public void TemporaryShutdown() { }
        public void ShutdownRevival() { }

        public ISourceVoice CurrentSourceVoice { get; private set; }
    }

    public class StreamSourceVoiceSlot : ISourceVoiceSlot
    {
        public StreamSourceVoiceSlot(Stream stream)
        {
            CurrentStream = stream;
        }

        public void Update() 
        {
#if JMOD_SANITY_CHECK
            if (CurrentChannel != null)
            {
                var st = CurrentChannel.State;
                if (st != Channel.ChannelState.Playing &&
                    st != Channel.ChannelState.WaitingToPlay &&
                    st != Channel.ChannelState.Paused)
                    throw new Exception(st.ToString());
            }
#endif
            if (CurrentSourceVoice != null && CurrentSourceVoice.Done)
                StopSourceVoice(StopReason.Ended);
        }

        public void ShutdownRevival()
        {
            CurrentStream.Load(savedSamplesPlayed);
            savedSamplesPlayed = 0;

            if (CurrentChannel != null)
                CurrentChannel.ReapplySettings();
        }

        public void TemporaryShutdown()
        {
            if (CurrentSourceVoice == null)
                throw new Exception();     // TEMP
            savedSamplesPlayed = (int)CurrentSourceVoice.SamplesPlayed;
            CurrentSourceVoice.Stop();
            CurrentStream.Dispose();
        }
        int savedSamplesPlayed = 0;

        public void Dispose() 
        {
            CurrentStream.Dispose();
            CurrentChannel = null;
        }
        public void StopSourceVoice(StopReason stopReason)
        {
            JMOD.Sound.DebugOutput("{0} - StopSourceVoice called in Stream SourceVoiceSlot", DateTime.Now.TimeOfDay);
            if (CurrentSourceVoice != null)
            {
                Result result;
                if (!CurrentSourceVoice.Done)
                {
                    result = CurrentSourceVoice.Stop();
                    if (result.IsFailure)       // to be removed later
                        throw new Exception(result.Code + " - " + result.Description);
                }
                result = CurrentSourceVoice.FlushSourceBuffers();
                if (result.IsFailure)
                    throw new Exception(result.Code + " - " + result.Description);
            }

            if (stopReason == StopReason.Ended)
                CurrentChannel.HandleFinishedSlot(true);
            else if (stopReason == StopReason.Stopped)
                CurrentChannel.HandleFinishedSlot(false);
            else
                throw new NotImplementedException();

            CurrentChannel = null;
        }

        public ISourceVoice CurrentSourceVoice { get { return CurrentStream.StreamSourceVoice; } }
        public Stream CurrentStream { get; private set; }
        public Channel CurrentChannel { get; set; }
    }

    public class SourceVoiceSlot : ISourceVoiceSlot
    {
        public SourceVoiceSlot(SoundSystem system) { this.system = system; ID = count++; }
        private static int count = 0;
        public int ID { get; private set; }

        public void SetupBuffer(Channel channel, bool looping)
        {
            if (CurrentChannel != null)
                throw new Exception("Slot is already busy!");

            WaveFormat format = channel.Sound.Format;
            ISourceVoice sv;

            if (!sourceVoices.TryGetValue(format, out sv))
                sourceVoices[format] = new NormalSourceVoice(system, format);
            sv = sourceVoices[format];

            JMOD.Sound.DebugOutput("{0} - Claimed SourceVoiceSlot {1}", DateTime.Now.TimeOfDay, ID);

            if (channel.State == Channel.ChannelState.Virtual)
                sv.SetupBuffer(channel.Sound, looping, (int)channel.SamplesPlayed);
            else
                sv.SetupBuffer(channel.Sound, looping);
            
            CurrentChannel = channel;
            CurrentSourceVoice = sv;
        }

        public void Steal()
        {
            // TODO: Check if the StreamEnd event is triggered by calling ::Stop()
            if (CurrentSourceVoice == null)
                throw new Exception("The slot is already free.");
            JMOD.Sound.DebugOutput("Steal()");
            StopSourceVoice(StopReason.Stolen);
        }

        public void StopSourceVoice(StopReason stopReason)
        {
            JMOD.Sound.DebugOutput("{0} - StopSourceVoice called in SourceVoiceSlot {1}", DateTime.Now.TimeOfDay, ID);
            if (CurrentSourceVoice != null && !CurrentSourceVoice.Done)
            {
                var result = CurrentSourceVoice.Stop();
                if (result.IsFailure)       // to be removed later
                    throw new Exception(result.Code + " - " + result.Description);
            }

            if (stopReason == StopReason.Ended)
                CurrentChannel.HandleFinishedSlot(true);
            else if (stopReason == StopReason.Stopped)
                CurrentChannel.HandleFinishedSlot(false);
            else if (stopReason == StopReason.Stolen)
                CurrentChannel.HandleStolenSlot();
            
            if (CurrentSourceVoice != null)
            {
                JMOD.Sound.DebugOutput("Flushing buffers");
                var result = CurrentSourceVoice.FlushSourceBuffers();
                if (result.IsFailure)       // to be removed later
                    throw new Exception(result.Code + " - " + result.Description);
            }
            system.ReleaseSourceVoiceSlot(this);
            CurrentChannel = null;
            CurrentSourceVoice = null;
        }

        public float Score
        {
            get
            {
                if (CurrentChannel == null)
                    return float.MaxValue;
                return CurrentChannel.Score;
            }
        }

        public void Update()
        {
#if JMOD_SANITY_CHECK
            
            if (CurrentChannel != null)
            {
                var st = CurrentChannel.State;
                if (st != Channel.ChannelState.Playing &&
                    st != Channel.ChannelState.WaitingToPlay &&
                    st != Channel.ChannelState.Paused)
                throw new Exception(st.ToString());
            }
#endif
            if (CurrentSourceVoice != null && CurrentSourceVoice.Done)
            {
                //JMOD.Sound.DebugOutput("Update()");
                StopSourceVoice(StopReason.Ended);
            }
        }

        public void ShutdownRevival()
        {
            if (savedSourceVoiceState == null)      // nothing to resume here
                return;

            if (savedSourceVoiceState.Done)
            {
                CurrentSourceVoice = null;
                StopSourceVoice(StopReason.Ended);      // WARNING: This may not work as intended (glanced over, but not tested)
                return;
            }

            var format = CurrentChannel.Sound.Format;
            NormalSourceVoice sourceVoice = new NormalSourceVoice(system, format);
            CurrentSourceVoice = sourceVoices[format] = sourceVoice;
            sourceVoice.SetupBuffer(CurrentChannel.Sound, CurrentChannel.Looping, (int)savedSourceVoiceState.SamplesPlayed);
            
            CurrentChannel.ReapplySettings();

            savedSourceVoiceState = null;
        }

        public void TemporaryShutdown()
        {
            if (CurrentSourceVoice != null)
            {
                CurrentSourceVoice.Stop();
                savedSourceVoiceState = new SourceVoiceState 
                { 
                    SamplesPlayed = CurrentSourceVoice.SamplesPlayed, 
                    Done = CurrentSourceVoice.Done,
                };
            }

            foreach (var sv in sourceVoices.Values)
                sv.Dispose();
            sourceVoices.Clear();
        }

        public void Dispose()
        {
            foreach (var svs in sourceVoices.Values)
                svs.Dispose();
        }

        public class SlotComparer : IComparer<SourceVoiceSlot>
        {
            public int Compare(SourceVoiceSlot a, SourceVoiceSlot b)
            {
                return a.Score.CompareTo(b.Score);
            }
        }

        private class SourceVoiceState
        {
            public long SamplesPlayed { get; set; }
            public bool Done { get; set; }
        }

        // changing variables
        public ISourceVoice CurrentSourceVoice { get; private set; }
        public Channel CurrentChannel { get; private set; }

        private SourceVoiceState savedSourceVoiceState;

        private Dictionary<WaveFormat, ISourceVoice> sourceVoices = new Dictionary<WaveFormat, ISourceVoice>();
        private SoundSystem system;
    }
}

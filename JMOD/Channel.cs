#define DEBUG_PAN_COEFF
//#define DEBUG_PAN_COEFF_OUTPUT
//#define JMOD_DEBUG_GATHER_DATA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.X3DAudio;
using SlimDX.XAudio2;

namespace JMOD
{
    public class Channel
    {
        public Channel(SoundSystem system, ISound sound, bool looping)
        {
            ID = count++;
            this.Sound = sound;
            this.system = system;

            JMOD.Sound.DebugOutput("{0} - Created Channel {1}", DateTime.Now.TimeOfDay, ID);
            DebugHistory("Created Channel (" + ID + ")");

            CarriesStream = sound is Stream;

            SourceVoiceSlot = system.AcquireSourceVoiceSlot(this, looping);

            Volume = Sound.DefaultVolume;
            Frequency = Sound.Frequency;
            // Muted
            // etc.
            Looping = looping;
            State = ChannelState.Paused;
            system.AddChannel(this);
        }
        private static int count = 0;
        public int ID { get; private set; }

        public void Set3DAttributes(Vector3 position, Vector3 velocity)
        {
            _3DPosition = position;
            _3DVelocity = velocity;
        }

        [Common.CodeState(State=Common.CodeState.Untested, Details="Not tested.")]
        public void Stop()
        {
            DebugHistory("Stop() called");
            if (!IsVirtual)
            {
                //SourceVoiceSlot.CurrentSourceVoice.Stop();
                SourceVoiceSlot.StopSourceVoice(StopReason.Stopped);
            }
            else
                HandleFinishedSlot(false);
        }

        public void StopLooping()
        {
            if (!IsVirtual)
                SourceVoiceSlot.CurrentSourceVoice.ExitLoop();
            Looping = false;
        }

        public void ReapplySettings()
        {
            bool wasPaused = Paused;
            Paused = true;

            //UpdateOutputMatrix(null);     // TODO: Get listener!

            UpdateProperties();
            if (Sound is Stream)
                ((Stream)Sound).StreamSourceVoice.Looping = Looping;

            Paused = wasPaused;
        }

        public bool Paused 
        { 
            get { return paused; }
            set
            {
                DebugHistory("Paused set to " + value);
                if (!IsVirtual && paused != value)
                {
                    if (value)
                    {
                        SourceVoiceSlot.CurrentSourceVoice.Stop();
                        State = ChannelState.Paused;
                        samplePausedAt = SourceVoiceSlot.CurrentSourceVoice.SamplesPlayed;
                    }
                    else
                    {
                        if (!CarriesStream)
                            UpdateOutputMatrix(system.Listener);
                        State = ChannelState.WaitingToPlay;
                    }
                } 
                paused = value;
            }
        }

        private string VectorString(Vector3 v)
        {
            return String.Format("({0:0.000}, {1:0.000}, {2:0.000})", v.X, v.Y, v.Z);
        }

#if DEBUG_PAN_COEFF_OUTPUT
        float panCoeffAcc = 0;
#endif
        public void Update(float dtime, Listener listener)
        {
            ChannelState test1 = ChannelState.Paused, test2 = ChannelState.Paused;
            UpdateScore(listener.Position);
            if (SourceVoiceSlot != null)
            {
#if JMOD_SANITY_CHECK
                if (SourceVoiceSlot is SourceVoiceSlot && ((SourceVoiceSlot)SourceVoiceSlot).CurrentChannel != this)
                    throw new Exception();
                if (SourceVoiceSlot is StreamSourceVoiceSlot && ((StreamSourceVoiceSlot)SourceVoiceSlot).CurrentChannel != this)
                    throw new Exception();
#endif
                test1 = State;
                if (State == ChannelState.Stopped)
                    //JMOD.Sound.DebugOutput("WARNING! State is Stopped, but SourceVoiceSlot not null");
                    throw new Exception("WARNING! State is Stopped, but SourceVoiceSlot not null [" + system.NUpdates + "]");
                SourceVoiceSlot.Update();
                test2 = State;
            }
#if JMOD_SANITY_CHECK
            else if (State != ChannelState.Virtual)
            {
                throw new Exception();
            }
#endif
            if (State == ChannelState.Virtual)
            {
                elapsedSamples += dtime * sampleRate;
                if (elapsedSamples >= totalSamples)
                {
                    JMOD.Sound.DebugOutput("Virtual channel reached end");
                    if (Looping)
                    {
                        JMOD.Sound.DebugOutput("Restarting...");
                        elapsedSamples -= totalSamples;
                    }
                    else
                    {
                        DebugHistory("Virtual stop detected in update");
                        JMOD.Sound.DebugOutput("Stopping...");
                        HandleFinishedSlot(true);
                        system.ReleaseVirtualChannel(this);
                    }
                }
                return;
            }

            if (State == ChannelState.WaitingToPlay)
            {
                DebugHistory("WaitingToPlay dealt with in Update()");
                if (!CarriesStream)
                    UpdateOutputMatrix(system.Listener);
                SourceVoiceSlot.CurrentSourceVoice.Start();
                State = ChannelState.Playing;
            }
            else if (Sound.Is3DSound && !IsVirtual)
            {
                UpdateOutputMatrix(listener);
            }
        }

        private void UpdateOutputMatrix(Listener listener)
        {
            if (Sound.Is3DSound)
            {
                int inputChannels = Sound.Format.Channels;
                int outputChannels = system.DeviceDetails.OutputFormat.Channels;

                Vector2 minMaxDistance = system._3DMinMaxDistance;
                float range = minMaxDistance.X + minMaxDistance.Y;
                float breakingPoint = minMaxDistance.X / range;

                Vector3 v = listener.Position - _3DPosition;
                Emitter e = new Emitter
                {
                    ChannelCount = 1,
                    VolumeCurve = new CurvePoint[] 
                    {
                        new CurvePoint { Distance = 0f, DspSetting = 1f },
                        new CurvePoint { Distance = breakingPoint, DspSetting = 1f },
                        new CurvePoint { Distance = 1f, DspSetting = 0f },
                    },
                    CurveDistance = range,
                    Position = _3DPosition,
                    Velocity = _3DVelocity,
                    OrientFront = v.Length() > 0 ? Vector3.Normalize(v) : Vector3.UnitX,
                    OrientTop = Vector3.UnitZ       // TODO: should be orthogonal with OrientFront
                };



                DspSettings dsp = system.X3DInstance.Calculate(listener, e, CalculateFlags.Matrix | CalculateFlags.LpfDirect,
                    inputChannels, outputChannels);       // TODO: look up flags

                PanScale(dsp.MatrixCoefficients, _3DPanLevel, inputChannels, outputChannels);

#if DEBUG_PAN_COEFF_OUTPUT
                panCoeffAcc += dtime;
                if (panCoeffAcc > 0.3f)
                {
                    string s = "";
                    float[] mcs = dsp.MatrixCoefficients;
                    foreach (var mc in mcs)
                        s += mc + ", ";
                    JMOD.Sound.DebugOutput(s);
                    panCoeffAcc = 0;
                }
#endif
                SourceVoiceSlot.CurrentSourceVoice.SetOutputMatrix(inputChannels, outputChannels, dsp.MatrixCoefficients);
            }
            else
            {
                SourceVoiceSlot.CurrentSourceVoice.ResetOutputMatrix();
            }
            
        }

        private void PanScale(float[] coeffs, float panScale, int soundChannels, int outputChannels)
        {
            // items are stored as [o0c0, o0c1, o0c2, o0c3, o0c4, o0c5, o1c0, o1c1, ...] with o: output channel and c: sound channel (6 input channels in this example)
#if DEBUG_PAN_COEFF
            if (coeffs.Length / soundChannels != outputChannels)
                throw new Exception("Unexpected length of coefficient array.");
#endif
            float[] outputSums = new float[soundChannels];
            for (int i = 0; i < outputChannels; i++)
            {
                for (int j = 0; j < soundChannels; j++)
                {
                    outputSums[j] += coeffs[i * soundChannels + j];
                }
            }
            int channelsNotZero = 0;
            foreach (float sum in outputSums)
                if (sum > 0)
                    channelsNotZero++;

            if (channelsNotZero == 0)
                return;     // sound is silent
#if DEBUG_PAN_COEFF
            else if (channelsNotZero != 1)
                throw new Exception(String.Format("Sound with {0} active channels found.", channelsNotZero));
#endif
            for (int j = 0; j < soundChannels; j++)
            {
                if (outputSums[j] == 0)
                    continue;
                float average = outputSums[j] / outputChannels;
                for (int i = 0; i < outputChannels; i++)
                {
                    int index = i * soundChannels + j;
                    coeffs[index] = (coeffs[index] * panScale + average * (1 - panScale)); /* Divide by channelsNotZero if we only have sounds with one active channel */ ;
                }
            }
        }

        private void PrintProgress()
        {
            long elapsedSamples = SourceVoiceSlot.CurrentSourceVoice.SamplesPlayed;
            long sampleRate = SourceVoiceSlot.CurrentSourceVoice.InputSampleRate;
            //long totalSamples = Sound.ByteLength / (Sound.Format.Channels * Sound.Format.BitsPerSample / 8);
            long totalSamples = JMOD.Sound.LengthInSamples(Sound.Format, Sound.ByteLength);
            JMOD.Sound.DebugOutput("Channel {0} progress: {1}/{2} ~= {3:0.00}%", ID, elapsedSamples, totalSamples, 100f * (elapsedSamples / (float)totalSamples));
        }

        //public void HandleFinishedSlot() { HandleFinishedSlot(true); }
        public void HandleFinishedSlot(bool naturalStop)
        {
            DebugHistory("HandleFinishedSlot() called with naturalStop = " + naturalStop);
            // debug
            if (SourceVoiceSlot != null && SourceVoiceSlot.CurrentSourceVoice != null)
            {
                PrintProgress();
                JMOD.Sound.DebugOutput("Channel {0} stopped.", ID);
            }
            else
                JMOD.Sound.DebugOutput("Channel {0} stopped. No Source Voice.", ID);

            if (naturalStop)
                sampleStoppedAt = JMOD.Sound.LengthInSamples(Sound.Format, Sound.ByteLength);
            else if (State == ChannelState.Virtual)
            {
                sampleStoppedAt = (long)elapsedSamples;
                system.ReleaseVirtualChannel(this);
            }
            else
                sampleStoppedAt = SourceVoiceSlot.CurrentSourceVoice.SamplesPlayed;

            State = ChannelState.Stopped;
            SourceVoiceSlot = null;
            system.RemoveChannel(this);

#if JMOD_SANITY_CHECK
            if (system.DebugHasChannel(this))
                throw new Exception();
#endif

            if (naturalStop && PlaybackStopped != null)
                PlaybackStopped(this, null);
        }

#if JMOD_SANITY_CHECK
        public void DebugNotifySystemAddition(bool isVirtual)
        {
            DebugHistory("Added to system's channels list" + (isVirtual ? " (virtual)" : ""));
        }

        public void DebugNotifySystemRemoval(bool isVirtual)
        {
            DebugHistory("Removed from system's channels list" + (isVirtual ? " (virtual)" : ""));
        }
#endif

        public void HandleStolenSlot()
        {
            DebugHistory("HandleStolenSlot called");
            // init variables for calculating position in stream
            elapsedSamples = SourceVoiceSlot.CurrentSourceVoice.SamplesPlayed;
            sampleRate = SourceVoiceSlot.CurrentSourceVoice.InputSampleRate;
            //totalSamples = Sound.ByteLength / (Sound.Format.Channels * Sound.Format.BitsPerSample / 8);
            totalSamples = JMOD.Sound.LengthInSamples(Sound.Format, Sound.ByteLength);

            PrintProgress();
            JMOD.Sound.DebugOutput("Stolen slot: " + ((SourceVoiceSlot)SourceVoiceSlot).ID);
            system.AddVirtualChannel(this);

            SourceVoiceSlot = null;
            State = ChannelState.Virtual;
            if (GoesVirtual != null)
                GoesVirtual(this, null);
        }

        public void HandleNewSlot(SourceVoiceSlot slot)
        {
#if JMOD_SANITY_CHECK
            if (CarriesStream)
                throw new Exception();
#endif
            DebugHistory("HandleNewSlot called (" + slot.ID + ")");
            slot.SetupBuffer(this, Looping);

            SourceVoiceSlot = slot;
            if (!Paused)
            {
                slot.CurrentSourceVoice.Start();
                State = ChannelState.Playing;
            }
            else
                State = ChannelState.Paused;
            
            if (LeavesVirtual != null)
                LeavesVirtual(this, null);
        }

        float sampleRate;

        public bool IsVirtual { get { return State == ChannelState.Stopped || State == ChannelState.Virtual || SourceVoiceSlot == null; } }   // state.virtual??

        public void SetTemporaryVolume(float volume)
        {   // bypasses storing (used by e.g. external muting)
            if (!IsVirtual)
                temporaryVolume = SourceVoiceSlot.CurrentSourceVoice.Volume = volume;
        }
        float temporaryVolume = float.NaN;

        public event EventHandler GoesVirtual;
        public event EventHandler LeavesVirtual;
        public event EventHandler PlaybackStopped;

        public float Volume
        {
            get { return channelVolume; }
            set
            {
                channelVolume = value;
                if (!IsVirtual)
                {
                    SourceVoiceSlot.CurrentSourceVoice.Volume =
                        channelVolume *
                        sound.DefaultVolume *
                        (sound.Is3DSound ? system.Volume3DScale : system.Volume2DScale) *       // scaling to get a good relation between 2d and 3d sound levels
                        (sound.SoundGroup != null ? sound.SoundGroup.Volume : 1f) *
                        system.MasterVolume;
                }
                temporaryVolume = float.NaN;
            }
        }

        public float Frequency 
        {
            get 
            { 
                if (!IsVirtual)
                    return SourceVoiceSlot.CurrentSourceVoice.FrequencyRatio * baseFrequency; 
                return frequencyRatio * baseFrequency; 
            }
            set 
            {
                frequencyRatio = value / baseFrequency;
                if (!IsVirtual)
                    SourceVoiceSlot.CurrentSourceVoice.FrequencyRatio = frequencyRatio;
            }
        }
        public ISound Sound
        {
            get { return sound; }
            private set 
            {
                sound = value;
                if (sound != null)
                {
                    baseFrequency = sound.Format.SamplesPerSecond;
                    Volume = sound.DefaultVolume;
                }
            }
        }

        public void UpdateProperties()
        {
            if (!IsVirtual)
            {
                SourceVoiceSlot.CurrentSourceVoice.FrequencyRatio = frequencyRatio;
                if (float.IsNaN(temporaryVolume))
                    Volume = channelVolume;
                else
                    SourceVoiceSlot.CurrentSourceVoice.Volume = temporaryVolume;
            }
        }

        
        private void UpdateScore(Vector3 listenerPosition)
        {
            if (Sound.Is3DSound && (_3DPosition - listenerPosition).Length() > system._3DMinMaxDistance.Y)
            {
                Score = float.MaxValue;          // redo
                return;
            }
            if (Volume == 0)
            {
                Score = float.MaxValue;          // redo
                return;
            }
            Score = Sound.Priority;
        }

        /// <summary>
        /// Determines how likely the sound is to get swapped out. Low is better.
        /// </summary>
        public float Score { get; private set; }

        public long SamplesPlayed
        {
            get
            {
                switch (State)
                { 
                    case ChannelState.Virtual:
                        return (long)elapsedSamples;
                    case ChannelState.Playing:
                        return SourceVoiceSlot.CurrentSourceVoice.SamplesPlayed;
                    case ChannelState.Stopped:
                        return sampleStoppedAt;
                    case ChannelState.Paused:
                        return samplePausedAt;
                    case ChannelState.WaitingToPlay:
                        return samplePausedAt;
                    default:
                        throw new NotImplementedException();
                }
            } 
        }

        /// <summary>
        /// Position in milliseconds played.
        /// </summary>
        public uint Position
        {
            get { return (uint)((SamplesPlayed * 1000) / Frequency); }
        }

        public class ImportanceComparer : IComparer<Channel>
        {
            public int Compare(Channel a, Channel b)
            {
                return a.Score.CompareTo(b.Score);
            }
        }

        private void DebugHistory(String s)
        {
#if JMOD_DEBUG_GATHER_DATA
            if (firstHistoryEntry == DateTime.MinValue)
                firstHistoryEntry = DateTime.Now;
            history.AppendFormat("{0:ss:ffff} {1} [{2}] ({3}){4}", DateTime.Now - firstHistoryEntry, s, system.NUpdates, System.Threading.Thread.CurrentThread.ManagedThreadId, Environment.NewLine);
#endif
        }
#if JMOD_DEBUG_GATHER_DATA
        StringBuilder history = new StringBuilder();
        DateTime firstHistoryEntry = DateTime.MinValue;
#endif

        public override string ToString()
        {
            return "Channel " + ID;
        }

        public enum ChannelState
        {
            Playing,
            WaitingToPlay,
            Virtual,
            Paused,
            Stopped
        }

        public ChannelState State { get; private set; }
        private bool paused = true;
        public ISourceVoiceSlot SourceVoiceSlot
        {
            get
            {
                return sourceVoiceSlot;
            }
            private set
            {
                if (value == null)
                {
                    if (CarriesStream)
                    {
                        if (value is SourceVoiceSlot && ((SourceVoiceSlot)value).CurrentChannel != this)
                            throw new Exception();
                        if (value is StreamSourceVoiceSlot && ((StreamSourceVoiceSlot)value).CurrentChannel != this)
                            throw new Exception();
                    }
                    DebugHistory("SourceVoiceSlot set to null (previously " + (sourceVoiceSlot == null ? "null" : "not null") + ")");
                    DebugHistory("\t" + Environment.StackTrace);
                }
                sourceVoiceSlot = value;
            }
        }
        private ISourceVoiceSlot sourceVoiceSlot = null;

        public bool CarriesStream { get; private set; }
        
        public float _3DPanLevel { get; set; }
        public Vector3 _3DPosition { get; private set; }
        public Vector3 _3DVelocity { get; private set; }
        public bool Looping { get; private set; }

        private float baseFrequency;
        private float frequencyRatio = 1f;
        private float channelVolume = 1f;

        private double elapsedSamples, totalSamples;
        private long sampleStoppedAt = -1, samplePausedAt = 0;

        private ISound sound;
        private SoundSystem system;
    }
}

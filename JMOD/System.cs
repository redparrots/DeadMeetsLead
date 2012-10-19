//#define DEBUG_JMOD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.X3DAudio;
using SlimDX.XAudio2;
using SlimDX.Multimedia;
using Common;

namespace JMOD
{
    public class SoundSystem
    {
        public SoundSystem(Vector2 minMaxDistance, int maxAudibleVoices, CustomReadFileMethodDelegate customReadFileMethod)
        {
            _3DMinMaxDistance = minMaxDistance;
            CustomReadFileMethod = customReadFileMethod;

#if DEBUG_JMOD
            AudioDevice = new XAudio2(XAudio2Flags.DebugEngine, ProcessorSpecifier.AnyProcessor);
#else
            AudioDevice = new XAudio2(XAudio2Flags.None, ProcessorSpecifier.AnyProcessor);
#endif

            freeVoices = new Stack<SourceVoiceSlot>(maxAudibleVoices);
            usedVoices = new InsertionSortList<SourceVoiceSlot>(new SourceVoiceSlot.SlotComparer());
            for (int i = 0; i < maxAudibleVoices; i++)
                freeVoices.Push(new SourceVoiceSlot(this));
            virtualChannels = new InsertionSortList<Channel>(new Channel.ImportanceComparer());
        }

        public bool TrySetAnyAudioDevice(out string errorString)
        {
            errorString = "";
            for (int i = 0; i < AudioDevice.DeviceCount; i++)
            {
                try
                {
                    SetAudioDevice(i);
                    // managed to set audio device, return happily
                    return true;
                }
                catch (Exception ex)
                {
                    errorString += ex.Message + Environment.NewLine;
                }
            }
            return false;
        }

        public void SetAudioDevice(string deviceID)
        {
            int newDeviceIndex = -1;

            DeviceDetails[] audioDevices = AudioDevices;
            for (int i = 0; i < audioDevices.Length; i++)
            {
                if (audioDevices[i].DeviceId == deviceID)
                {
                    newDeviceIndex = i;
                    break;
                }
            }

            if (newDeviceIndex == -1)
                throw new Exception(String.Format("Device not found (among {0} device{1}): {2}", audioDevices.Length, audioDevices.Length == 1 ? "" : "s", deviceID));
            else
                SetAudioDevice(newDeviceIndex);
        }

        private void SetAudioDevice(int deviceIndex)
        {
            if (streamSlots != null)
                foreach (var s in streamSlots.Values)
                    s.TemporaryShutdown();
            if (freeVoices != null)
                foreach (var v in freeVoices)
                    v.TemporaryShutdown();
            if (usedVoices != null)
                foreach (var v in usedVoices)
                    v.TemporaryShutdown();

            if (masteringVoice != null)
            {
                masteringVoice.Dispose();
                X3DInstance.Dispose();
                masteringVoice = null;
                X3DInstance = null;
            }

            masteringVoice = new MasteringVoice(AudioDevice, XAudio2.DefaultChannels, XAudio2.DefaultSampleRate, deviceIndex);
            DeviceDetails = AudioDevice.GetDeviceDetails(deviceIndex);
            X3DInstance = new X3DAudio(DeviceDetails.OutputFormat.ChannelMask, 340f);

            if (streamSlots != null)
                foreach (var s in streamSlots.Values)
                    s.ShutdownRevival();
            if (freeVoices != null)
                foreach (var v in freeVoices)
                    v.ShutdownRevival();
            if (usedVoices != null)
                foreach (var v in usedVoices)
                    v.ShutdownRevival();
        }

        public DeviceDetails[] AudioDevices { get { return GetAudioDevices(AudioDevice); } }

        public static DeviceDetails[] GetAudioDevices()
        {
            XAudio2 system = new XAudio2();
            var dd = GetAudioDevices(system);
            system.Dispose();
            return dd;
        }

        private static DeviceDetails[] GetAudioDevices(XAudio2 system)
        {
            DeviceDetails[] dd = new DeviceDetails[system.DeviceCount];
            for (int i = 0; i < dd.Length; i++)
                dd[i] = system.GetDeviceDetails(i);
            return dd;
        }

        #region Device debugging

        public string DeviceInformationString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                int deviceCount = AudioDevice.DeviceCount;

                if (deviceCount == 0)
                    sb.Append("No audio device was found.");
                else
                    sb.AppendFormat("Found device(s): {0}{1}", deviceCount, Environment.NewLine);
                for (int i = 0; i < deviceCount; i++)
                    sb.Append(FormatDeviceDetailsOutput(i, AudioDevice.GetDeviceDetails(i)));
                return sb.ToString();
            }
        }

        private static string FormatDeviceDetailsOutput(int deviceIndex, DeviceDetails details)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("------------------------------------------------------------");
            sb.AppendLine(String.Format("#{0} - {1}", deviceIndex, details.DisplayName));
            sb.AppendLine(String.Format("Role: {0}", details.Role.ToString()));
            sb.AppendLine(String.Format("ID: {0}", details.DeviceId));
            sb.AppendLine("Output format: ");

            WaveFormatExtensible format = details.OutputFormat;
            string pad = "\t";
            sb.AppendLine(String.Format("{0}ChanMask: {1}\tChannels: {2}", pad, format.ChannelMask, format.Channels));
            sb.AppendLine(String.Format("{0}BlockAlign: {1}\t\tSamplesPerBlock: {2}", pad, format.BlockAlignment, format.SamplesPerBlock));
            sb.AppendLine(String.Format("{0}BitsPerSample: {1}\tSamplesPerSecond: {2}", pad, format.BitsPerSample, format.SamplesPerSecond));
            sb.AppendLine(String.Format("{0}ValidBitsPerSample: {1}\tAvgBytesPerSecond: {2}", pad, format.ValidBitsPerSample, format.AverageBytesPerSecond));
            sb.AppendLine(String.Format("{0}Tag: {1}", pad, format.FormatTag));

            return sb.ToString();
        }

        #endregion

        public ISound CreateSound(string fileName, MODE flags)
        {
            System.IO.Stream s;
            if (CustomReadFileMethod != null)
                s = CustomReadFileMethod(fileName);
            else
                s = System.IO.File.OpenRead(fileName);

            WaveStream stream = new WaveStream(s);
            s.Close();

            AudioBuffer buffer = new AudioBuffer();
            buffer.AudioData = stream;
            buffer.AudioBytes = (int)stream.Length;
            //  indicates that there cannot be any buffers behind this buffer in the queue
            buffer.Flags = BufferFlags.EndOfStream;

            return new Sound(this, (flags & MODE._3D) != 0)
            { 
                ByteLength = stream.Length,
                Buffer = buffer,
                Stream = stream,
                DefaultVolume = 1.0f,
                Frequency = stream.Format.SamplesPerSecond,
                Priority = 255,
                SoundGroupName = ""
            };
        }

        public ISound CreateStream(string fileName, MODE flags)
        {
            // TODO: Close the stream!
            System.IO.Stream s;
            if (CustomReadFileMethod != null)
                s = CustomReadFileMethod(fileName);
            else
                s = System.IO.File.OpenRead(fileName);
            WaveStream stream = new WaveStream(s);

            var streamInstance = new Stream(this, (flags & MODE._3D) != 0)
            {
                WaveStream = stream,
                DefaultVolume = 1.0f,
                Priority = 255,
                SoundGroupName = ""
            };
            streamInstance.Load();
            streamSlots[streamInstance] = new StreamSourceVoiceSlot(streamInstance);
            return streamInstance;
        }
        Dictionary<Stream, StreamSourceVoiceSlot> streamSlots = new Dictionary<Stream,StreamSourceVoiceSlot>();

        public void AddChannel(Channel channel)
        {
#if JMOD_SANITY_CHECK
            if (channels.Contains(channel))
                throw new Exception();
            channel.DebugNotifySystemAddition(false);
#endif
            channels.Add(channel);
        }

        public void RemoveChannel(Channel channel)
        {
#if JMOD_SANITY_CHECK
            if (!DebugHasChannel(channel))
                throw new Exception();
#endif
            channels.Remove(channel);
#if JMOD_SANITY_CHECK
            if (DebugHasChannel(channel))
                throw new Exception();
            channel.DebugNotifySystemRemoval(false);
#endif
        }

#if JMOD_SANITY_CHECK
        public bool DebugHasChannel(Channel channel)
        {
            if (channels.Contains(channel))
                return true;

            int id = channel.ID;
            foreach (var c in channels)
                if (c.ID == id)
                    return true;
            return false;
        }
#endif

        public void AddVirtualChannel(Channel channel)
        {
            virtualChannels.InsertSort(channel);
#if JMOD_SANITY_CHECK
            channel.DebugNotifySystemAddition(true);
#endif
        }

        public void ReleaseVirtualChannel(Channel channel)
        {
#if JMOD_SANITY_CHECK
            if (!virtualChannels.Contains(channel))
                throw new Exception();
#endif
            virtualChannels.Remove(channel);
#if JMOD_SANITY_CHECK
            channel.DebugNotifySystemRemoval(true);
#endif
        }

        public void Set3DListenerAttributes(Vector3 position, Vector3 velocity, Vector3 forward, Vector3 up)
        {
            listener.Position = position;
            listener.Velocity = velocity;
            listener.OrientFront = forward;
            listener.OrientTop = up;
        }

        public void Update(float dtime)
        {
            NUpdates++;
            foreach (var channel in new List<Channel>(channels))
                channel.Update(dtime, listener);
            int swaps = System.Math.Min(freeVoices.Count, virtualChannels.Count);
            while (swaps-- > 0)
            {
                var channel = virtualChannels[0];
                ReleaseVirtualChannel(channel);
                var slot = freeVoices.Pop();
                channel.HandleNewSlot(slot);
                usedVoices.InsertSort(slot);
            }
        }
        public int NUpdates { get; private set; }

        public ISourceVoiceSlot AcquireSourceVoiceSlot(Channel channel, bool looping)
        {
            if (channel.CarriesStream)
            {
                var s = streamSlots[(Stream)channel.Sound];
#if JMOD_SANITY_CHECK
                if (s.CurrentChannel != null)
                    throw new Exception();
#endif
                s.CurrentChannel = channel;
                return s;
            }

            SourceVoiceSlot svs;

            bool debugWasFree = true;
            if (freeVoices.Count == 0)
            {
                usedVoices[0].Steal();
                debugWasFree = false;
            }
            svs = freeVoices.Pop();
            svs.SetupBuffer(channel, looping);
            usedVoices.InsertSort(svs);
            // debug
            if (!debugWasFree)
            {
                long samplesPlayed = svs.CurrentSourceVoice.SamplesPlayed;
                Sound.DebugOutput("Samples played after swap: " + samplesPlayed);
            }
            return svs;
        }

        public void ReleaseSourceVoiceSlot(SourceVoiceSlot svs)
        {
#if JMOD_SANITY_CHECK
            bool usedContains = usedVoices.Contains(svs);
            bool freeContains = freeVoices.Contains(svs);
            if (!usedContains || freeContains)
                throw new Exception("Balance not maintained");
#endif

            usedVoices.Remove(svs);
            freeVoices.Push(svs);
        }

        public SoundGroup CreateSoundGroup(string name)
        {
            var sg = new SoundGroup(this, name);
            if (soundGroups.ContainsKey(name))
                throw new Exception("Sound group already exists.");
            soundGroups[name] = sg;
            return sg;
        }

        public SoundGroup GetSoundGroup(string name)
        {
            return soundGroups[name];
        }

        public void ShutDown()
        {
            foreach (var s in streamSlots.Keys)
                s.Dispose();
            streamSlots.Clear();
            foreach (var svs in freeVoices)
                svs.Dispose();
            freeVoices.Clear();
            foreach (var svs in usedVoices)
                svs.Dispose();
            usedVoices.Clear();
            masteringVoice.Dispose();
            AudioDevice.Dispose();
        }

        public bool Muted
        {
            get { return muted; }
            set 
            { 
                if (value != muted) 
                { 
                    muted = value;
                    if (muted)
                        foreach (var channel in channels) channel.SetTemporaryVolume(0);
                    else
                        foreach (var channel in channels) channel.Volume = channel.Volume;
                }
            }
        }
        private bool muted = false;

        public float MasterVolume
        {
            get { return masterVolume; }
            set 
            {
                masterVolume = System.Math.Min(1f, System.Math.Max(0, value));
                UpdateChannelVolumes();
            }
        }
        private float masterVolume = 1.0f;

        private void UpdateChannelVolumes()
        {
            foreach (var svs in usedVoices)
                svs.CurrentChannel.Volume = svs.CurrentChannel.Volume;        // update volume for all playing channels
            foreach (var stream in streamSlots.Values)
                if (stream.CurrentChannel != null) stream.CurrentChannel.Volume = stream.CurrentChannel.Volume;
        }

        public float Volume2DScale
        {
            get { return volume2DScale; }
            set 
            { 
                volume2DScale = value;
                UpdateChannelVolumes();
            }
        }
        public float Volume3DScale 
        {
            get { return volume3DScale; }
            set 
            {
                volume3DScale = value;
                UpdateChannelVolumes();
            }
        }
        private float volume2DScale = 1f, volume3DScale = 1f;

        public Vector2 _3DMinMaxDistance { get; private set; }

        private Dictionary<String, SoundGroup> soundGroups = new Dictionary<string, SoundGroup>();

        public XAudio2 AudioDevice { get; private set; }
        public DeviceDetails DeviceDetails { get; private set; }
        public X3DAudio X3DInstance { get; private set; }

        List<Channel> channels = new List<Channel>();

        public Listener Listener { get { return listener; } }
        Listener listener = new Listener();
        MasteringVoice masteringVoice;

        private CustomReadFileMethodDelegate CustomReadFileMethod;
        Stack<SourceVoiceSlot> freeVoices;
        InsertionSortList<SourceVoiceSlot> usedVoices;
        InsertionSortList<Channel> virtualChannels;
    }

    public delegate System.IO.Stream CustomReadFileMethodDelegate(string fileName);

    public interface ISoundSystem
    {
        void Init();
        JMODRESULT CreateSound(string fileName, MODE flags, out ISound sound);       // flags, createSoundExInfo     [CreateStream]
        void Update(float dtime, Vector3 position, Vector3 velocity, Vector3 forward, Vector3 up);
    }

    public enum JMODRESULT
    {
        OK
    };

    public enum MODE
    {
        _2D = 0x00,
        _3D = 0x01,
    }
}

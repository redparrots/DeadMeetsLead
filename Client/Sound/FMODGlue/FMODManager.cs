using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound.FMODGlue
{
    public class FMODManager : IManagerGlue
    {
        public FMODManager() { Instance = this; }

        public void Init(AudioDevice audioDevice, Vector2 minMaxDistance, int nSoundChannels, JMOD.CustomReadFileMethodDelegate customReadFileMethodDelegate)
        {
            // NOTE: Ignore customReadFileMethodDelegate for now... This is getting messy...

            // NOTE: This isn't implemented, but we'll just ignore the choice of audio device (you're not supposed to use FMOD anyway)
            //if (AudioDevice.DeviceID.Length > 0)
            //    throw new NotImplementedException();

            uint version = 0;
            FMOD.RESULT result;
            FMOD.System system = null;
            this.minMaxDistance = minMaxDistance;
            
            LogInit("Init() running with arguments ({0}, {1})", minMaxDistance, nSoundChannels);

            LogInit(false, "Creating system... ");
            result = FMOD.Factory.System_Create(ref system);
            LogInit(result.ToString());
            ERRCHECK(result);

            FMODSystem = system;
            SystemGlue = new FMODSystem(system);
            LogInit("Created SystemGlue");

            result = FMODSystem.getVersion(ref version);
            LogInit("FMOD version {0}, {1} required", version.ToString("X"), FMOD.VERSION.number.ToString("X"));
            ERRCHECK(result);
            if (version < FMOD.VERSION.number)
            {
                LogInit("ERROR: Version mismatch");
                throw new Exception("Error!  You are using an old version of FMOD " + version.ToString("X") + ".  This program requires " + FMOD.VERSION.number.ToString("X") + ".");
            }

            result = FMODSystem.setHardwareChannels(0, 0, 0, 0);        // Skip EAX probe which can crash/corrupt some bad drivers
            LogInit("Disabling hardware channels. {0}", result.ToString());
            ERRCHECK(result);

            int nDrivers = -1;
            LogInit(false, "Checking number of drivers... ");
            result = FMODSystem.getNumDrivers(ref nDrivers);
            LogInit("{0} ({1})", result.ToString(), nDrivers);
            ERRCHECK(result);
            if (nDrivers == 0)
            {
                result = FMODSystem.setOutput(FMOD.OUTPUTTYPE.NOSOUND);
                LogInit("Output set to NOSOUND. " + result.ToString());
                ERRCHECK(result);
            }
            else
            {
                driverCaps = FMOD.CAPS.NONE;
                int minfrequency = 0, maxfrequency = 0;
                FMOD.SPEAKERMODE speakermode = FMOD.SPEAKERMODE.STEREO;

                result = FMODSystem.getDriverCaps(0, ref driverCaps, ref minfrequency, ref maxfrequency, ref speakermode);
                LogInit("Checking driver caps... " + result.ToString());
                ERRCHECK(result);
                LogInit("\tDriver caps: " + driverCaps);
                LogInit("\tFrequency range: {0}-{1}", minfrequency, maxfrequency);
                LogInit("\tSpeaker mode: {0}", speakermode);

                result = FMODSystem.setSpeakerMode(speakermode);
                LogInit("Set speaker mode to {0}. {1}", speakermode, result.ToString());
                ERRCHECK(result);

                if ((driverCaps & FMOD.CAPS.HARDWARE_EMULATED) == FMOD.CAPS.HARDWARE_EMULATED)      // The user has the 'Acceleration' slider set to off!  This is really bad for latency!
                {
                    LogInit("WARNING: Acceleration slider set to off");
                    result = FMODSystem.setDSPBufferSize(1024, 10);
                    LogInit("Set DSP buffer size to 10 * 1024. {0}", result.ToString());
                    ERRCHECK(result);
                }

                StringBuilder name = new StringBuilder(256);
                FMOD.GUID guid = new FMOD.GUID();

                result = FMODSystem.getDriverInfo(0, name, 256, ref guid);
                LogInit("Fetching driver 0. {0}", result.ToString());
                ERRCHECK(result);
                LogInit("\tName: " + name);

                string tmpGuidStr = "";
                for (int i = 0; i < guid.Data4.Length; i++)
                {
                    tmpGuidStr += guid.Data4[i].ToString("X");
                    if (i == 1)
                        tmpGuidStr += "-";
                }
                
                LogInit("\tGUID: {0}-{1}-{2}-{3}", guid.Data1.ToString("X"), guid.Data2.ToString("X"), guid.Data3.ToString("X"), tmpGuidStr);

                if (name.ToString().IndexOf("SigmaTel") != -1)
                {
                    result = FMODSystem.setSoftwareFormat(48000, FMOD.SOUND_FORMAT.PCMFLOAT, 0, 0, FMOD.DSP_RESAMPLER.LINEAR);
                    LogInit("Set SigmaTel-specific software format settings. {0}", result.ToString());
                    ERRCHECK(result);
                }

                if (nDrivers > 1)
                {
                    LogInit("======================================");
                    LogInit("Drivers:");
                    for (int i = 0; i < nDrivers; i++)
                    {
                        LogInit("--------------------------------------");
                        LogDeviceInformation(i);
                    }
                    LogInit("======================================");
                }
            }

            result = FMODSystem.init(nSoundChannels, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
            LogInit("Running init({0}, {1}, null). {2}", nSoundChannels, FMOD.INITFLAGS.NORMAL, result.ToString());
            if (result == FMOD.RESULT.ERR_OUTPUT_CREATEBUFFER)
            {
                LogInit("ERROR: Could not create buffer");
                result = FMODSystem.setSpeakerMode(FMOD.SPEAKERMODE.STEREO);
                LogInit("Set speaker mode to STEREO. {0}", result.ToString());
                ERRCHECK(result);

                result = FMODSystem.init(nSoundChannels, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
                LogInit("Running init({0}, {1}, null). {2}", nSoundChannels, FMOD.INITFLAGS.NORMAL, result.ToString());

                if (result == FMOD.RESULT.ERR_OUTPUT_CREATEBUFFER)
                {
                    LogInit("ERROR: Could not create buffer");
                    result = FMODSystem.setOutput(FMOD.OUTPUTTYPE.WINMM);
                    LogInit("Set output to WINMM. {0}", result.ToString());
                    ERRCHECK(result);

                    result = FMODSystem.init(nSoundChannels, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
                    LogInit("Running init({0}, {1}, null). {2}", nSoundChannels, FMOD.INITFLAGS.NORMAL, result.ToString());
                    ERRCHECK(result);
                }
            }

            result = FMODSystem.set3DSettings(1f, 1f, 1f);
            LogInit("Set 3D-settings to (1, 1, 1). {0}", result.ToString());
            ERRCHECK(result);

            NonBlockCallback = new FMOD.SOUND_NONBLOCKCALLBACK(NonBlockCallbackMethod);
        }

        public void LoadAnySound(SoundResource soundResource, string fullPath)
        {
            //FMOD.MODE flags = FMOD.MODE.HARDWARE;
            FMOD.MODE flags = FMOD.MODE.SOFTWARE | FMOD.MODE._3D_LINEARROLLOFF;
            if (soundResource.Is3DSound) flags |= FMOD.MODE._3D | FMOD.MODE._3D_IGNOREGEOMETRY;
            else flags |= FMOD.MODE._2D;

            FMOD.Sound sound = new FMOD.Sound();
            soundResource.SoundGlue = new FMODSound(sound);
            if (soundResource.IsStream)
            {
                flags = FMOD.MODE.SOFTWARE;
                flags |= FMOD.MODE.CREATESTREAM | FMOD.MODE.NONBLOCKING;
                FMOD.CREATESOUNDEXINFO createSoundExInfo = new FMOD.CREATESOUNDEXINFO();
                createSoundExInfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO));
                createSoundExInfo.nonblockcallback = NonBlockCallback;
                ERRCHECK(FMODSystem.createSound(fullPath, flags, ref createSoundExInfo, ref sound));
                int index = sound.getRaw().ToInt32();
                try
                {
                    loadingSoundResources[index] = soundResource;
                }
                catch (IndexOutOfRangeException)
                {
                    System.Diagnostics.Debugger.Break();        // please report the value of the index-variable defined above
                }
            }
            else
            {
                ERRCHECK(FMODSystem.createSound(fullPath, flags, ref sound));
                InitiateLoadedSound(soundResource);
            }
        }

        public void InitiateLoadedSound(SoundResource soundResource)
        {
            float dummy = 0f;
            int dummyI = 0;
            float baseFrequency = 0;
            FMOD.Sound sound = (FMOD.Sound)soundResource.SoundGlue.InnerSoundObject;

            ERRCHECK(sound.set3DMinMaxDistance(minMaxDistance.X, minMaxDistance.Y));
            ERRCHECK(sound.getDefaults(ref baseFrequency, ref dummy, ref dummy, ref dummyI));
            ERRCHECK(sound.setDefaults(soundResource.PlaybackSpeed * baseFrequency, soundResource.Volume, 0f, (int)soundResource.Priority));
            soundResource.BaseFrequency = baseFrequency;
            if (soundResource.SoundGroupEnum != SoundGroups.Default)
                ERRCHECK(sound.setSoundGroup((FMOD.SoundGroup)SoundManager.Instance.GetSoundGroup(soundResource.SoundGroupEnum).SoundGroupGlue.InnerSoundGroupObject));
        }

        public void Update(float dtime)
        {
            FMODSystem.update();
        }

        #region Pandora's box

        public static void ERRCHECK(FMOD.RESULT result)
        {
            // temporary fix for stolen channels. we might want to add some handling of this in the soundhandler-classes
            // so that the user is notified if he's trying to do something to a stolen channel
            if (result != FMOD.RESULT.OK && result != FMOD.RESULT.ERR_INVALID_HANDLE
                && result != FMOD.RESULT.ERR_CHANNEL_STOLEN)
                throw new Exception("FMOD error! " + result + " - " + FMOD.Error.String(result));
        }

        public string GetAllKindsOfInformation()
        {
            StringBuilder sb = new StringBuilder();

            uint version = 0;
            FMODSystem.getVersion(ref version);
            sb.AppendLine("FMOD Version: " + version);

            float cpuDSP = 0, cpuStream = 0, cpuGeometry = 0, cpuUpdate = 0, cpuTotal = 0;
            ERRCHECK(FMODSystem.getCPUUsage(ref cpuDSP, ref cpuStream, ref cpuGeometry, ref cpuUpdate, ref cpuTotal));
            sb.AppendFormat("CPU usage: (dsp {0:0.000}%, stream {1:0.000}%, geometry {2:0.000}%, update {3:0.000}%, total {4:0.000}%) {5}", cpuDSP, cpuStream, cpuGeometry, cpuUpdate, cpuTotal, Environment.NewLine);
            FMOD.MEMORY_USAGE_DETAILS memoryUsageDetails = new FMOD.MEMORY_USAGE_DETAILS();
            uint memoryUsed = 0;
            ERRCHECK(FMODSystem.getMemoryInfo((uint)FMOD.MEMBITS.ALL, (uint)FMOD.EVENT_MEMBITS.ALL, ref memoryUsed, ref memoryUsageDetails));
            sb.AppendFormat("Memory usage: {0:0,0} B{1}", memoryUsed, Environment.NewLine);
            /* // Sound RAM is dedicated RAM that exists on some devices, not ours though.
            int ramCurrentAllocated = 0, ramMaxAllocated = 0, ramTotal = 0;
            ERRCHECK(FMODSystem.getSoundRAM(ref ramCurrentAllocated, ref ramMaxAllocated, ref ramTotal));
            sb.AppendFormat("Sound RAM (current alloc., max alloc., total): ({0}, {1}, {2}){3}",
                ramCurrentAllocated, ramMaxAllocated, ramTotal, Environment.NewLine); */
            sb.AppendLine("");

            int nDrivers = 0;
            ERRCHECK(FMODSystem.getNumDrivers(ref nDrivers));
            /*  // Caps cannot be called while the system is up and running
            FMOD.CAPS caps = new FMOD.CAPS();
            int hardwareMinFrequency = 0, hardwareMaxFrequency = 0;
            FMOD.SPEAKERMODE controlPanelSpeakerMode = new FMOD.SPEAKERMODE();
            for (int i = 0; i < nDrivers; i++)
            {
                ERRCHECK(FMODSystem.getDriverCaps(i, ref caps, ref hardwareMinFrequency, ref hardwareMaxFrequency, ref controlPanelSpeakerMode));

                sb.AppendLine("Driver id: " + i);
                sb.AppendLine("Caps: " + caps.ToString());
                sb.AppendLine("Hardware minimum frequency: " + hardwareMinFrequency);
                sb.AppendLine("Hardware maximum frequency: " + hardwareMaxFrequency);
                sb.AppendLine("Control panel speaker mode: " + controlPanelSpeakerMode.ToString());
            }*/

            int currentDriver = -0;
            ERRCHECK(FMODSystem.getDriver(ref currentDriver));
            sb.AppendLine("Current driver: " + currentDriver);
            StringBuilder driverName = new StringBuilder(200);
            FMOD.GUID driverGuid = new FMOD.GUID();
            for (int i = 0; i < nDrivers; i++)
            {
                ERRCHECK(FMODSystem.getDriverInfo(i, driverName, 200, ref driverGuid));

                sb.AppendLine("Driver id: " + i + (currentDriver == i ? " (Selected)" : ""));
                sb.AppendLine("Device name: " + driverName.ToString());
                sb.AppendFormat("Driver GUID: ({0}, {1}, {2}){3}", driverGuid.Data1, driverGuid.Data2, driverGuid.Data3, Environment.NewLine);
                if (i == 0)
                {
                    sb.AppendLine("Drivers caps: " + driverCaps.ToString());
                }
                sb.AppendLine("");
            }

            int numHardwareChannels2D = 0, numHardwareChannels3D = 0, numHardwareChannelsTotal = 0;
            ERRCHECK(FMODSystem.getHardwareChannels(ref numHardwareChannels2D, ref numHardwareChannels3D, ref numHardwareChannelsTotal));
            sb.AppendFormat("Hardware channels: {0} ({1} 2D + {2} 3D){3}", numHardwareChannelsTotal, numHardwareChannels2D, numHardwareChannels3D, Environment.NewLine);
            int numSoftwareChannels2D = 0, numSoftwareChannels3D = 0, numSoftwareChannelsTotal = 0;
            ERRCHECK(FMODSystem.getHardwareChannels(ref numSoftwareChannels2D, ref numSoftwareChannels3D, ref numSoftwareChannelsTotal));
            sb.AppendFormat("Software channels: {0} ({1} 2D + {2} 3D){3}", numSoftwareChannelsTotal, numSoftwareChannels2D, numSoftwareChannels3D, Environment.NewLine);
            FMOD.SPEAKERMODE speakerMode = new FMOD.SPEAKERMODE();
            FMODSystem.getSpeakerMode(ref speakerMode);
            sb.AppendLine("Speaker mode: " + speakerMode.ToString());
            sb.AppendLine("Active speakers: ");
            foreach (FMOD.SPEAKER speaker in Enum.GetValues(typeof(FMOD.SPEAKER)))
            {
                if (speaker == FMOD.SPEAKER.MAX || speaker == FMOD.SPEAKER.MONO || speaker == FMOD.SPEAKER.NULL ||
                    speaker == FMOD.SPEAKER.SBL || speaker == FMOD.SPEAKER.SBR)
                    continue;

                float x = 0, y = 0;
                bool active = false;
                ERRCHECK(FMODSystem.get3DSpeakerPosition(speaker, ref x, ref y, ref active));
                if (active)
                    sb.AppendFormat("  {0} ({1}, {2}) {3}", Enum.GetName(speaker.GetType(), speaker), x, y, Environment.NewLine);
            }

            sb.AppendLine("");

            uint selectedOutputPlugin = 0;
            ERRCHECK(FMODSystem.getOutputByPlugin(ref selectedOutputPlugin));
            FMOD.OUTPUTTYPE outputType = new FMOD.OUTPUTTYPE();
            ERRCHECK(FMODSystem.getOutput(ref outputType));
            sb.AppendLine("Output type: " + outputType);
            int nOutputPlugins = 0;
            ERRCHECK(FMODSystem.getNumPlugins(FMOD.PLUGINTYPE.OUTPUT, ref nOutputPlugins));
            IntPtr outputPluginHandle = new IntPtr();
            ERRCHECK(FMODSystem.getOutputHandle(ref outputPluginHandle));
            sb.AppendLine("Output handle: " + outputPluginHandle.ToInt32());
            sb.AppendLine("Plugins: ");
            for (int i = 0; i < nOutputPlugins; i++)
            {
                uint pluginHandle = 0;
                ERRCHECK(FMODSystem.getPluginHandle(FMOD.PLUGINTYPE.OUTPUT, i, ref pluginHandle));
                StringBuilder pluginName = new StringBuilder(100);
                uint pluginVersion = 0;
                FMOD.PLUGINTYPE pluginType = FMOD.PLUGINTYPE.OUTPUT;
                ERRCHECK(FMODSystem.getPluginInfo(pluginHandle, ref pluginType, pluginName, 100, ref pluginVersion));

                sb.AppendFormat("{0}{1} ({2} {3}) {4}", selectedOutputPlugin == pluginHandle ? "* " : "  ", pluginName, pluginType.ToString(), pluginVersion, Environment.NewLine);
            }
            sb.AppendLine();

            sb.AppendLine("Reverb properties: ");
            FMOD.REVERB_PROPERTIES reverbProperties = new FMOD.REVERB_PROPERTIES();
            ERRCHECK(FMODSystem.getReverbProperties(ref reverbProperties));
            sb.Append(StringifyStructure<FMOD.REVERB_PROPERTIES>(reverbProperties, "  "));
            sb.AppendLine("");

            sb.AppendLine("Ambient Reverb properties: ");
            FMOD.REVERB_PROPERTIES reverbAmbientProperties = new FMOD.REVERB_PROPERTIES();
            ERRCHECK(FMODSystem.getReverbAmbientProperties(ref reverbAmbientProperties));
            sb.Append(StringifyStructure<FMOD.REVERB_PROPERTIES>(reverbAmbientProperties, "  "));
            sb.AppendLine("");

            // Returns non-initialised values for some reason
            sb.AppendLine("Advanced settings: ");
            FMOD.ADVANCEDSETTINGS advancedSettings = new FMOD.ADVANCEDSETTINGS();
            ERRCHECK(FMODSystem.getAdvancedSettings(ref advancedSettings));
            sb.Append(StringifyStructure<FMOD.ADVANCEDSETTINGS>(advancedSettings, "  "));
            sb.AppendLine("");

            return sb.ToString();
        }

        private string StringifyStructure<T>(T value, string pad) where T : struct
        {
            StringBuilder sb = new StringBuilder();
            Type t = value.GetType();
            System.Reflection.FieldInfo[] fields = t.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                var v = field.GetValue(value);
                Type ft = field.GetType();
                sb.Append(pad + field.Name + ": " + v);
                if (v is Array)
                {
                    var av = ((Array)v);
                    var enumerator = av.GetEnumerator();
                    sb.Append(" { ");
                    bool first = true;
                    while (enumerator.MoveNext())
                    {
                        if (first)
                            first = false;
                        else
                            sb.Append(", ");
                        sb.Append(enumerator.Current.ToString());
                    }
                    sb.AppendLine(" }");
                }
                else
                    sb.AppendLine("");
            }
            return sb.ToString();
        }

        public int DebugPlayingChannels()
        {
            int nChannels = -1;
            ERRCHECK(FMODSystem.getChannelsPlaying(ref nChannels));
            return nChannels;
        }

        public static FMOD.VECTOR Vector3ToFMODVector(Vector3 v)
        {
            return new FMOD.VECTOR { x = v.X, y = v.Z, z = -v.Y };
        }

        private Vector3 FMODDirectionVector(Vector3 slimdxPosition, FMOD.VECTOR fmodListenerPosition, out Vector3 fmodPosition)
        {
            fmodPosition = new Vector3(slimdxPosition.X, slimdxPosition.Z, -slimdxPosition.Y);
            Vector3 fmodListenerPos = new Vector3(fmodListenerPosition.x, fmodListenerPosition.y, fmodListenerPosition.z);
            return fmodPosition - fmodListenerPos;
        }

        FMOD.CAPS driverCaps;

        #endregion

        #region Logging

        private void LogDeviceInformation(int id)
        {
            FMOD.CAPS driverCaps = FMOD.CAPS.NONE;
            int minfrequency = 0, maxfrequency = 0;
            FMOD.SPEAKERMODE speakermode = FMOD.SPEAKERMODE.STEREO;

            FMOD.RESULT result = FMODSystem.getDriverCaps(id, ref driverCaps, ref minfrequency, ref maxfrequency, ref speakermode);
            LogInit("Checking driver caps... " + result.ToString());
            ERRCHECK(result);
            LogInit("\tDriver caps: " + driverCaps);
            LogInit("\tFrequency range: {0}-{1}", minfrequency, maxfrequency);
            LogInit("\tSpeaker mode: {0}", speakermode);

            StringBuilder name = new StringBuilder(256);
            FMOD.GUID guid = new FMOD.GUID();

            result = FMODSystem.getDriverInfo(id, name, 256, ref guid);
            LogInit("Fetching driver {0}. {1}", id, result.ToString());
            ERRCHECK(result);
            LogInit("\tName: " + name);

            string tmpGuidStr = "";
            for (int i = 0; i < guid.Data4.Length; i++)
            {
                tmpGuidStr += guid.Data4[i].ToString("X");
                if (i == 1)
                    tmpGuidStr += "-";
            }

            LogInit("\tGUID: {0}-{1}-{2}-{3}", guid.Data1.ToString("X"), guid.Data2.ToString("X"), guid.Data3.ToString("X"), tmpGuidStr);
        }

        private void LogInit(String format, params object[] args)
        {
            LogInit(true, format, args);
        }
        private void LogInit(bool newLine, String format, params object[] args)
        {
            string formatString;

            int nArgs = args.Length;
            object[] newArgs = new object[nArgs + 1];
            for (int i = 0; i < nArgs; i++)
                newArgs[i] = args[i];
            newArgs[nArgs] = DateTime.Now.TimeOfDay;

            if (initLogString.Length > 0 && !initLogString.ToString().EndsWith(Environment.NewLine))
                formatString = String.Format("{0}{1}", format, newLine ? Environment.NewLine : "");
            else
                formatString = String.Format("{0} - {1}{2}", "{" + nArgs + "}", format, newLine ? Environment.NewLine : "");
            initLogString.AppendFormat(formatString, newArgs);
        }

        StringBuilder initLogString = new StringBuilder();

        public string InitLogString
        {
            get { return initLogString.ToString(); }
        }

        #endregion

        public FMODChannel GetSoundChannel(int id)
        {
            return channelMapping[id];
        }

        public void SetChannelMapping(int id, FMODChannel channel)
        {
            if (channelMapping.ContainsKey(id))
                System.Diagnostics.Debugger.Break();
            channelMapping[id] = channel;
        }

        public void RemoveChannelMapping(int id)
        {
            channelMapping.Remove(id);
        }

        public bool Muted
        {
            get { return muted; }
            set
            {
                muted = value;
                FMOD.ChannelGroup channelGroup = null;
                ERRCHECK(FMODSystem.getMasterChannelGroup(ref channelGroup));
                ERRCHECK(channelGroup.setMute(value));
            }
        }
        private bool muted = false;

        public float Volume
        {
            get { return volume; }
            set
            {
                volume = Common.Math.Clamp(value, 0, 1);
                FMOD.ChannelGroup channelGroup = null;
                ERRCHECK(FMODSystem.getMasterChannelGroup(ref channelGroup));
                ERRCHECK(channelGroup.setVolume(volume));
            }
        }
        private float volume = 1.0f;

        private FMOD.RESULT NonBlockCallbackMethod(IntPtr soundRaw, FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
                throw new Exception("FMOD Error: " + result);

            int raw = soundRaw.ToInt32();
            if (!loadingSoundResources.ContainsKey(raw))
                return FMOD.RESULT.OK;

            SoundResource soundResource = loadingSoundResources[raw];
            loadingSoundResources.Remove(raw);
            InitiateLoadedSound(soundResource);
            return FMOD.RESULT.OK;
        }

        [Common.CodeState(State=Common.CodeState.Incomplete, Details="Not implemented fully since we aim not to use FMOD anymore.")]
        public void Release()
        {
            FMOD.RESULT result = FMODSystem.release();
            ERRCHECK(result);
        }

        public AudioDevice AudioDevice { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public IEnumerable<AudioDevice> AudioDeviceList { get { throw new NotImplementedException(); } }

        public static FMODManager Instance;
        public FMOD.System FMODSystem { get; set; }
        public ISystemGlue SystemGlue { get; private set; }

        private FMOD.SOUND_NONBLOCKCALLBACK NonBlockCallback;
        private Dictionary<int, SoundResource> loadingSoundResources = new Dictionary<int, SoundResource>();
        private Vector2 minMaxDistance;
        private Dictionary<int, FMODChannel> channelMapping = new Dictionary<int, FMODChannel>();
    }
}

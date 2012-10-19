using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound
{
    public interface ISoundManager
    {
        void LoadSounds(bool loadFullGameSounds);
        void Update(float dtime);
        void Update(float dtime, Vector3 position, Vector3 vel, Vector3 forward, Vector3 up);
        IPlayable GetSFX(SFX sfx);
        IPlayable GetStream(Stream stream);
        IPlayable GetSoundResourceGroup(params IPlayable[] availablePlayables);
        ISoundGroup GetSoundGroup(SoundGroups sg);
        List<ISoundChannel> GetChannelsFromSoundGroup(SoundGroups sg);
        string InitLogString { get; }
        void StopAllChannels();
        String ContentPath { get; set; }
        bool Muted { get; set; }
        float Volume { get; set; }
        AudioDevice AudioDevice { get; set; }
        IEnumerable<AudioDevice> AudioDeviceList { get; }
        Settings Settings { get; set; }
        void Release();
    }

    public enum ManagerGlue
    { 
        FMOD,
        JMOD
    }

    public class SoundManagerException : Exception
    {
        public string Message { get; internal set; }
        public string StackTrace { get; internal set; }
        public string InitLogString { get; internal set; }
    }

    public partial class SoundManager : ISoundManager
    {
        private IManagerGlue managerGlue;

        public SoundManager(AudioDevice audioDevice, ManagerGlue mg, float min3DDistance, float max3DDistance) : this(audioDevice, mg, min3DDistance, max3DDistance, null) { }
        public SoundManager(AudioDevice audioDevice, ManagerGlue mg, float min3DDistance, float max3DDistance, JMOD.CustomReadFileMethodDelegate customReadFileMethod)
        {
            Instance = this;
            Settings = new Settings();
            minMaxDistance = new Vector2(min3DDistance, max3DDistance);
#if DEBUG
            if (mg == ManagerGlue.FMOD)
                managerGlue = new FMODGlue.FMODManager();
            else 
                if (mg == ManagerGlue.JMOD)
#endif
                managerGlue = new JMODGlue.JMODManager();
            try
            {
                managerGlue.Init(audioDevice, minMaxDistance, nSoundChannels, customReadFileMethod);
            }
            catch (Exception ex)
            {
                throw new SoundManagerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InitLogString = managerGlue.InitLogString
                };
            }

            // adds all soundgroups in the SoundGroups enum
            foreach (SoundGroups sg in Enum.GetValues(typeof(SoundGroups)))
            {
                soundGroups.Add(sg, new SoundGroup(managerGlue.SystemGlue, sg.ToString()));
            }
            SetupSoundGroupSettings();
        }

        public void LoadSounds(bool loadFullGameSounds)
        {
            LoadSFX(loadFullGameSounds);
            LoadStreams(loadFullGameSounds);
        }

        public IPlayable GetSFX(SFX sfx) { return soundEffects[sfx]; }
        public IPlayable GetStream(Stream stream) { return soundStreams[stream]; }
        public IPlayable GetSoundResourceGroup(params IPlayable[] availablePlayables)
        {
            string id = SoundResourceGroupID(availablePlayables);
            if (!soundResourceGroups.ContainsKey(id))
                soundResourceGroups[id] = new SoundResourceGroup(availablePlayables);
            return soundResourceGroups[id];
        }

        public static String SoundResourceGroupID(IPlayable[] playables)
        {
            String s = "";
            for (int i = 0; i < playables.Length; i++)
                s += playables[i].Name + "||";
            return s;
        }

        public void LoadAnySound(SoundResource soundResource) 
        {
            if (!Common.FileSystem.Instance.FileExists(ContentPath + soundResource.Name))
                throw new System.IO.FileNotFoundException("File not found: " + ContentPath + soundResource.Name, ContentPath + soundResource.Name);
            managerGlue.LoadAnySound(soundResource, ContentPath + soundResource.Name);
        }

        public void LoadSound(SFX sfx, string filename, SoundResource soundResource)
        {   
            soundResource.Name = "SFX/" + filename;
            LoadAnySound(soundResource);
            soundEffects[sfx] = soundResource;
        }

        public void LoadSound(Stream stream, string filename, SoundResource soundResource)
        {
            soundResource.Name = "Music/" + filename;
            LoadAnySound(soundResource);
            soundStreams[stream] = soundResource;
        }

        public void RegisterChannel(IInternalSoundChannel channel)
        {
            channels.Add(channel);
        }

        public void DeregisterChannel(IInternalSoundChannel channel)
        {
            channels.Remove(channel);
        }

        public ISoundGroup GetSoundGroup(SoundGroups sg)
        {
            return soundGroups[sg];
        }

        public List<ISoundChannel> GetChannelsFromSoundGroup(SoundGroups sg)
        {
            List<ISoundChannel> retChannels = new List<ISoundChannel>();
            foreach (var channel in AllChannels)
            {
                if (channel is SoundChannel && channel.CurrentSoundResource != null)
                {
                    if (((SoundResource)channel.CurrentSoundResource).SoundGroupEnum == sg)
                        retChannels.Add(channel);
                }
            }
            return retChannels;
        }

        public bool Muted { get { return managerGlue.Muted; } set { managerGlue.Muted = value; } }
        public float Volume { get { return managerGlue.Volume; } set { managerGlue.Volume = value; } }
        
        public void Update(float dtime)
        {
            Update(dtime, cachedPosition, cachedVelocity, cachedForward, cachedUp);
        }

        public void Update(float dtime, Vector3 position, Vector3 velocity, Vector3 forward, Vector3 up)
        {
            cachedPosition = position;
            cachedVelocity = velocity;
            cachedForward = forward;
            cachedUp = up;
            if (Muted) return;

            if (Settings.VolumeScale2D3D != lastVolumeScale2D3D)
            {
                ((JMODGlue.JMODSystem)SystemGlue).SetVolumeScale2D3D(Settings.VolumeScale2D3D);
                lastVolumeScale2D3D = Settings.VolumeScale2D3D;
            }

            foreach (var channel in AllChannels)
                channel.Update(dtime);

            // Blend in 2D depending on distance
            foreach (var channel in AllChannels)
            {
                if (channel.CurrentSoundResource != null && ((IInternalSoundResource)channel.CurrentSoundResource).Is3DSound)
                {
                    float distance = (channel.Position - position).Length();
                    distance -= minMaxDistance.X;
                    float panScale = System.Math.Max(0, System.Math.Min(1, 1f - distance / (minMaxDistance.Y - minMaxDistance.X)));
                    channel.ChannelGlue._3DPanLevel = 1f - panScale * Common.Math.Clamp(Settings._3DPanMaxAdjustment, 0, 1);
                    DebugPanLevel = channel.ChannelGlue._3DPanLevel;
                }
            }

            SystemGlue.Set3DListenerAttributes(position, velocity, forward, up);
            managerGlue.Update(dtime);
        }
        private Vector2 lastVolumeScale2D3D;
        public float DebugPanLevel { get; set; }

        public float _3DPanLevel
        {
            get { return panLevel; }
            set 
            { 
                panLevel = System.Math.Min(1f, System.Math.Max(0f, value));
                foreach (var channel in AllChannels)
                    channel._3DPanLevel = panLevel;
            }
        }
        float panLevel = 1f;

        //public float _3DSpread
        //{
        //    get { return spread; }
        //    set
        //    {
        //        spread = value;
        //        foreach (var channel in AllChannels)
        //            channel._3DSpread = spread;
        //    }
        //}
        //private float spread = 0f;

        public Vector2 _3DMinMaxDistance
        {
            get { return minMaxDistance; }
            private set { minMaxDistance = value; }     // TODO: make it changeable while running
            //set 
            //{ 
            //    minMaxDistance = value;
            //    foreach (var resource in AllSoundResources)
            //        if (resource.Is3DSound)
            //            resource._3DMinMaxDistance = minMaxDistance;
            //    foreach (var channel in AllChannels)
            //        if (channel is SoundChannel)
            //            ((SoundChannel)channel)._3DMinMaxDistance = minMaxDistance;
            //}
        }
        private Vector2 minMaxDistance = new Vector2(2, 30);

        private IEnumerable<IInternalSoundChannel> AllChannels
        {
            get 
            {
                //foreach (IInternalSoundChannel channel in new List<IInternalSoundChannel>(channelMapping.Values))
                //    yield return channel;
                //foreach (IInternalSoundChannel channel in new List<IInternalSoundChannel>(intervalChannels))
                //    yield return (SoundChannel)channel;
                //foreach (IInternalSoundChannel channel in new List<IInternalSoundChannel>(waitingChannels))
                //    yield return (SoundChannel)channel;
                return new List<IInternalSoundChannel>(channels);
            }
        }

        private IEnumerable<SoundResource> AllSoundResources
        {
            get
            {
                foreach (SFX ip in soundEffects.Keys)
                    if (soundEffects[ip] is SoundResource)
                        yield return (SoundResource)soundEffects[ip];
                foreach (IPlayable ip in soundStreams.Values)
                    if (ip is SoundResource)
                        yield return (SoundResource)ip;
            }
        }

        public void StopAllChannels()
        {
            foreach (var channel in AllChannels)
                channel.Stop();

            //int channelsPlaying = 0;
            //ERRCHECK(FMODSystem.getChannelsPlaying(ref channelsPlaying));
            //if (channelMapping.Count > 0 || intervalChannels.Count > 0 || channelsPlaying > 0)
            //    throw new Exception("Did not manage to clear the played channels properly");
        }

        public void Release()
        { 
            // NOTE: This may not be sufficient. 
            // TODO: Have a look at this.
            managerGlue.Release();
        }

        #region Volume setup stuff

        public void SetSoundEffectBaseVolume(SFX sfx, float volume)
        {
            if (!oldVolumes.ContainsKey(sfx))
                oldVolumes[sfx] = soundEffects[sfx].Volume;
            soundEffects[sfx].Volume = volume;
        }
        
        public Dictionary<SFX, float> GetDifferingBaseVolumes()
        {
            Dictionary<SFX, float> dict = new Dictionary<SFX, float>();
            foreach (KeyValuePair<SFX, float> kvp in oldVolumes)
            {
                float newVolume = soundEffects[kvp.Key].Volume; 
                if (kvp.Value != newVolume)
                    dict.Add(kvp.Key, newVolume);
            }
            return dict;
        }
        
        public Dictionary<SFX, float> SoundVolumes
        {
            get 
            {
                Dictionary<SFX, float> dict = new Dictionary<SFX, float>(soundEffects.Count);
                foreach (KeyValuePair<SFX, SoundResource> kvp in soundEffects)
                    dict[kvp.Key] = kvp.Value.Volume;
                return dict;
            }
        }
        public Common.WindowsForms.DictionaryPropertyGridAdapter VolumesPropertyTestAdapter 
        { 
            get { return new Common.WindowsForms.DictionaryPropertyGridAdapter(SoundVolumes); } 
        }

        Dictionary<SFX, float> oldVolumes = new Dictionary<SFX, float>();

        #endregion

        public string InitLogString { get { return managerGlue.InitLogString; } }
        public ISystemGlue SystemGlue { get { return managerGlue.SystemGlue; } }
        public String ContentPath { get; set; }
        public AudioDevice AudioDevice
        {
            get { return managerGlue.AudioDevice; }
            set { managerGlue.AudioDevice = value; }
        }

        public IEnumerable<AudioDevice> AudioDeviceList
        {
            get { return managerGlue.AudioDeviceList; }
        }
        
        public Settings Settings
        { 
            get { return settings; }
            set { settings = value; lastVolumeScale2D3D = value.VolumeScale2D3D; }
        }
        private Settings settings;
        
        public static SoundManager Instance;

        private int nSoundChannels = 512;

        private Dictionary<SFX, SoundResource> soundEffects = new Dictionary<SFX, SoundResource>();
        private Dictionary<Stream, SoundResource> soundStreams = new Dictionary<Stream, SoundResource>();
        private Dictionary<String, SoundResourceGroup> soundResourceGroups = new Dictionary<string, SoundResourceGroup>();
        private Dictionary<SoundGroups, SoundGroup> soundGroups = new Dictionary<SoundGroups, SoundGroup>();

        //private List<IInternalSoundChannel> waitingChannels = new List<IInternalSoundChannel>();
        //private List<IInternalSoundChannel> intervalChannels = new List<IInternalSoundChannel>();
        List<IInternalSoundChannel> channels = new List<IInternalSoundChannel>();

        private Vector3 cachedPosition, cachedVelocity, cachedForward, cachedUp;
    }
}
 
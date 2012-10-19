using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SlimDX;
using Newtonsoft.Json;

namespace Client.Sound
{
    [TypeConverter(typeof(ExpandableObjectConverter)), JsonObject, Serializable]
    public class Settings
    {
        public float _3DPanMaxAdjustment { get; set; }
        public bool Muted { get; set; }
        public bool ListenAtCameraPosition { get; set; }
        public float MainCharacterListenHeight { get; set; }
        public Vector2 MinMaxDistance { get; set; }
        public float AmbientVolume { get; set; }
        public float MasterVolume { get; set; }
        public float MusicVolume { get; set; }
        public float SoundVolume { get; set; }
        private float interfaceVolume;
        public float InterfaceVolume
        {
            get { return interfaceVolume; }
            set { interfaceVolume = value; }
        }
        public Vector2 VolumeScale2D3D { get; set; }
        public AudioDevice AudioDevice { get; set; }  
        [Description("Needs program restart")]
        public ManagerGlue Engine { get; set; }

        public Settings()
        {
            _3DPanMaxAdjustment = 0.7f;
            Muted = false;
            ListenAtCameraPosition = false;
            MainCharacterListenHeight = 2;
            MinMaxDistance = new Vector2(1, 20);
            AmbientVolume = 0.5f;
            MasterVolume = 0.5f;
            MusicVolume = 0.5f;
            SoundVolume = 0.5f;
            InterfaceVolume = SoundVolume;
            VolumeScale2D3D = new Vector2(0.7f, 1);
            AudioDevice = new AudioDevice();
            Engine = ManagerGlue.JMOD;
        }
    }
}

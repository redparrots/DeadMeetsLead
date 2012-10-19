using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound
{
    public enum Priority // Priority range: 0 - 256, 0 is most important
    {
        VeryLow = 192,
        Low = 128,
        Medium = 64,
        High = 16,
        VeryHigh = 8
    }

    [Serializable]
    public struct AudioDevice
    {
        public AudioDevice(string deviceID, string name)
        {
            DeviceID = deviceID;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            AudioDevice b = (AudioDevice)obj;
            return DeviceID == b.DeviceID;
        }

        public override string ToString()
        {
            return Name ?? "";
        }

        public bool IsValid { get { return !string.IsNullOrEmpty(DeviceID); } }

        public readonly string DeviceID;
        public readonly string Name;
    }

    public interface ISoundGroup
    {
        float Volume { get; set; }
        String Name { get; }
        int MaxAudible { get; set; }
        ISoundGroupGlue SoundGroupGlue { get; }
    }

    public class SoundGroup : ISoundGroup
    {
        public SoundGroup(ISystemGlue system, String name)
        {
            this.name = name;
            SoundGroupGlue = system.CreateSoundGroup(name);
        }

        public float Volume
        {
            get { return volume; }
            set
            {
                volume = System.Math.Max(0, System.Math.Min(1, value));
                if (SoundGroupGlue != null)
                    SoundGroupGlue.Volume = volume;
            }
        }
        public String Name { get { return name; } }
        public int MaxAudible { get { return maxAudible; } set { maxAudible = value; SoundGroupGlue.SetMaxAudible(value); } }

        public ISoundGroupGlue SoundGroupGlue { get; private set; }

        private int maxAudible = -1;        // -1 is unlimited (default)
        private float volume = 1;           // 1 is full volume (default)
        private string name;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Client.Sound;

namespace Client
{
    public class PublicAudioSettings
    {
        public PublicAudioSettings(bool inGame)
        {
            this.inGame = inGame;
            settings = Program.Settings;
            //SettingsForm.ApplySettings += new Action(SettingsForm_ApplySettings);
            AudioOptionsWindow.ApplySettings += new Action(SettingsForm_ApplySettings);

            //muteAllSounds = settings.SoundSettings.Muted;
            masterVolume = settings.SoundSettings.MasterVolume;
            ambientVolume = settings.SoundSettings.AmbientVolume;
            musicVolume = settings.SoundSettings.MusicVolume;
            soundVolume = settings.SoundSettings.SoundVolume;
            //AudioDevice = settings.SoundSettings.AudioDevice;
            AudioDevice = (Program.Instance != null && Program.Instance.SoundManager != null) ? Program.Instance.SoundManager.AudioDevice : settings.SoundSettings.AudioDevice;
        }

        void SettingsForm_ApplySettings()
        {
            //settings.SoundSettings.Muted = muteAllSounds;
            settings.SoundSettings.MasterVolume = Program.Instance.SoundManager.Volume = masterVolume;
            settings.SoundSettings.AmbientVolume = Program.Instance.SoundManager.GetSoundGroup(Client.Sound.SoundGroups.Ambient).Volume = ambientVolume;
            settings.SoundSettings.MusicVolume = Program.Instance.SoundManager.GetSoundGroup(Client.Sound.SoundGroups.Music).Volume = musicVolume;
            settings.SoundSettings.SoundVolume = Program.Instance.SoundManager.GetSoundGroup(Client.Sound.SoundGroups.SoundEffects).Volume = soundVolume;
            Program.Instance.SoundManager.GetSoundGroup(Client.Sound.SoundGroups.Interface).Volume = soundVolume;
            
            if (!inGame && !settings.SoundSettings.AudioDevice.Equals(AudioDevice))
            {
                try
                {
                    Program.Instance.SoundManager.AudioDevice = AudioDevice;    // if this line fails we don't save the results
                    settings.SoundSettings.AudioDevice = AudioDevice;
                    if (AudioDevice.IsValid && Program.Instance.SoundManager is DummySoundManager)
                        Dialog.Show(Locale.Resource.SettingsNeedRestartTitle, Locale.Resource.SettingsNeedRestart);
                }
                catch (Exception ex)
                {
                    Dialog.Show(
                        Locale.Resource.ErrorFailSetAudioDeviceTitle, 
                        String.Format(Locale.Resource.ErrorFailSetAudioDevice, ex.Message));
                    settings.SoundSettings.AudioDevice = new AudioDevice("", "");
                    Program.Instance.SoundManager = new DummySoundManager();
                }
            }
        }

        private float masterVolume;
        [VolumeController]
        public float MasterVolume
        {
            get { return masterVolume; }
            set { masterVolume = value; }
        }

        private float ambientVolume;
        [VolumeController]
        public float AmbientVolume
        {
            get { return ambientVolume; }
            set { ambientVolume = value; }
        }

        private float musicVolume;
        [VolumeController]
        public float MusicVolume
        {
            get { return musicVolume; }
            set { musicVolume = value; }
        }

        private float soundVolume;
        [VolumeController]
        public float SoundVolume
        {
            get { return soundVolume; }
            set { soundVolume = value; }
        }

        public AudioDevice AudioDevice { get; set; }

        //private bool muteAllSounds;
        //[DescriptionAttribute("Mutes all sounds")]
        //public bool MuteAllSounds
        //{
        //    get { return muteAllSounds; }
        //    set { muteAllSounds = value; }
        //}        

        private bool inGame;
        private Settings settings;
    }

    [Serializable]
    public class PublicControlsSettings
    {
        public PublicControlsSettings()
        {
            MoveForward = System.Windows.Forms.Keys.W;
            MoveBackward = System.Windows.Forms.Keys.S;
            StrafeLeft = System.Windows.Forms.Keys.A;
            StrafeRight = System.Windows.Forms.Keys.D;
            Jump = System.Windows.Forms.Keys.Space;
            Attack = System.Windows.Forms.Keys.LButton;
            SpecialAttack = System.Windows.Forms.Keys.RButton;
            MeleeWeapon = System.Windows.Forms.Keys.D1;
            RangedWeapon = System.Windows.Forms.Keys.D2;
        }

        private System.Windows.Forms.Keys moveForward;
        [ControlsAttribute]
        public System.Windows.Forms.Keys MoveForward { get { return moveForward; } set { moveForward = value; } }

        private System.Windows.Forms.Keys moveBackward;
        [ControlsAttribute]
        public System.Windows.Forms.Keys MoveBackward { get { return moveBackward; } set { moveBackward = value; } }

        private System.Windows.Forms.Keys strafeLeft;
        [ControlsAttribute]
        public System.Windows.Forms.Keys StrafeLeft { get { return strafeLeft; } set { strafeLeft = value; } }

        private System.Windows.Forms.Keys strafeRight;
        [ControlsAttribute]
        public System.Windows.Forms.Keys StrafeRight { get { return strafeRight; } set { strafeRight = value; } }

        private System.Windows.Forms.Keys jump;
        [ControlsAttribute]
        public System.Windows.Forms.Keys Jump { get { return jump; } set { jump = value; } }

        //private System.Windows.Forms.Keys rage;
        //[ControlsAttribute]
        //public System.Windows.Forms.Keys Rage { get { return rage; } set { rage = value; } }

        //private System.Windows.Forms.Keys heal;
        //[ControlsAttribute]
        //public System.Windows.Forms.Keys Heal { get { return heal; } set { heal = value; } }

        private System.Windows.Forms.Keys meleeWeapon;
        [ControlsAttribute]
        public System.Windows.Forms.Keys MeleeWeapon { get { return meleeWeapon; } set { meleeWeapon = value; } }

        private System.Windows.Forms.Keys rangedWeapon;
        [ControlsAttribute]
        public System.Windows.Forms.Keys RangedWeapon { get { return rangedWeapon; } set { rangedWeapon = value; } }

        private System.Windows.Forms.Keys attack;
        public System.Windows.Forms.Keys Attack { get { return attack; } set { attack = value; } }

        private System.Windows.Forms.Keys specialAttack;
        public System.Windows.Forms.Keys SpecialAttack { get { return specialAttack; } set { specialAttack = value; } }
    }

    public enum VideoQualities
    {
        [Common.ResourceStringAttribute("VideoEnumLow")]
        Low,
        [Common.ResourceStringAttribute("VideoEnumMedium")]
        Medium,
        [Common.ResourceStringAttribute("VideoEnumHigh")]
        High,
        [Common.ResourceStringAttribute("VideoEnumUltra")]
        Ultra,
        [Common.ResourceStringAttribute("VideoEnumCustom")]
        Custom
    }
}
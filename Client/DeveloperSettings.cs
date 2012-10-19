using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class DeveloperSettings : Form
    {
        public DeveloperSettings()
        {
            InitializeComponent();
            Program.Instance.ProfileChanged += new EventHandler(Instance_ProfileChanged);
            Program.Instance.ProgramEvent += new ProgramEventHandler(Instance_ProgramEvent);
            Program.Instance.SoundLoaded += new EventHandler(Instance_SoundLoaded);
            volumesPropertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(volumesPropertyGrid_PropertyValueChanged);
            weaponGraphPropertyGrid.SelectedObject = damageVSRageGraphControl1.Settings;
        }

        void volumesPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Sound.SFX sfx = (Client.Sound.SFX)Enum.Parse(typeof(Client.Sound.SFX), e.ChangedItem.Label);
            float newValue = (float)e.ChangedItem.Value;
            newValue = Common.Math.Clamp(newValue, 0, 1);
            if (soundManager != null)
                soundManager.SetSoundEffectBaseVolume(sfx, newValue);
        }

        private void btnVolumesClipboard_Click(object sender, EventArgs e)
        {
            if (soundManager != null)
            {
                var dict = soundManager.GetDifferingBaseVolumes();
                StringBuilder sb = new StringBuilder();
                foreach (var kvp in dict)
                    sb.AppendFormat("{0}: {1}{2}", kvp.Key, kvp.Value, Environment.NewLine);
                if(sb.Length > 0)
                    Clipboard.SetText(sb.ToString());
            }
        }

        void Instance_SoundLoaded(object sender, EventArgs e)
        {
            soundManager = Program.Instance.SoundManager as Sound.SoundManager;
            if (soundManager != null)
                volumesPropertyGrid.SelectedObject = soundManager.VolumesPropertyTestAdapter;
        }

        void Instance_ProgramEvent(ProgramEvent e)
        {
            eventLogListBox.Items.Add(e);
        }

        void Instance_ProfileChanged(object sender, EventArgs e)
        {
            profilePropertyGrid.SelectedObject = Program.Instance.Profile;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                ((Graphics.Interface.InterfaceRenderer9)Program.Instance.InterfaceRenderer).Drawable =
                    (et) => et.AbsoluteTranslation.Z > zIndexTrackBar.Value / 10f;
            else
                ((Graphics.Interface.InterfaceRenderer9)Program.Instance.InterfaceRenderer).Drawable =
                    (et) => true;
        }

        private void zIndexTrackBar_Scroll(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                zIndexLabel.Text = "Draw all with ZIndex > " + zIndexTrackBar.Value / 10f;
        }

        private void forceZIndexRecalcButton_Click(object sender, EventArgs e)
        {
            ((Graphics.Interface.Control)Program.Instance.InterfaceScene.Root).RecalcChildrenZIndex();
        }

        private void updateWeaponGraphButton_Click(object sender, EventArgs e)
        {
            damageVSRageGraphControl1.UpdateGraph();
        }

        Sound.SoundManager soundManager;
    }
}

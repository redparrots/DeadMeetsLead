using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Editor
{
    public partial class CameraAnglesEditorControl : UserControl
    {
        public CameraAnglesEditorControl()
        {
            InitializeComponent();
        }
        List<Client.Game.Map.CameraAngle> cameraAngles;
        public List<Client.Game.Map.CameraAngle> CameraAngles { get { return cameraAngles; }
            set { cameraAngles = value; UpdateList(); }
        }
        public void UpdateList()
        {
            cameraAnglesListBox.Items.Clear();
            foreach (var v in cameraAngles)
                cameraAnglesListBox.Items.Add(v);
        }

        void SetupAngle(Client.Game.Map.CameraAngle a)
        {
            a.Position = MainWindow.Instance.worldView.Scene.Camera.Position;
            a.Lookat = ((Graphics.LookatCamera)MainWindow.Instance.worldView.Scene.Camera).Lookat;
            a.Up = ((Graphics.LookatCamera)MainWindow.Instance.worldView.Scene.Camera).Up;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Client.Game.Map.CameraAngle a = new Client.Game.Map.CameraAngle
            {
                Name = nameTextBox.Text,
            };
            SetupAngle(a);
            cameraAngles.Add(a);
            UpdateList();
        }

        private void setButton_Click(object sender, EventArgs e)
        {
            Client.Game.Map.CameraAngle a = cameraAnglesListBox.SelectedItem as Client.Game.Map.CameraAngle;
            SetupAngle(a);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            Client.Game.Map.CameraAngle a = cameraAnglesListBox.SelectedItem as Client.Game.Map.CameraAngle;
            cameraAnglesListBox.Items.Remove(a);
            cameraAngles.Remove(a);
        }

        private void useButton_Click(object sender, EventArgs e)
        {
            Client.Game.Map.CameraAngle a = cameraAnglesListBox.SelectedItem as Client.Game.Map.CameraAngle;
            if (a == null) return;

            var camera = (Graphics.LookatSphericalCamera)MainWindow.Instance.worldView.Scene.Camera;

            camera.Lookat = a.Lookat;
            camera.Position = a.Position;
            camera.Up = a.Up;
        }

        private void cameraAnglesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            useButton_Click(null, null);
        }
    }
}

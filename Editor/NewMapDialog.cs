using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Editor
{
    public partial class NewMapDialog : Form
    {
        public NewMapDialog()
        {
            InitializeComponent();
            Settings = new Client.Game.Map.MapSettings();
            settingsPropertyGrid.SelectedObject = Settings;
        }

        public Client.Game.Map.MapSettings Settings { get; private set; }
    }
}

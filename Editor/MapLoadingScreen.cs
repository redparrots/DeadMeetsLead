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
    public partial class MapLoadingScreen : UserControl
    {
        public MapLoadingScreen()
        {
            InitializeComponent();
        }

        public void AppendLoadingString(String s)
        {
            statusLabel.Text += "[" + ((int)Time.TotalSeconds) + "s] " + s + "\n";
        }

        public TimeSpan Time { get { return (DateTime.Now - start); } }

        DateTime start = DateTime.Now;
    }
}

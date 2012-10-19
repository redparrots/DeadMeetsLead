using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.WindowsForms
{
    public partial class SelectDialog : Form
    {
        public SelectDialog()
        {
            InitializeComponent();
        }
        public object SelectedObject { get { return selectComboBox.SelectedItem; } }
    }
}

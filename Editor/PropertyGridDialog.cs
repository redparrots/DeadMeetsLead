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
    public partial class PropertyGridDialog : Form
    {
        public PropertyGridDialog()
        {
            InitializeComponent();
        }

        public object SelectedObject { get { return propertyGrid1.SelectedObject; } set { propertyGrid1.SelectedObject = value; } }
    }
}

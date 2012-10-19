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
    public partial class PromptDialog : Form
    {
        public PromptDialog()
        {
            InitializeComponent();
        }

        public string Value { get { return textBox1.Text; } set { textBox1.Text = value; } }
    }
}

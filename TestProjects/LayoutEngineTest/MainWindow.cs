using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LayoutEngineTest
{
    partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = testLayoutEditorControl1.Root;
            testLayoutEditorControl1.SelectedChanged += new EventHandler(testLayoutEditorControl1_SelectedChanged);
            t.Interval = 500;
            t.Tick += new EventHandler(t_Tick);
            t.Enabled = true;
        }
         
        void testLayoutEditorControl1_SelectedChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = testLayoutEditorControl1.Selected;
        }

        void t_Tick(object sender, EventArgs e)
        {
            //testLayoutEditorControl1.Invalidate();
        }
        Timer t = new Timer();

        private void doLayoutButton_Click(object sender, EventArgs e)
        {
            testLayoutEditorControl1.Root.LayoutEngine.Layout(testLayoutEditorControl1.Root);
        }


        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            testLayoutEditorControl1.Invalidate();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;

namespace PathingTest
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            stateComboBox.SelectedIndex = 0;
            dropTypeComboBox.SelectedIndex = 0;
            //optionsPropertyGrid.SelectedObject = view1.CurrentOptions;
        }

        private void playPauseButton_Click(object sender, EventArgs e)
        {
            if (view1.PlayPause())
                playPauseButton.Text = "Pause";
            else
                playPauseButton.Text = "Play";
        }

        private void stepButton_Click(object sender, EventArgs e)
        {
            view1.Step();
        }

        private void dropTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            view1.DropType = (String)dropTypeComboBox.SelectedItem;
        }

        private void stateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((String)stateComboBox.SelectedItem) == "Drop")
                view1.ChangeState(new StateDrop(view1));
            else if (((String)stateComboBox.SelectedItem) == "Move")
                view1.ChangeState(new StateMove(view1));
            //else if (((String)stateComboBox.SelectedItem) == "UnitControl")
            //    view1.ChangeState(new StateUnitControl(view1));
        }

        private void patrolButton_Click(object sender, EventArgs e)
        {
            //foreach (Common.Motion.NPC npc in view1.NPCs)
            //    npc.Wander(new Random(), 0.2f, 0.2f, 20f);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            if (d.ShowDialog() == DialogResult.Cancel) return;
            view1.Save(d.FileName);
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            if (d.ShowDialog() == DialogResult.Cancel) return;
            view1.Load(d.FileName);
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            view1.Clear();
        }

        private void resetNetworkStatsButton_Click(object sender, EventArgs e)
        {
            //view1.ResetNetworkStats();
        }

        private void cbPlaceNavMesh_CheckedChanged(object sender, EventArgs e)
        {

        }

        
    }
}

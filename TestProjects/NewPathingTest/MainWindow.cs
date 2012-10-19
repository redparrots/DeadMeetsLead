using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;

namespace NewPathingTest
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadNavMesh_Click(object sender, EventArgs e)
        {
            pathingView.LoadNavMesh(defaultNavMeshFileName);
        }

        private void btnSaveNavMesh_Click(object sender, EventArgs e)
        {
            pathingView.SaveNavMesh(defaultNavMeshFileName);
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            //if (pathingView.npc != null)
            //{
            //    var umo = (Common.Motion.Unit)pathingView.npc.MotionObject;
            //    lblDebugText.Text = "Pos: " + umo.Position + "\nVelocity: " + umo.Velocity + "\nRunVelocity: " + umo.RunVelocity;
            //}
            var npc = pathingView.AddNewNPC(new Vector2(-5, -5));
            pathingView.PursueNPC((Common.IMotion.INPC)npc.MotionObject);
        }

        private string defaultNavMeshFileName = "navmesh";

        private void btnNextIH_Click(object sender, EventArgs e)
        {
            pathingView.NextInputHandler();
        }

        private void btnCreateGrid_Click(object sender, EventArgs e)
        {
            if (pathingView.Paused)
            {
                btnCreateGrid.Text = "Show Grid";
                pathingView.HideGrid();
            }
            else
            {
                btnCreateGrid.Text = "Hide Grid";
                pathingView.DisplayGrid();
            }
        }

        private void btnPursueNPC_Click(object sender, EventArgs e)
        {
            if (btnPursueNPC.Text == "Pursue NPC")
            {
                btnPursueNPC.Text = "Idle";
                pathingView.PursueNPC(true);
            }
            else
            {
                btnPursueNPC.Text = "Pursue NPC";
                pathingView.PursueNPC(false);
            }
        }

        private void cbDisplayGrids_CheckedChanged(object sender, EventArgs e)
        {
            pathingView.DisplayingGrids = !pathingView.DisplayingGrids;
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            btnCreateGrid.Text = "Show Grid";
            pathingView.HideGrid();
        }
    }
}

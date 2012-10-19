namespace NewPathingTest
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pathingView = new NewPathingTest.PathingView();
            this.cbDisplayGrids = new System.Windows.Forms.CheckBox();
            this.btnPursueNPC = new System.Windows.Forms.Button();
            this.btnCreateGrid = new System.Windows.Forms.Button();
            this.btnNextIH = new System.Windows.Forms.Button();
            this.btnDebug = new System.Windows.Forms.Button();
            this.lblDebugText = new System.Windows.Forms.Label();
            this.navmeshBox = new System.Windows.Forms.GroupBox();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnSaveNavMesh = new System.Windows.Forms.Button();
            this.btnLoadNavMesh = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.navmeshBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pathingView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnResume);
            this.splitContainer1.Panel2.Controls.Add(this.cbDisplayGrids);
            this.splitContainer1.Panel2.Controls.Add(this.btnPursueNPC);
            this.splitContainer1.Panel2.Controls.Add(this.btnCreateGrid);
            this.splitContainer1.Panel2.Controls.Add(this.btnNextIH);
            this.splitContainer1.Panel2.Controls.Add(this.btnDebug);
            this.splitContainer1.Panel2.Controls.Add(this.lblDebugText);
            this.splitContainer1.Panel2.Controls.Add(this.navmeshBox);
            this.splitContainer1.Size = new System.Drawing.Size(957, 552);
            this.splitContainer1.SplitterDistance = 698;
            this.splitContainer1.TabIndex = 0;
            // 
            // pathingView
            // 
            this.pathingView.BackColor = System.Drawing.Color.Black;
            this.pathingView.DisplayingGrids = false;
            this.pathingView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathingView.InputHandler = null;
            this.pathingView.Location = new System.Drawing.Point(0, 0);
            this.pathingView.GraphicsDevice = new Graphics.GraphicsDevice.GraphicsDevice9 { Settings = new Graphics.GraphicsDevice.Settings() };
            this.pathingView.Name = "pathingView";
            this.pathingView.Paused = false;
            this.pathingView.Size = new System.Drawing.Size(698, 552);
            this.pathingView.TabIndex = 0;
            this.pathingView.Window = this;
            // 
            // cbDisplayGrids
            // 
            this.cbDisplayGrids.AutoSize = true;
            this.cbDisplayGrids.Location = new System.Drawing.Point(10, 281);
            this.cbDisplayGrids.Name = "cbDisplayGrids";
            this.cbDisplayGrids.Size = new System.Drawing.Size(153, 17);
            this.cbDisplayGrids.TabIndex = 6;
            this.cbDisplayGrids.Text = "Display grids when created";
            this.cbDisplayGrids.UseVisualStyleBackColor = true;
            this.cbDisplayGrids.CheckedChanged += new System.EventHandler(this.cbDisplayGrids_CheckedChanged);
            // 
            // btnPursueNPC
            // 
            this.btnPursueNPC.Location = new System.Drawing.Point(168, 251);
            this.btnPursueNPC.Name = "btnPursueNPC";
            this.btnPursueNPC.Size = new System.Drawing.Size(75, 23);
            this.btnPursueNPC.TabIndex = 5;
            this.btnPursueNPC.Text = "Pursue NPC";
            this.btnPursueNPC.UseVisualStyleBackColor = true;
            this.btnPursueNPC.Click += new System.EventHandler(this.btnPursueNPC_Click);
            // 
            // btnCreateGrid
            // 
            this.btnCreateGrid.Location = new System.Drawing.Point(88, 251);
            this.btnCreateGrid.Name = "btnCreateGrid";
            this.btnCreateGrid.Size = new System.Drawing.Size(75, 23);
            this.btnCreateGrid.TabIndex = 4;
            this.btnCreateGrid.Text = "Create Grid";
            this.btnCreateGrid.UseVisualStyleBackColor = true;
            this.btnCreateGrid.Click += new System.EventHandler(this.btnCreateGrid_Click);
            // 
            // btnNextIH
            // 
            this.btnNextIH.Location = new System.Drawing.Point(7, 251);
            this.btnNextIH.Name = "btnNextIH";
            this.btnNextIH.Size = new System.Drawing.Size(75, 23);
            this.btnNextIH.TabIndex = 3;
            this.btnNextIH.Text = "Next IH";
            this.btnNextIH.UseVisualStyleBackColor = true;
            this.btnNextIH.Click += new System.EventHandler(this.btnNextIH_Click);
            // 
            // btnDebug
            // 
            this.btnDebug.Location = new System.Drawing.Point(10, 317);
            this.btnDebug.Name = "btnDebug";
            this.btnDebug.Size = new System.Drawing.Size(75, 23);
            this.btnDebug.TabIndex = 2;
            this.btnDebug.Text = "Debug";
            this.btnDebug.UseVisualStyleBackColor = true;
            this.btnDebug.Click += new System.EventHandler(this.btnDebug_Click);
            // 
            // lblDebugText
            // 
            this.lblDebugText.AutoSize = true;
            this.lblDebugText.Location = new System.Drawing.Point(7, 355);
            this.lblDebugText.Name = "lblDebugText";
            this.lblDebugText.Size = new System.Drawing.Size(35, 13);
            this.lblDebugText.TabIndex = 1;
            this.lblDebugText.Text = "label1";
            // 
            // navmeshBox
            // 
            this.navmeshBox.Controls.Add(this.btnSaveAs);
            this.navmeshBox.Controls.Add(this.btnSaveNavMesh);
            this.navmeshBox.Controls.Add(this.btnLoadNavMesh);
            this.navmeshBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.navmeshBox.Location = new System.Drawing.Point(0, 452);
            this.navmeshBox.Name = "navmeshBox";
            this.navmeshBox.Size = new System.Drawing.Size(255, 100);
            this.navmeshBox.TabIndex = 0;
            this.navmeshBox.TabStop = false;
            this.navmeshBox.Text = "NavMesh";
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Location = new System.Drawing.Point(174, 19);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAs.TabIndex = 2;
            this.btnSaveAs.Text = "Save as...";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnSaveNavMesh
            // 
            this.btnSaveNavMesh.Location = new System.Drawing.Point(88, 19);
            this.btnSaveNavMesh.Name = "btnSaveNavMesh";
            this.btnSaveNavMesh.Size = new System.Drawing.Size(75, 23);
            this.btnSaveNavMesh.TabIndex = 1;
            this.btnSaveNavMesh.Text = "Save";
            this.btnSaveNavMesh.UseVisualStyleBackColor = true;
            this.btnSaveNavMesh.Click += new System.EventHandler(this.btnSaveNavMesh_Click);
            // 
            // btnLoadNavMesh
            // 
            this.btnLoadNavMesh.Location = new System.Drawing.Point(7, 19);
            this.btnLoadNavMesh.Name = "btnLoadNavMesh";
            this.btnLoadNavMesh.Size = new System.Drawing.Size(75, 23);
            this.btnLoadNavMesh.TabIndex = 0;
            this.btnLoadNavMesh.Text = "Load";
            this.btnLoadNavMesh.UseVisualStyleBackColor = true;
            this.btnLoadNavMesh.Click += new System.EventHandler(this.btnLoadNavMesh_Click);
            // 
            // btnResume
            // 
            this.btnResume.Location = new System.Drawing.Point(168, 277);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(75, 23);
            this.btnResume.TabIndex = 7;
            this.btnResume.Text = "Resume";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 552);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainWindow";
            this.Text = "New Pathing Test";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.navmeshBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PathingView pathingView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox navmeshBox;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnSaveNavMesh;
        private System.Windows.Forms.Button btnLoadNavMesh;
        private System.Windows.Forms.Label lblDebugText;
        private System.Windows.Forms.Button btnDebug;
        private System.Windows.Forms.Button btnNextIH;
        private System.Windows.Forms.Button btnCreateGrid;
        private System.Windows.Forms.Button btnPursueNPC;
        private System.Windows.Forms.CheckBox cbDisplayGrids;
        private System.Windows.Forms.Button btnResume;
    }
}


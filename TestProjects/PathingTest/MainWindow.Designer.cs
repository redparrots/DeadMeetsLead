namespace PathingTest
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
            this.view1 = new PathingTest.View();
            this.optionsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.clearButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.dropTypeComboBox = new System.Windows.Forms.ComboBox();
            this.patrolButton = new System.Windows.Forms.Button();
            this.stateComboBox = new System.Windows.Forms.ComboBox();
            this.stepButton = new System.Windows.Forms.Button();
            this.playPauseButton = new System.Windows.Forms.Button();
            this.cbPlaceNavMesh = new System.Windows.Forms.CheckBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.view1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cbPlaceNavMesh);
            this.splitContainer1.Panel2.Controls.Add(this.optionsPropertyGrid);
            this.splitContainer1.Panel2.Controls.Add(this.clearButton);
            this.splitContainer1.Panel2.Controls.Add(this.loadButton);
            this.splitContainer1.Panel2.Controls.Add(this.saveButton);
            this.splitContainer1.Panel2.Controls.Add(this.dropTypeComboBox);
            this.splitContainer1.Panel2.Controls.Add(this.patrolButton);
            this.splitContainer1.Panel2.Controls.Add(this.stateComboBox);
            this.splitContainer1.Panel2.Controls.Add(this.stepButton);
            this.splitContainer1.Panel2.Controls.Add(this.playPauseButton);
            this.splitContainer1.Size = new System.Drawing.Size(828, 481);
            this.splitContainer1.SplitterDistance = 647;
            this.splitContainer1.TabIndex = 1;
            // 
            // view1
            // 
            this.view1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view1.Location = new System.Drawing.Point(0, 0);
            this.view1.Name = "view1";
            this.view1.Size = new System.Drawing.Size(647, 481);
            this.view1.TabIndex = 0;
            this.view1.Text = "view1";
            // 
            // optionsPropertyGrid
            // 
            this.optionsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.optionsPropertyGrid.HelpVisible = false;
            this.optionsPropertyGrid.Location = new System.Drawing.Point(0, 198);
            this.optionsPropertyGrid.Name = "optionsPropertyGrid";
            this.optionsPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.optionsPropertyGrid.Size = new System.Drawing.Size(177, 283);
            this.optionsPropertyGrid.TabIndex = 9;
            this.optionsPropertyGrid.ToolbarVisible = false;
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(119, 76);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(40, 24);
            this.clearButton.TabIndex = 7;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(67, 76);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(48, 24);
            this.loadButton.TabIndex = 6;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(13, 76);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(48, 24);
            this.saveButton.TabIndex = 5;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // dropTypeComboBox
            // 
            this.dropTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dropTypeComboBox.FormattingEnabled = true;
            this.dropTypeComboBox.Items.AddRange(new object[] {
            "NPC",
            "Unit",
            "Prop",
            "NPC + Zombie"});
            this.dropTypeComboBox.Location = new System.Drawing.Point(95, 13);
            this.dropTypeComboBox.Name = "dropTypeComboBox";
            this.dropTypeComboBox.Size = new System.Drawing.Size(64, 21);
            this.dropTypeComboBox.TabIndex = 2;
            this.dropTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.dropTypeComboBox_SelectedIndexChanged);
            // 
            // patrolButton
            // 
            this.patrolButton.Location = new System.Drawing.Point(13, 167);
            this.patrolButton.Name = "patrolButton";
            this.patrolButton.Size = new System.Drawing.Size(48, 25);
            this.patrolButton.TabIndex = 3;
            this.patrolButton.Text = "Patrol";
            this.patrolButton.UseVisualStyleBackColor = true;
            this.patrolButton.Click += new System.EventHandler(this.patrolButton_Click);
            // 
            // stateComboBox
            // 
            this.stateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stateComboBox.FormattingEnabled = true;
            this.stateComboBox.Items.AddRange(new object[] {
            "Drop",
            "Move",
            "UnitControl"});
            this.stateComboBox.Location = new System.Drawing.Point(13, 12);
            this.stateComboBox.Name = "stateComboBox";
            this.stateComboBox.Size = new System.Drawing.Size(76, 21);
            this.stateComboBox.TabIndex = 2;
            this.stateComboBox.SelectedIndexChanged += new System.EventHandler(this.stateComboBox_SelectedIndexChanged);
            // 
            // stepButton
            // 
            this.stepButton.Location = new System.Drawing.Point(93, 40);
            this.stepButton.Name = "stepButton";
            this.stepButton.Size = new System.Drawing.Size(66, 31);
            this.stepButton.TabIndex = 1;
            this.stepButton.Text = "Step";
            this.stepButton.UseVisualStyleBackColor = true;
            this.stepButton.Click += new System.EventHandler(this.stepButton_Click);
            // 
            // playPauseButton
            // 
            this.playPauseButton.Location = new System.Drawing.Point(13, 39);
            this.playPauseButton.Name = "playPauseButton";
            this.playPauseButton.Size = new System.Drawing.Size(76, 31);
            this.playPauseButton.TabIndex = 0;
            this.playPauseButton.Text = "Play";
            this.playPauseButton.UseVisualStyleBackColor = true;
            this.playPauseButton.Click += new System.EventHandler(this.playPauseButton_Click);
            // 
            // cbPlaceNavMesh
            // 
            this.cbPlaceNavMesh.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbPlaceNavMesh.Location = new System.Drawing.Point(13, 106);
            this.cbPlaceNavMesh.Name = "cbPlaceNavMesh";
            this.cbPlaceNavMesh.Size = new System.Drawing.Size(146, 24);
            this.cbPlaceNavMesh.TabIndex = 11;
            this.cbPlaceNavMesh.Text = "Place NavMesh";
            this.cbPlaceNavMesh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbPlaceNavMesh.UseVisualStyleBackColor = true;
            this.cbPlaceNavMesh.CheckedChanged += new System.EventHandler(this.cbPlaceNavMesh_CheckedChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 481);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private View view1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button stepButton;
        private System.Windows.Forms.Button playPauseButton;
        private System.Windows.Forms.ComboBox dropTypeComboBox;
        private System.Windows.Forms.ComboBox stateComboBox;
        private System.Windows.Forms.Button patrolButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.PropertyGrid optionsPropertyGrid;
        private System.Windows.Forms.CheckBox cbPlaceNavMesh;
    }
}


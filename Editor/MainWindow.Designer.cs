namespace Editor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.miscToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToMainCharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toogleEditorCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllEntitiesOfTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reinitSelectedEntitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setMapSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hacksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addOrientationToAllObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resaveAllMapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setStaticDataSourceMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createCameraPointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.placementBoundingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapBoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.physicsBoundingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.physicsBoundingsHideGroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFullChainsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pickingBoundingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aggroRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visualBoundingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stringEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.worldView = new Editor.WorldView();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.stateSettingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mouseGroundPositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tipsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.modeComboBox,
            this.miscToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.scriptEditorToolStripMenuItem,
            this.stringEditorToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1280, 27);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 23);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.openToolStripMenuItem.Text = "Open..";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.saveAsToolStripMenuItem.Text = "Save As..";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // modeComboBox
            // 
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(121, 23);
            this.modeComboBox.SelectedIndexChanged += new System.EventHandler(this.modeComboBox_SelectedIndexChanged);
            // 
            // miscToolStripMenuItem
            // 
            this.miscToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToMainCharacterToolStripMenuItem,
            this.toogleEditorCameraToolStripMenuItem,
            this.selectAllEntitiesOfTypeToolStripMenuItem,
            this.dumpContentToolStripMenuItem,
            this.reinitSelectedEntitiesToolStripMenuItem,
            this.setMapSizeToolStripMenuItem,
            this.hacksToolStripMenuItem,
            this.mapSettingsToolStripMenuItem,
            this.setStaticDataSourceMapToolStripMenuItem,
            this.createCameraPointsToolStripMenuItem});
            this.miscToolStripMenuItem.Name = "miscToolStripMenuItem";
            this.miscToolStripMenuItem.Size = new System.Drawing.Size(44, 23);
            this.miscToolStripMenuItem.Text = "Misc";
            // 
            // goToMainCharacterToolStripMenuItem
            // 
            this.goToMainCharacterToolStripMenuItem.Name = "goToMainCharacterToolStripMenuItem";
            this.goToMainCharacterToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.goToMainCharacterToolStripMenuItem.Text = "Go to Main character";
            this.goToMainCharacterToolStripMenuItem.Click += new System.EventHandler(this.goToMainCharacterToolStripMenuItem_Click);
            // 
            // toogleEditorCameraToolStripMenuItem
            // 
            this.toogleEditorCameraToolStripMenuItem.Name = "toogleEditorCameraToolStripMenuItem";
            this.toogleEditorCameraToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.toogleEditorCameraToolStripMenuItem.Text = "Reset camera";
            this.toogleEditorCameraToolStripMenuItem.Click += new System.EventHandler(this.toogleEditorCameraToolStripMenuItem_Click);
            // 
            // selectAllEntitiesOfTypeToolStripMenuItem
            // 
            this.selectAllEntitiesOfTypeToolStripMenuItem.Name = "selectAllEntitiesOfTypeToolStripMenuItem";
            this.selectAllEntitiesOfTypeToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.selectAllEntitiesOfTypeToolStripMenuItem.Text = "Select all entities of type..";
            this.selectAllEntitiesOfTypeToolStripMenuItem.Click += new System.EventHandler(this.selectAllEntitiesOfTypeToolStripMenuItem_Click);
            // 
            // dumpContentToolStripMenuItem
            // 
            this.dumpContentToolStripMenuItem.Name = "dumpContentToolStripMenuItem";
            this.dumpContentToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.dumpContentToolStripMenuItem.Text = "Dump content";
            this.dumpContentToolStripMenuItem.Click += new System.EventHandler(this.dumpContentToolStripMenuItem_Click);
            // 
            // reinitSelectedEntitiesToolStripMenuItem
            // 
            this.reinitSelectedEntitiesToolStripMenuItem.Name = "reinitSelectedEntitiesToolStripMenuItem";
            this.reinitSelectedEntitiesToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.reinitSelectedEntitiesToolStripMenuItem.Text = "Re-init selected entities";
            this.reinitSelectedEntitiesToolStripMenuItem.Click += new System.EventHandler(this.reinitSelectedEntitiesToolStripMenuItem_Click);
            // 
            // setMapSizeToolStripMenuItem
            // 
            this.setMapSizeToolStripMenuItem.Name = "setMapSizeToolStripMenuItem";
            this.setMapSizeToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.setMapSizeToolStripMenuItem.Text = "Set map size";
            this.setMapSizeToolStripMenuItem.Click += new System.EventHandler(this.setMapSizeToolStripMenuItem_Click);
            // 
            // hacksToolStripMenuItem
            // 
            this.hacksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addOrientationToAllObjectsToolStripMenuItem,
            this.resaveAllMapsToolStripMenuItem});
            this.hacksToolStripMenuItem.Name = "hacksToolStripMenuItem";
            this.hacksToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.hacksToolStripMenuItem.Text = "Hacks";
            // 
            // addOrientationToAllObjectsToolStripMenuItem
            // 
            this.addOrientationToAllObjectsToolStripMenuItem.Name = "addOrientationToAllObjectsToolStripMenuItem";
            this.addOrientationToAllObjectsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.addOrientationToAllObjectsToolStripMenuItem.Text = "Add orientation to all objects";
            this.addOrientationToAllObjectsToolStripMenuItem.Click += new System.EventHandler(this.addOrientationToAllObjectsToolStripMenuItem_Click);
            // 
            // resaveAllMapsToolStripMenuItem
            // 
            this.resaveAllMapsToolStripMenuItem.Name = "resaveAllMapsToolStripMenuItem";
            this.resaveAllMapsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.resaveAllMapsToolStripMenuItem.Text = "Resave all maps";
            this.resaveAllMapsToolStripMenuItem.Click += new System.EventHandler(this.resaveAllMapsToolStripMenuItem_Click);
            // 
            // mapSettingsToolStripMenuItem
            // 
            this.mapSettingsToolStripMenuItem.Name = "mapSettingsToolStripMenuItem";
            this.mapSettingsToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.mapSettingsToolStripMenuItem.Text = "Map settings";
            this.mapSettingsToolStripMenuItem.Click += new System.EventHandler(this.mapSettingsToolStripMenuItem_Click);
            // 
            // setStaticDataSourceMapToolStripMenuItem
            // 
            this.setStaticDataSourceMapToolStripMenuItem.Name = "setStaticDataSourceMapToolStripMenuItem";
            this.setStaticDataSourceMapToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.setStaticDataSourceMapToolStripMenuItem.Text = "Set static data source map";
            this.setStaticDataSourceMapToolStripMenuItem.Click += new System.EventHandler(this.setStaticDataSourceMapToolStripMenuItem_Click);
            // 
            // createCameraPointsToolStripMenuItem
            // 
            this.createCameraPointsToolStripMenuItem.Name = "createCameraPointsToolStripMenuItem";
            this.createCameraPointsToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.createCameraPointsToolStripMenuItem.Text = "Create camera points";
            this.createCameraPointsToolStripMenuItem.Click += new System.EventHandler(this.createCameraPointsToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placementBoundingsToolStripMenuItem,
            this.fogToolStripMenuItem,
            this.mapBoundsToolStripMenuItem,
            this.physicsBoundingsToolStripMenuItem,
            this.pickingBoundingsToolStripMenuItem,
            this.aggroRangeToolStripMenuItem,
            this.visualBoundingsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 23);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // placementBoundingsToolStripMenuItem
            // 
            this.placementBoundingsToolStripMenuItem.CheckOnClick = true;
            this.placementBoundingsToolStripMenuItem.Name = "placementBoundingsToolStripMenuItem";
            this.placementBoundingsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.placementBoundingsToolStripMenuItem.Text = "Placement boundings";
            // 
            // fogToolStripMenuItem
            // 
            this.fogToolStripMenuItem.Checked = true;
            this.fogToolStripMenuItem.CheckOnClick = true;
            this.fogToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fogToolStripMenuItem.Name = "fogToolStripMenuItem";
            this.fogToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.fogToolStripMenuItem.Text = "Fog";
            this.fogToolStripMenuItem.CheckedChanged += new System.EventHandler(this.fogToolStripMenuItem_CheckedChanged);
            // 
            // mapBoundsToolStripMenuItem
            // 
            this.mapBoundsToolStripMenuItem.CheckOnClick = true;
            this.mapBoundsToolStripMenuItem.Name = "mapBoundsToolStripMenuItem";
            this.mapBoundsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.mapBoundsToolStripMenuItem.Text = "Map bounds";
            // 
            // physicsBoundingsToolStripMenuItem
            // 
            this.physicsBoundingsToolStripMenuItem.CheckOnClick = true;
            this.physicsBoundingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.physicsBoundingsHideGroundToolStripMenuItem,
            this.showFullChainsToolStripMenuItem});
            this.physicsBoundingsToolStripMenuItem.Name = "physicsBoundingsToolStripMenuItem";
            this.physicsBoundingsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.physicsBoundingsToolStripMenuItem.Text = "Physics boundings";
            // 
            // physicsBoundingsHideGroundToolStripMenuItem
            // 
            this.physicsBoundingsHideGroundToolStripMenuItem.Checked = true;
            this.physicsBoundingsHideGroundToolStripMenuItem.CheckOnClick = true;
            this.physicsBoundingsHideGroundToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.physicsBoundingsHideGroundToolStripMenuItem.Name = "physicsBoundingsHideGroundToolStripMenuItem";
            this.physicsBoundingsHideGroundToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.physicsBoundingsHideGroundToolStripMenuItem.Text = "Hide ground";
            // 
            // showFullChainsToolStripMenuItem
            // 
            this.showFullChainsToolStripMenuItem.CheckOnClick = true;
            this.showFullChainsToolStripMenuItem.Name = "showFullChainsToolStripMenuItem";
            this.showFullChainsToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.showFullChainsToolStripMenuItem.Text = "Show full chains";
            // 
            // pickingBoundingsToolStripMenuItem
            // 
            this.pickingBoundingsToolStripMenuItem.CheckOnClick = true;
            this.pickingBoundingsToolStripMenuItem.Name = "pickingBoundingsToolStripMenuItem";
            this.pickingBoundingsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.pickingBoundingsToolStripMenuItem.Text = "Picking boundings";
            // 
            // aggroRangeToolStripMenuItem
            // 
            this.aggroRangeToolStripMenuItem.CheckOnClick = true;
            this.aggroRangeToolStripMenuItem.Name = "aggroRangeToolStripMenuItem";
            this.aggroRangeToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.aggroRangeToolStripMenuItem.Text = "Aggro range";
            // 
            // visualBoundingsToolStripMenuItem
            // 
            this.visualBoundingsToolStripMenuItem.CheckOnClick = true;
            this.visualBoundingsToolStripMenuItem.Name = "visualBoundingsToolStripMenuItem";
            this.visualBoundingsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.visualBoundingsToolStripMenuItem.Text = "Visual boundings";
            // 
            // scriptEditorToolStripMenuItem
            // 
            this.scriptEditorToolStripMenuItem.Name = "scriptEditorToolStripMenuItem";
            this.scriptEditorToolStripMenuItem.Size = new System.Drawing.Size(83, 23);
            this.scriptEditorToolStripMenuItem.Text = "Script editor";
            this.scriptEditorToolStripMenuItem.Click += new System.EventHandler(this.scriptEditorToolStripMenuItem_Click);
            // 
            // stringEditorToolStripMenuItem
            // 
            this.stringEditorToolStripMenuItem.Name = "stringEditorToolStripMenuItem";
            this.stringEditorToolStripMenuItem.Size = new System.Drawing.Size(84, 23);
            this.stringEditorToolStripMenuItem.Text = "String editor";
            this.stringEditorToolStripMenuItem.Click += new System.EventHandler(this.stringEditorToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.worldView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer5);
            this.splitContainer1.Size = new System.Drawing.Size(1280, 733);
            this.splitContainer1.SplitterDistance = 1058;
            this.splitContainer1.TabIndex = 2;
            // 
            // worldView
            // 
            this.worldView.BackColor = System.Drawing.Color.Black;
            this.worldView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.worldView.FogEnabled = true;
            this.worldView.InputHandler = null;
            this.worldView.Location = new System.Drawing.Point(0, 0);
            this.worldView.Name = "worldView";
            this.worldView.Size = new System.Drawing.Size(1058, 733);
            this.worldView.StateInputHandler = null;
            this.worldView.TabIndex = 0;
            this.worldView.Text = "worldView1";
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.Location = new System.Drawing.Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.stateSettingsPropertyGrid);
            this.splitContainer5.Size = new System.Drawing.Size(218, 733);
            this.splitContainer5.SplitterDistance = 613;
            this.splitContainer5.TabIndex = 1;
            // 
            // stateSettingsPropertyGrid
            // 
            this.stateSettingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateSettingsPropertyGrid.HelpVisible = false;
            this.stateSettingsPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.stateSettingsPropertyGrid.Name = "stateSettingsPropertyGrid";
            this.stateSettingsPropertyGrid.Size = new System.Drawing.Size(218, 116);
            this.stateSettingsPropertyGrid.TabIndex = 0;
            this.stateSettingsPropertyGrid.ToolbarVisible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fpsLabel,
            this.mouseGroundPositionLabel,
            this.tipsLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 760);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1280, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = false;
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(60, 17);
            this.fpsLabel.Text = "- fps";
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mouseGroundPositionLabel
            // 
            this.mouseGroundPositionLabel.AutoSize = false;
            this.mouseGroundPositionLabel.Name = "mouseGroundPositionLabel";
            this.mouseGroundPositionLabel.Size = new System.Drawing.Size(200, 17);
            this.mouseGroundPositionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tipsLabel
            // 
            this.tipsLabel.Name = "tipsLabel";
            this.tipsLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 782);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Leaditor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox modeComboBox;
        private System.Windows.Forms.PropertyGrid stateSettingsPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem miscToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToMainCharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toogleEditorCameraToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel mouseGroundPositionLabel;
        private System.Windows.Forms.ToolStripMenuItem selectAllEntitiesOfTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tipsLabel;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem placementBoundingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fogToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem mapBoundsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem physicsBoundingsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem physicsBoundingsHideGroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpContentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptEditorToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem pickingBoundingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setMapSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hacksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addOrientationToAllObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reinitSelectedEntitiesToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem showFullChainsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem aggroRangeToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem visualBoundingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resaveAllMapsToolStripMenuItem;
        public WorldView worldView;
        private System.Windows.Forms.ToolStripMenuItem setStaticDataSourceMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createCameraPointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stringEditorToolStripMenuItem;
    }
}


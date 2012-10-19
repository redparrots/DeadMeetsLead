namespace Client
{
    partial class DeveloperSettings
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
            Client.DamageVSRageGraphControlSettings damageVSRageGraphControlSettings3 = new Client.DamageVSRageGraphControlSettings();
            Client.DamageVSRageGraphControlAbilitySettings damageVSRageGraphControlAbilitySettings3 = new Client.DamageVSRageGraphControlAbilitySettings();
            this.SettingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.profilePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.forceZIndexRecalcButton = new System.Windows.Forms.Button();
            this.zIndexLabel = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.zIndexTrackBar = new System.Windows.Forms.TrackBar();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.eventLogListBox = new System.Windows.Forms.ListBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.damageVSRageGraphControl1 = new Client.DamageVSRageGraphControl();
            this.weaponGraphPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.updateWeaponGraphButton = new System.Windows.Forms.Button();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.volumesPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.btnVolumesClipboard = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zIndexTrackBar)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // SettingsPropertyGrid
            // 
            this.SettingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.SettingsPropertyGrid.Name = "SettingsPropertyGrid";
            this.SettingsPropertyGrid.Size = new System.Drawing.Size(418, 603);
            this.SettingsPropertyGrid.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(432, 635);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.SettingsPropertyGrid);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(424, 609);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.profilePropertyGrid);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(424, 609);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Profile";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // profilePropertyGrid
            // 
            this.profilePropertyGrid.Location = new System.Drawing.Point(23, 25);
            this.profilePropertyGrid.Name = "profilePropertyGrid";
            this.profilePropertyGrid.Size = new System.Drawing.Size(361, 377);
            this.profilePropertyGrid.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.forceZIndexRecalcButton);
            this.tabPage2.Controls.Add(this.zIndexLabel);
            this.tabPage2.Controls.Add(this.checkBox1);
            this.tabPage2.Controls.Add(this.zIndexTrackBar);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(424, 609);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Misc";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // forceZIndexRecalcButton
            // 
            this.forceZIndexRecalcButton.Location = new System.Drawing.Point(8, 121);
            this.forceZIndexRecalcButton.Name = "forceZIndexRecalcButton";
            this.forceZIndexRecalcButton.Size = new System.Drawing.Size(137, 24);
            this.forceZIndexRecalcButton.TabIndex = 3;
            this.forceZIndexRecalcButton.Text = "Force recalc ZIndex";
            this.forceZIndexRecalcButton.UseVisualStyleBackColor = true;
            this.forceZIndexRecalcButton.Click += new System.EventHandler(this.forceZIndexRecalcButton_Click);
            // 
            // zIndexLabel
            // 
            this.zIndexLabel.AutoSize = true;
            this.zIndexLabel.Location = new System.Drawing.Point(17, 105);
            this.zIndexLabel.Name = "zIndexLabel";
            this.zIndexLabel.Size = new System.Drawing.Size(35, 13);
            this.zIndexLabel.TabIndex = 2;
            this.zIndexLabel.Text = "label1";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(8, 29);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(137, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "ZIndex Interface Limiter";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // zIndexTrackBar
            // 
            this.zIndexTrackBar.Location = new System.Drawing.Point(8, 52);
            this.zIndexTrackBar.Maximum = 1000;
            this.zIndexTrackBar.Minimum = -100;
            this.zIndexTrackBar.Name = "zIndexTrackBar";
            this.zIndexTrackBar.Size = new System.Drawing.Size(408, 45);
            this.zIndexTrackBar.TabIndex = 0;
            this.zIndexTrackBar.TickFrequency = 100;
            this.zIndexTrackBar.Scroll += new System.EventHandler(this.zIndexTrackBar_Scroll);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.eventLogListBox);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(424, 609);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Event log";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // eventLogListBox
            // 
            this.eventLogListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventLogListBox.FormattingEnabled = true;
            this.eventLogListBox.Location = new System.Drawing.Point(3, 3);
            this.eventLogListBox.Name = "eventLogListBox";
            this.eventLogListBox.Size = new System.Drawing.Size(418, 602);
            this.eventLogListBox.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.damageVSRageGraphControl1);
            this.tabPage5.Controls.Add(this.weaponGraphPropertyGrid);
            this.tabPage5.Controls.Add(this.updateWeaponGraphButton);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(424, 609);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "WeaponDamageRageGraph";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // damageVSRageGraphControl1
            // 
            this.damageVSRageGraphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.damageVSRageGraphControl1.Location = new System.Drawing.Point(0, 0);
            this.damageVSRageGraphControl1.Name = "damageVSRageGraphControl1";
            damageVSRageGraphControlAbilitySettings3.Ability = typeof(Client.Game.Map.Units.SwordThrust);
            damageVSRageGraphControlAbilitySettings3.AbilityAttacksPerSec = System.Drawing.Color.Blue;
            damageVSRageGraphControlAbilitySettings3.AbilityCritsPerSec = System.Drawing.Color.Green;
            damageVSRageGraphControlAbilitySettings3.AbilityDamagePerHit = System.Drawing.Color.Yellow;
            damageVSRageGraphControlAbilitySettings3.AbilityDPS = System.Drawing.Color.Red;
            damageVSRageGraphControlAbilitySettings3.AbilityRageTimeToNextLevel = System.Drawing.Color.Turquoise;
            damageVSRageGraphControlSettings3.Abilities = new Client.DamageVSRageGraphControlAbilitySettings[] {
        damageVSRageGraphControlAbilitySettings3};
            damageVSRageGraphControlSettings3.NRageLevels = 13;
            damageVSRageGraphControlSettings3.Preset = Client.DamageVSRageGraphPresets.None;
            this.damageVSRageGraphControl1.Settings = damageVSRageGraphControlSettings3;
            this.damageVSRageGraphControl1.Size = new System.Drawing.Size(424, 377);
            this.damageVSRageGraphControl1.TabIndex = 0;
            // 
            // weaponGraphPropertyGrid
            // 
            this.weaponGraphPropertyGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.weaponGraphPropertyGrid.Location = new System.Drawing.Point(0, 377);
            this.weaponGraphPropertyGrid.Name = "weaponGraphPropertyGrid";
            this.weaponGraphPropertyGrid.Size = new System.Drawing.Size(424, 205);
            this.weaponGraphPropertyGrid.TabIndex = 1;
            // 
            // updateWeaponGraphButton
            // 
            this.updateWeaponGraphButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.updateWeaponGraphButton.Location = new System.Drawing.Point(0, 582);
            this.updateWeaponGraphButton.Name = "updateWeaponGraphButton";
            this.updateWeaponGraphButton.Size = new System.Drawing.Size(424, 27);
            this.updateWeaponGraphButton.TabIndex = 2;
            this.updateWeaponGraphButton.Text = "Update";
            this.updateWeaponGraphButton.UseVisualStyleBackColor = true;
            this.updateWeaponGraphButton.Click += new System.EventHandler(this.updateWeaponGraphButton_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.btnVolumesClipboard);
            this.tabPage6.Controls.Add(this.volumesPropertyGrid);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(424, 609);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Volumes";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // volumesPropertyGrid
            // 
            this.volumesPropertyGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.volumesPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.volumesPropertyGrid.Name = "volumesPropertyGrid";
            this.volumesPropertyGrid.Size = new System.Drawing.Size(424, 565);
            this.volumesPropertyGrid.TabIndex = 0;
            // 
            // btnVolumesClipboard
            // 
            this.btnVolumesClipboard.Location = new System.Drawing.Point(8, 578);
            this.btnVolumesClipboard.Name = "btnVolumesClipboard";
            this.btnVolumesClipboard.Size = new System.Drawing.Size(161, 23);
            this.btnVolumesClipboard.TabIndex = 1;
            this.btnVolumesClipboard.Text = "Copy changes to clipboard";
            this.btnVolumesClipboard.UseVisualStyleBackColor = true;
            this.btnVolumesClipboard.Click += new System.EventHandler(this.btnVolumesClipboard_Click);
            // 
            // DeveloperSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 635);
            this.Controls.Add(this.tabControl1);
            this.Name = "DeveloperSettings";
            this.Text = "DeveloperSettings";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zIndexTrackBar)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TrackBar zIndexTrackBar;
        public System.Windows.Forms.PropertyGrid SettingsPropertyGrid;
        private System.Windows.Forms.Label zIndexLabel;
        private System.Windows.Forms.Button forceZIndexRecalcButton;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.PropertyGrid profilePropertyGrid;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ListBox eventLogListBox;
        private System.Windows.Forms.TabPage tabPage5;
        private DamageVSRageGraphControl damageVSRageGraphControl1;
        private System.Windows.Forms.PropertyGrid weaponGraphPropertyGrid;
        private System.Windows.Forms.Button updateWeaponGraphButton;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.PropertyGrid volumesPropertyGrid;
        private System.Windows.Forms.Button btnVolumesClipboard;
    }
}
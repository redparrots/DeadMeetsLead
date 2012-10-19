namespace Editor
{
    partial class RegionsEditorControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.regionsListBox = new System.Windows.Forms.ListBox();
            this.newButton = new System.Windows.Forms.Button();
            this.newNameTextBox = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.removeButton = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.selectedRegionPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // regionsListBox
            // 
            this.regionsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.regionsListBox.FormattingEnabled = true;
            this.regionsListBox.Location = new System.Drawing.Point(0, 0);
            this.regionsListBox.Name = "regionsListBox";
            this.regionsListBox.Size = new System.Drawing.Size(295, 316);
            this.regionsListBox.TabIndex = 0;
            this.regionsListBox.SelectedIndexChanged += new System.EventHandler(this.regionsListBox_SelectedIndexChanged);
            // 
            // newButton
            // 
            this.newButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.newButton.Location = new System.Drawing.Point(162, 0);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(71, 25);
            this.newButton.TabIndex = 1;
            this.newButton.Text = "New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // newNameTextBox
            // 
            this.newNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newNameTextBox.Location = new System.Drawing.Point(0, 0);
            this.newNameTextBox.Name = "newNameTextBox";
            this.newNameTextBox.Size = new System.Drawing.Size(295, 20);
            this.newNameTextBox.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.regionsListBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.newButton);
            this.splitContainer1.Panel2.Controls.Add(this.removeButton);
            this.splitContainer1.Panel2.Controls.Add(this.newNameTextBox);
            this.splitContainer1.Size = new System.Drawing.Size(295, 356);
            this.splitContainer1.SplitterDistance = 327;
            this.splitContainer1.TabIndex = 3;
            // 
            // removeButton
            // 
            this.removeButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.removeButton.Location = new System.Drawing.Point(233, 0);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(62, 25);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.selectedRegionPropertyGrid);
            this.splitContainer2.Size = new System.Drawing.Size(295, 569);
            this.splitContainer2.SplitterDistance = 356;
            this.splitContainer2.TabIndex = 4;
            // 
            // selectedRegionPropertyGrid
            // 
            this.selectedRegionPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectedRegionPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.selectedRegionPropertyGrid.Name = "selectedRegionPropertyGrid";
            this.selectedRegionPropertyGrid.Size = new System.Drawing.Size(295, 209);
            this.selectedRegionPropertyGrid.TabIndex = 0;
            // 
            // RegionsEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer2);
            this.Name = "RegionsEditorControl";
            this.Size = new System.Drawing.Size(295, 569);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox regionsListBox;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.TextBox newNameTextBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PropertyGrid selectedRegionPropertyGrid;
        private System.Windows.Forms.Button removeButton;
    }
}

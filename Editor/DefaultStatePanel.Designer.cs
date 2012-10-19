namespace Editor
{
    partial class DefaultStatePanel
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.selectedEntityPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.selectedEntityLabel = new System.Windows.Forms.Label();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.selectedEntityPropertyGrid);
            this.splitContainer1.Panel2.Controls.Add(this.selectedEntityLabel);
            this.splitContainer1.Size = new System.Drawing.Size(372, 456);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.TabIndex = 0;
            // 
            // selectedEntityPropertyGrid
            // 
            this.selectedEntityPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectedEntityPropertyGrid.Location = new System.Drawing.Point(0, 20);
            this.selectedEntityPropertyGrid.Name = "selectedEntityPropertyGrid";
            this.selectedEntityPropertyGrid.Size = new System.Drawing.Size(372, 172);
            this.selectedEntityPropertyGrid.TabIndex = 0;
            // 
            // selectedEntityLabel
            // 
            this.selectedEntityLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.selectedEntityLabel.Location = new System.Drawing.Point(0, 0);
            this.selectedEntityLabel.Name = "selectedEntityLabel";
            this.selectedEntityLabel.Size = new System.Drawing.Size(372, 20);
            this.selectedEntityLabel.TabIndex = 1;
            this.selectedEntityLabel.Text = " - ";
            // 
            // DefaultStatePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DefaultStatePanel";
            this.Size = new System.Drawing.Size(372, 456);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid selectedEntityPropertyGrid;
        private System.Windows.Forms.Label selectedEntityLabel;
    }
}

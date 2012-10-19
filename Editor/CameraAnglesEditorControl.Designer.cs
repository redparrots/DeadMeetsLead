namespace Editor
{
    partial class CameraAnglesEditorControl
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
            this.cameraAnglesListBox = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.addButton = new System.Windows.Forms.Button();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.useButton = new System.Windows.Forms.Button();
            this.setButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cameraAnglesListBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(217, 422);
            this.splitContainer1.SplitterDistance = 358;
            this.splitContainer1.TabIndex = 0;
            // 
            // cameraAnglesListBox
            // 
            this.cameraAnglesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraAnglesListBox.FormattingEnabled = true;
            this.cameraAnglesListBox.Location = new System.Drawing.Point(0, 0);
            this.cameraAnglesListBox.Name = "cameraAnglesListBox";
            this.cameraAnglesListBox.Size = new System.Drawing.Size(217, 355);
            this.cameraAnglesListBox.TabIndex = 0;
            this.cameraAnglesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.cameraAnglesListBox_MouseDoubleClick);
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
            this.splitContainer2.Panel1.Controls.Add(this.addButton);
            this.splitContainer2.Panel1.Controls.Add(this.nameTextBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.useButton);
            this.splitContainer2.Panel2.Controls.Add(this.setButton);
            this.splitContainer2.Panel2.Controls.Add(this.removeButton);
            this.splitContainer2.Size = new System.Drawing.Size(217, 60);
            this.splitContainer2.SplitterDistance = 25;
            this.splitContainer2.TabIndex = 5;
            // 
            // addButton
            // 
            this.addButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addButton.Location = new System.Drawing.Point(157, 0);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(60, 25);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.nameTextBox.Location = new System.Drawing.Point(0, 0);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(157, 20);
            this.nameTextBox.TabIndex = 0;
            // 
            // useButton
            // 
            this.useButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.useButton.Location = new System.Drawing.Point(132, 0);
            this.useButton.Name = "useButton";
            this.useButton.Size = new System.Drawing.Size(85, 31);
            this.useButton.TabIndex = 4;
            this.useButton.Text = "Use";
            this.useButton.UseVisualStyleBackColor = true;
            this.useButton.Click += new System.EventHandler(this.useButton_Click);
            // 
            // setButton
            // 
            this.setButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.setButton.Location = new System.Drawing.Point(65, 0);
            this.setButton.Name = "setButton";
            this.setButton.Size = new System.Drawing.Size(67, 31);
            this.setButton.TabIndex = 2;
            this.setButton.Text = "Set";
            this.setButton.UseVisualStyleBackColor = true;
            this.setButton.Click += new System.EventHandler(this.setButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.removeButton.Location = new System.Drawing.Point(0, 0);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(65, 31);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // CameraAnglesEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CameraAnglesEditorControl";
            this.Size = new System.Drawing.Size(217, 422);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox cameraAnglesListBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button setButton;
        private System.Windows.Forms.Button useButton;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}

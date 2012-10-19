namespace Editor
{
    partial class ScriptsEditor
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
            this.scriptsListBox = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.scriptPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.downButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pasteButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.cutButton = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // scriptsListBox
            // 
            this.scriptsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptsListBox.FormattingEnabled = true;
            this.scriptsListBox.Location = new System.Drawing.Point(0, 0);
            this.scriptsListBox.Name = "scriptsListBox";
            this.scriptsListBox.Size = new System.Drawing.Size(249, 602);
            this.scriptsListBox.TabIndex = 0;
            this.scriptsListBox.SelectedIndexChanged += new System.EventHandler(this.scriptsListBox_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.addButton.Location = new System.Drawing.Point(0, 0);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(73, 63);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.removeButton.Location = new System.Drawing.Point(73, 0);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(55, 63);
            this.removeButton.TabIndex = 2;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // scriptPropertyGrid
            // 
            this.scriptPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.scriptPropertyGrid.Name = "scriptPropertyGrid";
            this.scriptPropertyGrid.Size = new System.Drawing.Size(498, 681);
            this.scriptPropertyGrid.TabIndex = 3;
            this.scriptPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.scriptPropertyGrid_PropertyValueChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.scriptPropertyGrid);
            this.splitContainer1.Size = new System.Drawing.Size(751, 681);
            this.splitContainer1.SplitterDistance = 249;
            this.splitContainer1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.scriptsListBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.downButton);
            this.splitContainer2.Panel2.Controls.Add(this.upButton);
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Panel2.Controls.Add(this.removeButton);
            this.splitContainer2.Panel2.Controls.Add(this.addButton);
            this.splitContainer2.Size = new System.Drawing.Size(249, 681);
            this.splitContainer2.SplitterDistance = 614;
            this.splitContainer2.TabIndex = 0;
            // 
            // downButton
            // 
            this.downButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downButton.Location = new System.Drawing.Point(184, 22);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(65, 41);
            this.downButton.TabIndex = 5;
            this.downButton.Text = "Down";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // upButton
            // 
            this.upButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.upButton.Location = new System.Drawing.Point(184, 0);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(65, 22);
            this.upButton.TabIndex = 4;
            this.upButton.Text = "Up";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pasteButton);
            this.panel1.Controls.Add(this.copyButton);
            this.panel1.Controls.Add(this.cutButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(128, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(56, 63);
            this.panel1.TabIndex = 6;
            // 
            // pasteButton
            // 
            this.pasteButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pasteButton.Location = new System.Drawing.Point(0, 42);
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(56, 21);
            this.pasteButton.TabIndex = 2;
            this.pasteButton.Text = "Paste";
            this.pasteButton.UseVisualStyleBackColor = true;
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.copyButton.Location = new System.Drawing.Point(0, 22);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(56, 20);
            this.copyButton.TabIndex = 1;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // cutButton
            // 
            this.cutButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.cutButton.Location = new System.Drawing.Point(0, 0);
            this.cutButton.Name = "cutButton";
            this.cutButton.Size = new System.Drawing.Size(56, 22);
            this.cutButton.TabIndex = 0;
            this.cutButton.Text = "Cut";
            this.cutButton.UseVisualStyleBackColor = true;
            this.cutButton.Click += new System.EventHandler(this.cutButton_Click);
            // 
            // ScriptsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ScriptsEditor";
            this.Size = new System.Drawing.Size(751, 681);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox scriptsListBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.PropertyGrid scriptPropertyGrid;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button pasteButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Button cutButton;
    }
}

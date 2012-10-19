namespace LayoutEngineTest
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
            LayoutEngineTest.Layoutable layoutable4 = new LayoutEngineTest.Layoutable();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            Graphics.ForwardLayoutEngine forwardLayoutEngine4 = new Graphics.ForwardLayoutEngine();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.testLayoutEditorControl1 = new LayoutEngineTest.TestLayoutEditorControl();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.doLayoutButton = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.testLayoutEditorControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Panel2.Controls.Add(this.doLayoutButton);
            this.splitContainer1.Size = new System.Drawing.Size(863, 507);
            this.splitContainer1.SplitterDistance = 632;
            this.splitContainer1.TabIndex = 0;
            // 
            // testLayoutEditorControl1
            // 
            this.testLayoutEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testLayoutEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.testLayoutEditorControl1.Name = "testLayoutEditorControl1";
            layoutable4.Color = System.Drawing.Color.Empty;
            layoutable4.Dock = System.Windows.Forms.DockStyle.None;
            layoutable4.LayoutEngine = forwardLayoutEngine4;
            layoutable4.Margin = new System.Windows.Forms.Padding(0);
            layoutable4.Position = new SlimDX.Vector2(0F, 0F);
            layoutable4.Size = new SlimDX.Vector2(632F, 507F);
            this.testLayoutEditorControl1.Root = layoutable4;
            this.testLayoutEditorControl1.Selected = null;
            this.testLayoutEditorControl1.Size = new System.Drawing.Size(632, 507);
            this.testLayoutEditorControl1.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(227, 467);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // doLayoutButton
            // 
            this.doLayoutButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.doLayoutButton.Location = new System.Drawing.Point(0, 467);
            this.doLayoutButton.Name = "doLayoutButton";
            this.doLayoutButton.Size = new System.Drawing.Size(227, 40);
            this.doLayoutButton.TabIndex = 2;
            this.doLayoutButton.Text = "Do layout";
            this.doLayoutButton.UseVisualStyleBackColor = true;
            this.doLayoutButton.Click += new System.EventHandler(this.doLayoutButton_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 507);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private TestLayoutEditorControl testLayoutEditorControl1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button doLayoutButton;
    }
}


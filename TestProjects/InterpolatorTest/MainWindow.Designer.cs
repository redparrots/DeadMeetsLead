namespace InterpolatorTest
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
            this.interpolatorVisualization1 = new InterpolatorTest.InterpolatorVisualization();
            this.SuspendLayout();
            // 
            // interpolatorVisualization1
            // 
            this.interpolatorVisualization1.GraphScale = new System.Drawing.SizeF(30F, 30F);
            this.interpolatorVisualization1.Interpolator = null;
            this.interpolatorVisualization1.Location = new System.Drawing.Point(24, 37);
            this.interpolatorVisualization1.Name = "interpolatorVisualization1";
            this.interpolatorVisualization1.Size = new System.Drawing.Size(543, 460);
            this.interpolatorVisualization1.TabIndex = 0;
            this.interpolatorVisualization1.Text = "interpolatorVisualization1";
            this.interpolatorVisualization1.UpdateStepSize = 1F;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 534);
            this.Controls.Add(this.interpolatorVisualization1);
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private InterpolatorVisualization interpolatorVisualization1;
    }
}


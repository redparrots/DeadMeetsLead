namespace SetupEmptyResXFile
{
    partial class Form1
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
            this.tbExtraLanguages = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.lblResults = new System.Windows.Forms.Label();
            this.lbMainFile = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // tbExtraLanguages
            // 
            this.tbExtraLanguages.Location = new System.Drawing.Point(306, 29);
            this.tbExtraLanguages.Multiline = true;
            this.tbExtraLanguages.Name = "tbExtraLanguages";
            this.tbExtraLanguages.Size = new System.Drawing.Size(100, 138);
            this.tbExtraLanguages.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Main resx-file";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(303, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Extra languages";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(306, 253);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(100, 28);
            this.btnGo.TabIndex = 4;
            this.btnGo.Text = "Go!";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new System.Drawing.Point(118, 118);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(0, 13);
            this.lblResults.TabIndex = 5;
            // 
            // lbMainFile
            // 
            this.lbMainFile.AllowDrop = true;
            this.lbMainFile.FormattingEnabled = true;
            this.lbMainFile.Location = new System.Drawing.Point(12, 29);
            this.lbMainFile.Name = "lbMainFile";
            this.lbMainFile.Size = new System.Drawing.Size(288, 251);
            this.lbMainFile.TabIndex = 6;
            this.lbMainFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbMainFile_DragDrop);
            this.lbMainFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.lbMainFile_DragEnter);
            this.lbMainFile.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbMainFile_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 288);
            this.Controls.Add(this.lbMainFile);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbExtraLanguages);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbExtraLanguages;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.ListBox lbMainFile;
    }
}


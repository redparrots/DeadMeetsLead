namespace Client
{
    partial class UpdateDownloader
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnAbort = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblDownloadText = new System.Windows.Forms.Label();
            this.btnInstall = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 71);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(351, 23);
            this.progressBar.TabIndex = 0;
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(288, 100);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 23);
            this.btnAbort.TabIndex = 1;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(12, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(205, 20);
            this.lblHeader.TabIndex = 2;
            this.lblHeader.Text = "Updating Dead Meets Lead";
            // 
            // lblDownloadText
            // 
            this.lblDownloadText.AutoSize = true;
            this.lblDownloadText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDownloadText.Location = new System.Drawing.Point(13, 39);
            this.lblDownloadText.Name = "lblDownloadText";
            this.lblDownloadText.Size = new System.Drawing.Size(102, 18);
            this.lblDownloadText.TabIndex = 3;
            this.lblDownloadText.Text = "Downloading: ";
            // 
            // btnInstall
            // 
            this.btnInstall.Enabled = false;
            this.btnInstall.Location = new System.Drawing.Point(207, 100);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 4;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // UpdateDownloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 130);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.lblDownloadText);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.progressBar);
            this.Name = "UpdateDownloader";
            this.Text = "Dead Meets Lead Updater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblDownloadText;
        private System.Windows.Forms.Button btnInstall;
    }
}
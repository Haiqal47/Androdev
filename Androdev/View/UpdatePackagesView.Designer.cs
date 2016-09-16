namespace Androdev.View
{
    partial class UpdatePackagesView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdatePackagesView));
            this.label1 = new System.Windows.Forms.Label();
            this.lblDownloadFileName = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.lblDownloadSize = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblDownloaded = new System.Windows.Forms.Label();
            this.lblQueue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Downloading:";
            // 
            // lblDownloadFileName
            // 
            this.lblDownloadFileName.AutoSize = true;
            this.lblDownloadFileName.Location = new System.Drawing.Point(85, 11);
            this.lblDownloadFileName.Name = "lblDownloadFileName";
            this.lblDownloadFileName.Size = new System.Drawing.Size(16, 13);
            this.lblDownloadFileName.TabIndex = 1;
            this.lblDownloadFileName.Text = "...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Progress:";
            // 
            // prgProgress
            // 
            this.prgProgress.Location = new System.Drawing.Point(15, 47);
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(337, 14);
            this.prgProgress.TabIndex = 3;
            // 
            // cmdCancel
            // 
            this.cmdCancel.ImageIndex = 0;
            this.cmdCancel.ImageList = this.imageList1;
            this.cmdCancel.Location = new System.Drawing.Point(230, 75);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(122, 27);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "Cancel download";
            this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "cancel.png");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Download size:";
            // 
            // lblDownloadSize
            // 
            this.lblDownloadSize.AutoSize = true;
            this.lblDownloadSize.Location = new System.Drawing.Point(92, 72);
            this.lblDownloadSize.Name = "lblDownloadSize";
            this.lblDownloadSize.Size = new System.Drawing.Size(32, 13);
            this.lblDownloadSize.TabIndex = 6;
            this.lblDownloadSize.Text = "0 MB";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Downloaded:";
            // 
            // lblDownloaded
            // 
            this.lblDownloaded.AutoSize = true;
            this.lblDownloaded.Location = new System.Drawing.Point(92, 89);
            this.lblDownloaded.Name = "lblDownloaded";
            this.lblDownloaded.Size = new System.Drawing.Size(32, 13);
            this.lblDownloaded.TabIndex = 8;
            this.lblDownloaded.Text = "0 MB";
            // 
            // lblQueue
            // 
            this.lblQueue.AutoSize = true;
            this.lblQueue.Location = new System.Drawing.Point(318, 31);
            this.lblQueue.Name = "lblQueue";
            this.lblQueue.Size = new System.Drawing.Size(34, 13);
            this.lblQueue.TabIndex = 9;
            this.lblQueue.Text = "0 of 0";
            // 
            // UpdatePackagesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 114);
            this.Controls.Add(this.lblQueue);
            this.Controls.Add(this.lblDownloaded);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblDownloadSize);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.prgProgress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblDownloadFileName);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdatePackagesView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update Components";
            this.Load += new System.EventHandler(this.FrmUpdate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDownloadFileName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar prgProgress;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblDownloadSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblDownloaded;
        private System.Windows.Forms.Label lblQueue;
    }
}
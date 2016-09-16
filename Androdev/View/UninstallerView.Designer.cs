namespace Androdev.View
{
    partial class UninstallerView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UninstallerView));
            this.label1 = new System.Windows.Forms.Label();
            this.cboDrive = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblProcessedFile = new System.Windows.Forms.Label();
            this.cmdInstallationCleaner = new System.Windows.Forms.Button();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Androdev installation drive:";
            // 
            // cboDrive
            // 
            this.cboDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDrive.FormattingEnabled = true;
            this.cboDrive.Location = new System.Drawing.Point(152, 12);
            this.cboDrive.Name = "cboDrive";
            this.cboDrive.Size = new System.Drawing.Size(143, 21);
            this.cboDrive.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Delete file:";
            // 
            // lblProcessedFile
            // 
            this.lblProcessedFile.AutoSize = true;
            this.lblProcessedFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessedFile.Location = new System.Drawing.Point(12, 60);
            this.lblProcessedFile.Name = "lblProcessedFile";
            this.lblProcessedFile.Size = new System.Drawing.Size(21, 20);
            this.lblProcessedFile.TabIndex = 3;
            this.lblProcessedFile.Text = "...";
            // 
            // cmdInstallationCleaner
            // 
            this.cmdInstallationCleaner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdInstallationCleaner.ImageIndex = 0;
            this.cmdInstallationCleaner.ImageList = this.imgList;
            this.cmdInstallationCleaner.Location = new System.Drawing.Point(96, 89);
            this.cmdInstallationCleaner.Name = "cmdInstallationCleaner";
            this.cmdInstallationCleaner.Size = new System.Drawing.Size(122, 27);
            this.cmdInstallationCleaner.TabIndex = 19;
            this.cmdInstallationCleaner.Text = "Clean installation";
            this.cmdInstallationCleaner.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdInstallationCleaner.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdInstallationCleaner.UseVisualStyleBackColor = true;
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "full_trash-48.png");
            // 
            // UninstallerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 128);
            this.Controls.Add(this.cmdInstallationCleaner);
            this.Controls.Add(this.lblProcessedFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboDrive);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UninstallerView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Uninstall Androdev";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDrive;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblProcessedFile;
        private System.Windows.Forms.Button cmdInstallationCleaner;
        private System.Windows.Forms.ImageList imgList;
    }
}
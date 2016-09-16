namespace Androdev.View.Dialogs
{
    partial class InstallConfigDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallConfigDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.cboInstallDir = new System.Windows.Forms.ComboBox();
            this.chkUAC = new System.Windows.Forms.CheckBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.lnkUac = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Install Androdev to:";
            // 
            // cboInstallDir
            // 
            this.cboInstallDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInstallDir.FormattingEnabled = true;
            this.cboInstallDir.Location = new System.Drawing.Point(136, 20);
            this.cboInstallDir.Name = "cboInstallDir";
            this.cboInstallDir.Size = new System.Drawing.Size(121, 21);
            this.cboInstallDir.TabIndex = 1;
            this.cboInstallDir.SelectedIndexChanged += new System.EventHandler(this.cboInstallDir_SelectedIndexChanged);
            // 
            // chkUAC
            // 
            this.chkUAC.AutoSize = true;
            this.chkUAC.Location = new System.Drawing.Point(100, 57);
            this.chkUAC.Name = "chkUAC";
            this.chkUAC.Size = new System.Drawing.Size(15, 14);
            this.chkUAC.TabIndex = 2;
            this.chkUAC.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(111, 99);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // lnkUac
            // 
            this.lnkUac.AutoSize = true;
            this.lnkUac.Location = new System.Drawing.Point(116, 56);
            this.lnkUac.Name = "lnkUac";
            this.lnkUac.Size = new System.Drawing.Size(89, 13);
            this.lnkUac.TabIndex = 4;
            this.lnkUac.TabStop = true;
            this.lnkUac.Text = "UAC compatibility";
            this.lnkUac.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUac_LinkClicked);
            // 
            // FrmInstallConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 134);
            this.Controls.Add(this.lnkUac);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.chkUAC);
            this.Controls.Add(this.cboInstallDir);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmInstallConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Installation";
            this.Shown += new System.EventHandler(this.FrmInstallConfig_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboInstallDir;
        private System.Windows.Forms.CheckBox chkUAC;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.LinkLabel lnkUac;
    }
}
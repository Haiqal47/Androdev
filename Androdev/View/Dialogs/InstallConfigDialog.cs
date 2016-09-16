using System;
using System.IO;
using System.Windows.Forms;
using Androdev.Core;
using Androdev.Core.IO;
using Androdev.Localization;

namespace Androdev.View.Dialogs
{
    public partial class InstallConfigDialog : Form
    {
        // Constructor
        public InstallConfigDialog()
        {
            InitializeComponent();
        }

        #region Methods
        private static int FindDriveIndex(string name, DriveInfo[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Name == name)
                {
                    return i;
                }
            }
            return 0;
        }
        #endregion

        #region Properties
        public bool UacCompatibility { get; set; }

        public string InstallRoot { get; set; }
        #endregion

        #region Event Subscriber
        private void lnkUac_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(TextResource.UacHelpText, TextResource.UacHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cboInstallDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkUAC.Checked = cboInstallDir.Text.StartsWith("C");
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            UacCompatibility = chkUAC.Checked;
            InstallRoot = cboInstallDir.Text;
            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FrmInstallConfig_Shown(object sender, EventArgs e)
        {
            var dataSource = FastIO.GetAvailiableDrives();

            chkUAC.Checked = UacCompatibility;
            cboInstallDir.DataSource = dataSource;
            if (InstallRoot != null)
            {
                cboInstallDir.SelectedIndex = FindDriveIndex(InstallRoot, dataSource);
            }
        }
        #endregion

    }
}

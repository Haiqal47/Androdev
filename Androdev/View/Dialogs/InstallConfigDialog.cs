//     This file is part of Androdev.
// 
//     Androdev is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Androdev is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with Androdev.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.IO;
using System.Windows.Forms;
using Androdev.Core;
using Androdev.Core.Installer;
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
            var dataSource = FastIo.GetAvailiableDrives();

            chkUAC.Checked = UacCompatibility;
            cboInstallDir.DataSource = dataSource;
            cboInstallDir.SelectedIndex = InstallationHelpers.FindAndrodevInstallation(dataSource);
        }
        #endregion

    }
}

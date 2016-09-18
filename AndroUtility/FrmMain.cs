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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Androdev.Core.Diagostic;
using Androdev.Core.IO;

namespace AndroUtility
{
    public partial class FrmMain : Form
    {
        private string _androidSdkPath;

        public FrmMain()
        {
            InitializeComponent();

            FindAndroidSdk();
        }
        

        #region BackgroundWorker
        private void bwReader_DoWork(object sender, DoWorkEventArgs e)
        {
            var pattern = new Regex(@"(?<date>\w{15})\t(?<level>.*)\t(?<type>\[.*\])(?<method>\[.*\]) (?<message>.*)", RegexOptions.Compiled);
            using (var sr = new StreamReader(e.Argument.ToString()))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var result = pattern.Match(line);
                    if (!result.Success) break;

                    // add date entry
                    var item = new ListViewItem(result.Groups["date"].Value)
                    {
                        UseItemStyleForSubItems = false
                    };

                    // add level entry
                    var level = result.Groups["level"].Value.Trim().Trim('[', ']'); 
                    item.SubItems.Add(level);
                    item.SubItems[1].ForeColor = LogLevelToColor(level);

                    // add class type name
                    item.SubItems.Add(result.Groups["type"].Value.Trim('[', ']'));

                    // add method name
                    item.SubItems.Add(result.Groups["method"].Value.Trim('[', ']'));

                    // add message
                    item.SubItems.Add(result.Groups["message"].Value);

                    // add to listview
                    listViewEx1.SafeAddItem(item);
                }
            }
        }

        private void bwReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatus.Text = "Ready.";
            cmdOpenLog.Enabled = true;
            cmdSend.Enabled = true;
        }
        #endregion

        #region Methods
        private string ToReadableDate(string isoDateTime)
        {
            var sb = new StringBuilder();
            sb.Append(isoDateTime.Substring(6, 2));
            sb.Append("/");
            sb.Append(isoDateTime.Substring(4, 2));
            sb.Append("/");
            sb.Append(isoDateTime.Substring(0, 4));
            sb.Append("/");

            sb.Append(isoDateTime.Substring(9, 2));
            sb.Append(":");
            sb.Append(isoDateTime.Substring(11, 2));
            sb.Append(":");
            sb.Append(isoDateTime.Substring(13, 2));

            return sb.ToString();
        }

        private void FindAndroidSdk()
        {
            var drives = FastIo.GetAvailiableDrives();
            for (int i = 0; i < drives.Length; i++)
            {
                var currentDrive = drives[i].Name;
                if (!InstallationHelpers.IsAndrodevDirectoryExist(currentDrive)) continue;

                _androidSdkPath = Path.Combine(currentDrive, "Androdev\\android-sdk");
                if (Directory.Exists(_androidSdkPath))
                {
                    break;
                }
            }
        }

        private Color LogLevelToColor(string level)
        {
            switch (level)
            {
                case "DEBUG":
                    return Color.Black;
                case "WARNING":
                    return Color.Yellow;
                case "INFO":
                    return Color.Blue;
                case "ERROR":
                    return Color.Red;
                default:
                    return Color.Black;
            }
        }
        #endregion

        #region Event Handler
        private void cmdOpenLog_Click(object sender, EventArgs e)
        {
            using (var openDlg = new OpenFileDialog())
            {
                openDlg.Filter = "Log file|*.txt";
                if (openDlg.ShowDialog() == DialogResult.OK)
                {
                    bwReader.RunWorkerAsync(openDlg.FileName);

                    lblStatus.Text = "Reading log...";
                    cmdOpenLog.Enabled = false;
                    cmdSend.Enabled = false;
                }
            }
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Currently automated log sending is not availiable.\n" +
                "You can send this log file to developer's email address at (fahminlb33@gmail.com). " +
                "Glad if you want to tell us about our application!",
                "Send log file.", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void cmdSdkManager_Click(object sender, EventArgs e)
        {
            if (_androidSdkPath == null)
            {
                MessageBox.Show(
                    "Existing Androdev installation cannot be found. " +
                    "Ensure you have installed Androdev on your system.",
                    "Androdev not found.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var sdkPath = Path.Combine(_androidSdkPath, "SDK Manager.exe");
            if (!File.Exists(sdkPath)) return;
            Process.Start(sdkPath);
        }

        private void cmdAdbTerminal_Click(object sender, EventArgs e)
        {
            if (_androidSdkPath == null)
            {
                MessageBox.Show(
                    "Existing Androdev installation cannot be found. " +
                    "Ensure you have installed Androdev on your system.",
                    "Androdev not found.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var adbPath = Path.Combine(_androidSdkPath, "platform-tools");
            if (!Directory.Exists(adbPath)) return;
            Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                WorkingDirectory = adbPath,
            });
        }

        private void cmdAbout_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmAbout())
            {
                frm.ShowDialog();
            }
        }

        private void listViewEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selItem = listViewEx1.FocusedItem;
            if (selItem == null) return;

            lblDate.Text = ToReadableDate(selItem.SubItems[0].Text);
            lblLevel.Text = selItem.SubItems[1].Text;
            lblLevel.ForeColor = selItem.SubItems[1].ForeColor;
            lblDeclaringType.Text = selItem.SubItems[2].Text;
            lblCallingMethod.Text = selItem.SubItems[3].Text;
            txtMessage.Text = selItem.SubItems[4].Text;
        }
        #endregion

    }
}

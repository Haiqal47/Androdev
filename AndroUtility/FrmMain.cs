using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AndroUtility
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
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
            MessageBox.Show("Currently automated log send if not availiable.\n" +
                "You can send this log file to developer's email address at (fahminlb33@gmail.com). " +
                "Glad if you want to tell us about our application!",
                "Send log file.", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cmdEnvironVar_Click(object sender, EventArgs e)
        {

        }

        private void cmdSdkManager_Click(object sender, EventArgs e)
        {

        }

        private void cmdAdbTerminal_Click(object sender, EventArgs e)
        {

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

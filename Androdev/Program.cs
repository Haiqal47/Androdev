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
using System.Windows.Forms;
using Androdev.Core;
using Androdev.Localization;
using Androdev.View;

namespace Androdev
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            LogManager.ConfigureLogger();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0 && args[0] == "/uninstall")
            {
                MessageBox.Show(TextResource.UninstallerWarningText, TextResource.UninstallConfirmationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Run(new UninstallerView());
            }
            else
            {
                Application.Run(new InstallerView());
            }
        }
    }
}

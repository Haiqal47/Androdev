using System;
using System.Windows.Forms;
using Androdev.Core;
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
                Application.Run(new UninstallerView());
            }
            else
            {
                Application.Run(new InstallerView());
            }
        }
    }
}

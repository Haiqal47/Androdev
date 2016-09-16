using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace Androdev.Core.Diagostic
{
    public static class InstallationHelpers
    {
        public const string JdkInstallArguments = "/s ADDLOCAL=\"ToolsFeature,SourceFeature,PublicjreFeature\"";

        private static readonly string JdkPath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\jdk-8u101-windows-i586.exe");
        private static readonly string AndroidSdkPath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\android-sdk.zip");
        private static readonly string EclipseIdePath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\eclipse-java-mars-2-win32.zip");
        private static readonly string AdtPath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\ADT-23.0.7.zip");

        public static bool CheckDependecies()
        {
            var jdkExist = File.Exists(JdkPath);
            var sdkExist = File.Exists(AndroidSdkPath);
            var ideExist = File.Exists(EclipseIdePath);
            var adtExist = File.Exists(AdtPath);

            return jdkExist && sdkExist && ideExist && adtExist;
        }

        public static bool IsAndrodevDirectoryExist(string root)
        {
            return Directory.Exists(Path.Combine(root, "Androdev"));
        }

        public static string GetJavaInstallationPath()
        {
            string javaHome = null;

            // find on first location (on 32-bit)
            try
            {
                var location1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\JavaSoft\\Java Development Kit\\1.8.0_101");
                if (location1 != null)
                    javaHome = location1.GetValue("JavaHome").ToString();
            }
            catch
            {
                /* ignored */
            }

            // found it?
            if (javaHome != null) return javaHome;

            // find on second location (on 64-bit)
            try
            {
                var location2 =
                    Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\JavaSoft\\Java Development Kit\\1.8.0_101");
                if (location2 != null)
                    javaHome = location2.GetValue("JavaHome").ToString();
            }
            catch
            {
                /* ignored */
            }

            return javaHome;
        }
    }
}

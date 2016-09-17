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
using System.Security.Permissions;
using Microsoft.Win32;

namespace Androdev.Core.Diagostic
{
    public static class InstallationHelpers
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();

        private const string JavaRegistryPath1 = "SOFTWARE\\JavaSoft\\Java Development Kit\\1.8.0_101";
        private const string JavaRegistryPath2 = "SOFTWARE\\Wow6432Node\\JavaSoft\\Java Development Kit\\1.8.0_101";
        private const string JdkInstallArguments = "/s ADDLOCAL=\"ToolsFeature,SourceFeature,PublicjreFeature\"";

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
            
            Logger.Debug(string.Format("JDK:{0}, SDK:{1}, IDE:{2}, ADT:{3}", jdkExist, sdkExist, ideExist, adtExist));

            return jdkExist && sdkExist && ideExist && adtExist;
        }

        public static bool IsAndrodevDirectoryExist(string root)
        {
            var exist = Directory.Exists(Path.Combine(root, "Androdev"));
            Logger.Debug("Androdev installation on " + root + " is " + exist);

            return exist;
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public static bool InstallJavaDevelopmentKit()
        {
            var jdkPath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\jdk-8u101-windows-i586.exe");
            var success = ProcessHelper.RunAndWait(jdkPath, JdkInstallArguments);
            Logger.Debug("JDK installation success: " + success);

            return success;
        }

        public static string GetJavaInstallationPath()
        {
            string javaHome = null;
            
            try
            {
                // find on first location (on 32-bit)
                var location1 = Registry.LocalMachine.OpenSubKey(JavaRegistryPath1);
                javaHome = location1?.GetValue("JavaHome").ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            // found it?
            if (javaHome != null) return javaHome;
            
            try
            {
                // find on second location (on 64-bit)
                var location2 = Registry.LocalMachine.OpenSubKey(JavaRegistryPath2);
                javaHome = location2?.GetValue("JavaHome").ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            Logger.Info("Java installation path detected in " + javaHome);
            return javaHome;
        }
    }
}

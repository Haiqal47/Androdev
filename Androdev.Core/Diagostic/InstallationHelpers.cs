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
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace Androdev.Core.Diagostic
{
    public static class InstallationHelpers
    {
        private static readonly LogManager _LogManager = LogManager.GetClassLogger();

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

            _LogManager.Debug(string.Format("JDK:{0}, SDK:{1}, IDE:{2}, ADT:{3}", jdkExist, sdkExist, ideExist, adtExist));

            return jdkExist && sdkExist && ideExist && adtExist;
        }

        public static bool IsAndrodevDirectoryExist(string root)
        {
            var exist = Directory.Exists(Path.Combine(root, "Androdev"));
            _LogManager.Debug("Androdev installation on " + root + " is " + exist);

            return exist;
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
            catch (Exception ex)
            {
                _LogManager.Error(ex);
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
            catch (Exception ex)
            {
                _LogManager.Error(ex);
            }
            
            _LogManager.Info("Java installation path detected in " + javaHome);
            return javaHome;
        }
    }
}

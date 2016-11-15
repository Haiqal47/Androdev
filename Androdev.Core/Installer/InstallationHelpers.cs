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
using Microsoft.Win32;

namespace Androdev.Core.Installer
{
    public static class InstallationHelpers
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();

        private const string JavaRegistryPath1 = "SOFTWARE\\JavaSoft\\Java Development Kit\\1.8.0_101";
        private const string JavaRegistryPath2 = "SOFTWARE\\Wow6432Node\\JavaSoft\\Java Development Kit\\1.8.0_101";

        public static readonly string JdkPath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\jdk-8u101-windows-i586.exe");
        public static readonly string AndroidSdkPath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\android-sdk.zip");
        public static readonly string EclipseIdePath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\eclipse-java-mars-2-win32.zip");
        public static readonly string AdtPath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\ADT-23.0.7.zip");

        public const double AndroidSdkFileCount = 77508d;
        public const double EclipseIdeFileCount = 1494d;

        private const string ManifestTemplate =
            "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
            "<assembly xmlns=\"urn:schemas-microsoft-com:asm.v1\" manifestVersion=\"1.0\">" +
            "    <trustInfo xmlns=\"urn:schemas-microsoft-com:asm.v3\">" +
            "        <security>" +
            "            <requestedPrivileges>" +
            "                <requestedExecutionLevel level=\"requireAdministrator\" uiAccess=\"false\"/>" +
            "            </requestedPrivileges>" +
            "        </security>" +
            "    </trustInfo>" +
            "</assembly>";

        /// <summary>
        /// Check for dependecies.
        /// </summary>
        /// <returns>Returns true if entire dependecies are present, otherwise false.</returns>
        public static bool CheckDependecies()
        {
            var jdkExist = File.Exists(JdkPath);
            var sdkExist = File.Exists(AndroidSdkPath);
            var ideExist = File.Exists(EclipseIdePath);
            var adtExist = File.Exists(AdtPath);
            
            Logger.Debug(string.Format("JDK:{0}, SDK:{1}, IDE:{2}, ADT:{3}", jdkExist, sdkExist, ideExist, adtExist));

            return jdkExist && sdkExist && ideExist && adtExist;
        }

        /// <summary>
        /// Checks if Androdev dircetory is exists.
        /// </summary>
        /// <param name="root">Root drive.</param>
        /// <returns>True is Androdev directory is found, otherwise false.</returns>
        public static bool IsAndrodevExist(string root)
        {
            var exist = Directory.Exists(Path.Combine(root, "Androdev"));
            Logger.Debug("Androdev installation on " + root + " is " + exist);

            return exist;
        }

        /// <summary>
        /// Find Androdev installation from specified drives.
        /// </summary>
        /// <param name="dataSource"><see cref="DriveInfo"/> array to be analyzed.</param>
        /// <returns></returns>
        public static int FindAndrodevInstallation(DriveInfo[] dataSource)
        {
            for (int i = 0; i < dataSource.Length; i++)
            {
                if (IsAndrodevExist(dataSource[i].Name))
                {
                    return i;
                }
            }
            return 0;
        }
        
        /// <summary>
        /// Gets Java Development Kit installation path (JAVA_HOME).
        /// </summary>
        /// <returns>Fullpath to JAVA_HOME.</returns>
        public static string GetJavaInstallPath()
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

        /// <summary>
        /// Creates manifest file.
        /// </summary>
        /// <param name="outputFile">Fullpath to .manifest file to be created.</param>
        public static void CreateManifestFile(string outputFile)
        {
            try
            {
                using (var fs = new StreamWriter(outputFile, false))
                {
                    fs.Write(ManifestTemplate);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}

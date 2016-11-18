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
using System.Linq;
using System.Text;

namespace Androdev.Core
{
    /// <summary>
    /// Provides path informations.
    /// </summary>
    public sealed class PathService
    {
        private const string AndroidSdkTools = "https://dl.google.com/android/android-sdk_r24.1.2-windows.zip";
        private const string JavaDevelopmentKit = "http://download.oracle.com/otn-pub/java/jdk/8u101-b13/jdk-8u101-windows-i586.exe";
        private const string AndroidDeveloperTools = "https://dl.google.com/android/ADT-23.0.7.zip";
        private const string EclipseMars2 = "http://eclipse.mirror.rafal.ca/technology/epp/downloads/release/mars/2/eclipse-java-mars-2-win32.zip";

        private string _installRoot;
        private static PathService _instance;

        #region Static Instance
        /// <summary>
        /// Gets latest instance of this class.
        /// </summary>
        public static PathService Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Initialize new instance of <see cref="PathService"/> class. 
        /// </summary>
        /// <param name="root"></param>
        public static void Initialize(string root)
        {
            _instance = new PathService(root);
        }
        #endregion

        #region Constructor
        private PathService(string root)
        {
            _installRoot = root; 
        }
        #endregion

        #region Properties
        /// <summary>
        /// Androdev install drive root.
        /// </summary>
        public string InstallRoot
        {
            get { return _installRoot; }
        }
        /// <summary>
        /// Androdev install directory ([InstallRoot]\Androdev);
        /// </summary>
        public string InstallPath
        {
            get { return Path.Combine(_installRoot, "Androdev"); }
        }
        /// <summary>
        /// Fullpath to Eclipse installation directory.
        /// </summary>
        public string EclipsePath
        {
            get { return Path.Combine(InstallPath, "eclipse"); }
        }
        /// <summary>
        /// Fullpath to <c>eclipse.exe</c> file.
        /// </summary>
        public string EclipsecFilePath
        {
            get { return Path.Combine(InstallPath, "eclipse\\eclipsec.exe"); }
        }
        /// <summary>
        /// Fullpath to Eclipse Workspace directory.
        /// </summary>
        public string EclipseWorkspacePath
        {
            get { return Path.Combine(InstallPath, "workspace"); }
        }
        /// <summary>
        /// Android SDK directory ([InstallRoot]\Androdev\android-sdk);
        /// </summary>
        public string AndroidSdkPath
        {
            get { return Path.Combine(InstallPath, "android-sdk"); }
        }
        #endregion

        #region Methods
         /// <summary>
        /// Gets file URI based on index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>0 : Android SDK, 1 : JDK, 2 : ADT, 3 : Eclipse Mars 2.</remarks>
        public static string GetUrlByIndex(int index)
        {
            switch (index)
            {
                case 1:
                    return JavaDevelopmentKit;
                case 2:
                    return AndroidDeveloperTools;
                case 3:
                    return EclipseMars2;
            }
            return AndroidSdkTools;
        }
        #endregion
    }
}

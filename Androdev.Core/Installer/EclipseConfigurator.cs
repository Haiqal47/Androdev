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
using Androdev.Core.IO;

namespace Androdev.Core.Installer
{
    /// <summary>
    /// Provides Eclipse Configurator functionality.
    /// </summary>
    public class EclipseConfigurator
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();

        private readonly string _installDirectory;

        #region Constructor
        internal EclipseConfigurator(string androdevPath)
        {
            _installDirectory = androdevPath;
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Fullpath to Eclipse installation directory.
        /// </summary>
        private string EclipsePath
        {
            get { return Path.Combine(_installDirectory, "eclipse"); }
        }
        /// <summary>
        /// Fullpath to eclipse.exe file.
        /// </summary>
        private string EclipsecFilePath
        {
            get { return Path.Combine(_installDirectory, "eclipse\\eclipsec.exe"); }
        }
        /// <summary>
        /// Fullpath to Eclipse Workspace directory.
        /// </summary>
        private string EclipseWorkspacePath
        {
            get { return Path.Combine(_installDirectory, "workspace"); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize Eclipse configuration for the first time.
        /// </summary>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public bool InitializeEclipseConfiguration()
        {
            return ProcessHelper.RunAndWait(EclipsecFilePath, EclipseCommandBuilder.Build_PrepareConfigArgument());
        }
        /// <summary>
        /// Configures Eclipse worksapce path.
        /// </summary>
        /// <returns></returns>
        public bool ConfigureWorkspaceDirectory()
        {
            try
            {
                var eclipseConfigPath = Path.Combine(EclipsePath, "configuration\\.settings");
                Directory.CreateDirectory(eclipseConfigPath);
                File.WriteAllText(eclipseConfigPath + "\\org.eclipse.ui.ide.prefs", EclipseCommandBuilder.Build_WorkspaceConfig(EclipseWorkspacePath));
                return true;
            }
            catch (Exception ex)
            {
               Logger.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// Configures ADT's Android SDK path.
        /// </summary>
        /// <param name="androidSdkPath"></param>
        /// <returns></returns>
        public bool ConfigureSdkPath(string androidSdkPath)
        {
            try
            {
                var eclipseConfigPath = Path.Combine(EclipseWorkspacePath, ".metadata\\.plugins\\org.eclipse.core.runtime\\.settings");
                Directory.CreateDirectory(eclipseConfigPath);
                File.WriteAllText(eclipseConfigPath + "\\com.android.ide.eclipse.adt.prefs",EclipseCommandBuilder.Build_AndroidSDKConfig(androidSdkPath));
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// Configure Code Assist activation trigger.
        /// </summary>
        /// <returns></returns>
        public bool ConfigureCodeAssist()
        {
            try
            {
                var eclipseConfigPath = Path.Combine(EclipseWorkspacePath, ".metadata\\.plugins\\org.eclipse.core.runtime\\.settings");
                Directory.CreateDirectory(eclipseConfigPath);
                File.WriteAllText(eclipseConfigPath + "\\org.eclipse.jdt.ui.prefs",EclipseCommandBuilder.Build_CodeAssistConfig());
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
        #endregion
    }
}

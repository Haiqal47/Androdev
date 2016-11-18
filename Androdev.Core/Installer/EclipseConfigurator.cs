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
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
    public class EclipseConfigurator
    {
        private const string EclipseConfigPath = "configuration\\.settings";
        private const string SdkConfigPath = ".metadata\\.plugins\\org.eclipse.core.runtime\\.settings";
        private const string CodeAssistConfigPath = ".metadata\\.plugins\\org.eclipse.core.runtime\\.settings";

        private static readonly LogManager Logger = LogManager.GetClassLogger();
        private PathService _paths;

        public EclipseConfigurator(string root)
        {
            _paths = new PathService(root);
        }

        #region Methods
        /// <summary>
        /// Initialize Eclipse configuration for the first time.
        /// </summary>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public bool InitializeEclipseConfiguration()
        {
            return ProcessHelper.RunWait(_paths.EclipsecFilePath, EclipseCommandBuilder.Build_PrepareConfigArgument());
        }
        /// <summary>
        /// Configures Eclipse worksapce path.
        /// </summary>
        /// <returns></returns>
        public bool ConfigureWorkspaceDirectory()
        {
            try
            {
                var configPath = Path.Combine(_paths.EclipsePath, EclipseConfigPath);
                var configFile = configPath + "\\org.eclipse.ui.ide.prefs";

                Directory.CreateDirectory(configPath);
                File.WriteAllText(configFile, EclipseCommandBuilder.Build_WorkspaceConfig(_paths.EclipseWorkspacePath));
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
                var configPath = Path.Combine(_paths.EclipseWorkspacePath, SdkConfigPath);
                var configFile = configPath + "\\com.android.ide.eclipse.adt.prefs";

                Directory.CreateDirectory(configPath);
                File.WriteAllText(configFile,EclipseCommandBuilder.Build_AndroidSDKConfig(androidSdkPath));
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
                var configPath = Path.Combine(_paths.EclipseWorkspacePath, CodeAssistConfigPath);
                var configFile = configPath + "\\org.eclipse.jdt.ui.prefs";

                Directory.CreateDirectory(configPath);
                File.WriteAllText(configFile, EclipseCommandBuilder.Build_CodeAssistConfig());
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

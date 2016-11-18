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
using System.Security.Permissions;
using System.Text;
using Androdev.Core.IO;

namespace Androdev.Core.Installer
{
    public static class PackageInstaller
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();
        private static readonly PathService Paths = PathService.Instance;

        private const string EclipsecSuccess = "Operation completed";
        private const string JdkInstallArguments = "/s ADDLOCAL=\"ToolsFeature,SourceFeature,PublicjreFeature\"";

        /// <summary>
        /// Installs Java Development Kit.
        /// </summary>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public static bool InstallJavaDevelopmentKit()
        {
            var success = ProcessHelper.RunWait(InstallationHelpers.JdkPath, JdkInstallArguments);
            Logger.Debug("JDK installation success: " + success);

            return success;
        }

        /// <summary>
        /// Installs Android Developer Tools to Eclipse.
        /// </summary>
        /// <param name="eclipsecPath">Fullpath to eclipsec.exe</param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public static bool InstallAdt()
        {
            var args = EclipseCommandBuilder.Build_ADTInstallCommand(InstallationHelpers.AdtPath);
            return ProcessHelper.RunWait(Paths.EclipsecFilePath, args, EclipsecSuccess);
        }

        /// <summary>
        /// Execute Eclipse post install command.
        /// </summary>
        /// <param name="androdevPath"></param>
        /// <param name="workspaceDirectory"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public static bool EclipsePostInstall()
        {
            Logger.Debug("Eclipse IDE Post-Install action...");

            // configure Eclipse
            var eclipseConfigService = new EclipseConfigurator();

            // initialize Eclipse configuration
            if (!eclipseConfigService.InitializeEclipseConfiguration())
            {
                Logger.Error("Cannot initialize Eclipse configuration.");
                return false;
            }

            // configure workspace path
            Directory.CreateDirectory(Paths.EclipseWorkspacePath);
            if (!eclipseConfigService.ConfigureWorkspaceDirectory())
            {
                Logger.Error("Cannot change Eclipse Workspace directory.");
                return false;
            }

            Logger.Debug("Workspace directory created and has been set to Eclipse configuration.");
            return true;
        }
    }
}

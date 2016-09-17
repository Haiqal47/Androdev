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
using Androdev.Core.Diagostic;
using Androdev.Core.IO;

namespace Androdev.Core
{
    public class EclipseConfigurator
    {
        private static LogManager _logManager = LogManager.GetClassLogger();
        private const string EclipsecSuccess = "Operation completed";

        private readonly string _eclipseCommandLinePath;
        private readonly string _eclipsePath;
        private readonly string _workspacePath;

        public EclipseConfigurator(string eclipsePath, string workspacePath)
        {
            _eclipseCommandLinePath = Path.Combine(eclipsePath, "eclipsec.exe");
            _eclipsePath = eclipsePath;
            _workspacePath = workspacePath;
        }

        #region Command Arguments
        private const string EclipsePrepareConfigArguments = "-initialize";

        private static string BuildEclipseWorkspaceConfiguration(string workspacePath)
        {
            var sb = new StringBuilder();
            sb.Append("MAX_RECENT_WORKSPACES=5\n");
            sb.AppendFormat("RECENT_WORKSPACES={0}\n", Commons.ToEclipseCompliantPath(workspacePath));
            sb.Append("RECENT_WORKSPACES_PROTOCOL = 3\n");
            sb.Append("SHOW_WORKSPACE_SELECTION_DIALOG=false\n");
            sb.Append("eclipse.preferences.version=1\n");

            return sb.ToString();
        }

        private static string BuildEclipseAndroidSdkConfiguration(string sdkPath)
        {
            var sb = new StringBuilder();
            sb.Append("com.android.ide.eclipse.adt.fixLegacyEditors=1\n");
            sb.AppendFormat("com.android.ide.eclipse.adt.sdk = {0}\n", Commons.ToEclipseCompliantPath(sdkPath));
            sb.Append("eclipse.preferences.version=1\n");

            return sb.ToString();
        }

        private static string BuildEclipseIdeCodeAssistConfiguration()
        {
            var sb = new StringBuilder();
            sb.Append("content_assist_autoactivation_triggers_java=.(abcdefghijklmnopqrstuvwxyz\n");
            sb.Append("content_assist_proposals_background=255,255,255\n");
            sb.Append("content_assist_proposals_foreground=0,0,0\n");
            sb.Append("eclipse.preferences.version=1\n");
            sb.Append("fontPropagated = true\n");
            sb.Append("org.eclipse.jdt.ui.editor.tab.width =\n");
            sb.Append("org.eclipse.jdt.ui.exception.name = e\n");
            sb.Append("org.eclipse.jdt.ui.formatterprofiles.version = 12\n");
            sb.Append("org.eclipse.jdt.ui.gettersetter.use.is= true\n");
            sb.Append("org.eclipse.jdt.ui.javadoclocations.migrated = true\n");
            sb.Append("org.eclipse.jdt.ui.overrideannotation = true\n");
            sb.Append("org.eclipse.jface.textfont = 1 | Consolas | 10.0 | 0 | WINDOWS | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0 | 0 | 0 | 0 | Consolas;\n");
            sb.Append("proposalOrderMigrated = true\n");
            sb.Append("sourceHoverBackgroundColor = 255,255,225\n");
            sb.Append("spelling_locale_initialized = true\n");
            sb.Append("tabWidthPropagated = true\n");
            sb.Append("useAnnotationsPrefPage = true\n");
            sb.Append("useQuickDiffPrefPage = true\n");

            return sb.ToString();
        }

        private static string BuildAdtInstallArgument(string adtPath)
        {
            var sb = new StringBuilder();
            sb.Append("-clean ");
            sb.Append("-purgeHistory ");
            sb.Append("-application org.eclipse.equinox.p2.director ");
            sb.AppendFormat("-repository jar:{0}!/ ", new Uri(adtPath).AbsoluteUri);
            sb.Append("-installIU com.android.ide.eclipse.adt.feature.feature.group ");
            sb.Append("-installIU com.android.ide.eclipse.ddms.feature.feature.group ");
            sb.Append("-installIU com.android.ide.eclipse.gldebugger.feature.feature.group ");
            sb.Append("-installIU com.android.ide.eclipse.hierarchyviewer.feature.feature.group ");
            sb.Append("-installIU com.android.ide.eclipse.ndk.feature.feature.group ");
            sb.Append("-installIU com.android.ide.eclipse.traceview.feature.feature.group ");
            sb.Append("-installIU org.eclipse.cdt.feature.group ");
            sb.Append("-installIU org.eclipse.cdt.gdb.feature.group ");
            sb.Append("-installIU org.eclipse.cdt.gnu.build.feature.group ");
            sb.Append("-installIU org.eclipse.cdt.gnu.debug.feature.group ");
            sb.Append("-installIU org.eclipse.cdt.gnu.dsf.feature.group ");
            sb.Append("-installIU org.eclipse.cdt.platform.feature.group");

            return sb.ToString();
        }
        #endregion

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public bool InitializeEclipseConfiguration()
        {
            return ProcessHelper.RunAndWait(_eclipseCommandLinePath, EclipsePrepareConfigArguments);
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public bool InstallAdt(string adtPackagePath)
        {
            return ProcessHelper.RunAndWait(_eclipseCommandLinePath, BuildAdtInstallArgument(adtPackagePath), EclipsecSuccess);
        }
        
        public bool ConfigureWorkspaceDirectory()
        {
            try
            {
                var eclipseConfigPath = Path.Combine(_eclipsePath, "configuration\\.settings");
                Directory.CreateDirectory(eclipseConfigPath);
                File.WriteAllText(eclipseConfigPath + "\\org.eclipse.ui.ide.prefs", BuildEclipseWorkspaceConfiguration(_workspacePath));
                return true;
            }
            catch (Exception ex)
            {
               _logManager.Error(ex);
                return false;
            }
        }
        
        public bool ConfigureSdkPath(string androidSdkPath)
        {
            try
            {
                var eclipseConfigPath = Path.Combine(_workspacePath, ".metadata\\.plugins\\org.eclipse.core.runtime\\.settings");
                Directory.CreateDirectory(eclipseConfigPath);
                File.WriteAllText(eclipseConfigPath + "\\com.android.ide.eclipse.adt.prefs", BuildEclipseAndroidSdkConfiguration(androidSdkPath));
                return true;
            }
            catch (Exception ex)
            {
                _logManager.Error(ex);
                return false;
            }
        }

        public bool ConfigureCodeAssist()
        {
            try
            {
                var eclipseConfigPath = Path.Combine(_workspacePath, ".metadata\\.plugins\\org.eclipse.core.runtime\\.settings");
                Directory.CreateDirectory(eclipseConfigPath);
                File.WriteAllText(eclipseConfigPath + "\\org.eclipse.jdt.ui.prefs", BuildEclipseIdeCodeAssistConfiguration());
                return true;
            }
            catch (Exception ex)
            {
                _logManager.Error(ex);
                return false;
            }
        }
    }
}

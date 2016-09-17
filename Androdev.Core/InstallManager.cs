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
using System.ComponentModel;
using System.IO;
using System.Threading;
using Androdev.Core.Diagostic;
using Androdev.Core.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Androdev.Core
{
    public class InstallManager : IDisposable
    {
        private readonly LogManager _logManager = LogManager.GetClassLogger();

        private string _installDirectory;
        private bool _uacCompatibility;
        private string _installRoot;

        private int _extractedFile;
        private readonly BackgroundWorker _bwWorker;

        public InstallManager()
        {
            _logManager.Debug("InstallManager is constructing...");
            UseAutomatedConfig();

            // configure BGW
            _bwWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _bwWorker.DoWork += BwWorker_DoWork;
            _bwWorker.RunWorkerCompleted += BwWorker_RunWorkerCompleted;

            _logManager.Debug("InstallManager constructed.");
        }

        #region Properties
        public string InstallDirectory
        {
            get { return _installDirectory; }
        }

        public string InstallRoot
        {
            get { return _installRoot; }
        }

        public bool UacCompatibility
        {
            get { return _uacCompatibility; }
        }
        #endregion

        #region Events
        public event EventHandler InstallStarted;
        public event EventHandler<InstallProgressChangedEventArgs> InstallProgressChanged;
        public event EventHandler InstallFinished;
        #endregion

        #region Configurator Methods
        public void UseAutomatedConfig()
        {
            // find instal directory
            var systemDisk = Path.GetPathRoot(Environment.SystemDirectory);
            _logManager.Debug("System disk: " + systemDisk);

            var installDrive = systemDisk;
            var drives = FastIO.GetAvailiableDrives();
            for (var i = 0; i < drives.Length; i++)
            {
                _logManager.Debug("Found drive: " + drives[i].Name);

                // is system disk?
                if (systemDisk.StartsWith(drives[i].Name)) continue;

                // the choosen one
                installDrive = drives[i].Name;
                _logManager.Info("Androdev install location selected. Drive " + installDrive);
                break;
            }

            // use current config
            _installDirectory = Path.Combine(installDrive, "Androdev");
            _installRoot = installDrive;
            _uacCompatibility = (installDrive == systemDisk);
        }

        public void SetInstallRoot(string driveRoot)
        {
            if (string.IsNullOrEmpty(driveRoot))
            {
                _logManager.Debug("Null object passed to InstallDirectory.");
                throw new ArgumentException("Argument is null or empty", nameof(driveRoot));
            }

            _installDirectory = Path.Combine(driveRoot, "Androdev");
            _logManager.Debug("Changed install path to " + _installDirectory);
        }

        public void SetUacCompatibility(bool enabled)
        {
            _uacCompatibility = enabled;
            _logManager.Debug("UAC Compatibility state: " + _uacCompatibility.ToString());
        }
        #endregion

        #region Control Methods
        public void BeginInstall()
        {
            if (_bwWorker.IsBusy)
            {
                _logManager.Debug("BackgroundWorker is busy.");
                throw new InvalidOperationException("Thread is busy!");
            }

            InstallStarted?.Invoke(this, EventArgs.Empty);
            _bwWorker.RunWorkerAsync();
            _logManager.Debug("Install process started.");
        }

        public void EndInstall()
        {
            if (_bwWorker.IsBusy)
            {
                _logManager.Debug("Stopping install process.");
                _bwWorker.CancelAsync();
            }
        }
        #endregion

        #region Thread Worker
        private void WorkerReportProgress(int arg1, int arg2, string arg3, string arg4 = "")
        {
            InstallProgressChanged?.Invoke(this, new InstallProgressChangedEventArgs()
            {
                OverallProgressPercentage = arg1,
                CurrentProgressPercentage = arg2,
                StatusText = arg3,
                ExtraStatusText = arg4,
            });
        }

        private void BwWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InstallFinished?.Invoke(this, EventArgs.Empty);
        }

        private void BwWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // before step - check for dependecies
            if (!InstallationHelpers.CheckDependecies())
            {
                WorkerReportProgress(0, 0, "Dependecies missing!", "Click [Update] to download necessary items.");
                _bwWorker.CancelAsync();
                return;
            }

            // before step - check existing installation
            if (InstallationHelpers.IsAndrodevDirectoryExist(_installRoot))
            {
                WorkerReportProgress(0, 0, "Existing installation detected.", "Please remove existing Androdev installation.");
                _bwWorker.CancelAsync();
                return;
            }

            // step 1 - install JDK
            _logManager.Info("==========Java Development Kit [Started]===========");
            InstallJdk();
            _logManager.Info("==========Java Development Kit [Finished]===========");

            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 2 - config PATH
            _logManager.Info("==========Configuring Envirnment Variables [Started]===========");
            ConfigurePathEnvironment();
            _logManager.Info("==========Configuring Envirnment Variables [Finished]===========");

            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 3 - install Android SDK
            _logManager.Info("==========Android SDK Tools [Started]===========");
            InstallAndroidSdk();
            _logManager.Info("==========Android SDK Tools [Finished]===========");

            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 4 - install Eclipse IDE
            _logManager.Info("==========Eclipse Mars 2 IDE [Started]===========");
            InstallEclipseIde();
            _logManager.Info("==========Eclipse Mars 2 IDE [Finished]===========");

            // add manifest if necessary
            if (_uacCompatibility)
            {
                _logManager.Info("==========UAC Elevation Manifest [Started]===========");
                InstallManifests();
                _logManager.Info("==========UAC Elevation Manifest [Finished]===========");
            }

            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 5 - install ADT
            _logManager.Info("==========Android Developer Tools Plugin [Started]===========");
            InstallAdt();
            _logManager.Info("==========Android Developer Tools Plugin [Finished]===========");

            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 6 - configure Eclipse
            _logManager.Info("==========Configure Eclipse IDE for First Time [Started]===========");
            ConfigureEclipse();
            _logManager.Info("==========Configure Eclipse IDE for First Time [Finished]===========");

            // step 7 - create shortcuts
            _logManager.Info("==========Install Shortcuts on Desktop [Started]===========");
            InstallShortcuts();
            _logManager.Info("==========Install Shortcuts on Desktop [Finished]===========");
        }
        #endregion

        #region Core Installation Features
        private void InstallJdk()
        {
            if (InstallationHelpers.GetJavaInstallationPath() != null)
            {
                WorkerReportProgress(14, 100, "Installing JDK 8u101...", "JDK already installed.");
                return;
            }

            WorkerReportProgress(7, 99, "Installing JDK 8u101...", "Executing Windows Installer...");

            var jdkPath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\jdk-8u101-windows-i586.exe");
            if (!ProcessHelper.ExecuteProcessAndWait(jdkPath, InstallationHelpers.JdkInstallArguments))
            {
                _bwWorker.CancelAsync();
                WorkerReportProgress(14, 100, "JDK 8u101 cannot be installed.", "Installation timeout.");
                return;
            }

            WorkerReportProgress(14, 100, "JDK 8u101 installed.");
        }

        private void ConfigurePathEnvironment()
        {
            WorkerReportProgress(14, 50, "Configuring environment variables...");

            // find Java install path
            var javaHome = InstallationHelpers.GetJavaInstallationPath();
            var javaBinaries = Path.Combine(javaHome, "bin");
            if (javaHome == null)
            {
                _logManager.Debug("Java home directory not found.");
                _bwWorker.CancelAsync();
                WorkerReportProgress(14, 58, "Java installation directory not found!");
                return;
            }
            _logManager.Debug("Java home directory founded: " + javaHome);

            try
            {
                // set JAVA_HOME
                Environment.SetEnvironmentVariable("JAVA_HOME", javaHome, EnvironmentVariableTarget.User);
                _logManager.Debug("Environment variable set: JAVA_HOME to " + javaHome);

                // set Java binaries in Path
                var initialPath = Environment.GetEnvironmentVariable("Path");
                if (initialPath != null && initialPath.Contains(javaBinaries)) return;
              
                var newPath = initialPath + ";" + javaBinaries;
                Environment.SetEnvironmentVariable("Path", newPath, EnvironmentVariableTarget.Machine);
                _logManager.Debug("Environment variable set: added " + Path.Combine(javaHome, "bin") + " to PATH variable.");

                WorkerReportProgress(28, 100, "Environment variables updated.");
            }
            catch (Exception ex)
            {
                _bwWorker.CancelAsync();
                _logManager.Error("Cannot configure environment variables.", ex);
                WorkerReportProgress(28, 89, "Cannot update environment variables.");
            }
        }

        private void InstallAndroidSdk()
        {
            WorkerReportProgress(28, 0, "Installing Android SDK Tools...", "Preparing to unpack...");

            // define zip source
            var extractPath = Path.Combine(_installDirectory, "android-sdk");
            var zipSource = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\android-sdk.zip");
            Directory.CreateDirectory(extractPath);
            Interlocked.Exchange(ref _extractedFile, 0);
            _logManager.Debug("Android SDK will extracted to: " + extractPath);

            // define event subscriber
            FastZipEvents zipEvents = new FastZipEvents();
            zipEvents.ProcessFile += delegate (object sender, ScanEventArgs args)
            {
                // check for cancellation
                args.ContinueRunning = !_bwWorker.CancellationPending;
                if (!args.ContinueRunning) return;

                // use Interlocked as atomic operation
                Interlocked.Increment(ref _extractedFile);
                var currentProgress = Interlocked.CompareExchange(ref _extractedFile, 0, 0) / 77508D * 100;
                var overallProgress = currentProgress / 100 * 12;

                // report progress
                WorkerReportProgress(Convert.ToInt32(overallProgress) + 24, Convert.ToInt32(currentProgress),
                    "Installing Android SDK Tools...", "Extracting: " + Commons.ElipsisText(Path.GetFileName(args.Name)));
            };
 
            // extract files
            _logManager.Debug("Extracting Android SDK Tools...");

            FastZip androidZip = new FastZip(zipEvents);
            androidZip.ExtractZip(zipSource, extractPath, FastZip.Overwrite.Always, name => true, null, null, true);

            _logManager.Debug("Android SDK Tools installed.");
            WorkerReportProgress(42, 100, "Android SDK Tools installed.");
        }

        private void InstallEclipseIde()
        {
            WorkerReportProgress(42, 0, "Installing Eclipse Mars 2...", "Preparing to unpack...");

            // define zip source
            var zipSource = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\eclipse-java-mars-2-win32.zip");
            var zipDestination = Path.Combine(_installDirectory, "eclipse");
            _logManager.Debug("Eclipse IDE will extracted to: " + zipDestination);
            Interlocked.Exchange(ref _extractedFile, 0);
            Directory.CreateDirectory(zipDestination);

            // define event subscriber
            FastZipEvents zipEvents = new FastZipEvents();
            zipEvents.ProcessFile += delegate (object sender, ScanEventArgs args)
            {
                // check for cancellation
                args.ContinueRunning = !_bwWorker.CancellationPending;
                if (!args.ContinueRunning) return;

                // use Interlocked as atomic operation
                Interlocked.Increment(ref _extractedFile);
                var currentProgress = Interlocked.CompareExchange(ref _extractedFile, 0, 0) / 1494D * 100;
                var overallProgress = currentProgress / 100 * 12;

                // report progress
                WorkerReportProgress(Convert.ToInt32(overallProgress) + 36, Convert.ToInt32(currentProgress),
                    "Installing Eclipse Mars 2...", "Extracting: " + Commons.ElipsisText(Path.GetFileName(args.Name)));
            };
            
            // extract files
            _logManager.Debug("Extracting Eclipse IDE...");

            FastZip eclipseZip = new FastZip(zipEvents);
            eclipseZip.ExtractZip(zipSource, _installDirectory, FastZip.Overwrite.Always, name => true, null, null, true);

            _logManager.Debug("Eclipse IDE installed.");

            // configure Eclipse
            var workspaceDirectory = Path.Combine(_installDirectory, "workspace");
            var eclipseConfigService = new EclipseConfigurator(Path.Combine(_installDirectory, "eclipse"), workspaceDirectory);

            // initialize Eclipse configuration
            var initialized = eclipseConfigService.InitializeEclipseConfiguration();
            if (!initialized)
            {
                _bwWorker.CancelAsync();
                _logManager.Error("Cannot initialize Eclipse configuration.");
                WorkerReportProgress(68, 60, "Cannot initialize Eclipse configuration.");
                return;
            }
            _logManager.Debug("Eclipse configuration initialized successfully.");


            WorkerReportProgress(56, 100, "Eclipse Mars 2 installed.");
        }

        private void InstallManifests()
        {
            WorkerReportProgress(56, 63, "Creating manifest...");

            // create manifest for SDK Manager
            Manifester.CreateManifestFile(Path.Combine(_installDirectory, "android-sdk\\SDK Manager.exe.manifest"));
            _logManager.Debug("Manifest added to SDK Manager.");

            // create manifest for AVD Manager
            Manifester.CreateManifestFile(Path.Combine(_installDirectory, "android-sdk\\AVD Manager.exe.manifest"));
            _logManager.Debug("Manifest added to AVD Manager.");

            // create manifest for Eclipse IDE
            Manifester.CreateManifestFile(Path.Combine(_installDirectory, "eclipse\\eclipse.exe.manifest"));
            _logManager.Debug("Manifest added to Eclipse IDE.");

            // create manifest for Eclipse Command Line
            Manifester.CreateManifestFile(Path.Combine(_installDirectory, "eclipse\\eclipsec.exe.manifest"));
            _logManager.Debug("Manifest added to Eclipse Command Line.");

            WorkerReportProgress(56, 100, "Manifest installed.");
        }

        private void InstallAdt()
        {
            WorkerReportProgress(56, 30, "Initializing Eclipse configuration...");

            // configurator service
            var workspaceDirectory = Path.Combine(_installDirectory, "workspace");
            var eclipseConfigService = new EclipseConfigurator(Path.Combine(_installDirectory, "eclipse"), workspaceDirectory);

            // Eclipse path
            var eclipseInstallPath = Path.Combine(_installDirectory, "eclipse");
            var adtPath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\ADT-23.0.7.zip");
            _logManager.Debug("Eclipse installation path: " + eclipseInstallPath);
            _logManager.Debug("ADT package path: " + adtPath);
            
            // install ADT
            WorkerReportProgress(56, 63, "Installing Android Developer Tools...");
            var adtInstalled = eclipseConfigService.InstallAdt(adtPath);
            if (!adtInstalled)
            {
                _bwWorker.CancelAsync();
                WorkerReportProgress(68, 64, "Cannot install Android Developer Tools.");
                _logManager.Error("Cannot install Android Developer Tools.");
                return;
            }

            _logManager.Debug("ADT installed successfully.");
            WorkerReportProgress(70, 100, "Android Developer Tools installed.");
        }

        private void ConfigureEclipse()
        {
            WorkerReportProgress(70, 63, "Configuring Eclipse IDE...");

            // variables
            var workspaceDirectory = Path.Combine(_installDirectory, "workspace");
            var eclipseConfigService = new EclipseConfigurator(Path.Combine(_installDirectory, "eclipse"), workspaceDirectory);

            // configure workspace path
            Directory.CreateDirectory(workspaceDirectory);
            eclipseConfigService.ConfigureWorkspaceDirectory();
            _logManager.Debug("Workspace directory created and has been set to Eclipse configuration.");

            // configure SDK path
              var androidSdkPath = Path.Combine(_installDirectory, "android-sdk");
            eclipseConfigService.ConfigureSdkPath(androidSdkPath);
            _logManager.Debug("Android SDK path has configured in Eclipse configuration.");

            // configure code assist
            eclipseConfigService.ConfigureCodeAssist();

            _logManager.Debug("Code assist auto-activation has been changed.");
            WorkerReportProgress(86, 63, "Configuration completed.");
        }

        private void InstallShortcuts()
        {
            WorkerReportProgress(86, 24, "Installing shortcuts...");
            _logManager.Debug("Installing shortcuts...");

            FastIO.CreateShortcut(new ShortcutProperties()
            {
                Target = Path.Combine(_installDirectory, "android-sdk\\SDK Manager.exe"),
                Name = "Android SDK Tools",
                Comment = "Launch Android SDK Tools.",
            });

            FastIO.CreateShortcut(new ShortcutProperties()
            {
                Target = Path.Combine(_installDirectory, "eclipse\\eclipse.exe"),
                Name = "Eclipse Mars for Android",
                Comment = "Launch Eclipse Mars IDE for Android.",
            });
            
            FastIO.CreateShortcut(new ShortcutProperties()
            {
                Target = Path.Combine(_installDirectory, "workspace"),
                Name = "Eclipse Workspace",
                Comment = "Eclipse workspace directory.",
            });

            _logManager.Debug("Shortcuts installed.");
            WorkerReportProgress(100, 100, "Shortcuts installed.");
        }
        #endregion

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                _bwWorker?.Dispose();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

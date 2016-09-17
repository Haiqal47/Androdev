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
        private static readonly LogManager Logger = LogManager.GetClassLogger();

        private string _installDirectory;
        private bool _uacCompatibility;
        private string _installRoot;

        private int _extractedFile;
        private readonly BackgroundWorker _bwWorker;

        public InstallManager()
        {
            UseAutomatedConfig();

            // configure BGW
            _bwWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _bwWorker.DoWork += BwWorker_DoWork;
            _bwWorker.RunWorkerCompleted += BwWorker_RunWorkerCompleted;
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
            Logger.Debug("System disk: " + systemDisk);

            var installDrive = systemDisk;
            var drives = FastIo.GetAvailiableDrives();
            for (var i = 0; i < drives.Length; i++)
            {
                // is system disk?
                if (systemDisk.StartsWith(drives[i].Name)) continue;

                // the choosen one
                installDrive = drives[i].Name;
                Logger.Info("Androdev install location selected. Drive " + installDrive);
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
                throw new ArgumentException("Argument is null or empty", nameof(driveRoot));
            }

            _installDirectory = Path.Combine(driveRoot, "Androdev");
            Logger.Debug("Changed install path to " + _installDirectory);
        }

        public void SetUacCompatibility(bool enabled)
        {
            _uacCompatibility = enabled;
            Logger.Debug("UAC Compatibility state: " + _uacCompatibility.ToString());
        }
        #endregion

        #region Control Methods
        public void BeginInstall()
        {
            if (_bwWorker.IsBusy)
            {
                throw new InvalidOperationException("Thread is busy!");
            }

            InstallStarted?.Invoke(this, EventArgs.Empty);
            _bwWorker.RunWorkerAsync();
            Logger.Debug("Install process started.");
        }

        public void EndInstall()
        {
            if (!_bwWorker.IsBusy) return;
            Logger.Debug("Stopping install process.");
            _bwWorker.CancelAsync();
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
            Logger.Info("==========Java Development Kit [Started]===========");
            InstallJdk();
            Logger.Info("==========Java Development Kit [Finished]===========");

            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 2 - config PATH
            Logger.Info("==========Configuring Envirnment Variables [Started]===========");
            ConfigurePathEnvironment();
            Logger.Info("==========Configuring Envirnment Variables [Finished]===========");

            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 3 - install Android SDK
            Logger.Info("==========Android SDK Tools [Started]===========");
            InstallAndroidSdk();
            Logger.Info("==========Android SDK Tools [Finished]===========");

            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 4 - install Eclipse IDE
            Logger.Info("==========Eclipse Mars 2 IDE [Started]===========");
            InstallEclipseIde();
            Logger.Info("==========Eclipse Mars 2 IDE [Finished]===========");

            // add manifest if necessary
            if (_uacCompatibility)
            {
                Logger.Info("==========UAC Elevation Manifest [Started]===========");
                InstallManifests();
                Logger.Info("==========UAC Elevation Manifest [Finished]===========");
            }

            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 5 - install ADT
            Logger.Info("==========Android Developer Tools Plugin [Started]===========");
            InstallAdt();
            Logger.Info("==========Android Developer Tools Plugin [Finished]===========");

            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 6 - configure Eclipse
            Logger.Info("==========Configure Eclipse IDE for First Time [Started]===========");
            ConfigureEclipse();
            Logger.Info("==========Configure Eclipse IDE for First Time [Finished]===========");

            // step 7 - create shortcuts
            Logger.Info("==========Install Shortcuts on Desktop [Started]===========");
            InstallShortcuts();
            Logger.Info("==========Install Shortcuts on Desktop [Finished]===========");

            // all finish
            WorkerReportProgress(0, 0, "Androdev has been installed successfully.");
            Logger.Info("Androdev has been installed successfully.");
        }
        #endregion

        #region Methods
        private void EclipseIdePostInstallation()
        {
            Logger.Debug("Eclipse IDE Post-Install action...");
            WorkerReportProgress(50, 99, "Eclipse IDE Post-Install action...");

            // configure Eclipse
            var workspaceDirectory = Path.Combine(_installDirectory, "workspace");
            var eclipseInstallPath = Path.Combine(_installDirectory, "eclipse");
            var eclipseConfigService = new EclipseConfigurator(eclipseInstallPath, workspaceDirectory);

            // initialize Eclipse configuration
            if (!eclipseConfigService.InitializeEclipseConfiguration())
            {
                Logger.Error("Cannot initialize Eclipse configuration.");
                WorkerReportProgress(0, 0, "Cannot initialize Eclipse configuration.");
                _bwWorker.CancelAsync();
                return;
            }

            // configure workspace path
            Directory.CreateDirectory(workspaceDirectory);
            if (!eclipseConfigService.ConfigureWorkspaceDirectory())
            {
                Logger.Error("Cannot change Eclipse Workspace directory.");
                WorkerReportProgress(0, 0, "Cannot change Eclipse Workspace directory.");
                _bwWorker.CancelAsync();
                return;
            }

            Logger.Debug("Workspace directory created and has been set to Eclipse configuration.");
            WorkerReportProgress(56, 100, "Eclipse IDE Post-Action completed.");
        }
        #endregion

        #region Core Installation Features
        private void InstallJdk()
        {

            // check if JDK is already installed
            if (InstallationHelpers.GetJavaInstallationPath() != null)
            {
                WorkerReportProgress(14, 100, "Installing JDK 8u101...", "JDK already installed.");
                Logger.Debug("JDK is already installed.");
                return;
            }

            WorkerReportProgress(7, 99, "Installing JDK 8u101...", "Executing Windows Installer...");
            
            // install JDK
            if (!InstallationHelpers.InstallJavaDevelopmentKit())
            {
                WorkerReportProgress(0, 0, "JDK 8u101 cannot be installed.", "Installation timeout.");
                _bwWorker.CancelAsync();
                return;
            }

            WorkerReportProgress(14, 100, "JDK 8u101 installed.");
        }

        private void ConfigurePathEnvironment()
        {
            WorkerReportProgress(14, 99, "Configuring environment variables...");

            // find Java install path
            var javaHome = InstallationHelpers.GetJavaInstallationPath();
            var javaBinaries = Path.Combine(javaHome, "bin");

            // check if Java is successfully installed
            if (javaHome == null)
            {
                Logger.Debug("Java home directory not found.");
                WorkerReportProgress(0, 0, "Java installation directory not found!");
                _bwWorker.CancelAsync();
                return;
            }
            Logger.Debug("Java home directory founded: " + javaHome);

            try
            {
                // set JAVA_HOME
                Environment.SetEnvironmentVariable("JAVA_HOME", javaHome, EnvironmentVariableTarget.User);
                Logger.Debug("Environment variable set: JAVA_HOME to " + javaHome);

                // set Java binaries in Path
                var initialPath = Environment.GetEnvironmentVariable("Path");
                if (initialPath != null && initialPath.Contains(javaBinaries)) return;
              
                var newPath = initialPath + ";" + javaBinaries;
                Environment.SetEnvironmentVariable("Path", newPath, EnvironmentVariableTarget.Machine);

                Logger.Debug("Environment variable set: added " + Path.Combine(javaHome, "bin") + " to PATH variable.");
                WorkerReportProgress(28, 100, "Environment variables updated.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                WorkerReportProgress(0, 0, "Cannot update environment variables.");
                _bwWorker.CancelAsync();
            }
        }

        private void InstallAndroidSdk()
        {
            WorkerReportProgress(28, 0, "Installing Android SDK Tools...", "Preparing to unpack...");

            // define zip source
            var extractPath = Path.Combine(_installDirectory, "android-sdk");
            var zipSource = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\android-sdk.zip");

            // create directory and set count to 0
            Directory.CreateDirectory(extractPath);
            Interlocked.Exchange(ref _extractedFile, 0);
            Logger.Debug("Android SDK will extracted to: " + extractPath);

            // define event subscriber
            FastZipEvents zipEvents = new FastZipEvents();
            zipEvents.ProcessFile += delegate (object sender, ScanEventArgs args)
            {
                // check for cancellation
                args.ContinueRunning = !_bwWorker.CancellationPending;
                if (!args.ContinueRunning) return;

                // use Interlocked as atomic operation
                Interlocked.Increment(ref _extractedFile);
                var currentProgress = Interlocked.CompareExchange(ref _extractedFile, 0, 0) / InstallationHelpers.AndroidSdkFileCount * 100;
                var overallProgress = (currentProgress / 100 * 12) + 24;

                // report progress
                WorkerReportProgress(Convert.ToInt32(overallProgress), Convert.ToInt32(currentProgress),
                    "Installing Android SDK Tools...", "Extracting: " + Commons.ElipsisText(Path.GetFileName(args.Name)));
            };
 
            try
            {
                // extract files
                Logger.Debug("Extracting Android SDK Tools...");

                FastZip androidZip = new FastZip(zipEvents);
                androidZip.ExtractZip(zipSource, extractPath, FastZip.Overwrite.Always, name => true, null, null, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                WorkerReportProgress(0, 0, "Android SDK Tools cannot be installed.");
                _bwWorker.CancelAsync();
            }

            Logger.Debug("Android SDK Tools installed.");
            WorkerReportProgress(42, 100, "Android SDK Tools installed.");
        }

        private void InstallEclipseIde()
        {
            WorkerReportProgress(42, 0, "Installing Eclipse Mars 2...", "Preparing to unpack...");

            // define zip source
            var zipSource = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\eclipse-java-mars-2-win32.zip");
            var zipDestination = Path.Combine(_installDirectory, "eclipse");

            // create output dir and set count to 0
            Directory.CreateDirectory(zipDestination);
            Interlocked.Exchange(ref _extractedFile, 0);
            Logger.Debug("Eclipse IDE will extracted to: " + zipDestination);

            // define event subscriber
            FastZipEvents zipEvents = new FastZipEvents();
            zipEvents.ProcessFile += delegate(object sender, ScanEventArgs args)
            {
                // check for cancellation
                args.ContinueRunning = !_bwWorker.CancellationPending;
                if (!args.ContinueRunning) return;

                // use Interlocked as atomic operation
                Interlocked.Increment(ref _extractedFile);
                var currentProgress = Interlocked.CompareExchange(ref _extractedFile, 0, 0)/InstallationHelpers.EclipseIdeFileCount*100;
                var overallProgress = currentProgress/100*12;

                // report progress
                WorkerReportProgress(Convert.ToInt32(overallProgress) + 36, Convert.ToInt32(currentProgress),
                    "Installing Eclipse Mars 2...", "Extracting: " + Commons.ElipsisText(Path.GetFileName(args.Name)));
            };

            try
            {
                // extract files
                Logger.Debug("Extracting Eclipse IDE...");

                FastZip eclipseZip = new FastZip(zipEvents);
                eclipseZip.ExtractZip(zipSource, _installDirectory, FastZip.Overwrite.Always, name => true, null, null, true);

                Logger.Debug("Eclipse IDE installed.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                WorkerReportProgress(0, 0, "Eclipse IDE cannot be installed.");
                _bwWorker.CancelAsync();
            }

            // do post installation
            EclipseIdePostInstallation();

            Logger.Debug("Eclipse configuration initialized successfully.");
        }

        private void InstallManifests()
        {
            WorkerReportProgress(56, 63, "Creating manifest...");

            // create manifest for SDK Manager
            Manifester.CreateManifestFile(Path.Combine(_installDirectory, "android-sdk\\SDK Manager.exe.manifest"));
            Logger.Debug("Manifest added to SDK Manager.");

            // create manifest for AVD Manager
            Manifester.CreateManifestFile(Path.Combine(_installDirectory, "android-sdk\\AVD Manager.exe.manifest"));
            Logger.Debug("Manifest added to AVD Manager.");

            // create manifest for Eclipse IDE
            Manifester.CreateManifestFile(Path.Combine(_installDirectory, "eclipse\\eclipse.exe.manifest"));
            Logger.Debug("Manifest added to Eclipse IDE.");

            // create manifest for Eclipse Command Line
            Manifester.CreateManifestFile(Path.Combine(_installDirectory, "eclipse\\eclipsec.exe.manifest"));
            Logger.Debug("Manifest added to Eclipse Command Line.");

            WorkerReportProgress(56, 100, "Manifest installed.");
        }

        private void InstallAdt()
        {
            WorkerReportProgress(56, 30, "Initializing Eclipse configuration...");

            // configurator service
            var workspaceDirectory = Path.Combine(_installDirectory, "workspace");
            var eclipseInstallPath = Path.Combine(_installDirectory, "eclipse");
            var eclipseConfigService = new EclipseConfigurator(eclipseInstallPath, workspaceDirectory);

            // locate ADT
            var adtPath = Path.Combine(Commons.GetBaseDirectoryPath(), "bin\\ADT-23.0.7.zip");
            Logger.Debug("Eclipse installation path: " + eclipseInstallPath);
            Logger.Debug("ADT package path: " + adtPath);
            
            // install ADT
            WorkerReportProgress(56, 63, "Installing Android Developer Tools...");
            if (!eclipseConfigService.InstallAdt(adtPath))
            {
                WorkerReportProgress(0, 0, "Cannot install Android Developer Tools.");
                Logger.Error("Cannot install Android Developer Tools.");
                _bwWorker.CancelAsync();
                return;
            }

            Logger.Debug("ADT installed successfully.");
            WorkerReportProgress(70, 100, "Android Developer Tools installed.");
        }

        private void ConfigureEclipse()
        {
            WorkerReportProgress(70, 63, "Configuring Eclipse IDE...");

            // variables
            var workspaceDirectory = Path.Combine(_installDirectory, "workspace");
            var eclipseInstallPath = Path.Combine(_installDirectory, "eclipse");
            var eclipseConfigService = new EclipseConfigurator(eclipseInstallPath, workspaceDirectory);

            // configure SDK path
            var androidSdkPath = Path.Combine(_installDirectory, "android-sdk");
            if (!eclipseConfigService.ConfigureSdkPath(androidSdkPath))
            {
                Logger.Error("Cannot change Android SDK path in ADT.");
                WorkerReportProgress(0, 0, "Cannot configure Android SDK path.");
                _bwWorker.CancelAsync();
                return;
            }
            Logger.Debug("Android SDK path has configured in Eclipse configuration.");

            // configure code assist
            if (!eclipseConfigService.ConfigureCodeAssist())
            {
                Logger.Error("Cannot configure Code Assist in Eclipse.");
                WorkerReportProgress(0, 0, "Cannot configure Code Assist in Eclipse.");
                _bwWorker.CancelAsync();
                return;
            }
            Logger.Debug("Code assist auto-activation has been changed.");

            WorkerReportProgress(86, 63, "Configuration completed.");
        }

        private void InstallShortcuts()
        {
            WorkerReportProgress(86, 24, "Installing shortcuts...");
            Logger.Debug("Installing shortcuts...");

            FastIo.CreateShortcut(new ShortcutProperties()
            {
                Target = Path.Combine(_installDirectory, "android-sdk\\SDK Manager.exe"),
                Name = "Android SDK Tools",
                Comment = "Launch Android SDK Tools.",
                IconFile = Path.Combine(_installDirectory, "android-sdk\\tools\\emulator.exe"),
            });

            FastIo.CreateShortcut(new ShortcutProperties()
            {
                Target = Path.Combine(_installDirectory, "eclipse\\eclipse.exe"),
                Name = "Eclipse Mars for Android",
                Comment = "Launch Eclipse Mars IDE for Android.",
            });
            
            FastIo.CreateShortcut(new ShortcutProperties()
            {
                Target = Path.Combine(_installDirectory, "workspace"),
                Name = "Eclipse Workspace",
                Comment = "Eclipse workspace directory.",
            });

            Logger.Debug("Shortcuts installed.");
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

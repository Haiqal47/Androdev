﻿//     This file is part of Androdev.
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
using Androdev.Core.Installer;
using Androdev.Core.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Androdev.Core
{
    /// <summary>
    /// Provides installation interface for Androdev.
    /// </summary>
    public sealed class InstallManager : IDisposable
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();

        private struct ExtractProcessInfo
        {
            public string StatusText { get; set; } 
            public int PercentageIncrement { get; set; }
            public double TotalFiles { get; set; }
        }

        private ExtractProcessInfo _extractInfo;
        private int _extractedFile;
        private readonly BackgroundWorker _bwWorker;

        #region Constructor
        public InstallManager()
        {
            // configure BGW
            _bwWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _bwWorker.DoWork += BwWorker_DoWork;
            _bwWorker.RunWorkerCompleted += BwWorker_RunWorkerCompleted;
        }
        #endregion

        #region Properties
        public string InstallRoot { get; private set; }

        public bool UacCompatibility { get; private set; }
        #endregion

        #region Protected Properties
        public string InstallDirectory
        {
            get { return Path.Combine(InstallRoot, "Androdev"); }
        }

        public string EclipsePath
        {
            get { return Path.Combine(InstallDirectory, "eclipse"); }
        }

        public string EclipsecFilePath
        {
            get { return Path.Combine(InstallDirectory, "eclipse\\eclipsec.exe"); }
        }

        public string EclipseWorkspacePath
        {
            get { return Path.Combine(InstallDirectory, "workspace"); }
        }

        public string AndroidSdkPath
        {
            get { return Path.Combine(InstallDirectory, "android-sdk"); }
        }
        #endregion

        #region Events
        public event EventHandler InstallStarted;
        public event EventHandler<ProgressChangedEventArgs> InstallProgressChanged;
        public event EventHandler InstallFinished;
        #endregion

        #region Methods
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
            InstallRoot = installDrive;
            UacCompatibility = (installDrive == systemDisk);
        }

        public void SetInstallRoot(string driveRoot)
        {
            if (string.IsNullOrEmpty(driveRoot))
            {
                throw new ArgumentException("Argument is null or empty", nameof(driveRoot));
            }

            InstallRoot = driveRoot;
            Logger.Debug("Changed install path to " + InstallDirectory);
        }

        public void SetUacCompatibility(bool enabled)
        {
            UacCompatibility = enabled;
            Logger.Debug("UAC Compatibility state: " + UacCompatibility.ToString());
        }
        
        private void OnProcessFile(object sender, ScanEventArgs args)
        {
            // check for cancellation
            args.ContinueRunning = !_bwWorker.CancellationPending;
            if (!args.ContinueRunning) return;

            // use Interlocked as atomic operation
            Interlocked.Increment(ref _extractedFile);
            var currentProgress = Interlocked.CompareExchange(ref _extractedFile, 0, 0) / _extractInfo.TotalFiles * 100;
            var overallProgress = (int)(currentProgress / 100 * 12) + _extractInfo.PercentageIncrement ;
            var currentFile = "Extracting: " + Commons.ElipsisText(Path.GetFileName(args.Name));

            // report progress
            WorkerReportProgress(overallProgress, Convert.ToInt32(currentProgress), _extractInfo.StatusText, currentFile);
        }
        #endregion

        #region Thread Worker
        private void WorkerReportProgress(int arg1, int arg2, string arg3, string arg4 = "")
        {
            InstallProgressChanged?.Invoke(this, new ProgressChangedEventArgs()
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
            if (InstallationHelpers.IsAndrodevExist(InstallRoot))
            {
                WorkerReportProgress(0, 0, "Existing installation detected.", "Please remove existing Androdev installation.");
                _bwWorker.CancelAsync();
                return;
            }

            // step 1 - install JDK
            Install_JavaDevelopmentKit();
            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 2 - config PATH
            Install_PathEnvironmentVars();
            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 3 - install Android SDK
            Install_AndroidSdk();
            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 4 - install Eclipse IDE
            Install_EclipseMarsTwo();
            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // add manifest if necessary
            if (UacCompatibility) Install_Manifests();
            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 5 - install ADT
            Install_ADT();
            if (_bwWorker.CancellationPending) return; // cancellation boundary

            // step 6 - configure Eclipse
            Install_ConfigureEclipse();
            
            // step 7 - create shortcuts
            Install_Shortcuts();

            // all finish
            WorkerReportProgress(0, 0, "Androdev has been installed successfully.");
            Logger.Info("Androdev has been installed successfully.");
        }
        #endregion
       
        #region Core Installation Features
        private void Install_JavaDevelopmentKit()
        {
            Logger.Info("==========Java Development Kit [Started]===========");
            WorkerReportProgress(14, 100, "Installing JDK 8u101...", "Pre-install checks...");

            // check if JDK is already installed
            if (InstallationHelpers.GetJavaInstallPath() != null)
            {
                WorkerReportProgress(14, 100, "Installing JDK 8u101...", "JDK already installed.");
                Logger.Debug("JDK is already installed.");
                return;
            }

            WorkerReportProgress(7, 99, "Installing JDK 8u101...", "Executing Windows Installer...");
            
            // install JDK
            if (!PackageInstaller.InstallJavaDevelopmentKit())
            {
                WorkerReportProgress(0, 0, "JDK 8u101 cannot be installed.", "Installation timeout.");
                _bwWorker.CancelAsync();
                return;
            }

            WorkerReportProgress(14, 100, "JDK 8u101 installed.");
            Logger.Info("==========Java Development Kit [Finished]===========");
        }

        private void Install_PathEnvironmentVars()
        {
            Logger.Info("==========Configuring Envirnment Variables [Started]===========");
            WorkerReportProgress(14, 99, "Configuring environment variables...");

            // find Java install path
            var javaHome = InstallationHelpers.GetJavaInstallPath();
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
            Logger.Info("==========Configuring Envirnment Variables [Finished]===========");
        }

        private void Install_AndroidSdk()
        {
            Logger.Info("==========Android SDK Tools [Started]===========");
            WorkerReportProgress(28, 0, "Installing Android SDK Tools...", "Preparing to unpack...");

            // create directory and set count to 0
            Directory.CreateDirectory(AndroidSdkPath);
            Interlocked.Exchange(ref _extractedFile, 0);
            Logger.Debug("Android SDK will extracted to: " + AndroidSdkPath);

            // define event subscriber
            var zipEvents = new FastZipEvents();
            zipEvents.ProcessFile += OnProcessFile;
            _extractInfo = new ExtractProcessInfo()
            {
                PercentageIncrement = 24,
                StatusText = "Installing Android SDK Tools...",
                TotalFiles = InstallationHelpers.AndroidSdkFileCount,
            };


            try
            {
                Logger.Debug("Extracting Android SDK Tools...");

                // extract files
                var androidZip = new FastZip(zipEvents);
                androidZip.ExtractZip(InstallationHelpers.AndroidSdkPath, AndroidSdkPath, FastZip.Overwrite.Always, name => true, null, null, true);

                Logger.Debug("Android SDK Tools installed.");
                WorkerReportProgress(42, 100, "Android SDK Tools installed.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                WorkerReportProgress(0, 0, "Android SDK Tools cannot be installed.");
                _bwWorker.CancelAsync();
            }
            Logger.Info("==========Android SDK Tools [Finished]===========");
        }

        private void Install_EclipseMarsTwo()
        {
            Logger.Info("==========Eclipse Mars 2 IDE [Started]===========");
            WorkerReportProgress(42, 0, "Installing Eclipse Mars 2...", "Preparing to unpack...");
            
            // create output dir and set count to 0
            Directory.CreateDirectory(EclipsePath);
            Interlocked.Exchange(ref _extractedFile, 0);
            Logger.Debug("Eclipse IDE will extracted to: " + EclipsePath);

            // define event subscriber
            var zipEvents = new FastZipEvents();
            zipEvents.ProcessFile += OnProcessFile;
            _extractInfo = new ExtractProcessInfo()
            {
                PercentageIncrement = 32,
                StatusText = "Installing Eclipse Mars 2...",
                TotalFiles = InstallationHelpers.EclipseIdeFileCount,
            };

            try
            {
                Logger.Debug("Extracting Eclipse IDE...");

                // extract files
                var eclipseZip = new FastZip(zipEvents);
                eclipseZip.ExtractZip(InstallationHelpers.EclipseIdePath, InstallDirectory, FastZip.Overwrite.Always, name => true, null, null, true);

                Logger.Debug("Eclipse IDE installed.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                WorkerReportProgress(0, 0, "Eclipse IDE cannot be installed.");
                _bwWorker.CancelAsync();
            }

            // do post installation
            PackageInstaller.EclipsePostInstall(InstallDirectory, EclipseWorkspacePath);

            Logger.Debug("Eclipse configuration initialized successfully.");
            Logger.Info("==========Eclipse Mars 2 IDE [Finished]===========");
        }

        private void Install_Manifests()
        {
            Logger.Info("==========UAC Elevation Manifest [Started]===========");
            WorkerReportProgress(56, 63, "Creating manifest...");

            // create manifest for SDK Manager
            InstallationHelpers.CreateManifestFile(Path.Combine(InstallDirectory, "android-sdk\\SDK Manager.exe.manifest"));
            Logger.Debug("Manifest added to SDK Manager.");

            // create manifest for AVD Manager
            InstallationHelpers.CreateManifestFile(Path.Combine(InstallDirectory, "android-sdk\\AVD Manager.exe.manifest"));
            Logger.Debug("Manifest added to AVD Manager.");

            // create manifest for Eclipse IDE
            InstallationHelpers.CreateManifestFile(Path.Combine(InstallDirectory, "eclipse\\eclipse.exe.manifest"));
            Logger.Debug("Manifest added to Eclipse IDE.");

            // create manifest for Eclipse Command Line
            InstallationHelpers.CreateManifestFile(Path.Combine(InstallDirectory, "eclipse\\eclipsec.exe.manifest"));
            Logger.Debug("Manifest added to Eclipse Command Line.");

            WorkerReportProgress(56, 100, "Manifest installed.");
            Logger.Info("==========UAC Elevation Manifest [Finished]===========");
        }

        private void Install_ADT()
        {
            Logger.Info("==========Android Developer Tools Plugin [Started]===========");
            WorkerReportProgress(56, 30, "Initializing Eclipse configuration...");
            
            // install ADT
            WorkerReportProgress(56, 63, "Installing Android Developer Tools...");
            if (!PackageInstaller.InstallAdt(EclipsecFilePath))
            {
                WorkerReportProgress(0, 0, "Cannot install Android Developer Tools.");
                Logger.Error("Cannot install Android Developer Tools.");
                _bwWorker.CancelAsync();
                return;
            }

            Logger.Debug("ADT installed successfully.");
            WorkerReportProgress(70, 100, "Android Developer Tools installed.");
            Logger.Info("==========Android Developer Tools Plugin [Finished]===========");
        }

        private void Install_ConfigureEclipse()
        {
            Logger.Info("==========Configure Eclipse IDE for First Time [Started]===========");
            WorkerReportProgress(70, 63, "Configuring Eclipse IDE...");

            // variables
            var eclipseConfigService = new EclipseConfigurator(InstallDirectory);

            // configure SDK path
            if (!eclipseConfigService.ConfigureSdkPath(AndroidSdkPath))
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
            Logger.Info("==========Configure Eclipse IDE for First Time [Finished]===========");
        }

        private void Install_Shortcuts()
        {
            Logger.Info("==========Install Shortcuts on Desktop [Started]===========");
            WorkerReportProgress(86, 24, "Installing shortcuts...");
            Logger.Debug("Installing shortcuts...");

            FastIo.CreateShortcut(new ShortcutProperties()
            {
                Target = Path.Combine(InstallDirectory, "android-sdk\\SDK Manager.exe"),
                Name = "Android SDK Tools",
                Comment = "Launch Android SDK Tools.",
                IconFile = Path.Combine(InstallDirectory, "android-sdk\\tools\\emulator.exe"),
            });

            FastIo.CreateShortcut(new ShortcutProperties()
            {
                Target = Path.Combine(InstallDirectory, "eclipse\\eclipse.exe"),
                Name = "Eclipse Mars for Android",
                Comment = "Launch Eclipse Mars IDE for Android.",
            });
            
            FastIo.CreateShortcut(new ShortcutProperties()
            {
                Target = Path.Combine(InstallDirectory, "workspace"),
                Name = "Eclipse Workspace",
                Comment = "Eclipse workspace directory.",
            });

            Logger.Debug("Shortcuts installed.");
            WorkerReportProgress(100, 100, "Shortcuts installed.");
            Logger.Info("==========Install Shortcuts on Desktop [Finished]===========");
        }
        #endregion

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        private void Dispose(bool disposing)
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

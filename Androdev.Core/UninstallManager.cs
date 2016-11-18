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
using Androdev.Core.Installer;
using Androdev.Core.IO;

namespace Androdev.Core
{
    /// <summary>
    /// Provides uninstallation interface for Androdev.
    /// </summary>
    public sealed class UninstallManager : IDisposable
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();
        private static readonly PathService Paths = PathService.Instance;
        private readonly BackgroundWorker _bwWorker;

        #region Properties
        /// <summary>
        /// Drive root path.
        /// </summary>
        public string InstallRoot
        {
            get { return Paths.InstallRoot; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Argument is null or empty", nameof(InstallRoot));

                PathService.Initialize(value);
                Logger.Debug("Changed install root to "  + value);
            }
        }
        #endregion
        
        #region Constructor
        public UninstallManager()
        {
            // configure BGW
            _bwWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true,
            };
            _bwWorker.DoWork += BwWorker_DoWork;
            _bwWorker.RunWorkerCompleted += BwWorker_RunWorkerCompleted;
            _bwWorker.ProgressChanged += BwWorker_ProgressChanged;
        }

        #endregion

        #region Events
        public event EventHandler UninstallStarted;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler UninstallFinished;
        #endregion

        #region Methods
        /// <summary>
        /// Starts uninstallation process.
        /// </summary>
        public void BeginUninstall()
        {
            if (_bwWorker.IsBusy)
                throw new InvalidOperationException("Thread is busy!");

            UninstallStarted?.Invoke(this, EventArgs.Empty);
            _bwWorker.RunWorkerAsync();
            Logger.Debug("Uninstall process started.");
        }
        /// <summary>
        /// Stops uninstallation process.
        /// </summary>
        public void EndUninstall()
        {
            if (!_bwWorker.IsBusy) return;
            Logger.Debug("Stopping uninstall process.");
            _bwWorker.CancelAsync();
        }
        /// <summary>
        /// Report progress to view.
        /// </summary>
        private void WorkerReportProgress(int overall, string status)
        {
            var state = new ProgressChangedEventArgs()
            {
                OverallProgressPercentage = overall,
                StatusText = status,
            };
            _bwWorker.ReportProgress(20, state);
        }
        #endregion

        #region Thread Worker Subscriber
        private void BwWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UninstallFinished?.Invoke(this, EventArgs.Empty);
        }

        private void BwWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, (ProgressChangedEventArgs) e.UserState);
        }

        private void BwWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // check existing installation
            if (InstallationHelpers.IsAndrodevExist(InstallRoot))
            {
                WorkerReportProgress(0,"Existing installation detected.");
                _bwWorker.CancelAsync();
                return;
            }

            // delete all files
            using (var enumer = FastIo.EnumerateFiles(Paths.InstallPath, SearchOption.AllDirectories).GetEnumerator())
            {
                var errorAttempt = 0;
                while (enumer.MoveNext())
                {
                    // checks if current is null
                    if (enumer.Current == null) continue;

                    // checks for deleteion attempt
                    if (errorAttempt >= 10)
                    {
                        WorkerReportProgress(0, "Cannot remove Androdev. See log file.");
                        return;
                    }
                    
                    try
                    {
                        // try to delete
                        File.Delete(enumer.Current.FullPath);
                        WorkerReportProgress(10, enumer.Current.Name);
                    }
                    catch (Exception ex)
                    {
                        // unable to delete
                        errorAttempt++;
                        WorkerReportProgress(0,enumer.Current.Name + ": delete file failed.");
                        Logger.Error(ex);
                    }
                } // end while
            } // end using

            // delete old directory
            try
            {
                Directory.Delete(Paths.InstallPath, true);
            }
            catch (Exception ex)
            {
                WorkerReportProgress(0, "Delete folder failed.");
                Logger.Error(ex);
            }

            // all finish
            WorkerReportProgress(0, "Androdev has been uninstalled.");
            Logger.Info("Androdev has been uninstalled.");
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
        }
        #endregion
    }
}

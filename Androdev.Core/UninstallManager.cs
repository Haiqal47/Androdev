using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Androdev.Core.Installer;

namespace Androdev.Core
{
    /// <summary>
    /// Provides uninstallation interface for Androdev.
    /// </summary>
    public sealed class UninstallManager : IDisposable
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();

        private readonly BackgroundWorker _bwWorker;

        #region Constructor
        public UninstallManager()
        {
            // configure BGW
            _bwWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _bwWorker.DoWork += BwWorker_DoWork;
            _bwWorker.RunWorkerCompleted += BwWorker_RunWorkerCompleted;
        }
        #endregion

        #region Events
        public event EventHandler UninstallStarted;
        public event EventHandler<ProgressChangedEventArgs> UninstallProgressChanged;
        public event EventHandler UninstallFinished;
        #endregion

        #region Thread Worker Subscriber
        private void BwWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UninstallFinished?.Invoke(this, EventArgs.Empty);
        }

        private void BwWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // check existing installation
            if (InstallationHelpers.IsAndrodevExist(InstallRoot))
            {
                WorkerReportProgress(0, 0, "Existing installation detected.", "Please remove existing Androdev installation.");
                _bwWorker.CancelAsync();
                return;
            }

            

            // all finish
            WorkerReportProgress(0, 0, "Androdev has been installed successfully.");
            Logger.Info("Androdev has been installed successfully.");
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

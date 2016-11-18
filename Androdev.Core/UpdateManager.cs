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
using System.Net;
using Androdev.Core.Args;
using Androdev.Core.IO;

namespace Androdev.Core
{
    /// <summary>
    /// Provides update interface.
    /// </summary>
    public sealed class UpdateManager : IDisposable
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();
        private PathService _paths;
        private readonly BackgroundWorker _bwDownloader;
        
        #region Events
        public event EventHandler UpdateStarted; 
        public event EventHandler<DownloadStartedEventArgs> DownloadStarted;
        public event EventHandler<UpdateProgressChangedEventArgs> ProgressChanged;
        public event EventHandler UpdateFinished;
        #endregion

        #region Constructor
        public UpdateManager(string root)
        {
            _bwDownloader = new BackgroundWorker { WorkerSupportsCancellation = true };
            _bwDownloader.DoWork += Worker_DoWork;
            _bwDownloader.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _bwDownloader.ProgressChanged += Worker_ProgressChanged;

            _paths = new PathService(root);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts update.
        /// </summary>
        public void StartUpdate()
        {
            if (_bwDownloader.IsBusy)
                throw new InvalidOperationException("Thread is busy!");

            _bwDownloader.RunWorkerAsync();
            UpdateStarted?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Stops update.
        /// </summary>
        public void StopUpdate()
        {
            if (_bwDownloader.IsBusy)
                _bwDownloader.CancelAsync();
        }
        /// <summary>
        /// Update download progress
        /// </summary>
        private void UpdateProgress(long downloaded, int progress)
        {
            var arg = new UpdateProgressChangedEventArgs()
            {
                Downloaded = Commons.RoundByteSize(downloaded),
                ProgressPercentage = progress,
            };
            _bwDownloader.ReportProgress(1, arg);
        }
        /// <summary>
        /// Update download information
        /// </summary>
        private void UpdateDownloadInfo(string filename, long downloadSize, string queue)
        {
            var arg = new DownloadStartedEventArgs()
            {
                FileName = filename,
                FileSize = Commons.RoundByteSize(downloadSize),
                Queue = queue,
            };
            _bwDownloader.ReportProgress(2, arg);
        }
        #endregion

        #region Core Update Methods
        private void Update_DeleteOld()
        {
            // remove files
            Logger.Debug("Attempt to remove Androdev binary files.");
            using (var enumer = FastIo.EnumerateFiles(_paths.InstallPath, SearchOption.AllDirectories).GetEnumerator())
            {
                while (enumer.MoveNext())
                {
                    // checks if current is null
                    if (enumer.Current == null) continue;
                    try
                    {
                        // try to delete
                        File.Delete(enumer.Current.FullPath);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Can't delete dependecy.", ex);
                    }
                } // end while
            } // end using

            // remove folder
            Logger.Debug("Attempt to remove Androdev binaries directory.");
            try
            {
                Directory.Delete(_paths.BinariesPath, true);
                Logger.Debug("Removed Androdev binaries directory.");
            }
            catch (Exception ex)
            {
                Logger.Error("Can't delete Androdev binaries directory.", ex);
            }
        }

        private void Update_DownloadThis(int index)
        {
            // prepare variables
            HttpWebRequest fileRequest;
            HttpWebResponse fileResponse = null;
            var downloadUri = new Uri(PathService.GetUrlByIndex(index));

            // request file
            fileRequest = (HttpWebRequest)WebRequest.Create(downloadUri);
            try
            {
                fileResponse = (HttpWebResponse)fileRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            // check for null
            if (fileResponse == null)
            {

                _bwDownloader.CancelAsync();
                return;
            }

            // cancellation boundary
            if (_bwDownloader.CancellationPending) return;

            // prepare variables
            var fileSize = fileResponse.ContentLength;
            var fileName = Commons.GetFilenameFromUri(downloadUri);
            var fullFilePath = Path.Combine(_paths.BinariesPath, fileName);

            // update progress
            Logger.Debug($"Requesting file: {fileName} ({fileSize})");
            UpdateDownloadInfo(fileName, fileSize, $"{index + 1} of 4");

            // get response stream and open output stream
            using (var remoteStream = fileResponse.GetResponseStream())
            using (var fs = new FileStream(fullFilePath, FileMode.Create))
            {
                // no response, break
                if (remoteStream == null) return;

                // prepare variables
                var bytesProcessed = 0L;
                int bytesRead;
                var buffer = new byte[1024];

                do
                {
                    // cancellation boundary
                    if (_bwDownloader.CancellationPending) return;

                    // read data from server
                    bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                    // write buffer
                    fs.Write(buffer, 0, bytesRead);

                    // calculate and report
                    var progressPercentage = (double)bytesProcessed / fileSize * 100;
                    UpdateProgress(bytesProcessed, Convert.ToInt32(progressPercentage));

                    // do while there is a data
                    bytesProcessed += bytesRead;
                } while (bytesRead != 0);
            }
        }
        #endregion  

        #region BackgroundWorker
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateFinished?.Invoke(this, EventArgs.Empty);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                ProgressChanged?.Invoke(this, (UpdateProgressChangedEventArgs)e.UserState);
            }
            else
            {
                DownloadStarted?.Invoke(this, (DownloadStartedEventArgs)e.UserState);
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            // remove old bin folder
            Update_DeleteOld();
            if (_bwDownloader.CancellationPending) return;

            // create new bin folder
            Logger.Debug("Fresh binaries directory created.");
            Directory.CreateDirectory(_paths.BinariesPath);

            // download each files
            for (int i = 0; i < 5; i++)
            {
                if (_bwDownloader.CancellationPending) return; // cancellation boundary

                // download the file
                Update_DownloadThis(i);
            }
        }
        #endregion

        #region IDisposable Support
        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                _bwDownloader?.Dispose();
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

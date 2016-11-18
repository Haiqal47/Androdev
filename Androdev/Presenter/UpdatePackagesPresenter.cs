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
using System.Windows.Forms;
using Androdev.Core;
using Androdev.Localization;
using Androdev.Model;
using Androdev.View;
using System.IO;
using System.Net;

namespace Androdev.Presenter
{
    public class UpdatePackagesPresenter : IDisposable
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();
        private readonly BackgroundWorker _bwDownloader;

        private readonly UpdatePackagesModel _model;
        private readonly UpdatePackagesView _view;
        
        private delegate void UpdateDownloadInfoDelegate(string filename, long downloadSize, string queue);
        private delegate void UpdateProgressDelegate(long downloaded, int progress);

        public UpdatePackagesModel Model
        {
            get { return _model; }
        }

        public UpdatePackagesPresenter(UpdatePackagesView view)
        {
            _view = view;
            _model = new UpdatePackagesModel();

            _bwDownloader = new BackgroundWorker {WorkerSupportsCancellation = true};
            _bwDownloader.DoWork += BwDownloaderOnDoWork;
            _bwDownloader.RunWorkerCompleted += BwDownloaderOnRunWorkerCompleted;

            ConfigureCancelButtonDelegate();
        }

        #region BackgroundWorker
        private void BwDownloaderOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            // remove old bin folder
            var binDirectory = Path.Combine(Commons.GetBaseDirectoryPath(), "bin");
            try
            {
                if (Directory.Exists(binDirectory))
                {
                    Directory.Delete(binDirectory, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            // create new bin folder
            Directory.CreateDirectory(binDirectory);

            // download each files
            for (int i = 0; i < 5; i++)
            {
                // cancelled
                if (_bwDownloader.CancellationPending) return;

                // prepare variables
                HttpWebRequest fileRequest;
                HttpWebResponse fileResponse = null;

                try // try to get file online
                {
                    // request file
                    var downloadUri = new Uri(PathService.GetUrlByIndex(i));
                    fileRequest = (HttpWebRequest)WebRequest.Create(downloadUri);
                    fileResponse = (HttpWebResponse)fileRequest.GetResponse();

                    // prepare variables
                    var fileSize = fileResponse.ContentLength;
                    var fileName = Commons.GetFilenameFromUri(downloadUri);
                    var outputFile = (i == 0 ? "android-sdk.zip" : Path.Combine(binDirectory, fileName));

                    Logger.Debug(string.Format("Requesting file: {0} ({1})", fileName, fileSize));
                    UpdateDownloadInfo(fileName, fileSize, string.Format("{0} of 4", i + 1));

                    // get response stream and open output stream
                    using (var remoteStream = fileResponse.GetResponseStream())
                    using (var fs = new FileStream(outputFile, FileMode.Create))
                    {
                        // no response, break
                        if (remoteStream == null) return;

                        // prepare variables
                        var nRead = 0;
                        var bytesRead = 0;
                        var buffer = new byte[4096];

                        do
                        {
                            // cancelled
                            if (_bwDownloader.CancellationPending) return;

                            // write buffer
                            nRead += bytesRead;
                            fs.Write(buffer, 0, bytesRead);

                            // calculate and report
                            var progressPercentage = (double)nRead / fileSize * 100;
                            UpdateProgress(nRead, Convert.ToInt32(progressPercentage));

                            // do while there is a data
                        } while ((bytesRead = remoteStream.Read(buffer, 0, buffer.Length)) != 0);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    UpdateDownloadInfo("An error occured. Download cancelled.", 0, "0 of 0");
                    _bwDownloader.CancelAsync();
                }
                finally
                {
                    fileResponse?.Close();
                }
            }
        }

        private void BwDownloaderOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Model.DownloadSizeText = "0 MB";
            Model.DownloadedText = "0 MB";
            Model.ProgressPercentage = 0;
            Model.QueueText = "4 of 4";
            Model.DownloadFileNameText = "...";
        }
        #endregion

        #region Methods
        private void UpdateProgress(long downloaded, int progress)
        {
            var inv = new UpdateProgressDelegate((delegate (long l, int i)
            {
                Model.DownloadedText = Commons.RoundByteSize(l);
                Model.ProgressPercentage = i;
            }));
            _view.Invoke(inv, downloaded, progress);
        }

        private void UpdateDownloadInfo(string filename, long downloadSize, string queue)
        {
            var inv = new UpdateDownloadInfoDelegate((delegate (string s, long size, string queue1)
            {
                Model.DownloadFileNameText = s;
                Model.DownloadSizeText = Commons.RoundByteSize(size);
                Model.QueueText = queue1;
            }));
            _view.Invoke(inv, filename, downloadSize, queue);
        }

        public void StartUpdate()
        {
            if (!_bwDownloader.IsBusy)
            {
                _bwDownloader.RunWorkerAsync();
            }
        }
        #endregion

        #region Delegates
        public EventHandler CancelButtonClickEventHandler;

        private void ConfigureCancelButtonDelegate()
        {
            if (MessageBox.Show(TextResource.CancelPackageDownloadText, TextResource.CancelPackageDownloadTitle,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            if (_bwDownloader.IsBusy)
            {
                _bwDownloader.CancelAsync();
            }
        }
        #endregion
        
        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
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
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

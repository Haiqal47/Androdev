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
        private readonly UpdatePackagesModel _model;
        private UpdatePackagesView _view;

        private BackgroundWorker bwDownloader;

        private delegate void UpdateDownloadInfoDelegate(string filename, long downloadSize, string queue);
        private delegate void UpdateProgressDelegate(long downloaded, int progress);

        // Model Properties
        public UpdatePackagesModel Model
        {
            get { return _model; }
        }

        public UpdatePackagesPresenter(UpdatePackagesView view)
        {
            _view = view;
            _model = new UpdatePackagesModel();

            bwDownloader = new BackgroundWorker {WorkerSupportsCancellation = true};
            bwDownloader.DoWork += BwDownloaderOnDoWork;
            bwDownloader.RunWorkerCompleted += BwDownloaderOnRunWorkerCompleted;

            ConfigureCancelButtonDelegate();
        }

        #region BackgroundWorker
        private void BwDownloaderOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var binDirectory = Path.Combine(Commons.GetBaseDirectoryPath(), "bin");
            try
            {
                if (Directory.Exists(binDirectory))
                {
                    Directory.Delete(binDirectory, true);
                }
            }
            catch { /* ignored */ }

            Directory.CreateDirectory(binDirectory);

            for (int i = 0; i < 5; i++)
            {
                if (bwDownloader.CancellationPending) return;

                // prepare variables
                HttpWebRequest fileRequest;
                HttpWebResponse fileResponse = null;

                try // try to get file online
                {
                    // request file
                    var downloadUri = new Uri(UriProvider.GetUrlByIndex(i));
                    fileRequest = (HttpWebRequest)WebRequest.Create(downloadUri);
                    fileResponse = (HttpWebResponse)fileRequest.GetResponse();

                    // prepare variables
                    var fileSize = fileResponse.ContentLength;
                    var fileName = Commons.GetFilenameFromUri(downloadUri);
                    var outputFile = (i == 0 ? "android-sdk.zip" : Path.Combine(binDirectory, fileName));

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
                            if (bwDownloader.CancellationPending) return;

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
                catch
                {
                    // opps, error occured
                    doWorkEventArgs.Cancel = true;
                    bwDownloader.CancelAsync();
                    UpdateDownloadInfo("An error occured. Download cancelled.", 0, "0 of 0");
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
            if (!bwDownloader.IsBusy)
            {
                bwDownloader.RunWorkerAsync();
            }
        }

        public void StopUpdate()
        {
            if (bwDownloader.IsBusy)
            {
                bwDownloader.CancelAsync();
            }
        }
        #endregion

        #region Delegates
        public EventHandler CancelButtonClickEventHandler;

        private void ConfigureCancelButtonDelegate()
        {
            if (MessageBox.Show(TextResource.CancelPackageDownloadText, TextResource.CancelPackageDownloadTitle,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bwDownloader.CancelAsync();
            }
        }
        #endregion

        

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                bwDownloader?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            _disposedValue = true;
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UpdatePackagesPresenter() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

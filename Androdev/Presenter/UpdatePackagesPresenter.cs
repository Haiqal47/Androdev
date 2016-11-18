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
using Androdev.Core.Args;
using Androdev.Core.Installer;

namespace Androdev.Presenter
{
    public class UpdatePackagesPresenter : IDisposable
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();
        private readonly UpdateManager _manager;

        private readonly UpdatePackagesModel _model;
        private readonly UpdatePackagesView _view;
          
        public UpdatePackagesModel Model
        {
            get { return _model; }
        }

        public UpdatePackagesPresenter(UpdatePackagesView view)
        {
            _view = view;
            _model = new UpdatePackagesModel();
            _manager = new UpdateManager(InstallationHelpers.FindAndrodevInstallation());
            _manager.UpdateStarted += Manager_UpdateStarted;
            _manager.DownloadStarted += Manager_DownloadStarted;
            _manager.ProgressChanged += Manager_ProgressChanged;
            _manager.UpdateFinished += Manager_UpdateFinished;

            FormLoadEventHandler = FormLoad_Handler;
            CancelButtonClickEventHandler = CancelClick_Handler;
        }

        #region Update Manager Subscriber
        private void Manager_UpdateStarted(object sender, EventArgs e)
        {
            Model.DownloadFileNameText = "Starting...";
        }

        private void Manager_DownloadStarted(object sender, Core.Args.DownloadStartedEventArgs e)
        {
            Model.DownloadFileNameText = e.FileName;
            Model.DownloadSizeText = e.FileSize;
            Model.QueueText = e.Queue;
        }

        private void Manager_ProgressChanged(object sender, UpdateProgressChangedEventArgs e)
        {
            Model.ProgressPercentage = e.ProgressPercentage;
            Model.DownloadedText = e.Downloaded;
        }

        private void Manager_UpdateFinished(object sender, EventArgs e)
        {
            Model.DownloadFileNameText = "...";
            Model.DownloadSizeText = "0 MB";
            Model.DownloadedText = "0 MB";
            Model.QueueText = "4 of 4";
            Model.ProgressPercentage = 0;
        }
        #endregion

        #region Delegates
        public EventHandler CancelButtonClickEventHandler;
        public EventHandler FormLoadEventHandler;

        private void FormLoad_Handler(object sender, EventArgs e)
        {
            _manager.StartUpdate();
        }

        private void CancelClick_Handler(object sender, EventArgs e)
        {
            if (MessageBox.Show(TextResource.CancelPackageDownloadText, TextResource.CancelPackageDownloadTitle,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            _manager.StopUpdate();
        }
        #endregion
        
        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                if (_manager != null)  _manager.Dispose();
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


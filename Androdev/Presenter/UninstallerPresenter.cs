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
using System.Windows.Forms;
using Androdev.Core;
using Androdev.Core.Installer;
using Androdev.Core.IO;
using Androdev.Model;
using Androdev.View;
using Androdev.Localization;

namespace Androdev.Presenter
{
    public class UninstallerPresenter : IDisposable
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();
        private readonly UninstallerModel _model;
        private readonly UninstallerView _view;

        private readonly UninstallManager _manager;

        public UninstallerModel Model
        {
            get { return _model; }
        }

        public UninstallerPresenter(UninstallerView view)
        {
            _view = view;
            _model = new UninstallerModel();

            var dataSource = FastIo.GetAvailiableDrives();
            _view.DrivesDataSource = dataSource;
            _view.SelectedDriveIndex = InstallationHelpers.FindAndrodevInstallation(dataSource);

            _manager = new UninstallManager();
            _manager.UninstallStarted += UninstallManager_UninstallStarted;
            _manager.ProgressChanged += UninstallManager_ProgressChanged;
            _manager.UninstallFinished += UninstallManager_UninstallFinished;

            ConfigureDelegates();
        }

        #region BackgroundWorker Subscriber
        private void UninstallManager_UninstallStarted(object sender, EventArgs eventArgs)
        {
            _model.UninstallButtonEnabled = false;
            _model.CboDrivesEnabled = false;
        }

        private void UninstallManager_ProgressChanged(object sender, Core.ProgressChangedEventArgs e)
        {
            _model.FileName = Commons.ElipsisText(e.StatusText);
        }

        private void UninstallManager_UninstallFinished(object sender, EventArgs eventArgs)
        {
            _model.UninstallButtonEnabled = true;
            _model.CboDrivesEnabled = true;
        }
        #endregion

        #region Delegates
        public EventHandler StartUninstallationEventHandler;

        private void ConfigureDelegates()
        {
            StartUninstallationEventHandler = StartUninstallationClick_Handler;
        }

        private void StartUninstallationClick_Handler(object sender, EventArgs e)
        {
            if (!InstallationHelpers.IsAndrodevExist(_view.SelectedDriveName))
            {
                Logger.Debug("Existing Androdev installation not found.");
                MessageBox.Show(TextResource.NoExistingInstallationText, TextResource.NoExistingInstallationTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (MessageBox.Show(TextResource.UninstallConfirmationText, TextResource.UninstallConfirmationTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                Logger.Debug("User cancelled uninstallation.");
                return;
            }

          _manager.BeginUninstall();
        }
        #endregion

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                if (_manager != null) _manager.Dispose();
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

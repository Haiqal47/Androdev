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
using Androdev.Core.IO;
using Androdev.Model;
using Androdev.View;
using Androdev.Core.Diagostic;
using Androdev.Localization;

namespace Androdev.Presenter
{
    public class UninstallerPresenter : IDisposable
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();
        private readonly UninstallerModel _model;
        private readonly UninstallerView _view;

        private readonly BackgroundWorker _bwWorker;

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
            for (int i = 0; i < dataSource.Length; i++)
            {
                if (InstallationHelpers.IsAndrodevDirectoryExist(dataSource[i].Name))
                {
                    _view.SelectedDriveIndex = i;
                }
            }

            _bwWorker = new BackgroundWorker {WorkerReportsProgress = true};
            _bwWorker.DoWork += BwWorker_DoWork;
            _bwWorker.RunWorkerCompleted += BwWorker_RunWorkerCompleted;
            _bwWorker.ProgressChanged += BwWorker_ProgressChanged;

            ConfigureUninstallButtonEventHandler();
        }
        
        #region BackgroundWorker
        private void BwWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _model.UninstallButtonEnabled = true;
            _model.CboDrivesEnabled = true;
        }

        private void BwWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _model.FileName = Commons.ElipsisText(e.UserState.ToString());
        }

        private void BwWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var deletePath = Path.Combine(e.Argument.ToString(), "Androdev");
                using (var enumer = FastIo.EnumerateFiles(deletePath, SearchOption.AllDirectories).GetEnumerator())
                {
                    while (enumer.MoveNext())
                    {
                        if (enumer.Current == null) continue;
                        File.Delete(enumer.Current.FullPath);
                        _bwWorker.ReportProgress(10, enumer.Current.Name);
                    }
                }
                Directory.Delete(deletePath, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        #endregion

        #region Delegates
        public EventHandler StartUninstallationEventHandler;

        private void ConfigureUninstallButtonEventHandler()
        {
            if (InstallationHelpers.IsAndrodevDirectoryExist(_view.SelectedDriveName))
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

            if (_bwWorker.IsBusy) return;
            Logger.Debug("User started Androdev uninstallation.");
            _bwWorker.RunWorkerAsync(_view.SelectedDriveName);
            _model.UninstallButtonEnabled = false;
        }
        #endregion

        #region IDisposable Support
        private bool _disposedValue;

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

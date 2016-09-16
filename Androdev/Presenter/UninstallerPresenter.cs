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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
        private static readonly LogManager _logManager = LogManager.GetClassLogger();
        private readonly UninstallerModel _model;
        private readonly UninstallerView _view;

        private readonly BackgroundWorker bwWorker;

        public UninstallerModel Model
        {
            get { return _model; }
        }

        public UninstallerPresenter(UninstallerView view)
        {
            _view = view;
            _model = new UninstallerModel();

            var dataSource = FastIO.GetAvailiableDrives();
            _view.DrivesDataSource = dataSource;
            for (int i = 0; i < dataSource.Length; i++)
            {
                if (InstallationHelpers.IsAndrodevDirectoryExist(dataSource[i].Name))
                {
                    _view.SelectedDriveIndex = i;
                }
            }

            bwWorker = new BackgroundWorker {WorkerReportsProgress = true};
            bwWorker.DoWork += BwWorker_DoWork;
            bwWorker.RunWorkerCompleted += BwWorker_RunWorkerCompleted;
            bwWorker.ProgressChanged += BwWorker_ProgressChanged;

            ConfigureUninstallButtonEventHandler();
        }
        
        #region BackgroundWorker
        private void BwWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _model.UninstallButtonEnabled = true;
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
                using (var enumer = FastIO.EnumerateFiles(deletePath, SearchOption.AllDirectories).GetEnumerator())
                {
                    while (enumer.MoveNext())
                    {
                        if (enumer.Current == null) continue;
                        File.Delete(enumer.Current.FullPath);
                        bwWorker.ReportProgress(10, enumer.Current.Name);
                    }
                }
                Directory.Delete(deletePath, true);
            }
            catch (Exception ex)
            {
                _logManager.Error("Exception occured on Uninstall process Thread.", ex);
            }
        }
        #endregion

        #region Delegates
        public EventHandler StartUninstallationEventHandler;

        private void ConfigureUninstallButtonEventHandler()
        {
            if (MessageBox.Show(TextResource.UninstallConfirmationText, TextResource.UninstallConfirmationTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                return;
            }
            if (InstallationHelpers.IsAndrodevDirectoryExist(_view.SelectedDriveName))
            {
                MessageBox.Show(TextResource.NoExistingInstallationText, TextResource.NoExistingInstallationTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!bwWorker.IsBusy)
            {
                bwWorker.RunWorkerAsync(_view.SelectedDriveName);
                _model.UninstallButtonEnabled = false;
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
                bwWorker?.Dispose();
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

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
using Androdev.Core;
using Androdev.Localization;
using Androdev.Model;
using Androdev.View;
using Androdev.View.Dialogs;
using System;
using System.Windows.Forms;

namespace Androdev.Presenter
{
    public class InstallerPresenter : IDisposable
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();
        private readonly InstallerModel _model;
        private readonly InstallerView _view;

        private readonly InstallManager _installManager;

        public InstallerModel Model
        {
            get { return _model; }
        }

        public InstallerPresenter(InstallerView view)
        {
            _view = view;
            _model = new InstallerModel();

            _installManager = new InstallManager();
            _installManager.InstallFinished += InstallManager_InstallFinished;
            _installManager.InstallProgressChanged += InstallManager_InstallProgressChanged;
            _installManager.InstallStarted += InstallManager_InstallStarted;

            ConfigureDelegates();
        }

        #region Install Manager Subscriber
        private void InstallManager_InstallStarted(object sender, EventArgs e)
        {
            UpdateSetupButton(1, true);
            _model.LoadingAnimationVisible = true;
            ResetUi();
        }

        private void InstallManager_InstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            var invoker = new Action<InstallProgressChangedEventArgs>(args =>
            {
                _model.ProgressStyle = (e.CurrentProgressPercentage == 99 ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks);
                _model.CurrentProgressPercentage = e.CurrentProgressPercentage;
                _model.OverallProgressPercentage = e.OverallProgressPercentage;
                _model.StatusText = e.StatusText;
                _model.DescriptionText = e.ExtraStatusText;
            });
            _view.Invoke(invoker, e);
        }

        private void InstallManager_InstallFinished(object sender, EventArgs e)
        {
            UpdateSetupButton(0, true);
            _model.LoadingAnimationVisible = false;
            ResetUi();
        }
        #endregion

        #region Methods
        private void ResetUi()
        {
            _model.CurrentProgressPercentage = 0;
            _model.DescriptionText = "";
            _model.OverallProgressPercentage = 0;
            _model.ProgressStyle = ProgressBarStyle.Blocks;
        }

        private void UpdateSetupButton(int imgIndex, bool enabled)
        {
            _model.SetupButtonImageIndex = imgIndex;
            _model.SetupButtonEnabled = enabled;

            switch (imgIndex)
            {
                case 0:
                    _model.SetupButtonText = "Start installation";
                    break;
                case 1:
                    _model.SetupButtonText = "Stop installation";
                    break;
            }
        }
        #endregion

        #region Delegates
        public EventHandler SetupClickEventHandler;
        public EventHandler UpdatePackagesClickEventHandler;
        public EventHandler SettingsClickEventHandler;
        public EventHandler HelpClickEventHandler;
        public EventHandler AboutClickEventHandler;

        private void ConfigureDelegates()
        {
            SetupClickEventHandler = InstallHandler;
            UpdatePackagesClickEventHandler = UpdatePackagesHandler;
            SettingsClickEventHandler = SettingsClickHandler;
            HelpClickEventHandler = HelpHandler;
            AboutClickEventHandler = AboutHandler;
        }

        private void InstallHandler(object sender, EventArgs e)
        {
            switch (_model.SetupButtonImageIndex)
            {
                case 0:
                    Logger.Debug("User starts installation process.");
                    UpdateSetupButton(0, false);
                    _installManager.BeginInstall();
                    break;

                case 1:
                    Logger.Debug("User stops installation process.");
                    UpdateSetupButton(1, false);
                    _installManager.EndInstall();
                    break;
            }
        }

        private void UpdatePackagesHandler(object sender, EventArgs e)
        {
            if (MessageBox.Show(TextResource.UpdateConfirmationText, TextResource.UpdateConfirmationTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                return;

            using (var frm = new UpdatePackagesView())
            {
                Logger.Debug("User confirmed to update packages.");
                frm.ShowDialog();
            }
        }

        private void SettingsClickHandler(object sender, EventArgs e)
        {
            using (var frm = new InstallConfigDialog())
            {
                frm.InstallRoot = _installManager.InstallRoot;
                frm.UacCompatibility = _installManager.UacCompatibility;
                if (frm.ShowDialog() != DialogResult.OK) return;

                try
                {
                    _installManager.SetInstallRoot(frm.InstallRoot);
                    _installManager.SetUacCompatibility(frm.UacCompatibility);
                    UpdateSetupButton(0, true);
                    Logger.Debug(string.Format("Install parameter changed. Root:{0}, UAC:{1}", frm.InstallRoot, frm.UacCompatibility));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    MessageBox.Show(TextResource.CantChangeConfigText, TextResource.CantChangeConfigTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void HelpHandler(object sender, EventArgs e)
        {
            MessageBox.Show(TextResource.HelpUnderConstructionText, TextResource.HelpUnderConstructionTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AboutHandler(object sender, EventArgs e)
        {
            using (var frm = new AboutDialog())
            {
                frm.ShowDialog();
            }
        }
        #endregion

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue) return;
            if (disposing)
            {
                _installManager?.Dispose();
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

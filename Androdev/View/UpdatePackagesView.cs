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
using System.Windows.Forms;
using Androdev.Core;
using Androdev.Localization;
using Androdev.Presenter;

namespace Androdev.View
{
    public partial class UpdatePackagesView : Form
    {
        private readonly UpdatePackagesPresenter _presenter;

        public UpdatePackagesView()
        {
            InitializeComponent();

            _presenter = new UpdatePackagesPresenter(this);
            ConfigureWiring();
        }

        #region Form Events
        #endregion

        #region Methods
        private void ConfigureWiring()
        {
            Load += _presenter.FormLoadEventHandler;
            cmdCancel.Click += _presenter.CancelButtonClickEventHandler;

            lblDownloaded.DataBindings.Add("Text", _presenter.Model, "DownloadedText");
            lblDownloadFileName.DataBindings.Add("Text", _presenter.Model, "DownloadFileNameText");
            lblDownloadSize.DataBindings.Add("Text", _presenter.Model, "DownloadSizeText");
            lblQueue.DataBindings.Add("Text", _presenter.Model, "QueueText");
            prgProgress.DataBindings.Add("Value", _presenter.Model, "ProgressPercentage");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null) components.Dispose();
                if (_presenter != null) _presenter.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

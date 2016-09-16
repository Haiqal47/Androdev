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
        UpdatePackagesPresenter _presenter;

        public UpdatePackagesView()
        {
            InitializeComponent();

            _presenter = new UpdatePackagesPresenter(this);
            ConfigureWiring();
        }

        private void ConfigureWiring()
        {
            cmdCancel.Click += _presenter.CancelButtonClickEventHandler;

            lblDownloaded.DataBindings.Add("Text", _presenter.Model, "DownloadedText");
            lblDownloadFileName.DataBindings.Add("Text", _presenter.Model, "DownloadFileNameText");
            lblDownloadSize.DataBindings.Add("Text", _presenter.Model, "DownloadSizeText");
            lblQueue.DataBindings.Add("Text", _presenter.Model, "QueueText");
        }

        #region Form Events
        private void FrmUpdate_Load(object sender, EventArgs e)
        {
            _presenter.StartUpdate();
        }
        #endregion
    }
}

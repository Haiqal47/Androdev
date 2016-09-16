using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Androdev.Core;
using Androdev.Presenter;

namespace Androdev.View
{
    public partial class UninstallerView : Form
    {
        private readonly UninstallerPresenter _presenter;

        public UninstallerView()
        {
            InitializeComponent();

            _presenter = new UninstallerPresenter(this);
            ConfigureWiring();
        }

        #region Properties
        public object DrivesDataSource
        {
            get { return cboDrive.DataSource; }
            set { cboDrive.DataSource = value; }
        }

        public string SelectedDriveName
        {
            get { return cboDrive.Text; }
        }

        public int SelectedDriveIndex
        {
            get { return cboDrive.SelectedIndex; }
            set { cboDrive.SelectedIndex = value; }
        }
        #endregion

        private void ConfigureWiring()
        {
            cmdInstallationCleaner.Click += _presenter.StartUninstallationEventHandler;
            cmdInstallationCleaner.DataBindings.Add("Enabled", _presenter.Model, "UninstallButtonEnabled");
            lblProcessedFile.DataBindings.Add("Text", _presenter.Model, "FileName");
            cboDrive.DataBindings.Add("Enabled", _presenter.Model, "CboDrivesEnabled");
        }

    }
}

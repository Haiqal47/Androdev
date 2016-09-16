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

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
using System.Windows.Forms;
using Androdev.Presenter;

namespace Androdev.View
{
    public partial class InstallerView : Form
    {
        private readonly InstallerPresenter _presenter;

        public InstallerView()
        {
            InitializeComponent();

            _presenter = new InstallerPresenter(this);
            ConfigureWiring();
        }

        private void ConfigureWiring()
        {
            cmdAbout.Click += _presenter.AboutClickEventHandler;
            cmdHelp.Click += _presenter.HelpClickEventHandler;
            cmdOnlineInstaller.Click += _presenter.UpdatePackagesClickEventHandler;
            cmdSettings.Click += _presenter.SettingsClickEventHandler;
            cmdSetup.Click += _presenter.SetupClickEventHandler;

            cmdSetup.DataBindings.Add("Text", _presenter.Model, "SetupButtonText");
            cmdSetup.DataBindings.Add("Enabled", _presenter.Model, "SetupButtonEnabled");
            cmdSetup.DataBindings.Add("ImageIndex", _presenter.Model, "SetupButtonImageIndex");

            prgCurrent.DataBindings.Add("Value", _presenter.Model, "CurrentProgressPercentage");
            prgCurrent.DataBindings.Add("Style", _presenter.Model, "ProgressStyle");
            prgOverall.DataBindings.Add("Value", _presenter.Model, "OverallProgressPercentage");

            lblStatus.DataBindings.Add("Text", _presenter.Model, "StatusText");
            lblExtraDesc.DataBindings.Add("Text", _presenter.Model, "DescriptionText");

            picStatus.DataBindings.Add("Visible", _presenter.Model, "LoadingAnimationVisible");
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
    }
}

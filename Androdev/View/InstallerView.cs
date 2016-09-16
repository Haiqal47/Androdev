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
   
    }
}

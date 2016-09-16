using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Androdev.Model
{
    public class InstallerModel : ModelBase
    {
        private ProgressBarStyle _prgStyle;
        private int _currentProgress;
        private int _overallProgress;
        private string _statusText;
        private string _descriptionText;
 
        private bool _picLoadingVisible;

        private bool _setupButtonEnabled;
        private int _setupButtonImageIndex;
        private string _setupButtonText;

        public InstallerModel()
        {
            _setupButtonEnabled = true;
            _statusText = "Ready.";
            _setupButtonText = "Start installation";
        }

        public ProgressBarStyle ProgressStyle
        {
            get { return _prgStyle; }
            set
            {
                _prgStyle = value;
                OnPropertyChanged(nameof(ProgressStyle));
            }
        }

        public int CurrentProgressPercentage
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgressPercentage));
            }
        }

        public int OverallProgressPercentage
        {
            get { return _overallProgress; }
            set
            {
                _overallProgress = value;
                OnPropertyChanged(nameof(OverallProgressPercentage));
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public string DescriptionText
        {
            get { return _descriptionText; }
            set
            {
                _descriptionText = value;
                OnPropertyChanged(nameof(DescriptionText));
            }
        }

        public int SetupButtonImageIndex
        {
            get { return _setupButtonImageIndex; }
            set
            {
                _setupButtonImageIndex = value;
                OnPropertyChanged(nameof(SetupButtonImageIndex));
            }
        }

        public bool SetupButtonEnabled
        {
            get { return _setupButtonEnabled; }
            set
            {
                _setupButtonEnabled = value;
                OnPropertyChanged(nameof(SetupButtonEnabled));
            }
        }

        public string SetupButtonText
        {
            get { return _setupButtonText; }
            set
            {
                _setupButtonText = value;
                OnPropertyChanged(nameof(SetupButtonText));
            }
        }

        public bool LoadingAnimationVisible
        {
            get { return _picLoadingVisible; }
            set
            {
                _picLoadingVisible = value;
                OnPropertyChanged(nameof(LoadingAnimationVisible));
            }
        }
    }
}

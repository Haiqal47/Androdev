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

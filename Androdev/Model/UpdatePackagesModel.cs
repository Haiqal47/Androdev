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
    public class UpdatePackagesModel : ModelBase
    {
        private string _downloadedText;
        private int _progressPercentage;
        private string _downloadFileName;
        private string _downloadSize;
        private string _queueText;

        public string DownloadedText
        {
            get { return _downloadedText; }
            set
            {
                _downloadedText = value;
                OnPropertyChanged(nameof(DownloadedText));
            }
        }

        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set
            {
                _progressPercentage = value;
                OnPropertyChanged(nameof(ProgressPercentage));
            }
        }

        public string DownloadFileNameText
        {
            get { return _downloadFileName; }
            set
            {
                _downloadFileName = value;
                OnPropertyChanged(nameof(DownloadFileNameText));
            }
        }

        public string DownloadSizeText
        {
            get { return _downloadSize; }
            set
            {
                _downloadSize = value;
                OnPropertyChanged(nameof(DownloadSizeText));
            }
        }

        public string QueueText
        {
            get { return _queueText; }
            set
            {
                _queueText = value;
                OnPropertyChanged(nameof(QueueText));
            }
        }
    }
}

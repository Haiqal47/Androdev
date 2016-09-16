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

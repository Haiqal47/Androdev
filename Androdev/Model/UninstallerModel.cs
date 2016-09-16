using System;
using System.Collections.Generic;
using System.Text;
using Androdev.Core;

namespace Androdev.Model
{
    public class UninstallerModel : ModelBase
    {
        private bool _uninstallButtonEnabled;
        private bool _cboDrivesEnabled;
        private string _filename;

        public bool UninstallButtonEnabled
        {
            get { return _uninstallButtonEnabled; }
            set
            {
                _uninstallButtonEnabled = value;
                OnPropertyChanged(nameof(UninstallButtonEnabled));
            }
        }

        public bool CboDrivesEnabled
        {
            get { return _cboDrivesEnabled; }
            set
            {
                _cboDrivesEnabled = value;
                OnPropertyChanged(nameof(CboDrivesEnabled));
            }
        }

        public string FileName
        {
            get { return Commons.ElipsisText(_filename); }
            set
            {
                _filename = value;
                OnPropertyChanged(nameof(FileName));
            }
        }
        
    }
}

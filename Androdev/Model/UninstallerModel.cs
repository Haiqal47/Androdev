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
using Androdev.Core;

namespace Androdev.Model
{
    public class UninstallerModel : ModelBase
    {
        private bool _uninstallButtonEnabled;
        private bool _cboDrivesEnabled;
        private string _filename;
        private string _status;

        public UninstallerModel()
        {
            _uninstallButtonEnabled = true;
            _cboDrivesEnabled = true;
            _filename = "...";
            _status = "...";
        }

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
            get { return _filename; }
            set
            {
                _filename = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _filename = value;
                OnPropertyChanged(nameof(Status));
            }
        }

    }
}

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
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;

namespace AndroUtility
{
    [SecurityCritical]
    public class ListViewEx : ListView
    {
        private const uint WmErasebkgnd = 0x14;

        private delegate void ChangeItemDelegate(int index, int imageIndex, string[] subitems);
        private delegate string[] GetListItemDelegate(int index);

        public ListViewEx()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != WmErasebkgnd)
                base.OnNotifyMessage(m);
        }

        public void SafeAddItem(ListViewItem item)
        {
            if (InvokeRequired)
            {
                var addDelegate = new Action<ListViewItem>(SafeAddItem);
                Invoke(addDelegate, item);
            }
            else
            {
                Items.Add(item);
            }
        }

        public string[] SafeGetItem(int index)
        {
            if (InvokeRequired)
            {
                var getDelegate = new GetListItemDelegate(SafeGetItem);
                return (string[])Invoke(getDelegate, index);
            }
            else
            {
                var subitemsList = new List<string>();
                for (var i = 0; i < Items[index].SubItems.Count; i++)
                {
                    subitemsList.Add(Items[index].SubItems[i].Text);
                }
                return subitemsList.ToArray();
            }
        }

        public void SafeChangeItem(int index, int imageIndex = -1, string[] subitems = null)
        {
            if (InvokeRequired)
            {
                var getDelegate = new ChangeItemDelegate(SafeChangeItem);
                Invoke(getDelegate, index, imageIndex, subitems);
            }
            else
            {
                if (imageIndex != -1)
                    Items[index].ImageIndex = imageIndex;

                if (subitems == null) return;
                for (int i = 0; i < subitems.Length; i++)
                {
                    Items[index].SubItems[i].Text = subitems[i];
                } //End for loop
            } //End invoke logic
        }

    }
}

using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows.Forms;

namespace AndroUtility
{
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

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
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

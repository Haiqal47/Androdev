using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Androdev.Core.Diagostic;
using Androdev.Core.Native;
using IWshRuntimeLibrary;

namespace Androdev.Core.IO
{
    public class FastIO
    {
        internal const string UnicodePrefix = @"\\?\";

        public static IEnumerable<FileData> EnumerateFiles(string path, SearchOption option)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if ((option != SearchOption.AllDirectories) && (option != SearchOption.TopDirectoryOnly))
            {
                throw new ArgumentOutOfRangeException(nameof(option));
            }
            
            var winFindData = new WIN32_FIND_DATA();
            var qDirectories = new Queue<string>();
            qDirectories.Enqueue(Path.GetFullPath(path));

            while (qDirectories.Count > 0)
            {
                var currentPath = qDirectories.Dequeue();
                var currentSearch = Path.Combine(currentPath, "*");
                var hndFindFile = NativeMethods.FindFirstFile(currentSearch, winFindData);

                if (hndFindFile.IsInvalid) continue;
                do
                {
                    var curPath = Path.Combine(currentPath, winFindData.cFileName);
                    if ((winFindData.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        if ("." != winFindData.cFileName && ".." != winFindData.cFileName && option == SearchOption.AllDirectories)
                        {
                            qDirectories.Enqueue(curPath);
                        }
                    }
                    else
                    {
                        yield return new FileData(currentPath, winFindData);
                    }
                } while (NativeMethods.FindNextFile(hndFindFile, winFindData));
            }
        }

        public static DriveInfo[] GetAvailiableDrives()
        {
            var drives = DriveInfo.GetDrives();
            var dataSource = new List<DriveInfo>();
            for (var i = 0; i < drives.Length; i++)
            {
                // is ready?
                if (!drives[i].IsReady) continue;
                // is NTFS?
                if (drives[i].DriveFormat != "NTFS") continue;
                // has more than 2GB free space?
                if (drives[i].AvailableFreeSpace < 2000) continue;

                dataSource.Add(drives[i]);
            }
            return dataSource.Count > 0 ? dataSource.ToArray() : null;
        }

        public static void CreateShortcut(string targetExe, string shortcutName, string desc, string iconPath = null)
        {
            var shortcutLocation = Path.Combine(Commons.GetDesktopPath(), shortcutName + ".lnk");
            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = desc;
            if (iconPath != null) shortcut.IconLocation = iconPath;
            shortcut.TargetPath = targetExe;
            shortcut.Save();
        }
    }
}

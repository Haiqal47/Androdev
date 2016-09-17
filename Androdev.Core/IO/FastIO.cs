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
using System.IO;
using System.Security;
using Androdev.Core.Native;
using IWshRuntimeLibrary;

namespace Androdev.Core.IO
{
    [SecurityCritical]
    public static class FastIo
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();
        //private const string UnicodePrefix = @"\\?\";

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
            
            // queue first directory
            var winFindData = new WIN32_FIND_DATA();
            var qDirectories = new Queue<string>();
            qDirectories.Enqueue(Path.GetFullPath(path));

            while (qDirectories.Count > 0)
            {
                // dequeue path
                var currentPath = qDirectories.Dequeue();
                var hndFindFile = NativeMethods.FindFirstFile(Path.Combine(currentPath, "*"), winFindData);

                // if the handle is invalid, continue to next dir
                if (hndFindFile.IsInvalid) continue;
                do
                {
                    // found a file
                    var curPath = Path.Combine(currentPath, winFindData.cFileName);
                    if ((winFindData.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        // if the file is a Directory and AllDriectiories is selected, enqueue it.
                        if ("." != winFindData.cFileName && ".." != winFindData.cFileName && option == SearchOption.AllDirectories)
                        {
                            qDirectories.Enqueue(curPath);
                        }
                    }
                    else
                    {
                        // it's a file. Return it.
                        yield return new FileData(currentPath, winFindData);
                    }
                    // find next file
                } while (NativeMethods.FindNextFile(hndFindFile, winFindData));
            }
        }

        public static DriveInfo[] GetAvailiableDrives()
        {
            var drives = DriveInfo.GetDrives();
            var dataSource = new List<DriveInfo>();
            for (var i = 0; i < drives.Length; i++)
            {
                var currentDrive = drives[i];

                // is ready?
                if (!currentDrive.IsReady) continue;
                // is NTFS?
                if (currentDrive.DriveFormat != "NTFS") continue;
                // has more than 2GB free space?
                if (currentDrive.AvailableFreeSpace < 2000) continue;

                dataSource.Add(currentDrive);
                Logger.Debug("Found drive: " + currentDrive.Name);
            }
            return dataSource.Count > 0 ? dataSource.ToArray() : null;
        }

        public static void CreateShortcut(ShortcutProperties properties)
        {
            try
            {
                var shortcutLocation = Path.Combine(Commons.GetDesktopPath(), properties.Name + ".lnk");
                var shell = new WshShell();
                var shortcut = (IWshShortcut) shell.CreateShortcut(shortcutLocation);

                if (properties.IconFile != null) shortcut.IconLocation = properties.IconFile;
                shortcut.Description = properties.Comment;
                shortcut.TargetPath = properties.Target;

                shortcut.Save();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}

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
using System.IO;
using Androdev.Core.Native;

namespace Androdev.Core.IO
{
    /// <summary>
    /// File informations.
    /// </summary>
    [Serializable]
    public class FileData
    {
        #region Constructor
        internal FileData(string dir, WIN32_FIND_DATA findData)
        {
            Attributes = findData.dwFileAttributes;
            CreationTimeUtc = ConvertDateTime(findData.ftCreationTime_dwHighDateTime, findData.ftCreationTime_dwLowDateTime);
            LastAccessTimeUtc = ConvertDateTime(findData.ftLastAccessTime_dwHighDateTime, findData.ftLastAccessTime_dwLowDateTime);
            LastWriteTimeUtc = ConvertDateTime(findData.ftLastWriteTime_dwHighDateTime, findData.ftLastWriteTime_dwLowDateTime);
            Size = CombineHighLowInts(findData.nFileSizeHigh, findData.nFileSizeLow);
            Name = findData.cFileName;
            FullPath = Path.Combine(dir, findData.cFileName).TrimEnd(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
        #endregion

        #region Properties
        public FileAttributes Attributes { get; }

        public DateTime CreationTime => CreationTimeUtc.ToLocalTime();

        public DateTime CreationTimeUtc { get; }

        public DateTime LastAccesTime => LastAccessTimeUtc.ToLocalTime();

        public DateTime LastAccessTimeUtc { get; }

        public DateTime LastWriteTime => LastWriteTimeUtc.ToLocalTime();

        public DateTime LastWriteTimeUtc { get; }

        public long Size { get; }

        public string Name { get; }

        public string FullPath { get; }
        #endregion
       
        #region Methods
        private static long CombineHighLowInts(uint high, uint low)
        {
            return (((long)high) << 0x20) | low;
        }

        private static DateTime ConvertDateTime(uint high, uint low)
        {
            var fileTime = CombineHighLowInts(high, low);
            return DateTime.FromFileTimeUtc(fileTime);
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}

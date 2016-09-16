using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Androdev.Core.Native;

namespace Androdev.Core.IO
{
    [Serializable]
    public class FileData
    {
        public readonly FileAttributes Attributes;

        public DateTime CreationTime => this.CreationTimeUtc.ToLocalTime();

        public readonly DateTime CreationTimeUtc;

        public DateTime LastAccesTime => this.LastAccessTimeUtc.ToLocalTime();

        public readonly DateTime LastAccessTimeUtc;

        public DateTime LastWriteTime => this.LastWriteTimeUtc.ToLocalTime();

        public readonly DateTime LastWriteTimeUtc;

        public readonly long Size;

        public readonly string Name;

        public readonly string FullPath;

        public override string ToString()
        {
            return this.Name;
        }

        internal FileData(string dir, WIN32_FIND_DATA findData)
        {
            this.Attributes = findData.dwFileAttributes;
            this.CreationTimeUtc = ConvertDateTime(findData.ftCreationTime_dwHighDateTime, findData.ftCreationTime_dwLowDateTime);
            this.LastAccessTimeUtc = ConvertDateTime(findData.ftLastAccessTime_dwHighDateTime, findData.ftLastAccessTime_dwLowDateTime);
            this.LastWriteTimeUtc = ConvertDateTime(findData.ftLastWriteTime_dwHighDateTime, findData.ftLastWriteTime_dwLowDateTime);
            this.Size = CombineHighLowInts(findData.nFileSizeHigh, findData.nFileSizeLow);
            this.Name = findData.cFileName;
            this.FullPath = System.IO.Path.Combine(dir, findData.cFileName).TrimEnd(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        private static long CombineHighLowInts(uint high, uint low)
        {
            return (((long)high) << 0x20) | low;
        }

        private static DateTime ConvertDateTime(uint high, uint low)
        {
            var fileTime = CombineHighLowInts(high, low);
            return DateTime.FromFileTimeUtc(fileTime);
        }
    }
}

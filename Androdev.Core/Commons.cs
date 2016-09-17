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
using System.Security.Cryptography;
using System.Text;

namespace Androdev.Core
{
    public static class Commons
    {
        public const int Wait15MinMilis = 900000;
        
        public static string GetDesktopPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public static string GetBaseDirectoryPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string ElipsisText(string originalText, int neededLength = 32)
        {
            const string delimiter = "...";
            const int delimlen = 3;

            //no path provided
            if (String.IsNullOrEmpty(originalText))
            {
                return "";
            }

            int namelen = originalText.Length;
            int idealminlen = namelen + delimlen;

            //file name condensing
            if (neededLength < idealminlen)
            {
                return delimiter + originalText.Substring(0, (neededLength - (2 * delimlen))) + delimiter;
            }
            else
            {
                return originalText;
            }
        }

        public static string RoundByteSize(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0) return "0 " + suf[0];

            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(byteCount, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 2);
            return String.Format("{0} {1}", (Math.Sign(byteCount) * num), suf[place]);
        }

        public static string GetFilenameFromUri(Uri uriPath)
        {
            var relativePath = uriPath.AbsolutePath;
            var lastIndex = relativePath.LastIndexOf("/", StringComparison.Ordinal);
            var newpath = relativePath.Substring(lastIndex + 1);

            return newpath;
        }

        [Obsolete]
        public static bool CompareSha1Hash(string filePath, string hash)
        {
            StringBuilder formatted = new StringBuilder(2 * hash.Length);
            BufferedStream bs = new BufferedStream(File.OpenRead(filePath));
            SHA1Managed sha1 = new SHA1Managed();

            byte[] fileHash = sha1.ComputeHash(bs);
            foreach (byte b in fileHash)
            {
                formatted.AppendFormat("{0:X2}", b);
            }

            bs.Close();
            sha1.Clear();

            return hash == formatted.ToString();
        }

        public static string ToEclipseCompliantPath(string origPath)
        {
            var driveRootChar = origPath.Substring(0, 1);
            var relativePath = origPath.Substring(3);
            var sb = new StringBuilder();
            sb.Append(driveRootChar);
            sb.Append("\\:\\\\");
            sb.Append(relativePath.Replace("\\", "\\\\"));
            return sb.ToString();
        }
    }
}

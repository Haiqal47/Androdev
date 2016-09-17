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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Androdev.Core;

namespace Androdev.Tests
{
    [TestClass]
    public class CommonsTest
    {
        [TestMethod]
        public void ToEclipseCompliantPathTest()
        {
            var expected = "D\\:\\\\androdev";
            var original = "D:\\androdev";

            var output = Commons.ToEclipseCompliantPath(original);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetDesktopPathTest()
        {
            var desktopPath = Commons.GetDesktopPath();

            Assert.IsTrue(desktopPath.Contains("Desktop"));
        }

        [TestMethod]
        public void GetBaseDirectoryPathTest()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var output = Commons.GetBaseDirectoryPath();

            Assert.AreEqual(currentDirectory, output);
        }

        [TestMethod]
        public void ElipsisTextTest()
        {
            
        }

        [TestMethod]
        public void RoundByteSizeTest()
        {
            
        }

        [TestMethod]
        public void GetFilenameFromUriTest()
        {
 
        }


    }
}

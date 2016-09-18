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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Androdev.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Androdev.Tests
{
    [TestClass]
    public class LogManagerTests
    {
        [TestMethod]
        public void LogInfoTest()
        {
            Dow();
        }

        private void Dow()
        {
            Ddd();
        }

        private void Ddd()
        {
            var frame = new StackFrame(2);
            var caller = frame.GetMethod().Name;
            
            Assert.AreEqual(caller, nameof(LogInfoTest));
        }
    }
}

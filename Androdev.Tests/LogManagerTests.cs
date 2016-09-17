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

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
using System.Diagnostics;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Androdev.Core.IO
{
    /// <summary>
    /// Provides <see cref="Process"/> helper methods.
    /// </summary>
    public static class ProcessHelper
    {
        private static readonly LogManager Logger = LogManager.GetClassLogger();

        #region Methods
        /// <summary>
        /// Execute specified file and wait for exit. Listen to stdout and match it with given condition.
        /// </summary>
        /// <param name="filePath">Fullpath to file to be executed.</param>
        /// <param name="cmdLine">Additional command-line parameter.</param>
        /// <param name="ouputTextShouldTrue">Text to compare at end of execution.</param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public static bool RunWait(string filePath, string cmdLine, string ouputTextShouldTrue)
        {
            Process installerProcess = null;
            AutoResetEvent outputWaitHandle = null;
            try
            {
                installerProcess = new Process();
                outputWaitHandle = new AutoResetEvent(false);

                // config Process
                installerProcess.StartInfo = new ProcessStartInfo()
                {
                    Arguments = cmdLine,
                    FileName = filePath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                };

                // output buffer
                var outputBuffer = new StringBuilder();

                // reset event to manage async
                installerProcess.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                        outputWaitHandle?.Set();
                    else
                        outputBuffer.AppendLine(e.Data);
                };

                // start installer
                if (!installerProcess.Start()) return false;

                // redirect
                installerProcess.BeginOutputReadLine();

                // wait for executable
                var success = installerProcess.WaitForExit(Commons.Wait15MinMilis) &&
                              outputWaitHandle.WaitOne(Commons.Wait15MinMilis);
                // check if process is still running
                if (!installerProcess.HasExited)
                {
                    installerProcess.Kill();
                }

                // check output lines
                return success && outputBuffer.ToString().Contains(ouputTextShouldTrue);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            finally
            {
                installerProcess?.Close();
                outputWaitHandle?.Close();
            }
        }

        /// <summary>
        /// Execute specified file and wait for exit.
        /// </summary>
        /// <param name="filePath">Fullpath to file to be executed.</param>
        /// <param name="cmdLine">Additional command-line parameter.</param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public static bool RunWait(string filePath, string cmdLine)
        {
            Process installerProcess = null;
            try
            {
                installerProcess = new Process();

                // config Process
                installerProcess.StartInfo = new ProcessStartInfo()
                {
                    Arguments = cmdLine,
                    FileName = filePath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                };

                // start installer
                if (!installerProcess.Start()) return false;

                // wait for installer
                if (installerProcess.WaitForExit(Commons.Wait15MinMilis)) return true;

                // kill if installer running too long
                installerProcess.Kill();
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
            finally
            {
                installerProcess?.Close();
            }
        }
        #endregion
    }
}


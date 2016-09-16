using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Androdev.Core.Diagostic
{
    public class ProcessHelper
    {
        public static bool ExecuteProcessWaitAndCheckForOutput(string filePath, string cmdLine, string ouputTextShouldTrue)
        {
            bool shouldContinue = false;
            using (var installerProcess = new Process())
            {
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
                StringBuilder outputBuffer = new StringBuilder();

                // reset event to manage async
                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                {
                    installerProcess.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                            outputWaitHandle?.Set();
                        else
                            outputBuffer.AppendLine(e.Data);
                    };

                    // start installer
                    if (installerProcess.Start())
                    {
                        // redirect
                        installerProcess.BeginOutputReadLine();

                        // wait for executable
                        shouldContinue = installerProcess.WaitForExit(Commons.Wait15MinMilis) && outputWaitHandle.WaitOne(Commons.Wait15MinMilis);

                        // check if process is still running
                        if (!installerProcess.HasExited)
                        {
                            installerProcess.Kill();
                        }
                    }
                }

                // check output lines
                return shouldContinue && outputBuffer.ToString().Contains(ouputTextShouldTrue);
            }
        }

        public static bool ExecuteProcessAndWait(string filePath, string cmdLine)
        {
            using (var installerProcess = new Process())
            {
                // config Process
                installerProcess.StartInfo = new ProcessStartInfo()
                {
                    Arguments = cmdLine,
                    FileName = filePath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                };

                // start installer
                if (installerProcess.Start())
                {
                    // wait for installer
                    if (installerProcess.WaitForExit(Commons.Wait15MinMilis)) return true;

                    // kill if installer running too long
                    installerProcess.Kill();
                    return false;
                }
                return false;
            }
        }

    }
}


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Androdev.Core;

namespace Androdev.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var workspacePath = "D:\\testWorkspace";
            var eclipsecPath = "D:\\eclipse";

            var configService = new EclipseConfigurator(eclipsecPath, workspacePath);
            bool lastSuccess;

            // create workspace dir
            Directory.CreateDirectory(workspacePath);

            // initialize
            //lastSuccess = configService.InitializeEclipseConfiguration();
            //Console.WriteLine("Initialize Eclipse: " + lastSuccess);

            // set workspace path
            //lastSuccess = configService.ConfigureWorkspaceDirectory();
            //Console.WriteLine("Config workspace: " + lastSuccess);

            // install ADT
            //lastSuccess = configService.InstallAdt("D:\\android\\installer\\bin\\ADT-23.0.7.zip");
            //Console.WriteLine("Install ADT: " + lastSuccess);

            // config sdk path
            lastSuccess = configService.ConfigureSdkPath("D:\\android\\android-sdk");
            Console.WriteLine("Change SDK path: " + lastSuccess);

            // config code assist
            lastSuccess = configService.ConfigureCodeAssist();
            Console.WriteLine("Code assist: " + lastSuccess);

            Console.ReadKey();
        }
    }
}

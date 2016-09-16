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

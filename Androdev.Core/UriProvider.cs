using System;
using System.Collections.Generic;
using System.Text;

namespace Androdev.Core
{
    public static class UriProvider
    {
        public static readonly string AndroidSdkTools = "https://dl.google.com/android/android-sdk_r24.1.2-windows.zip";
        public static readonly string JavaDevelopmentKit = "http://download.oracle.com/otn-pub/java/jdk/8u101-b13/jdk-8u101-windows-i586.exe";
        public static readonly string AndroidDeveloperTools = "https://dl.google.com/android/ADT-23.0.7.zip";
        public static readonly string EclipseMars2 = "http://eclipse.mirror.rafal.ca/technology/epp/downloads/release/mars/2/eclipse-java-mars-2-win32.zip";

        public static string GetUrlByIndex(int index)
        {
            if (index != 0)
            {
                if (index == 1)
                {
                    return JavaDevelopmentKit;
                }
                if (index == 2)
                {
                    return AndroidDeveloperTools;
                }
                if (index == 3)
                {
                    return EclipseMars2;
                }
                return AndroidSdkTools;
            }
            return AndroidSdkTools;
        }
    }
}

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
namespace Androdev.Core
{
    public static class UriProvider
    {
        private const string AndroidSdkTools = "https://dl.google.com/android/android-sdk_r24.1.2-windows.zip";
        private const string JavaDevelopmentKit = "http://download.oracle.com/otn-pub/java/jdk/8u101-b13/jdk-8u101-windows-i586.exe";
        private const string AndroidDeveloperTools = "https://dl.google.com/android/ADT-23.0.7.zip";
        private const string EclipseMars2 = "http://eclipse.mirror.rafal.ca/technology/epp/downloads/release/mars/2/eclipse-java-mars-2-win32.zip";

        /// <summary>
        /// Gets file URI based on index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>0 : Android SDK, 1 : JDK, 2 : ADT, 3 : Eclipse Mars 2.</remarks>
        public static string GetUrlByIndex(int index)
        {
            switch (index)
            {
                case 1:
                    return JavaDevelopmentKit;
                case 2:
                    return AndroidDeveloperTools;
                case 3:
                    return EclipseMars2;
            }
            return AndroidSdkTools;
        }
    }
}

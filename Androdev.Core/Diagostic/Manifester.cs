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

namespace Androdev.Core.Diagostic
{
    public static class Manifester
    {
        private static readonly LogManager _logManager = LogManager.GetClassLogger();

        private static readonly string ManifestTemplate =
            "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
            "<assembly xmlns=\"urn:schemas-microsoft-com:asm.v1\" manifestVersion=\"1.0\">" +
            "    <trustInfo xmlns=\"urn:schemas-microsoft-com:asm.v3\">" +
            "        <security>" +
            "            <requestedPrivileges>" +
            "                <requestedExecutionLevel level=\"requireAdministrator\" uiAccess=\"false\"/>" +
            "            </requestedPrivileges>" +
            "        </security>" +
            "    </trustInfo>" +
            "</assembly>";

        public static void CreateManifestFile(string outputFile)
        {
            try
            {
                using (var fs = new StreamWriter(outputFile, false))
                {
                    fs.Write(ManifestTemplate);
                }
            }
            catch (Exception ex)
            {
                _logManager.Error(ex);
            }
        }
    }
}

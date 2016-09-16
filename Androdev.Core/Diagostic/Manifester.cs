using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Androdev.Core.Diagostic
{
    public static class Manifester
    {
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
            using (var fs = new StreamWriter(outputFile, false))
            {
                fs.Write(ManifestTemplate);
            }
        }
    }
}

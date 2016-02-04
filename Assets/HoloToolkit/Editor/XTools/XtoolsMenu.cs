using UnityEditor;

namespace HoloToolkit.XTools
{
    public static class XToolsMenu
    {
        [MenuItem("XTools/Launch Session Server", false)]
        public static void LaunchSessionServer()
        {
            Utilities.ExternalProcess.FindAndLaunch(@"HoloToolkit\XTools\SessionServer\SessionServer.exe", @"-local");
        }        
    }
}

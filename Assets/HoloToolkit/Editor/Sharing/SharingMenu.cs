using UnityEditor;

namespace HoloToolkit.Sharing
{
    public static class SharingMenu
    {
        [MenuItem("HoloToolkit/Launch Sharing Service", false)]
        public static void LaunchSessionServer()
        {
            Utilities.ExternalProcess.FindAndLaunch(@"HoloToolkit\Sharing\SharingService\SharingService.exe", @"-local");
        }        
    }
}
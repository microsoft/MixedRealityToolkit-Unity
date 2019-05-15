#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LinuxStandalone : IPlatformSupport
{
    public bool Check()
    {
#if UNITY_EDITOR
        var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        return activeBuildTarget == BuildTarget.StandaloneLinux || activeBuildTarget == BuildTarget.StandaloneLinux64 || activeBuildTarget == BuildTarget.StandaloneLinuxUniversal;
#else
        return Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.LinuxPlayer;
#endif
    }

    public bool Check(RuntimePlatform runtimePlatform)
    {
        return (runtimePlatform == RuntimePlatform.LinuxEditor || runtimePlatform == RuntimePlatform.LinuxPlayer) && Check();
    }

#if UNITY_EDITOR
    public bool Check(BuildTarget buildTarget)
    {
        return (buildTarget == BuildTarget.StandaloneLinux || buildTarget == BuildTarget.StandaloneLinux64 || buildTarget == BuildTarget.StandaloneLinuxUniversal) && Check();
    }
#endif
}

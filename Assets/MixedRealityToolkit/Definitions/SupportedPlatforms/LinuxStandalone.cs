#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LinuxStandalone : IPlatformSupport
{
    public bool IsEditorOrRuntimePlatform()
    {
#if UNITY_EDITOR
        var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        return activeBuildTarget == BuildTarget.StandaloneLinux || activeBuildTarget == BuildTarget.StandaloneLinux64 || activeBuildTarget == BuildTarget.StandaloneLinuxUniversal;
#else
        return Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.LinuxPlayer;
#endif
    }

    public bool IsCurrentRuntimePlatformSameAs(RuntimePlatform runtimePlatform)
    {
        return (runtimePlatform == RuntimePlatform.LinuxEditor || runtimePlatform == RuntimePlatform.LinuxPlayer) && IsEditorOrRuntimePlatform();
    }
}

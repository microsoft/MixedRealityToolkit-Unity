#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class WindowsStandalone : IPlatformSupport
{
    public bool Check()
    {
#if UNITY_EDITOR
        var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        return activeBuildTarget == BuildTarget.StandaloneWindows || activeBuildTarget == BuildTarget.StandaloneWindows64;
#else
        return Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
#endif
    }
    public bool Check(RuntimePlatform runtimePlatform)
    {
        return (runtimePlatform == RuntimePlatform.WindowsPlayer || runtimePlatform == RuntimePlatform.WindowsEditor) && Check();
    }

#if UNITY_EDITOR
    public bool Check(BuildTarget buildTarget)
    {
        return buildTarget == BuildTarget.StandaloneWindows || buildTarget == BuildTarget.StandaloneWindows64 && Check();
    }
#endif
}

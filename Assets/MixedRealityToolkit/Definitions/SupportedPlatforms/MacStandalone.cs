#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MacStandalone : IPlatformSupport
{
    public bool Check()
    {
#if UNITY_EDITOR
        var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        return activeBuildTarget == BuildTarget.StandaloneOSX;
#else
        return Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;
#endif
    }
    public bool Check(RuntimePlatform runtimePlatform)
    {
        return (runtimePlatform == RuntimePlatform.OSXPlayer || runtimePlatform == RuntimePlatform.OSXEditor) && Check();
    }

#if UNITY_EDITOR
    public bool Check(BuildTarget buildTarget)
    {
        return buildTarget == BuildTarget.StandaloneOSX && Check();
    }
#endif
}

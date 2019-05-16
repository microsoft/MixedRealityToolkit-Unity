#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MacStandalone : IPlatformSupport
{
    public bool IsThisEditorOrRuntimePlatform()
    {
#if UNITY_EDITOR
        var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        return activeBuildTarget == BuildTarget.StandaloneOSX;
#else
        return Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;
#endif
    }
    public bool IsThisTheCurrentRuntimePlatform(RuntimePlatform runtimePlatform)
    {
        return (runtimePlatform == RuntimePlatform.OSXPlayer || runtimePlatform == RuntimePlatform.OSXEditor) && IsThisEditorOrRuntimePlatform();
    }
}

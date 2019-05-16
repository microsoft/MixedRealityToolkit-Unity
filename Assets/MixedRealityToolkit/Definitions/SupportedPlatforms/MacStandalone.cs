#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MacStandalone : IPlatformSupport
{
    public bool IsEditorOrRuntimePlatform()
    {
#if UNITY_EDITOR
        var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        return activeBuildTarget == BuildTarget.StandaloneOSX;
#else
        return Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;
#endif
    }
    public bool IsCurrentRuntimePlatformSameAs(RuntimePlatform runtimePlatform)
    {
        return (runtimePlatform == RuntimePlatform.OSXPlayer || runtimePlatform == RuntimePlatform.OSXEditor) && IsEditorOrRuntimePlatform();
    }
}

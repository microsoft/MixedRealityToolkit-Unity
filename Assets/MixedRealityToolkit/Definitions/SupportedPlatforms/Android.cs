#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Android : IPlatformSupport
{
    public bool IsThisEditorOrRuntimePlatform()
    {
#if UNITY_EDITOR
        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
#else
        return Application.platform == RuntimePlatform.Android;
#endif
    }

    public bool IsThisTheCurrentRuntimePlatform(RuntimePlatform runtimePlatform)
    {
        return runtimePlatform == RuntimePlatform.Android && IsThisEditorOrRuntimePlatform();
    }
}

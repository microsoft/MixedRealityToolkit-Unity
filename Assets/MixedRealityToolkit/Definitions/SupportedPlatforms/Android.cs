#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Android : IPlatformSupport
{
    public bool IsEditorOrRuntimePlatform()
    {
#if UNITY_EDITOR
        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
#else
        return Application.platform == RuntimePlatform.Android;
#endif
    }

    public bool IsCurrentRuntimePlatformSameAs(RuntimePlatform runtimePlatform)
    {
        return runtimePlatform == RuntimePlatform.Android && IsEditorOrRuntimePlatform();
    }
}

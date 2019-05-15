#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Android : IPlatformSupport
{
    public bool Check()
    {
#if UNITY_EDITOR
        var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        return activeBuildTarget == BuildTarget.Android;
#else
        return Application.platform == RuntimePlatform.Android;
#endif
    }

    public bool Check(RuntimePlatform runtimePlatform)
    {
        return runtimePlatform == RuntimePlatform.Android && Check();
    }

#if UNITY_EDITOR
    public bool Check(BuildTarget buildTarget)
    {
        return buildTarget == BuildTarget.Android && Check();
    }
#endif
}

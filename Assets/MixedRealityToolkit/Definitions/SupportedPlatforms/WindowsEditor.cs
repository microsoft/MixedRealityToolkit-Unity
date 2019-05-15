#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class WindowsEditor : IPlatformSupport
{
    public bool Check()
    {
        return Application.platform == RuntimePlatform.WindowsEditor;
    }
    public bool Check(RuntimePlatform runtimePlatform)
    {
        return runtimePlatform == RuntimePlatform.WindowsEditor && Check();
    }

#if UNITY_EDITOR
    public bool Check(BuildTarget buildTarget)
    {
        return Check();
    }
#endif
}

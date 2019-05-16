#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class WindowsEditor : IPlatformSupport
{
    public bool IsThisEditorOrRuntimePlatform()
    {
        return Application.platform == RuntimePlatform.WindowsEditor;
    }
    public bool IsThisTheCurrentRuntimePlatform(RuntimePlatform runtimePlatform)
    {
        return runtimePlatform == RuntimePlatform.WindowsEditor && IsThisEditorOrRuntimePlatform();
    }
}

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class WindowsEditor : IPlatformSupport
{
    public bool IsEditorOrRuntimePlatform()
    {
        return Application.platform == RuntimePlatform.WindowsEditor;
    }
    public bool IsCurrentRuntimePlatformSameAs(RuntimePlatform runtimePlatform)
    {
        return runtimePlatform == RuntimePlatform.WindowsEditor && IsEditorOrRuntimePlatform();
    }
}

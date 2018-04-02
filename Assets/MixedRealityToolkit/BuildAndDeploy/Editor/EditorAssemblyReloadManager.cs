using UnityEditor;
using UnityEngine;

public static class EditorAssemblyReloadManager
{
    private static bool locked = false;

    public static bool LockReloadAssemblies
    {
        set
        {
            locked = value;

            if (locked)
            {
                EditorApplication.LockReloadAssemblies();
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Assembly reloading temporarily paused."));
            }
            else
            {
                EditorApplication.UnlockReloadAssemblies();
                EditorApplication.delayCall += () => AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Assembly reloading resumed."));
            }
        }
    }
}
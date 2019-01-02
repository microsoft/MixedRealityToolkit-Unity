using UnityEditor;

#if UNITY_EDITOR

[InitializeOnLoad]
internal class EditorProjectUtilities
{
    /// <summary>
    /// Static constructor that allows for executing code on project load.
    /// </summary>
    static EditorProjectUtilities()
    {
        CheckMinimumEditorVersion();
    }

    /// <summary>
    /// Checks that a supported version of Unity is being used with this project.
    /// </summary>
    /// <remarks>
    /// This method displays a message to the user allowing them to continue or to exit the editor.
    /// </remarks>
    public static void CheckMinimumEditorVersion()
    {
#if !UNITY_2018_3_OR_NEWER
        DisplayIncorrectEditorVersionDialog();
#endif
    }

    /// <summary>
    /// Displays a message indicating that a project was loaded in an unsupported version of Unity and allows the user
    /// to continu or exit.
    /// </summary>
    private static void DisplayIncorrectEditorVersionDialog()
    {
        if (!EditorUtility.DisplayDialog(
            "Mixed Reality Toolkit", 
            "The Mixed Reality Toolkit requires Unity 2018.3 or newer.\n\nUsing an older version of Unity may result in compile errors or incorrect behavior.", 
            "Continue", "Close Editor"))
        {
            EditorApplication.Exit(0);
        }
    }
}

#endif // UNITY_EDITOR
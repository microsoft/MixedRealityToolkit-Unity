using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(CullArea))]
public class CullAreaEditor : Editor
{
    private bool alignEditorCamera, showHelpEntries;

    private CullArea cullArea;
    
    private enum UP_AXIS_OPTIONS
    {
        SideScrollerMode = 0,
        TopDownOr3DMode = 1
    }

    private UP_AXIS_OPTIONS upAxisOptions;

    public void OnEnable()
    {
        cullArea = (CullArea) target;

        // Destroying the newly created cull area if there is already one existing
        if (FindObjectsOfType<CullArea>().Length > 1)
        {
            Debug.LogWarning("Destroying newly created cull area because there is already one existing in the scene.");

            DestroyImmediate(cullArea);

            return;
        }

        // Prevents the dropdown from resetting
        if (cullArea != null)
        {
            upAxisOptions = cullArea.YIsUpAxis ? UP_AXIS_OPTIONS.SideScrollerMode : UP_AXIS_OPTIONS.TopDownOr3DMode;
        }
    }
    
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        
        if (Application.isEditor && !Application.isPlaying)
        {
            OnInspectorGUIEditMode();
        }
        else
        {
            OnInspectorGUIPlayMode();
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    ///     Represents the inspector GUI when edit mode is active.
    /// </summary>
    private void OnInspectorGUIEditMode()
    {
        EditorGUI.BeginChangeCheck();

        #region DEFINE_UP_AXIS

        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Select game type", EditorStyles.boldLabel);
            upAxisOptions = (UP_AXIS_OPTIONS) EditorGUILayout.EnumPopup("Game type", upAxisOptions);
            cullArea.YIsUpAxis = (upAxisOptions == UP_AXIS_OPTIONS.SideScrollerMode);
            EditorGUILayout.EndVertical();
        }

        #endregion

        EditorGUILayout.Space();

        #region SUBDIVISION

        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Set the number of subdivisions", EditorStyles.boldLabel);
            cullArea.NumberOfSubdivisions = EditorGUILayout.IntSlider("Number of subdivisions", cullArea.NumberOfSubdivisions, 0, CullArea.MAX_NUMBER_OF_SUBDIVISIONS);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            if (cullArea.NumberOfSubdivisions != 0)
            {
                for (int index = 0; index < cullArea.Subdivisions.Length; ++index)
                {
                    if ((index + 1) <= cullArea.NumberOfSubdivisions)
                    {
                        string countMessage = (index + 1) + ". Subdivision: row / column count";

                        EditorGUILayout.BeginVertical();
                        cullArea.Subdivisions[index] = EditorGUILayout.Vector2Field(countMessage, cullArea.Subdivisions[index]);
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.Space();
                    }
                    else
                    {
                        cullArea.Subdivisions[index] = new UnityEngine.Vector2(1, 1);
                    }
                }
            }
        }

        #endregion

        EditorGUILayout.Space();

        #region UPDATING_MAIN_CAMERA

        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("View and camera options", EditorStyles.boldLabel);
            alignEditorCamera = EditorGUILayout.Toggle("Automatically align editor view with grid", alignEditorCamera);

            if (Camera.main != null)
            {
                if (GUILayout.Button("Align main camera with grid"))
                {
                    Undo.RecordObject(Camera.main.transform, "Align main camera with grid.");

                    float yCoord = cullArea.YIsUpAxis ? cullArea.Center.y : Mathf.Max(cullArea.Size.x, cullArea.Size.y);
                    float zCoord = cullArea.YIsUpAxis ? -Mathf.Max(cullArea.Size.x, cullArea.Size.y) : cullArea.Center.y;

                    Camera.main.transform.position = new Vector3(cullArea.Center.x, yCoord, zCoord);
                    Camera.main.transform.LookAt(cullArea.transform.position);
                }

                EditorGUILayout.LabelField("Current main camera position is " + Camera.main.transform.position.ToString());
            }

            EditorGUILayout.EndVertical();
        }

        #endregion

        if (EditorGUI.EndChangeCheck())
        {
            cullArea.RecreateCellHierarchy = true;

            AlignEditorView();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        showHelpEntries = EditorGUILayout.Foldout(showHelpEntries, "Need help with this component?");
        if (showHelpEntries)
        {
            EditorGUILayout.HelpBox("To find help you can either follow the tutorial or have a look at the forums by clicking on the buttons below.", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open the tutorial"))
            {
                Application.OpenURL("https://doc.photonengine.com/en-us/pun/current/demos-and-tutorials/package-demos/culling-demo");
            }
            if (GUILayout.Button("Take me to the forums"))
            {
                Application.OpenURL("https://forum.photonengine.com/categories/unity-networking-plugin-pun");
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    ///     Represents the inspector GUI when play mode is active.
    /// </summary>
    private void OnInspectorGUIPlayMode()
    {
        EditorGUILayout.LabelField("No changes allowed when game is running. Please exit play mode first.", EditorStyles.boldLabel);
    }

    public void OnSceneGUI()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(Screen.width - 110, Screen.height - 90, 100, 60));

        if (GUILayout.Button("Reset position"))
        {
            cullArea.transform.position = Vector3.zero;
        }

        if (GUILayout.Button("Reset scaling"))
        {
            cullArea.transform.localScale = new Vector3(25.0f, 25.0f, 25.0f);
        }

        GUILayout.EndArea();
        Handles.EndGUI();

        // Checking for changes of the transform
        if (cullArea.transform.hasChanged)
        {
            // Resetting position
            float posX = cullArea.transform.position.x;
            float posY = cullArea.YIsUpAxis ? cullArea.transform.position.y : 0.0f;
            float posZ = !cullArea.YIsUpAxis ? cullArea.transform.position.z : 0.0f;

            cullArea.transform.position = new Vector3(posX, posY, posZ);

            // Resetting scaling
            if (cullArea.Size.x < 1.0f || cullArea.Size.y < 1.0f)
            {
                float scaleX = (cullArea.transform.localScale.x < 1.0f) ? 1.0f : cullArea.transform.localScale.x;
                float scaleY = (cullArea.transform.localScale.y < 1.0f) ? 1.0f : cullArea.transform.localScale.y;
                float scaleZ = (cullArea.transform.localScale.z < 1.0f) ? 1.0f : cullArea.transform.localScale.z;
                
                cullArea.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

                Debug.LogWarning("Scaling on a single axis can not be lower than 1. Resetting...");
            }

            cullArea.RecreateCellHierarchy = true;

            AlignEditorView();
        }
    }
    
    /// <summary>
    ///     Aligns the editor view with the created grid.
    /// </summary>
    private void AlignEditorView()
    {
        if (!alignEditorCamera)
        {
            return;
        }

        // This creates a temporary game object in order to align the editor view.
        // The created game object is destroyed afterwards.
        GameObject tmpGo = new GameObject();

        float yCoord = cullArea.YIsUpAxis ? cullArea.Center.y : Mathf.Max(cullArea.Size.x, cullArea.Size.y);
        float zCoord = cullArea.YIsUpAxis ? -Mathf.Max(cullArea.Size.x, cullArea.Size.y) : cullArea.Center.y;

        tmpGo.transform.position = new Vector3(cullArea.Center.x, yCoord, zCoord);
        tmpGo.transform.LookAt(cullArea.transform.position);

        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.AlignViewToObject(tmpGo.transform);
        }

        DestroyImmediate(tmpGo);
    }
}
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit;

[CustomEditor(typeof(BoundsControl))]
[CanEditMultipleObjects]
public class BoundsControlInspector : Editor
{
    private SerializedProperty targetObject;
    private SerializedProperty boundsOverride;
    private SerializedProperty boundsCalculationMethod;
    private SerializedProperty activationType;
    private SerializedProperty controlPadding;
    private SerializedProperty flattenAxis;

    private SerializedProperty smoothingActive;
    private SerializedProperty rotateLerpTime;
    private SerializedProperty scaleLerpTime;

    // configs
    private SerializedProperty boxDisplayConfiguration;
    private SerializedProperty linksConfiguration;
    private SerializedProperty scaleHandlesConfiguration;
    private SerializedProperty rotationHandlesConfiguration;
    private SerializedProperty proximityEffectConfiguration;

    // debug
    private SerializedProperty debugText;
    private SerializedProperty hideElementsInHierarchyEditor;

    // events
    private SerializedProperty rotateStartedEvent;
    private SerializedProperty rotateStoppedEvent;
    private SerializedProperty scaleStartedEvent;
    private SerializedProperty scaleStoppedEvent;


    private BoundsControl boundsControl;


    private static bool showBoxConfiguration = false;
    private static bool showScaleHandlesConfiguration = false;
    private static bool showRotationHandlesConfiguration = false;
    private static bool showLinksConfiguration = false;
    private static bool showProximityConfiguration = false;

    private void OnEnable()
    {
        boundsControl = (BoundsControl)target;

        targetObject = serializedObject.FindProperty("targetObject");
        activationType = serializedObject.FindProperty("activation");
        boundsOverride = serializedObject.FindProperty("boundsOverride");
        boundsCalculationMethod = serializedObject.FindProperty("boundsCalculationMethod");
        flattenAxis = serializedObject.FindProperty("flattenAxis");
        controlPadding = serializedObject.FindProperty("boxPadding");

        smoothingActive = serializedObject.FindProperty("smoothingActive");
        rotateLerpTime = serializedObject.FindProperty("rotateLerpTime");
        scaleLerpTime = serializedObject.FindProperty("scaleLerpTime");

        boxDisplayConfiguration = serializedObject.FindProperty("boxDisplayConfiguration");
        linksConfiguration = serializedObject.FindProperty("linksConfiguration");
        scaleHandlesConfiguration = serializedObject.FindProperty("scaleHandlesConfiguration");
        rotationHandlesConfiguration = serializedObject.FindProperty("rotationHandlesConfiguration");
        proximityEffectConfiguration = serializedObject.FindProperty("handleProximityEffectConfiguration");

        debugText = serializedObject.FindProperty("debugText");
        hideElementsInHierarchyEditor = serializedObject.FindProperty("hideElementsInInspector");

        rotateStartedEvent = serializedObject.FindProperty("rotateStarted");
        rotateStoppedEvent = serializedObject.FindProperty("rotateStopped");
        scaleStartedEvent = serializedObject.FindProperty("scaleStarted");
        scaleStoppedEvent = serializedObject.FindProperty("scaleStopped");


    }

    public override void OnInspectorGUI()
    {
        if (target != null)
        {
            // notification section - first thing to show in bounds control component
            DrawRigidBodyWarning();

            // help url
            InspectorUIUtility.RenderHelpURL(target.GetType());

            //data section
            {
                //needed? serializedObject.Update();
                // EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(targetObject);
                EditorGUILayout.PropertyField(activationType);
                EditorGUILayout.PropertyField(boundsOverride);
                EditorGUILayout.PropertyField(boundsCalculationMethod);
                EditorGUILayout.PropertyField(controlPadding);
                EditorGUILayout.PropertyField(flattenAxis);


                EditorGUILayout.PropertyField(smoothingActive);
                EditorGUILayout.PropertyField(scaleLerpTime);
                EditorGUILayout.PropertyField(rotateLerpTime);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent("Visuals Configuration", "Bounds Control Visual configurations"), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                using (new EditorGUI.IndentLevelScope())
                {

                    showBoxConfiguration = DrawConfigFoldout(boxDisplayConfiguration, "Box Configuration", showBoxConfiguration);
                    showScaleHandlesConfiguration = DrawConfigFoldout(scaleHandlesConfiguration, "Scale Handles Configuration", showScaleHandlesConfiguration);
                    showRotationHandlesConfiguration = DrawConfigFoldout(rotationHandlesConfiguration, "Rotation Handles Configuration", showRotationHandlesConfiguration);
                    showLinksConfiguration = DrawConfigFoldout(linksConfiguration, "Links Configuration", showLinksConfiguration);
                    showProximityConfiguration = DrawConfigFoldout(proximityEffectConfiguration, "Proximity Configuration", showProximityConfiguration);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent("Events", "Bounds Control Events"), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                {
                    EditorGUILayout.PropertyField(rotateStartedEvent);
                    EditorGUILayout.PropertyField(rotateStoppedEvent);
                    EditorGUILayout.PropertyField(scaleStartedEvent);
                    EditorGUILayout.PropertyField(scaleStoppedEvent);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent("Debug", "Bounds Control Debug section"), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                {
                    //EditorGUILayout.PropertyField(debugText);
                    EditorGUILayout.PropertyField(hideElementsInHierarchyEditor);
                }
            }
        }
    }


    private bool DrawConfigFoldout(SerializedProperty configuration, string description, bool isCollapsed)
    {
        isCollapsed = EditorGUILayout.Foldout(isCollapsed, description, true, MixedRealityStylesUtility.BoldFoldoutStyle);
        if (isCollapsed)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(configuration);
                if (!configuration.objectReferenceValue.IsNull())
                {
                    MixedRealityInspectorUtility.DrawSubProfileEditor(configuration.objectReferenceValue, true);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        return isCollapsed;
    }

    private void DrawRigidBodyWarning()
    {    
        // check if rigidbody is attached - if so show warning in case input profile is not configured for individual collider raycast
        Rigidbody rigidBody = boundsControl.GetComponent<Rigidbody>();

        if (rigidBody != null)
        {
            MixedRealityInputSystemProfile profile = Microsoft.MixedReality.Toolkit.CoreServices.InputSystem?.InputSystemProfile;
            if (profile != null && profile.FocusIndividualCompoundCollider == false)
            {
                EditorGUILayout.Space();
                // show warning and button to reconfigure profile
                EditorGUILayout.HelpBox($"When using Bounds Control in combination with Rigidbody 'Focus Individual Compound Collider' must be enabled in Input Profile.", UnityEditor.MessageType.Warning);
                if (GUILayout.Button($"Enable 'Focus Individual Compound Collider' in Input Profile"))
                {
                    profile.FocusIndividualCompoundCollider = true;
                }

                EditorGUILayout.Space();
            }
        }
    }
}

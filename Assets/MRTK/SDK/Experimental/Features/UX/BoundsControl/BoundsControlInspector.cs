using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Editor;

[CustomEditor(typeof(BoundsControl))]
[CanEditMultipleObjects]
public class BoundsControlInspector : Editor
{
    private SerializedProperty targetObject;
    private SerializedProperty boundsOverride;
    private SerializedProperty boundsCalculationMethod;
    private SerializedProperty activationType;
    private SerializedProperty handlesIgnoreCollider;
    private SerializedProperty flattenAxis;

    // visuals
    private SerializedProperty drawTetherWhenManipulating;
    private SerializedProperty controlPadding;

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

    private void OnEnable()
    {
        boundsControl = (BoundsControl)target;

        targetObject = serializedObject.FindProperty("targetObject");
        boundsOverride = serializedObject.FindProperty("boundsOverride");
        boundsCalculationMethod = serializedObject.FindProperty("boundsCalculationMethod");
        activationType = serializedObject.FindProperty("activation");
        handlesIgnoreCollider = serializedObject.FindProperty("handlesIgnoreCollider");
        flattenAxis = serializedObject.FindProperty("flattenAxis");

        drawTetherWhenManipulating = serializedObject.FindProperty("drawTetherWhenManipulating");
        controlPadding = serializedObject.FindProperty("boxPadding");

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
            // check if rigidbody is attached - if so show warning in case input profile is not configured for individual collider raycast
            BoundsControl boundingBox = (BoundsControl)target;
            Rigidbody rigidBody = boundingBox.GetComponent<Rigidbody>();

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

            // help url
            InspectorUIUtility.RenderHelpURL(target.GetType());

            // data section
            //{
            //    //needed? serializedObject.Update();
            //    // EditorGUI.BeginChangeCheck();

            //    EditorGUILayout.PropertyField(targetObject);
            //    EditorGUILayout.PropertyField(boundsOverride);
            //    EditorGUILayout.PropertyField(boundsCalculationMethod);
            //    EditorGUILayout.PropertyField(activationType);
            //    EditorGUILayout.PropertyField(handlesIgnoreCollider);
            //    EditorGUILayout.PropertyField(flattenAxis);

            //    EditorGUILayout.Space();

            //    EditorGUILayout.Foldout(true, "Box Configuration", true, MixedRealityStylesUtility.BoldFoldoutStyle);
            //    using (new EditorGUI.IndentLevelScope())
            //    {
            //        EditorGUILayout.PropertyField(boxDisplayConfiguration);
            //        MixedRealityInspectorUtility.DrawSubProfileEditor(boxDisplayConfiguration.objectReferenceValue, true);
            //    }
            //}

        }

        DrawDefaultInspector();
    }

    //protected static void RenderFoldout(ref bool currentState, string title, Action renderContent)
    //{
    //    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        
    //    bool state = currentState;
    //    if (isValidPreferenceKey)
    //    {
    //        state = SessionState.GetBool(preferenceKey, currentState);
    //    }

    //    currentState = EditorGUILayout.Foldout(state, title, true, MixedRealityStylesUtility.BoldFoldoutStyle);

    //    if (isValidPreferenceKey && currentState != state)
    //    {
    //        SessionState.SetBool(preferenceKey, currentState);
    //    }

    //    if (currentState)
    //    {
    //        renderContent();
    //    }

    //    EditorGUILayout.EndVertical();
    //}


}

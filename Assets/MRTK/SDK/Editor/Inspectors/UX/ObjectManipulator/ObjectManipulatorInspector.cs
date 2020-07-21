//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//

using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom inspector for ObjectManipulator used to separate
    /// ObjectManipulator options into distinct foldout panels.
    /// </summary>
    [CustomEditor(typeof(ObjectManipulator))]
    [CanEditMultipleObjects]
    public class ObjectManipulatorInspector : UnityEditor.Editor
    {
        private SerializedProperty hostTransform;
        private SerializedProperty manipulationType;
        private SerializedProperty allowFarManipulation;

        private SerializedProperty oneHandRotationModeNear;
        private SerializedProperty oneHandRotationModeFar;

        private SerializedProperty twoHandedManipulationType;

        private SerializedProperty releaseBehavior;

        private SerializedProperty smoothingActive;
        private SerializedProperty moveLerpTime;
        private SerializedProperty rotateLerpTime;
        private SerializedProperty scaleLerpTime;

        private SerializedProperty onManipulationStarted;
        private SerializedProperty onManipulationEnded;
        private SerializedProperty onHoverEntered;
        private SerializedProperty onHoverExited;

        private SerializedProperty elasticConfigurationObject;

        bool oneHandedFoldout = true;
        bool twoHandedFoldout = true;
        bool constraintsFoldout = true;
        bool physicsFoldout = true;
        bool smoothingFoldout = true;
        bool elasticsFoldout = true;
        //bool sharedElasticConfigurationFoldout = false;
        bool eventsFoldout = true;

        public void OnEnable()
        {
            // General properties
            hostTransform = serializedObject.FindProperty("hostTransform");
            manipulationType = serializedObject.FindProperty("manipulationType");
            allowFarManipulation = serializedObject.FindProperty("allowFarManipulation");

            // One handed
            oneHandRotationModeNear = serializedObject.FindProperty("oneHandRotationModeNear");
            oneHandRotationModeFar = serializedObject.FindProperty("oneHandRotationModeFar");

            // Two handed
            twoHandedManipulationType = serializedObject.FindProperty("twoHandedManipulationType");

            // Physics
            releaseBehavior = serializedObject.FindProperty("releaseBehavior");

            // Smoothing
            smoothingActive = serializedObject.FindProperty("smoothingActive");
            moveLerpTime = serializedObject.FindProperty("moveLerpTime");
            rotateLerpTime = serializedObject.FindProperty("rotateLerpTime");
            scaleLerpTime = serializedObject.FindProperty("scaleLerpTime");

            // Manipulation Events
            onManipulationStarted = serializedObject.FindProperty("onManipulationStarted");
            onManipulationEnded = serializedObject.FindProperty("onManipulationEnded");
            onHoverEntered = serializedObject.FindProperty("onHoverEntered");
            onHoverExited = serializedObject.FindProperty("onHoverExited");

            // Elastic configuration (ScriptableObject)
            elasticConfigurationObject = serializedObject.FindProperty("elasticConfigurationObject");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(hostTransform);
            EditorGUILayout.PropertyField(manipulationType);
            EditorGUILayout.PropertyField(allowFarManipulation);

            var handedness = (ManipulationHandFlags)manipulationType.intValue;

            EditorGUILayout.Space();
            GUIStyle style = EditorStyles.foldout;
            FontStyle previousStyle = style.fontStyle;
            style.fontStyle = FontStyle.Bold;
            oneHandedFoldout = EditorGUILayout.Foldout(oneHandedFoldout, "One Handed Manipulation", true);

            if (oneHandedFoldout)
            {
                if (handedness.HasFlag(ManipulationHandFlags.OneHanded))
                {
                    EditorGUILayout.PropertyField(oneHandRotationModeNear);
                    EditorGUILayout.PropertyField(oneHandRotationModeFar);
                }
                else
                {
                    EditorGUILayout.HelpBox("One handed manipulation disabled. If you wish to enable one handed manipulation select it as a Manipulation Type above.", MessageType.Info);
                }
            }

            EditorGUILayout.Space();
            twoHandedFoldout = EditorGUILayout.Foldout(twoHandedFoldout, "Two Handed Manipulation", true);

            if (twoHandedFoldout)
            {
                if (handedness.HasFlag(ManipulationHandFlags.TwoHanded))
                {
                    EditorGUILayout.PropertyField(twoHandedManipulationType);
                }
                else
                {
                    EditorGUILayout.HelpBox("Two handed manipulation disabled. If you wish to enable two handed manipulation select it as a Manipulation Type above.", MessageType.Info);
                }
            }

            var mh = (ObjectManipulator)target;
            var rb = mh.HostTransform.GetComponent<Rigidbody>();

            EditorGUILayout.Space();
            constraintsFoldout = EditorGUILayout.Foldout(constraintsFoldout, "Constraints", true);

            if (constraintsFoldout)
            {
                if (EditorGUILayout.DropdownButton(new GUIContent("Add Constraint"), FocusType.Keyboard))
                {
                    // create the menu and add items to it
                    GenericMenu menu = new GenericMenu();

                    var type = typeof(TransformConstraint);
                    var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetLoadableTypes())
                        .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);

                    foreach (var derivedType in types)
                    {
                        menu.AddItem(new GUIContent(derivedType.Name), false, t => mh.gameObject.AddComponent((Type)t), derivedType);
                    }

                    menu.ShowAsContext();
                }

                var constraints = mh.GetComponents<TransformConstraint>();

                foreach (var constraint in constraints)
                {
                    EditorGUILayout.BeginHorizontal();
                    string constraintName = constraint.GetType().Name;
                    EditorGUILayout.LabelField(constraintName);
                    if (GUILayout.Button("Go to component"))
                    {
                        Highlighter.Highlight("Inspector", $"{ObjectNames.NicifyVariableName(constraintName)} (Script)");
                        EditorGUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space();
            physicsFoldout = EditorGUILayout.Foldout(physicsFoldout, "Physics", true);

            if (physicsFoldout)
            {
                if (rb != null)
                {
                    EditorGUILayout.PropertyField(releaseBehavior);
                }
                else
                {
                    EditorGUILayout.HelpBox("Physics options disabled. If you wish to enable physics options, add a Rigidbody component to this object.", MessageType.Info);
                }
            }

            EditorGUILayout.Space();
            smoothingFoldout = EditorGUILayout.Foldout(smoothingFoldout, "Smoothing", true);

            if (smoothingFoldout)
            {
                EditorGUILayout.PropertyField(smoothingActive);
                EditorGUILayout.PropertyField(moveLerpTime);
                EditorGUILayout.PropertyField(rotateLerpTime);
                EditorGUILayout.PropertyField(scaleLerpTime);
            }

            EditorGUILayout.Space();
            elasticsFoldout = EditorGUILayout.Foldout(elasticsFoldout, "Elastics", true);

            if (elasticsFoldout)
            {

                // If no ScriptableObject is currently assigned...
                if(elasticConfigurationObject.objectReferenceValue == null)
                {
                    // We just create a fresh one. This will be populated
                    // by the default values specified by the ScriptableObject.

                    // This will NOT be backed by a file/asset until we tell it to be.

                    // It WILL, however, be automatically serialized along with the rest
                    // of the properties of our object! Yay, exactly what we want.
                    elasticConfigurationObject.objectReferenceValue = ScriptableObject.CreateInstance<ElasticConfiguration>();
                }

                // Determine whether the current ScriptableObject configuration is actually an asset/file in the project.
                bool isAssetBacked = AssetDatabase.Contains(elasticConfigurationObject.objectReferenceValue);

                // If the ScriptableObject configuration is actually backed by a real asset/file in the user's project...
                if (isAssetBacked)
                {
                    // It is a shared configuration! We don't want to let them edit it from this inspector, because may
                    // mess up other objects that are sharing this configuration asset.

                    var sharedAssetPath = AssetDatabase.GetAssetPath(elasticConfigurationObject.objectReferenceValue);
                    EditorGUILayout.HelpBox("Editing a shared configuration asset file, located at " + sharedAssetPath, MessageType.Warning);
                    EditorGUILayout.PropertyField(elasticConfigurationObject, new GUIContent("Configuration slot (shared asset!):"));
                }
                else
                {
                    // If this ScriptableObject configuration is not actually an asset/file, then it is just an individual
                    // config that is serialized along with the component; the user can feel free to edit as they please.

                    EditorGUILayout.HelpBox("Editing a per-object configuration.", MessageType.Info);
                    EditorGUILayout.PropertyField(elasticConfigurationObject, new GUIContent("Configuration slot (currently non-asset):"));
                    using (new EditorGUI.IndentLevelScope())
                    {
                        MixedRealityInspectorUtility.DrawSubProfileEditor(elasticConfigurationObject.objectReferenceValue, true);
                    }
                }
            }

            EditorGUILayout.Space();
            eventsFoldout = EditorGUILayout.Foldout(eventsFoldout, "Manipulation Events", true);

            if (eventsFoldout)
            {
                EditorGUILayout.PropertyField(onManipulationStarted);
                EditorGUILayout.PropertyField(onManipulationEnded);
                EditorGUILayout.PropertyField(onHoverEntered);
                EditorGUILayout.PropertyField(onHoverExited);
            }

            // reset foldouts style
            style.fontStyle = previousStyle;

            serializedObject.ApplyModifiedProperties();
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
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        private SerializedProperty elasticTypes;
        private SerializedProperty translationElasticConfigurationObject;
        private SerializedProperty rotationElasticConfigurationObject;
        private SerializedProperty scaleElasticConfigurationObject;
        private SerializedProperty translationElasticExtent;
        private SerializedProperty rotationElasticExtent;
        private SerializedProperty scaleElasticExtent;

        bool oneHandedFoldout = true;
        bool twoHandedFoldout = true;
        bool constraintsFoldout = true;
        bool physicsFoldout = true;
        bool smoothingFoldout = true;
        bool elasticsFoldout = true;
        bool translationElasticFoldout = false;
        bool rotationElasticFoldout = false;
        bool scaleElasticFoldout = false;
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
            translationElasticConfigurationObject = serializedObject.FindProperty("translationElasticConfigurationObject");
            rotationElasticConfigurationObject = serializedObject.FindProperty("rotationElasticConfigurationObject");
            scaleElasticConfigurationObject = serializedObject.FindProperty("scaleElasticConfigurationObject");
            translationElasticExtent = serializedObject.FindProperty("translationElasticExtent");
            rotationElasticExtent = serializedObject.FindProperty("rotationElasticExtent");
            scaleElasticExtent = serializedObject.FindProperty("scaleElasticExtent");
            elasticTypes = serializedObject.FindProperty("elasticTypes");
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
                // This two-way enum cast is required because EnumFlagsField does not play nicely with
                // SerializedProperties and custom enum flags.
                var newElasticTypesValue = EditorGUILayout.EnumFlagsField("Manipulation types using elastic feedback: ", (TransformFlags)elasticTypes.intValue);
                elasticTypes.intValue = (int)(TransformFlags)newElasticTypesValue;

                // If the particular elastic type is requested, we offer the user the ability
                // to configure the elastic system.
                TransformFlags currentFlags = (TransformFlags)elasticTypes.intValue;

                translationElasticFoldout = DrawElasticConfiguration<ElasticConfiguration>(
                    "Translation Elastic",
                    translationElasticFoldout,
                    translationElasticConfigurationObject,
                    translationElasticExtent,
                    TransformFlags.Move,
                    currentFlags);

                rotationElasticFoldout = DrawElasticConfiguration<ElasticConfiguration>(
                    "Rotation Elastic",
                    rotationElasticFoldout,
                    rotationElasticConfigurationObject,
                    rotationElasticExtent,
                    TransformFlags.Rotate,
                    currentFlags);

                scaleElasticFoldout = DrawElasticConfiguration<ElasticConfiguration>(
                    "Scale Elastic",
                    scaleElasticFoldout,
                    scaleElasticConfigurationObject,
                    scaleElasticExtent,
                    TransformFlags.Scale,
                    currentFlags);
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

        private bool DrawElasticConfiguration<T>(
            string name,
            bool expanded,
            SerializedProperty elasticProperty,
            SerializedProperty extentProperty,
            TransformFlags requiredFlag,
            TransformFlags providedFlags) where T : ElasticConfiguration
        {
            if (providedFlags.HasFlag(requiredFlag))
            {
                bool result = false;
                using (new EditorGUI.IndentLevelScope())
                {
                    result = InspectorUIUtility.DrawScriptableFoldout<T>(
                        elasticProperty,
                        name,
                        expanded);
                    EditorGUILayout.PropertyField(extentProperty, includeChildren: true);
                }
                return result;
            }
            else
            {
                return false;
            }
        }
    }
}

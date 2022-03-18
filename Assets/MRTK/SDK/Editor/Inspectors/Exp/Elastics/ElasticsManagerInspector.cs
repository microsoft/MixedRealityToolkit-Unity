// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Editor
{
    /// <summary>
    /// A custom inspector for ElasticsManager used to separate
    /// Elastics configurations into distinct foldout panels.
    /// </summary>
    [CustomEditor(typeof(ElasticsManager))]
    [CanEditMultipleObjects]
    public class ElasticsManagerInspector : UnityEditor.Editor
    {
        private SerializedProperty elasticTypes;
        private SerializedProperty translationElasticConfigurationObject;
        private SerializedProperty rotationElasticConfigurationObject;
        private SerializedProperty scaleElasticConfigurationObject;
        private SerializedProperty translationElasticExtent;
        private SerializedProperty rotationElasticExtent;
        private SerializedProperty scaleElasticExtent;

        bool translationElasticFoldout = false;
        bool rotationElasticFoldout = false;
        bool scaleElasticFoldout = false;

        public void OnEnable()
        {
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
            if (providedFlags.IsMaskSet(requiredFlag))
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


        /// <summary>
        /// Draws a property of type ElasticsManager, showing a button to create a component to link to
        /// </summary>
        /// <param name="elasticsManager">ElasticsManager property of component this ui field gets drawn in.</param>
        /// <param name="hostObject">GameObject the component this is added to is attached to.</param>
        public static void DrawElasticsManagerLink(SerializedProperty elasticsManager, GameObject hostObject)
        {
            EditorGUILayout.LabelField(new GUIContent("Elastics"), EditorStyles.boldLabel);
            {
                EditorGUILayout.PropertyField(elasticsManager);
                EditorGUILayout.HelpBox("Elastics System is currently in experimental state.", MessageType.Warning);

                if (elasticsManager.objectReferenceValue == null)
                {
                    if (GUILayout.Button("Add Elastics Manager"))
                    {
                        elasticsManager.objectReferenceValue = hostObject.AddComponent<ElasticsManager>();
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    string displayName = elasticsManager.displayName;
                    EditorGUILayout.LabelField(displayName);
                    if (GUILayout.Button("Go to component"))
                    {
                        Highlighter.Highlight("Inspector", $"{ObjectNames.NicifyVariableName(displayName)} (Script)");
                        EditorGUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}

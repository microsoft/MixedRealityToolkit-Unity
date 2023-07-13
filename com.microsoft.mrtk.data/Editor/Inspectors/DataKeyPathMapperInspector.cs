﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Data.Editor
{
    [CustomEditor(typeof(DataKeyPathMapperGODictionary.ViewToDataKeypathMap))]
    public class DataKeyPathMapperInspector : UnityEditor.Editor
    {
        private SerializedProperty viewKeypathToDataKeypathMapper;

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary> 
        private void OnEnable()
        {
            viewKeypathToDataKeypathMapper = serializedObject.FindProperty("viewKeypathToDataKeypathMapper");
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(viewKeypathToDataKeypathMapper, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Data.Editor
{
    [CustomEditor(typeof(DataConsumerSpriteLookup.ValueToSpriteInfo))]
    public class DataConsumerSpriteInspector : UnityEditor.Editor
    {
        private SerializedProperty valueToSpriteLookup;

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary> 
        private void OnEnable()
        {
            valueToSpriteLookup = serializedObject.FindProperty("valueToSpriteLookup");
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(valueToSpriteLookup, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

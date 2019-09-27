// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tools.Runtime.Editor
{
    /// <summary>
    /// Custom profile inspector for the DisplayInputResult sample
    /// </summary>
    [CustomEditor(typeof(DisplayInputResult))]
    public class DisplayInputResultInspector : UnityEditor.Editor
    {
        private SerializedProperty displayTextMesh;
        private SerializedProperty inputType;
        private SerializedProperty axisNumber;
        private SerializedProperty buttonNumber;

        private void OnEnable()
        {
            displayTextMesh = serializedObject.FindProperty("displayTextMesh");
            inputType = serializedObject.FindProperty("inputType");
            axisNumber = serializedObject.FindProperty("axisNumber");
            buttonNumber = serializedObject.FindProperty("buttonNumber");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(displayTextMesh);
            EditorGUILayout.PropertyField(inputType);

            switch ((AxisType)inputType.intValue)
            {
                case AxisType.Digital:
                    EditorGUILayout.PropertyField(buttonNumber);
                    break;
                case AxisType.SingleAxis:
                    EditorGUILayout.PropertyField(axisNumber);
                    break;
                case AxisType.None:
                    EditorGUILayout.HelpBox("Will display all active buttons and axes.", MessageType.Info);
                    break;
                default:
                    EditorGUILayout.HelpBox("This axis type isn't currently supported with this script.", MessageType.Warning);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

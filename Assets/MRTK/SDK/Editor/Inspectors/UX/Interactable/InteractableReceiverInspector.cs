// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [CustomEditor(typeof(InteractableReceiver))]
    public class InteractableReceiverInspector : InteractableReceiverListInspector
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            RenderInspectorHeader();

            SerializedProperty events = serializedObject.FindProperty("Events");

            if (events.arraySize < 1)
            {
                AddEvent(0);
            }
            else
            {
                SerializedProperty eventItem = events.GetArrayElementAtIndex(0);
                InteractableEventInspector.RenderEvent(eventItem, false);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

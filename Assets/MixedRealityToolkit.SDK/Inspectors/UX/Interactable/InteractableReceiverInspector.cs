// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [CustomEditor(typeof(InteractableReceiver))]
    public class InteractableReceiverInspector : InteractableReceiverListInspector
    {
        protected override void OnEnable()
        {
            eventList = ((InteractableReceiver)target).Events;
            SetupEventOptions();
        }

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
                RenderEventSettings(eventItem, 0, eventOptions, ChangeEvent, null);
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected override void RemoveEvent(int index, SerializedProperty prop = null)
        {
            // do not remove events
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Inspectors.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(InputAction))]
    public class InputActionPropertyDrawer : PropertyDrawer
    {
        private static MixedRealityInputActionsProfile profile = null;
        private static GUIContent[] actionLabels = { new GUIContent("Missing Input Action Profile") };
        private static int[] actionIds = { 0 };

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent content)
        {
            if (!MixedRealityManager.IsInitialized)
            {
                profile = null;
                actionLabels = new[] { new GUIContent("Missing Mixed Reality Manager") };
                actionIds = new[] { 0 };
            }

            if (profile == null)
            {
                if (MixedRealityManager.HasActiveProfile)
                {
                    profile = MixedRealityManager.Instance.ActiveProfile.InputActionsProfile;
                    actionLabels = profile.InputActions.Select(action => new GUIContent(action.Description)).Prepend(new GUIContent("None")).ToArray();
                    actionIds = profile.InputActions.Select(action => (int)action.Id).Prepend(0).ToArray();
                }
            }
            else
            {
                if (!MixedRealityManager.IsInitialized || !MixedRealityManager.HasActiveProfile || MixedRealityManager.Instance.ActiveProfile.InputActionsProfile == null)
                {
                    profile = null;
                    actionLabels = new[] { new GUIContent("Missing Input Action Profile") };
                    actionIds = new[] { 0 };
                }
            }

            var label = EditorGUI.BeginProperty(rect, content, property);
            var inputActionId = property.FindPropertyRelative("id");

            if (profile == null || actionLabels == null || actionIds == null)
            {
                GUI.enabled = false;
                EditorGUI.IntPopup(rect, label, inputActionId.intValue.ResetIfGreaterThan(0), actionLabels, actionIds);
                GUI.enabled = true;
            }
            else
            {
                inputActionId.intValue = EditorGUI.IntPopup(rect, label, inputActionId.intValue.ResetIfGreaterThan(profile.InputActions.Length), actionLabels, actionIds);
            }

            EditorGUI.EndProperty();
        }
    }
}

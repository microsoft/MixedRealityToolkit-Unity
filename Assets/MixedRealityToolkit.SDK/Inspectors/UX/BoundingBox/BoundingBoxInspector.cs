//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(BoundingBox))]
    [CanEditMultipleObjects]
    public class BoundingBoxInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (target != null)
            {
                // check if rigidbody is attached - if so show warning in case input profile is not configured for individual collider raycast
                BoundingBox boundingBox = (BoundingBox)target;
                Rigidbody rigidBody = boundingBox.GetComponent<Rigidbody>();

                if (rigidBody != null)
                {
                    MixedRealityInputSystemProfile profile = CoreServices.InputSystem?.InputSystemProfile;
                    if (profile != null && profile.FocusIndividualCompoundCollider == false)
                    {
                        EditorGUILayout.Space();
                        // show warning and button to reconfigure profile
                        EditorGUILayout.HelpBox($"When using Bounding Box in combination with Rigidbody 'Focus Individual Compound Collider' must be enabled in Input Profile.", UnityEditor.MessageType.Warning);
                        if (GUILayout.Button($"Enable 'Focus Individual Compound Collider' in Input Profile"))
                        {
                            profile.FocusIndividualCompoundCollider = true;
                        }

                        EditorGUILayout.Space();
                    }
                }

                InspectorUIUtility.RenderHelpURL(target.GetType());
            }

            DrawDefaultInspector();
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Inspectors.EditorScript;
using MixedRealityToolkit.UX.Buttons;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.UX.EditorScript
{
    [UnityEditor.CustomEditor(typeof(CompoundButton))]
    public class CompoundButtonEditor : MRTKEditor
    {
        /// <summary>
        /// Validate button settings to ensure button will work in current scene
        /// TODO strengthen this check against new MRTK system
        /// </summary>
        protected override void DrawCustomFooter()
        {

            CompoundButton cb = (CompoundButton)target;

            // Don't perform this check at runtime
            if (!Application.isPlaying)
            {
                // First, check our colliders
                // Get the components we need for the button to be visible
                Rigidbody parentRigidBody = cb.GetComponent<Rigidbody>();
                Collider parentCollider = cb.GetComponent<Collider>();
                // Get all child colliders that AREN'T the parent collider
                HashSet<Collider> childColliders = new HashSet<Collider>(cb.GetComponentsInChildren<Collider>());
                childColliders.Remove(parentCollider);

                bool foundError = false;
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                if (parentCollider == null)
                {
                    if (childColliders.Count == 0)
                    {
                        foundError = true;
                        DrawError("Button must have at least 1 collider to be visible, preferably on the root transform.");
                        if (GUILayout.Button("Fix now"))
                        {
                            cb.gameObject.AddComponent<BoxCollider>();
                        }
                    }
                    else if (parentRigidBody == null)
                    {
                        foundError = true;
                        DrawError("Button requires a Rigidbody if colliders are only present on child transforms.");
                        if (GUILayout.Button("Fix now"))
                        {
                            Rigidbody rb = cb.gameObject.AddComponent<Rigidbody>();
                            rb.isKinematic = true;
                        }
                    }
                    else if (!parentRigidBody.isKinematic)
                    {
                        foundError = true;
                        GUI.color = warningColor;
                        DrawWarning("Warning: Button rigid body is not kinematic - this is not recommended.");
                        if (GUILayout.Button("Fix now"))
                        {
                            parentRigidBody.isKinematic = true;
                        }
                    }
                }

                if (!foundError)
                {
                    GUI.color = successColor;
                    UnityEditor.EditorGUILayout.LabelField("Button is good to go!", UnityEditor.EditorStyles.wordWrappedLabel);
                }
                UnityEditor.EditorGUILayout.EndVertical();
            }
        }
    }
}
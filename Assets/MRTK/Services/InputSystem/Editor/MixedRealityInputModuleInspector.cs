// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [UnityEditor.CustomEditor(typeof(MixedRealityInputModule))]
    public class MixedRealityInputModuleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MixedRealityInputModule inputModule = (MixedRealityInputModule)target;
            if (Application.isPlaying && inputModule.RaycastCamera != null)
            {
                foreach (var pointer in inputModule.ActiveMixedRealityPointers)
                {
                    if (pointer.Rays != null && pointer.Rays.Length > 0)
                    {
                        inputModule.RaycastCamera.transform.position = pointer.Rays[0].Origin;
                        inputModule.RaycastCamera.transform.rotation = Quaternion.LookRotation(pointer.Rays[0].Direction, Vector3.up);

                        inputModule.RaycastCamera.Render();

                        GUILayout.Label(pointer.PointerName);
                        GUILayout.Label(pointer.ToString());
                        GUILayout.Label(pointer.PointerId.ToString());
                        GUILayout.Label(inputModule.RaycastCamera.targetTexture);
                    }
                }
            }
        }

        public override bool RequiresConstantRepaint()
        {
            MixedRealityInputModule inputModule = (MixedRealityInputModule)target;
            if (Application.isPlaying && inputModule.RaycastCamera != null)
            {
                return true;
            }

            return base.RequiresConstantRepaint();
        }
    }
}
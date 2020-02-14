// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Object that represents a cursor in 3D space controlled by gaze.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/MeshCursor")]
    public class MeshCursor : BaseCursor
    {
        [Serializable]
        public struct MeshCursorDatum
        {
            public string Name;
            public CursorStateEnum CursorState;
            public Mesh CursorMesh;
            public Vector3 LocalScale;
            public Vector3 LocalOffset;
        }

        [SerializeField]
        public MeshCursorDatum[] CursorStateData;

        /// <summary>
        /// Sprite renderer to change.  If null find one in children
        /// </summary>
        public MeshRenderer TargetRenderer;

        /// <summary>
        /// On enable look for a sprite renderer on children
        /// </summary>
        protected override void OnEnable()
        {
            if (CursorStateData == null)
            {
                CursorStateData = Array.Empty<MeshCursorDatum>();
            }

            if (TargetRenderer == null)
            {
                TargetRenderer = GetComponentInChildren<MeshRenderer>();
            }

            base.OnEnable();
        }

        /// <summary>
        /// Override OnCursorState change to set the correct animation
        /// state for the cursor
        /// </summary>
        public override void OnCursorStateChange(CursorStateEnum state)
        {
            base.OnCursorStateChange(state);

            if (state != CursorStateEnum.Contextual)
            {
                for (int i = 0; i < CursorStateData.Length; i++)
                {
                    if (CursorStateData[i].CursorState == state)
                    {
                        SetCursorState(CursorStateData[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Based on the type of state info pass it through to the mesh renderer
        /// </summary>
        private void SetCursorState(MeshCursorDatum stateDatum)
        {
            // Return if we do not have an animator
            if (TargetRenderer != null)
            {
                var filter = TargetRenderer.gameObject.GetComponent<MeshFilter>();
                if (filter != null && stateDatum.CursorMesh != null)
                {
                    filter.mesh = stateDatum.CursorMesh;
                }

                TargetRenderer.transform.localPosition = stateDatum.LocalOffset;
                TargetRenderer.transform.localScale = stateDatum.LocalScale;
            }
        }
    }
}

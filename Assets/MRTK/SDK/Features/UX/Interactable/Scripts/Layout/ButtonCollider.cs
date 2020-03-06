// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Scales the collider on one object relative to the transform's scale of another object
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Scripts/MRTK/SDK/ButtonCollider")]
    public class ButtonCollider : MonoBehaviour
    {

        [Tooltip("the object to copy the scale from")]
        public Transform CopyFrom;

        [Tooltip("the percentage amounts to offset the scale")]
        public Vector3 ScaleFactor = Vector3.one;

        public Vector3 Expand = Vector3.zero;

        [Tooltip("should this only run in Edit mode, to avoid updating as items move?")]
        public bool OnlyInEditMode;

        private Collider copyTo;

        private void Awake()
        {
            copyTo = GetComponent<Collider>();
        }

        private void SetScale()
        {
            if (copyTo != null && CopyFrom != null)
            {
                BoxCollider box = copyTo as BoxCollider;
                if (box != null)
                {
                    box.size = Vector3.Scale(CopyFrom.transform.localScale, ScaleFactor) + Expand;
                    return;
                }

                CapsuleCollider capsule = copyTo as CapsuleCollider;
                if (capsule != null)
                {
                    capsule.radius = CopyFrom.transform.localScale.x * ScaleFactor.x + Expand.x;
                    capsule.height = CopyFrom.transform.localScale.y * ScaleFactor.y + Expand.y;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if ((Application.isPlaying && !OnlyInEditMode) || (!Application.isPlaying))
            {
                SetScale();
            }
        }
    }
}

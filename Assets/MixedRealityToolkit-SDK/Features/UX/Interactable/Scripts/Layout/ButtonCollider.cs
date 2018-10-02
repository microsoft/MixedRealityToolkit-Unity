// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Scales the collider on one object relative to the transform's scale of another object
    /// Works best with box colliders
    /// </summary>
    [ExecuteInEditMode]
    public class ButtonCollider : MonoBehaviour
    {
        /// <summary>
        /// A the transform to copy the scale values from
        /// </summary>
        [Tooltip("the object to copy the scale from")]
        [SerializeField]
        private Transform CopyFrom;

        /// <summary>
        /// The scale that is copied to the collider can be scaled.
        /// Default value is a 1:1 copy
        /// </summary>
        [Tooltip("the percentage amounts to offset the scale")]
        [SerializeField]
        private Vector3 ScaleFactor = Vector3.one;

        /// <summary>
        /// An absolute value to add or remove from the scale being copied.
        /// Use for consistent buffering added to the copied scale even if
        ///     the width, height or depth is not the same.
        /// </summary>
        [Tooltip("the amount to add to the source transform's scale")]
        [SerializeField]
        private Vector3 Expand = Vector3.zero;

        /// <summary>
        /// These scales are applied in Unity Editor only while doing layout.
        /// Turn off for responsive UI type results when editing ItemSize during runtime.
        /// Scales will be applied each frame.
        /// </summary>
        [Tooltip("should this only run in Edit mode, to avoid updating as items move?")]
        [SerializeField]
        private bool OnlyInEditMode;

        // the Collider to copy the scale to
        private Collider copyTo;

        private void Awake()
        {
            copyTo = GetComponent<Collider>();
        }

        // apply the scale, based on the settings, to the collider
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

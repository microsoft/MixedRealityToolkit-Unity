// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using MixedRealityToolkit.Common.Extensions;

namespace MixedRealityToolkit.UX.ToolTips
{
    [RequireComponent(typeof(MeshFilter))]

    /// <summary>
    /// Base class for a tool tip background
    /// Automatically finds a ToolTip and subscribes to ContentChange action
    /// Resizes its content to match ToolTip's content in ScaleToFitContent()
    /// </summary>
    public abstract class ToolTipBackground : MonoBehaviour
    {
        [SerializeField]
        protected ToolTip toolTip;

        protected virtual void OnEnable()
        {
            toolTip = gameObject.EnsureComponent<ToolTip>();

            if (toolTip == null)
            {
                Debug.LogError("No tooltip found in ToolTipMeshBackground");
                enabled = false;
                return;
            }
            
            toolTip.ContentChange += ContentChange;
        }

        protected virtual void ContentChange()
        {
            ScaleToFitContent();
        }

        protected abstract void ScaleToFitContent();
    }
}
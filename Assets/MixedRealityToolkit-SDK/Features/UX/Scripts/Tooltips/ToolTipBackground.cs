// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Core.Extensions;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{

    /// <summary>
    /// Base class for a tool tip background
    /// Automatically finds a ToolTip and subscribes to ContentChange action
    /// Resizes its content to match ToolTip's content in ScaleToFitContent()
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public abstract class ToolTipBackground : MonoBehaviour
    {
        [SerializeField]
        private ToolTip toolTipContent;

        public ToolTip ToolTipContent
        {
            set
            {
                toolTipContent = value;
            }
            get
            {
                return toolTipContent;
            }
        }

        protected virtual void OnEnable()
        {
            toolTipContent = gameObject.EnsureComponent<ToolTip>();

            if (toolTipContent == null)
            {
                Debug.LogError("No tooltip found in ToolTipMeshBackground");
                enabled = false;
                return;
            }

            toolTipContent.ContentChange += ContentChange;
        }

        protected virtual void ContentChange()
        {
            ScaleToFitContent();
        }

        protected abstract void ScaleToFitContent();
    }
}
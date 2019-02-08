// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.Renderers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// Renders an outline around tooltip background
    /// </summary>
    public class ToolTipLineHighlight : MonoBehaviour, IToolTipHighlight
    {
        public bool ShowHighlight
        {
            set
            {
                lineRenderer.enabled = value;
            }
        }

        [SerializeField]
        private BaseMixedRealityLineRenderer lineRenderer = null;
    }
}

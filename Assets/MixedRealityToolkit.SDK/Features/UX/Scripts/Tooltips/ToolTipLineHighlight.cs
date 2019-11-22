// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Renders an outline around tooltip background
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/ToolTipLineHighlight")]
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

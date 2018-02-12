// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Examples.UX.Widgets;
using UnityEngine;
namespace MixedRealityToolkit.Examples.UX.Controls
{
    /// <summary>
    /// A widget the activates/deactivates a child or other assigned element
    /// </summary>
    public class ElementSelectedActiveWidget : InteractiveWidget
    {
        public GameObject TargetObject;

        /// <summary>
        /// On the selected state, activate this game object
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            TargetObject.SetActive(InteractiveHost.IsSelected);
        }
    }
}

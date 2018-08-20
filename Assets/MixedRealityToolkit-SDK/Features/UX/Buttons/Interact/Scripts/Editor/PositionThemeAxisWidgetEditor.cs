// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Interact.Widgets
{
    [CustomEditor(typeof(PositionThemeAxisWidget))]
    public class PositionThemeAxisWidgetEditor : InteractiveWidgetEditor
    {
        protected PositionThemeAxisWidget widget;

        protected override void OnEnable()
        {
            ShowBlendOptions = true;
            widget = (PositionThemeAxisWidget)target;

            base.OnEnable();
        }
    }
}

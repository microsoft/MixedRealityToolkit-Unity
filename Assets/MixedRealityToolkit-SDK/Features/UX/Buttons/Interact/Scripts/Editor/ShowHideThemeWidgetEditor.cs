// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Interact.Widgets
{
    [CustomEditor(typeof(ShowHideThemeWidget))]
    public class ShowHideThemeWidgetEditor : InteractiveWidgetEditor
    {
        protected ShowHideThemeWidget widget;
        
        protected override void SetInspectorRef()
        {
            widget = (ShowHideThemeWidget)target;
            base.SetInspectorRef();
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.SDK.UX.Pointers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.UX.Pointers
{
    [CustomEditor(typeof(MousePointer))]
    public class MousePointerInspector : BaseControllerPointerInspector
    {
        protected override void OnEnable()
        {
            DrawBasePointerActions = false;
            base.OnEnable();
        }
    }
}
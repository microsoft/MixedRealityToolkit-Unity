// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Themes
{
    public class FloatInteractiveTheme : InteractiveTheme<float>
    {
        private void OnEnable()
        {
            InteractiveThemeManager.AddFloatTheme(this);
        }

        private void OnDisable()
        {
            InteractiveThemeManager.RemoveFloatTheme(this.Tag);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Themes
{
    public class ColorInteractiveTheme : InteractiveTheme<Color>
    {
        private void OnEnable()
        {
            InteractiveThemeManager.AddColorTheme(this);
        }

        private void OnDisable()
        {
            InteractiveThemeManager.RemoveColorTheme(this.Tag);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Themes
{
    public class TextureInteractiveTheme : InteractiveTheme<Texture>
    {
        private void OnEnable()
        {
            InteractiveThemeManager.AddTextureTheme(this);
        }

        private void OnDisable()
        {
            InteractiveThemeManager.RemoveTextureTheme(this.Tag);
        }
    }
}

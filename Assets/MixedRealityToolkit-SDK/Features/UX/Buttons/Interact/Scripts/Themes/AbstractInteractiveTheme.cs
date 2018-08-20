// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Themes
{
    public abstract class AbstractInteractiveTheme : MonoBehaviour
    {

        [Tooltip("Tag to help distinguish themes")]
        public string Tag = "default";
    }
}

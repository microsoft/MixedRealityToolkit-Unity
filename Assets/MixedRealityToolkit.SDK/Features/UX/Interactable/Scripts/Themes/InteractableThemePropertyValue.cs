// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Themes
{
    /// <summary>
    /// Base values of a theme property, used for serialization
    /// </summary>
    
    [System.Serializable]
    public class InteractableThemePropertyValue
    {
        public string Name;
        public string String;
        public bool Bool;
        public int Int;
        public float Float;
        public Texture Texture;
        public Material Material;
        public GameObject GameObject;
        public Vector2 Vector2;
        public Vector3 Vector3;
        public Vector4 Vector4;
        public Color Color;
        public Quaternion Quaternion;
        public AudioClip AudioClip;
        public Animation Animation;

        public void Reset()
        {
            Name = string.Empty;
            String = string.Empty;
            Bool = false;
            Int = 0;
            Float = 0;
            Texture = null;
            Material = null;
            GameObject = null;
            Vector2 = default(Vector2);
            Vector3 = default(Vector3);
            Vector4 = default(Vector4);
            Color = default(Color);
            Quaternion = default(Quaternion);
            AudioClip = null;
            Animation = null;
        }
    }
}


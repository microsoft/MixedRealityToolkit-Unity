// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Base values of a theme property, used for serialization
    /// </summary>
    [System.Serializable]
    public class ThemePropertyValue
    {
        public string Name;
        public string String;
        public bool Bool;
        public int Int;
        public float Float;
        public Texture Texture;
        public Material Material;
        public Shader Shader;
        public GameObject GameObject;
        public Vector2 Vector2;
        public Vector3 Vector3;
        public Vector4 Vector4;
        public Color Color;
        public Quaternion Quaternion;
        public AudioClip AudioClip;
        public Animation Animation;

        /// <summary>
        /// Create new ThemePropertyValue and copy over internal data
        /// </summary>
        /// <returns>New ThemePropertyValue with identical primitive and reference values as this ThemePropertyValue</returns>
        public ThemePropertyValue Copy()
        {
            return new ThemePropertyValue()
            {
                Name = this.Name,
                String = this.String,
                Bool = this.Bool,
                Int = this.Int,
                Float = this.Float,
                Texture = this.Texture,
                Material = this.Material,
                Shader = this.Shader,
                GameObject = this.GameObject,
                Vector2 = this.Vector2,
                Vector3 = this.Vector3,
                Vector4 = this.Vector4,
                Color = this.Color,
                Quaternion = this.Quaternion,
                AudioClip = this.AudioClip,
                Animation = this.Animation,
            };
        }

        /// <summary>
        /// Reset all fields to default type values
        /// </summary>
        public void Reset()
        {
            Name = string.Empty;
            String = string.Empty;
            Bool = false;
            Int = 0;
            Float = 0;
            Texture = null;
            Material = null;
            Shader = null;
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


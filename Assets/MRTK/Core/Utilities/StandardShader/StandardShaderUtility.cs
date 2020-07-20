// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Mixed Reality standard shader utility class with commonly used constants, types and convenience methods.
    /// </summary>
    public static class StandardShaderUtility
    {
        /// <summary>
        /// The string name of the Mixed Reality Toolkit/Standard shader which can be used to identify a shader or for shader lookups.
        /// </summary>
        public static readonly string MrtkStandardShaderName = "Mixed Reality Toolkit/Standard";

        /// <summary>
        /// Returns an instance of the Mixed Reality Toolkit/Standard shader.
        /// </summary>
        public static Shader MrtkStandardShader
        {
            get
            {
                if (mrtkStandardShader == null)
                {
                    mrtkStandardShader = Shader.Find(MrtkStandardShaderName);
                }

                return mrtkStandardShader;
            }

            private set
            {
                mrtkStandardShader = value;
            }
        }

        private static Shader mrtkStandardShader = null;

        /// <summary>
        /// Checks if a material is using the Mixed Reality Toolkit/Standard shader.
        /// </summary>
        /// <param name="material">The material to check.</param>
        /// <returns>True if the material is using the Mixed Reality Toolkit/Standard shader</returns>
        public static bool IsUsingMrtkStandardShader(Material material)
        {
            return IsMrtkStandardShader((material != null) ? material.shader : null);
        }

        /// <summary>
        /// Checks if a shader is the Mixed Reality Toolkit/Standard shader.
        /// </summary>
        /// <param name="shader">The shader to check.</param>
        /// <returns>True if the shader is the Mixed Reality Toolkit/Standard shader.</returns>
        public static bool IsMrtkStandardShader(Shader shader)
        {
            return shader == MrtkStandardShader;
        }
    }
}

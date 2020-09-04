// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Rendering
{
    /// <summary>
    /// Optional interface to use with objects which need to take ownership of <see cref="MaterialInstance"/>(s).
    /// </summary>
    public interface IMaterialInstanceOwner
    {
        /// <summary>
        /// Method which is invoked by a <see cref="MaterialInstance"/> when an external material change is detected.
        /// This normally occurs when materials are changed via <see href="https://docs.unity3d.com/ScriptReference/Renderer-material.html">Renderer.material</see>, 
        /// <see href="https://docs.unity3d.com/ScriptReference/Renderer-materials.html">Renderer.materials</see>, or via the editor.
        /// </summary>
        /// <param name="materialInstance">The material instance which contains the updated materials.</param>
        void OnMaterialChanged(MaterialInstance materialInstance);
    }
}

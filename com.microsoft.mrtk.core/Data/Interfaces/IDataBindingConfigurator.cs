// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using System.Collections;

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Configures one or more data consumers on a gameobject according to the
    /// provided profile.  This can bind for both theming and data.
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public interface IDataBindingConfigurator
    {
        /// <summary>
        /// Use the specific data binding profile to configure one or more
        /// data consumers on the specified game object.
        /// </summary>
        /// <param name="gameObject">The gameObject to be used for adding the binding.</param>
        /// <param name="bindingProfile">The binding profile to use for processing the binding.</param>
        void ConfigureBinding(GameObject gameObject, UXBindingProfileTemplate bindingProfile);
    }
}

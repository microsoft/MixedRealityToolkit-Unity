// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using System.Collections;

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Configures one or more data consumers on a gameobject according to the
    /// provided profile.  This can bind for both theming and data.
    /// </summary>
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

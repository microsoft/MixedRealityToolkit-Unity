// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers
{
    // No Scriptable Object Menu constructor attributes here, as this class is meant to be overwritten.

    /// <summary>
    /// Custom Mixed Reality Controller class to inherit from to enable custom profile support in mapping profile.
    /// </summary>
    public class CustomMixedRealityControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.None;
    }
}
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;

#if WINDOWS_UWP && !ENABLE_IL2CPP
using System.Reflection;
using Microsoft.MixedReality.Toolkit;
#endif // WINDOWS_UWP && !ENABLE_IL2CPP

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Attach to a controller device class to make it show up in the controller mapping profile.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MixedRealityControllerAttribute : Attribute
    {
        /// <summary>
        /// The SupportedControllerType to which the controller device belongs to.
        /// </summary>
        public SupportedControllerType SupportedControllerType { get; }

        /// <summary>
        /// List of handedness values supported by the respective controller.
        /// </summary>
        public Handedness[] SupportedHandedness { get; }

        /// <summary>
        /// Path to image file used when displaying an icon in the UI.
        /// </summary>
        public string TexturePath { get; }

        /// <summary>
        /// Additional flags for configuring controller capabilities.
        /// </summary>
        public MixedRealityControllerConfigurationFlags Flags { get; }

        /// <summary>
        /// 
        /// </summary>
        public MixedRealityControllerAttribute(
            SupportedControllerType supportedControllerType,
            Handedness[] supportedHandedness,
            string texturePath = "",
            MixedRealityControllerConfigurationFlags flags = 0)
        {
            SupportedControllerType = supportedControllerType;
            SupportedHandedness = supportedHandedness;
            TexturePath = texturePath;
            Flags = flags;
        }

        /// <summary>
        /// Convenience function for retrieving the attribute given a certain class type.
        /// </summary>
        public static MixedRealityControllerAttribute Find(Type type)
        {
            return type.GetCustomAttributes(typeof(MixedRealityControllerAttribute), true).FirstOrDefault() as MixedRealityControllerAttribute;
        }
    }
}

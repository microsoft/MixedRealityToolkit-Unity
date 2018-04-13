// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.Pointers;
using MixedRealityToolkit.InputModule.Utilities;
using System.Collections;

namespace MixedRealityToolkit.InputModule.InputSources
{
    /// <summary>
    /// Interface for an input source.
    /// An input source can be anything that a user can use to interact with a device.
    /// </summary>
    public interface IInputSource : IEqualityComparer
    {
        uint SourceId { get; }

        string SourceName { get; }

        IPointer[] Pointers { get; }

        /// <summary>
        /// Returns the input info that the input source can provide.
        /// </summary>
        SupportedInputInfo GetSupportedInputInfo();

        /// <summary>
        /// Returns whether the input source supports the specified input info type.
        /// </summary>
        /// <param name="inputInfo">Input info type that we want to get information about.</param>
        bool SupportsInputInfo(SupportedInputInfo inputInfo);
    }
}

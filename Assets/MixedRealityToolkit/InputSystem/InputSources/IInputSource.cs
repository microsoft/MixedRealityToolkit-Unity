// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;

namespace Microsoft.MixedReality.Toolkit.InputSystem.InputSources
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
        InputType[] Capabilities { get; }

        /// <summary>
        /// Returns whether the input source supports the specified input types.
        /// </summary>
        /// <param name="inputInfo">Input types that we want to get information about.</param>
        bool SupportsInputCapability(InputType[] inputInfo);
    }
}

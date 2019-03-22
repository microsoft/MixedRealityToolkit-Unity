// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Marshalling
{
    /// <summary>
    /// A readonly frame queue stat
    /// </summary>
    public interface IReadonlyFrameQueueStat
    {
        // <summary>
        /// The average value calculated for the stat
        /// </summary>
        float Value
        {
            get;
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    public class CalculatedCameraExtrinsics : CameraExtrinsics
    {
        /// <summary>
        /// True if the calculation succeeded, otherwise false
        /// </summary>
        public bool Succeeded;

        /// <inheritdoc />
        public CalculatedCameraExtrinsics() : base() { }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Succeeded: {Succeeded} {base.ToString()}";
        }
    }
}

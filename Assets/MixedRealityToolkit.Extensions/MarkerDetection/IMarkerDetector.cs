// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.MarkerDetection
{
    public delegate void MarkersUpdatedHandler(Dictionary<int, Marker> markers);

    public interface IMarkerDetector
    {
        event MarkersUpdatedHandler MarkersUpdated;
        void StartDetecting();
        void StopDetecting();
        void SetMarkerSize(float size);
    }
}

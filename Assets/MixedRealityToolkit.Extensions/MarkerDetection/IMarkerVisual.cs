// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.MarkerDetection
{
    public interface IMarkerVisual
    {
        void ShowMarker(int id);
        void HideMarker();
        void SetMarkerSize(float size);
    }
}

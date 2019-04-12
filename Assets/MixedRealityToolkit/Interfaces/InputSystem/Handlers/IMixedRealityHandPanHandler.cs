// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for handling touch pointers.
    /// </summary>
    public interface IMixedRealityHandPanHandler
    {
        void OnPanStarted(HandPanEventData eventData);
        void OnPanning(HandPanEventData eventData);
        void OnPanEnded(HandPanEventData eventData);
    }
}
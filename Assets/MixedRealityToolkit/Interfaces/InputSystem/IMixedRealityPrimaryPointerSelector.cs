// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMixedRealityPrimaryPointerSelector
    {
        IMixedRealityPointer PrimaryPointer { get; }

        void RegisterPointer(IMixedRealityPointer pointer);

        void UnregisterPointer(IMixedRealityPointer pointer);

        void Update();
    }
}
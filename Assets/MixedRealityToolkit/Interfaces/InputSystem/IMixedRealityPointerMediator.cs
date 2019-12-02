// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for handling groups of pointers resolving conflicts between them.
    /// E.g., ensuring that far pointers are disabled when a near pointer is active.
    /// </summary>
    public interface IMixedRealityPointerMediator
    {
        void RegisterPointers(IMixedRealityPointer[] pointer);
        void UnregisterPointers(IMixedRealityPointer[] pointers);

        void UpdatePointers();
    }
}
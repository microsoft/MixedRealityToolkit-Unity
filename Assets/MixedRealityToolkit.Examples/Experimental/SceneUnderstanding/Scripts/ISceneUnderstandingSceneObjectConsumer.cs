// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;

/// <summary>
/// Scripts that implement this interface will have these methods called by the
/// DemoSceneUnderstandingController to respond to state changes in the system
/// </summary>

public interface ISceneUnderstandingSceneObjectConsumer
{
    void OnSpatialAwarenessSceneObjectCreated(SpatialAwarenessSceneObject sceneObject);
}

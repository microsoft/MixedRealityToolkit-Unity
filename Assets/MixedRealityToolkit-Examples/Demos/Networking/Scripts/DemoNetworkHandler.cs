// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Networking;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEngine;

public class DemoNetworkHandler : MonoBehaviour,
    IMixedRealityNetworkingHandler<float>,
    IMixedRealityNetworkingHandler<string>
{
    void Start()
    {
        MixedRealityToolkit.NetworkingSystem.Register(gameObject);
        MixedRealityToolkit.NetworkingSystem.SendData("Hi");
        MixedRealityToolkit.NetworkingSystem.SendData(5f);
        MixedRealityToolkit.NetworkingSystem.SendData(Vector3.zero);
    }

    /// <inheritdoc />
    public void OnDataReceived(BaseNetworkingEventData<float> eventData)
    {
    }

    /// <inheritdoc />
    public void OnDataReceived(BaseNetworkingEventData<string> eventData)
    {
    }
}
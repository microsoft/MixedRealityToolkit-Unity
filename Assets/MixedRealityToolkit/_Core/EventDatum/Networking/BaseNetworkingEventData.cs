// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum;
using UnityEngine.EventSystems;

public class BaseNetworkingEventData<T> : GenericBaseEventData
{
    public T Data { get; private set; }

    public BaseNetworkingEventData(EventSystem eventSystem) : base(eventSystem) { }

    public void Initialize(T data)
    {
        BaseInitialize(null);
        Data = data;
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.Events
{
    public class GenericBaseEventData : BaseEventData
    {
        public IEventSource EventSource { get; private set; }

        public GenericBaseEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IEventSource eventSource)
        {
            Reset();
            EventSource = eventSource;
        }
    }
}
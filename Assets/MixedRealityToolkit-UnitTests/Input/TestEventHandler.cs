// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputHandlers;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.Tests.Input
{
    public interface ITestEventSystemHandler : IEventSystemHandler
    {
        void OnTest(BaseEventData eventData);
    }

    public class TestEventHandler : FocusTarget, ITestEventSystemHandler
    {
        public bool IsGlobal;

        public static readonly ExecuteEvents.EventFunction<ITestEventSystemHandler> OnTestHandler =
        delegate (ITestEventSystemHandler handler, BaseEventData eventData)
        {
            handler.OnTest(eventData);
        };

        public Action<GameObject, BaseEventData> EventFiredCallback;

        public void OnTest(BaseEventData eventData)
        {
            if (!IsGlobal)
            {
                eventData.Use();
            }

            EventFiredCallback(gameObject, eventData);
        }

        public override void OnFocusEnter(FocusEventData eventData)
        {
            base.OnFocusEnter(eventData);

            EventFiredCallback(gameObject, null);
        }

        public override void OnFocusExit(FocusEventData eventData)
        {
            base.OnFocusExit(eventData);

            EventFiredCallback(gameObject, null);
        }

        public override void OnBeforeFocusChange(FocusEventData eventData)
        {
            base.OnBeforeFocusChange(eventData);

            EventFiredCallback(gameObject, null);
        }
    }
}

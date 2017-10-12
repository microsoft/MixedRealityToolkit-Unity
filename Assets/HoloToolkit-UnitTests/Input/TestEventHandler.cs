// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.Tests
{
    public interface ITestEventSystemHandler : IEventSystemHandler
    {
        void OnTest(BaseEventData eventData);
    }

    public class TestEventHandler : MonoBehaviour, ITestEventSystemHandler, IFocusable
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

        public void OnFocusEnter()
        {
            EventFiredCallback(gameObject, null);
        }

        public void OnFocusExit()
        {
            EventFiredCallback(gameObject, null);
        }
    }
}

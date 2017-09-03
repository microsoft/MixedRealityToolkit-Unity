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
        void OnTest();
    }

    public class TestEventHandler : MonoBehaviour, ITestEventSystemHandler, IFocusable
    {
        public static readonly ExecuteEvents.EventFunction<ITestEventSystemHandler> OnTestHandler =
        delegate (ITestEventSystemHandler handler, BaseEventData eventData)
        {
            handler.OnTest();
        };


        public Action<GameObject> EventFiredCallback;

        public void OnTest()
        {
            EventFiredCallback(gameObject);
        }

        public void OnFocusEnter()
        {
            EventFiredCallback(gameObject);
        }

        public void OnFocusExit()
        {
            EventFiredCallback(gameObject);
        }
    }
}

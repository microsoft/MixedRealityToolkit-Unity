// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // For InputSystemGlobalListener
#pragma warning disable 0618
    [AddComponentMenu("Scripts/MRTK/Tests/TestInputGlobalListener")]
    internal class TestInputGlobalListener : InputSystemGlobalListener, IMixedRealityPointerHandler, IMixedRealitySpeechHandler
    {
        // Parameters, which are set by child classes
        protected bool useObjectBasedRegistration = false;
        protected bool registerSpeechOnly = false;

        // Values changed by class to validate event receiving
        public int pointerDownCount = 0;
        public int pointerDraggedCount = 0;
        public int pointerUpCount = 0;
        public int pointerClickedCount = 0;
        public List<string> speechCommandsReceived = new List<string>();

        protected override void OnEnable()
        {
            pointerDownCount = 0;
            pointerDraggedCount = 0;
            pointerUpCount = 0;
            pointerClickedCount = 0;
            speechCommandsReceived = new List<string>();

            if (useObjectBasedRegistration)
            {
                base.OnEnable();
            }
            else if (CoreServices.InputSystem != null)
            {
                if (registerSpeechOnly)
                {
                    CoreServices.InputSystem.RegisterHandler<IMixedRealitySpeechHandler>(this);
                }
                else
                {
                    CoreServices.InputSystem.RegisterHandler<IMixedRealitySpeechHandler>(this);
                    CoreServices.InputSystem.RegisterHandler<IMixedRealityPointerHandler>(this);
                }
            }
        }

        protected override void Start()
        {
            if (useObjectBasedRegistration)
            {
                base.Start();
            }
        }

        protected override void OnDisable()
        {
            if (useObjectBasedRegistration)
            {
                base.OnDisable();
            }
            else if (CoreServices.InputSystem != null)
            {
                if (registerSpeechOnly)
                {
                    CoreServices.InputSystem.UnregisterHandler<IMixedRealitySpeechHandler>(this);
                }
                else
                {
                    CoreServices.InputSystem.UnregisterHandler<IMixedRealitySpeechHandler>(this);
                    CoreServices.InputSystem.UnregisterHandler<IMixedRealityPointerHandler>(this);
                }
            }
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            pointerDownCount++;
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            pointerDraggedCount++;
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            pointerUpCount++;
        }

        public virtual void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            pointerClickedCount++;
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            speechCommandsReceived.Add(eventData.Command.Keyword);
        }
    }

    internal class TestInputGlobalListenerException : TestInputGlobalListener
    {
        public const string ExceptionMessage = "Test exception thrown during event fired for global listener";
        public override void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            throw new Exception(ExceptionMessage);
        }
    }

    internal class TestInputGlobalListenerObjectBased : TestInputGlobalListener
    {
        TestInputGlobalListenerObjectBased()
        {
            useObjectBasedRegistration = true;
            registerSpeechOnly = false;
        }
    }

    internal class TestInputGlobalListenerHandlerBasedAllHandlers : TestInputGlobalListener
    {
        TestInputGlobalListenerHandlerBasedAllHandlers()
        {
            useObjectBasedRegistration = false;
            registerSpeechOnly = false;
        }
    }

    internal class TestInputGlobalListenerHandlerBasedSpeechHandler : TestInputGlobalListener
    {
        TestInputGlobalListenerHandlerBasedSpeechHandler()
        {
            useObjectBasedRegistration = false;
            registerSpeechOnly = true;
        }
    }

    internal class TestInputGlobalHandlerListener : InputSystemGlobalHandlerListener, IMixedRealityHandJointHandler, IMixedRealitySpeechHandler, IMixedRealityInputHandler<float>
    {
        // Values changed by class to validate event receiving
        public int handJointCount = 0;
        public int inputChangedCount = 0;
        public int speechCount = 0;

        public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            handJointCount++;
        }

        public void OnInputChanged(InputEventData<float> eventData)
        {
            inputChangedCount++;
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            speechCount++;
        }

        protected override void RegisterHandlers()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<float>>(this);
        }

        protected override void UnregisterHandlers()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<float>>(this);
        }
    }
#pragma warning restore 0618
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Luis
{
    public interface IBaseInputHandler : IEventSystemHandler {}

    public interface IInputHandler : IBaseInputHandler
    {
        void OnInputDown(InputEventData eventData);
        void OnInputUp(InputEventData eventData);
    }

    public interface ISpeechHandler : IBaseInputHandler
    {
        void OnSpeechKeywordRecognized(SpeechEventData eventData);
    }

    public interface IActionHandler : IBaseInputHandler
    {
        void OnActionStarted(BaseInputEventData eventData);
        void OnActionEnded(BaseInputEventData eventData);
    }

    class InputSystem
    {

        private static readonly ExecuteEvents.EventFunction<ISpeechHandler> speechEventFunction =
            delegate (ISpeechHandler handler, BaseEventData eventData)
            {
                var speechData = ExecuteEvents.ValidateEventData<SpeechEventData>(eventData);
                handler.OnSpeechKeywordRecognized(speechData);
            };

        private static readonly ExecuteEvents.EventFunction<IBaseInputHandler> speechAndActionEventFunction =
            delegate (IBaseInputHandler handler, BaseEventData eventData)
            {
                var speechData = ExecuteEvents.ValidateEventData<SpeechEventData>(eventData);
                Debug.Assert(speechData.MixedRealityInputAction != MixedRealityInputAction.None);

                var speechHandler = handler as ISpeechHandler;
                if (speechHandler != null)
                {
                    speechHandler.OnSpeechKeywordRecognized(speechData);
                }

                var actionHandler = handler as IActionHandler;
                if (actionHandler != null)
                {
                    actionHandler.OnActionStarted(speechData);
                    actionHandler.OnActionEnded(speechData);
                }
            };

        void RaiseSpeechCommandRecognized(SpeechEventData eventData)
        {
            if (eventData.MixedRealityInputAction != MixedRealityInputAction.None)
            {
                //ExecuteEvents.ExecuteHierarchy(focusedObject, speechEventData, speechAndActionEventFunction);
            }
            else
            {
                //ExecuteEvents.ExecuteHierarchy(focusedObject, speechEventData, speechEventFunction);
            }

        }
    }
}
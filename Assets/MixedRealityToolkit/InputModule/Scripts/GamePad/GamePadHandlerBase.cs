// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.GamePad
{
    public class GamePadHandlerBase : MonoBehaviour, ISourceStateHandler
    {
        [SerializeField]
        [Tooltip("True, if gaze is not required for Input")]
        protected bool IsGlobalListener = true;

        protected string GamePadName = string.Empty;

        private void OnEnable()
        {
            if (IsGlobalListener)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }
        }

        protected virtual void OnDisable()
        {
            if (IsGlobalListener && InputManager.Instance != null)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        public virtual void OnSourceDetected(SourceStateEventData eventData)
        {
            // Override and name your GamePad source.
        }

        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            GamePadName = string.Empty;
        }
    }
}

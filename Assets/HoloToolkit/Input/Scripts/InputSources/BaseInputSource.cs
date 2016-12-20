// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{ 
    /// <summary>
    /// Base class for an input source.
    /// </summary>
    public abstract class BaseInputSource : MonoBehaviour, IInputSource
    {
        public event EventHandler<InputSourceEventArgs> SourceUp;
        public event EventHandler<InputSourceEventArgs> SourceDown;
        public event EventHandler<SourceClickEventArgs> SourceClicked;
        public event EventHandler<InputSourceEventArgs> SourceDetected;
        public event EventHandler<InputSourceEventArgs> SourceLost;
        public event EventHandler<HoldEventArgs> HoldStarted;
        public event EventHandler<HoldEventArgs> HoldCompleted;
        public event EventHandler<HoldEventArgs> HoldCanceled;
        public event EventHandler<ManipulationEventArgs> ManipulationStarted;
        public event EventHandler<ManipulationEventArgs> ManipulationUpdated;
        public event EventHandler<ManipulationEventArgs> ManipulationCompleted;
        public event EventHandler<ManipulationEventArgs> ManipulationCanceled;
        public event EventHandler<NavigationEventArgs> NavigationStarted;
        public event EventHandler<NavigationEventArgs> NavigationUpdated;
        public event EventHandler<NavigationEventArgs> NavigationCompleted;
        public event EventHandler<NavigationEventArgs> NavigationCanceled;
        public event EventHandler<SpeechKeywordRecognizedEventArgs> SpeechKeywordRecognized;

        public abstract SupportedInputEvents SupportedEvents { get; }
        
        private bool isRegistered = false;

        protected virtual void Start()
        {
            // Must register on start, because the InputManager might not be initialized when 
            // registering in OnEnable
            RegisterWithInputManager();
        }

        protected virtual void OnDestroy()
        {
            UnregisterFromInputManager();
        }

        protected virtual void OnEnable()
        {
            RegisterWithInputManager();
        }

        protected virtual void OnDisable()
        {
            UnregisterFromInputManager();
        }

        /// <summary>
        /// Enables the input source so that the input manager can use it.
        /// </summary>
        public void EnableInputSource()
        {
            RegisterWithInputManager();
        }

        /// <summary>
        /// Disables the input source so that the input manager stops using it.
        /// </summary>
        public void DisableInputSource()
        {
            UnregisterFromInputManager();
        }

        /// <summary>
        /// Register this input source with the input manager.
        /// </summary>
        private void RegisterWithInputManager()
        {
            if (!isRegistered && InputManager.Instance != null)
            {
                InputManager.Instance.RegisterInputSource(this);
                isRegistered = true;
            }
        }

        /// <summary>
        /// Unregister this input source from the input manager.
        /// </summary>
        private void UnregisterFromInputManager()
        {
            if (isRegistered)
            {
                InputManager.Instance.UnregisterInputSource(this);
                isRegistered = false;
            }
        }

        public abstract SupportedInputInfo GetSupportedInputInfo(uint sourceId);

        public bool SupportsInputInfo(uint sourceId, SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo(sourceId) & inputInfo) != 0;
        }

        public abstract bool TryGetPosition(uint sourceId, out Vector3 position);

        public abstract bool TryGetOrientation(uint sourceId, out Quaternion orientation);

        #region Events wrappers

        protected void RaiseSourceUpEvent(InputSourceEventArgs e)
        {
            EventHandler<InputSourceEventArgs> handler = SourceUp;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseSourceDownEvent(InputSourceEventArgs e)
        {
            EventHandler<InputSourceEventArgs> handler = SourceDown;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseSourceClickedEvent(SourceClickEventArgs e)
        {
            EventHandler<SourceClickEventArgs> handler = SourceClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseSourceDetectedEvent(InputSourceEventArgs e)
        {
            EventHandler<InputSourceEventArgs> handler = SourceDetected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseSourceLostEvent(InputSourceEventArgs e)
        {
            EventHandler<InputSourceEventArgs> handler = SourceLost;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseHoldStartedEvent(HoldEventArgs e)
        {
            EventHandler<HoldEventArgs> handler = HoldStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseHoldCanceledEvent(HoldEventArgs e)
        {
            EventHandler<HoldEventArgs> handler = HoldCanceled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseHoldCompletedEvent(HoldEventArgs e)
        {
            EventHandler<HoldEventArgs> handler = HoldCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseManipulationStartedEvent(ManipulationEventArgs e)
        {
            EventHandler<ManipulationEventArgs> handler = ManipulationStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseManipulationUpdatedEvent(ManipulationEventArgs e)
        {
            EventHandler<ManipulationEventArgs> handler = ManipulationUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseManipulationCompletedEvent(ManipulationEventArgs e)
        {
            EventHandler<ManipulationEventArgs> handler = ManipulationCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseManipulationCanceledEvent(ManipulationEventArgs e)
        {
            EventHandler<ManipulationEventArgs> handler = ManipulationCanceled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseNavigationStartedEvent(NavigationEventArgs e)
        {
            EventHandler<NavigationEventArgs> handler = NavigationStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseNavigationUpdatedEvent(NavigationEventArgs e)
        {
            EventHandler<NavigationEventArgs> handler = NavigationUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseNavigationCompletedEvent(NavigationEventArgs e)
        {
            EventHandler<NavigationEventArgs> handler = NavigationCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseNavigationCanceledEvent(NavigationEventArgs e)
        {
            EventHandler<NavigationEventArgs> handler = NavigationCanceled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseSpeechKeywordRecognizedEvent(SpeechKeywordRecognizedEventArgs e)
        {
            EventHandler<SpeechKeywordRecognizedEventArgs> handler = SpeechKeywordRecognized;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}

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
        public event Action<IInputSource, uint> SourceUp;
        public event Action<IInputSource, uint> SourceDown;
        public event Action<IInputSource, uint> SourceClicked;
        public event Action<IInputSource, uint> SourceDetected;
        public event Action<IInputSource, uint> SourceLost;
        public event Action<IInputSource, uint> HoldStarted;
        public event Action<IInputSource, uint> HoldCompleted;
        public event Action<IInputSource, uint> HoldCanceled;
        public event Action<IInputSource, uint, Vector3> ManipulationStarted;
        public event Action<IInputSource, uint, Vector3> ManipulationUpdated;
        public event Action<IInputSource, uint, Vector3> ManipulationCompleted;
        public event Action<IInputSource, uint, Vector3> ManipulationCanceled;
        public event Action<IInputSource, uint, Vector3> NavigationStarted;
        public event Action<IInputSource, uint, Vector3> NavigationUpdated;
        public event Action<IInputSource, uint, Vector3> NavigationCompleted;
        public event Action<IInputSource, uint, Vector3> NavigationCanceled;

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

        protected void RaiseSourceUpEvent(uint sourceId)
        {
            if (SourceUp != null)
            {
                SourceUp(this, sourceId);
            }
        }

        protected void RaiseSourceDownEvent(uint sourceId)
        {
            if (SourceDown != null)
            {
                SourceDown(this, sourceId);
            }
        }

        protected void RaiseSourceClickedEvent(uint sourceId)
        {
            if (SourceClicked != null)
            {
                SourceClicked(this, sourceId);
            }
        }

        protected void RaiseSourceDetectedEvent(uint sourceId)
        {
            if (SourceDetected != null)
            {
                SourceDetected(this, sourceId);
            }
        }

        protected void RaiseSourceLostEvent(uint sourceId)
        {
            if (SourceLost != null)
            {
                SourceLost(this, sourceId);
            }
        }

        protected void RaiseHoldStartedEvent(uint sourceId)
        {
            if (HoldStarted != null)
            {
                HoldStarted(this, sourceId);
            }
        }

        protected void RaiseHoldCanceledEvent(uint sourceId)
        {
            if (HoldCanceled != null)
            {
                HoldCanceled(this, sourceId);
            }
        }

        protected void RaiseHoldCompletedEvent(uint sourceId)
        {
            if (HoldCompleted != null)
            {
                HoldCompleted(this, sourceId);
            }
        }

        protected void RaiseManipulationStartedEvent(uint sourceId, Vector3 cumulativeDelta)
        {
            if (ManipulationStarted != null)
            {
                ManipulationStarted(this, sourceId, cumulativeDelta);
            }
        }

        protected void RaiseManipulationUpdatedEvent(uint sourceId, Vector3 cumulativeDelta)
        {
            if (ManipulationUpdated != null)
            {
                ManipulationUpdated(this, sourceId, cumulativeDelta);
            }
        }

        protected void RaiseManipulationCompletedEvent(uint sourceId, Vector3 cumulativeDelta)
        {
            if (ManipulationCompleted != null)
            {
                ManipulationCompleted(this, sourceId, cumulativeDelta);
            }
        }

        protected void RaiseManipulationCanceledEvent(uint sourceId, Vector3 cumulativeDelta)
        {
            if (ManipulationCanceled != null)
            {
                ManipulationCanceled(this, sourceId, cumulativeDelta);
            }
        }

        protected void RaiseNavigationStartedEvent(uint sourceId, Vector3 cumulativeDelta)
        {
            if (NavigationStarted != null)
            {
                NavigationStarted(this, sourceId, cumulativeDelta);
            }
        }

        protected void RaiseNavigationUpdatedEvent(uint sourceId, Vector3 cumulativeDelta)
        {
            if (NavigationUpdated != null)
            {
                NavigationUpdated(this, sourceId, cumulativeDelta);
            }
        }

        protected void RaiseNavigationCompletedEvent(uint sourceId, Vector3 cumulativeDelta)
        {
            if (NavigationCompleted != null)
            {
                NavigationCompleted(this, sourceId, cumulativeDelta);
            }
        }

        protected void RaiseNavigationCanceledEvent(uint sourceId, Vector3 cumulativeDelta)
        {
            if (NavigationCanceled != null)
            {
                NavigationCanceled(this, sourceId, cumulativeDelta);
            }
        }

        #endregion
    }
}

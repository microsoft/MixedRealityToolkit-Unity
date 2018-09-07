// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Utilities.Solvers
{
    /// <summary>
    /// ControllerFinder is a base class providing simple event handling for getting/releasing MotionController Transforms.
    /// </summary>
    public abstract class ControllerFinder : MonoBehaviour, IMixedRealitySourceStateHandler
    {
        [SerializeField]
        private Handedness handedness = Handedness.None;

        public Handedness Handedness
        {
            get { return handedness; }
            set
            {
                // We need to refresh which controller we're attached to if we switch handedness.
                if (handedness != value)
                {
                    handedness = value;
                    RefreshControllerTransform();
                }
            }
        }

        protected Transform ControllerTransform;

        private IMixedRealityDeviceManager BaseDeviceManager => baseDeviceManager ?? (baseDeviceManager = MixedRealityManager.Instance.GetManager<IMixedRealityDeviceManager>());
        private IMixedRealityDeviceManager baseDeviceManager = null;

        #region MonoBehaviour Implementation

        protected virtual void OnEnable()
        {
            if (BaseDeviceManager == null)
            {
                // The base device manager has not been set up yet.
                return;
            }

            // Look if the controller has loaded.
            RefreshControllerTransform();
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealitySourceStateHandler Implementation

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            if (eventData.Controller.ControllerHandedness == handedness)
            {
                AddControllerTransform(eventData.Controller);
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.Controller.ControllerHandedness == handedness)
            {
                RemoveControllerTransform();
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation

        /// <summary>
        /// Looks to see if the controller model already exists and registers it if so.
        /// </summary>
        protected virtual void TryAndAddControllerTransform()
        {
            // Look if the controller was already loaded. This could happen if the
            // GameObject was instantiated at runtime and the model loaded event has already fired.
            if (BaseDeviceManager == null)
            {
                // The BaseDeviceManager could not be found.
                return;
            }

            IMixedRealityController[] controllers = BaseDeviceManager.GetActiveControllers();

            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i].ControllerHandedness == handedness)
                {
                    AddControllerTransform(controllers[i]);
                    return;
                }
            }
        }

        protected virtual void AddControllerTransform(IMixedRealityController newController)
        {
            if (newController.ControllerHandedness == handedness && newController.Transform != null && !newController.Transform.Equals(ControllerTransform))
            {
                ControllerTransform = newController.Transform;

                OnControllerFound();
            }
        }

        protected virtual void RemoveControllerTransform()
        {
            if (ControllerTransform != null)
            {
                OnControllerLost();

                ControllerTransform = null;
            }
        }

        protected virtual void RefreshControllerTransform()
        {
            if (ControllerTransform != null)
            {
                RemoveControllerTransform();
            }

            TryAndAddControllerTransform();
        }

        /// <summary>
        /// Override this method to act when the correct controller is actually found.
        /// This provides similar functionality to overriding AddControllerTransform,
        /// without the overhead of needing to check that handedness matches.
        /// </summary>
        protected virtual void OnControllerFound() { }

        /// <summary>
        /// Override this method to act when the correct controller is actually lost.
        /// This provides similar functionality to overriding AddControllerTransform,
        /// without the overhead of needing to check that handedness matches.
        /// </summary>
        protected virtual void OnControllerLost() { }
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="Microsoft.MixedReality.Toolkit.Teleport.IMixedRealityTeleportSystem"/>
    /// </summary>
    public class MixedRealityTeleportSystem : BaseCoreSystem, IMixedRealityTeleportSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public MixedRealityTeleportSystem(
            IMixedRealityServiceRegistrar registrar) : base(registrar, null) // Teleport system does not use a profile
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MixedRealityTeleportSystem() : base(null) // Teleport system does not use a profile
        {
            IsInputSystemEnabled = CoreServices.InputSystem != null;
        }

        private TeleportEventData teleportEventData;

        private bool isTeleporting = false;
        private bool isProcessingTeleportRequest = false;

        private Vector3 targetPosition = Vector3.zero;
        private Vector3 targetRotation = Vector3.zero;

        /// <summary>
        /// only used to clean up event system when shutting down if this system created one.
        /// </summary>
        private GameObject eventSystemReference = null;

        #region IMixedRealityService Implementation

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Mixed Reality Teleport System";

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
        }

        private void InitializeInternal()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                var eventSystems = Object.FindObjectsOfType<EventSystem>();

                if (eventSystems.Length == 0)
                {
                    if (!IsInputSystemEnabled)
                    {
                        eventSystemReference = new GameObject("Event System");
                        eventSystemReference.AddComponent<EventSystem>();
                    }
                    else
                    {
                        Debug.Log("The Input System didn't properly add an event system to your scene. Please make sure the Input System's priority is set higher than the teleport system.");
                    }
                }
                else if (eventSystems.Length > 1)
                {
                    Debug.Log("Too many event systems in the scene. The Teleport System requires only one.");
                }
            }
#endif // UNITY_EDITOR

            teleportEventData = new TeleportEventData(EventSystem.current);
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            if (eventSystemReference != null)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(eventSystemReference);
                }
                else
                {
                    Object.Destroy(eventSystemReference);
                }
            }
        }

        #endregion IMixedRealityService Implementation

        #region IEventSystemManager Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            Debug.Assert(eventData != null);
            var teleportData = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
            Debug.Assert(teleportData != null);
            Debug.Assert(!teleportData.used);

            // Process all the event listeners
            base.HandleEvent(teleportData, eventHandler);
        }

        /// <summary>
        /// Unregister a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> from listening to Teleport events.
        /// </summary>
        public override void Register(GameObject listener)
        {
            base.Register(listener);
        }

        /// <summary>
        /// Unregister a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> from listening to Teleport events.
        /// </summary>
        public override void Unregister(GameObject listener)
        {
            base.Unregister(listener);
        }

        #endregion IEventSystemManager Implementation

        #region IMixedRealityTeleportSystem Implementation
        /// <summary>
        /// Is there an input system registered.
        /// </summary>
        private bool IsInputSystemEnabled = false;

        private float teleportDuration = 0.25f;

        /// <inheritdoc />
        public float TeleportDuration
        {
            get { return teleportDuration; }
            set
            {
                if (isProcessingTeleportRequest)
                {
                    Debug.LogWarning("Couldn't change teleport duration. Teleport in progress.");
                    return;
                }

                teleportDuration = value;
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportRequestHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportRequest(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportRequest(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
            HandleEvent(teleportEventData, OnTeleportRequestHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportStartedHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportStarted(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportStarted(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            if (isTeleporting)
            {
                Debug.LogError("Teleportation already in progress");
                return;
            }

            isTeleporting = true;

            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
            HandleEvent(teleportEventData, OnTeleportStartedHandler);

            ProcessTeleportationRequest(teleportEventData);
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportCompletedHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportCompleted(casted);
            };

        /// <summary>
        /// Raise a teleportation completed event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target</param>
        private void RaiseTeleportComplete(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            if (!isTeleporting)
            {
                Debug.LogError("No Active Teleportation in progress.");
                return;
            }

            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
            HandleEvent(teleportEventData, OnTeleportCompletedHandler);

            isTeleporting = false;
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityTeleportHandler> OnTeleportCanceledHandler =
            delegate (IMixedRealityTeleportHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
                handler.OnTeleportCanceled(casted);
            };

        /// <inheritdoc />
        public void RaiseTeleportCanceled(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot)
        {
            // initialize event
            teleportEventData.Initialize(pointer, hotSpot);

            // Pass handler
            HandleEvent(teleportEventData, OnTeleportCanceledHandler);
        }

        #endregion IMixedRealityTeleportSystem Implementation

        private void ProcessTeleportationRequest(TeleportEventData eventData)
        {
            isProcessingTeleportRequest = true;

            targetRotation = Vector3.zero;
            var teleportPointer = eventData.Pointer as IMixedRealityTeleportPointer;
            if (teleportPointer != null)
            {
                targetRotation.y = teleportPointer.PointerOrientation;
            }
            targetPosition = eventData.Pointer.Result.Details.Point;

            if (eventData.HotSpot != null)
            {
                targetPosition = eventData.HotSpot.Position;

                if (eventData.HotSpot.OverrideTargetOrientation)
                {
                    targetRotation.y = eventData.HotSpot.TargetOrientation;
                }
            }

            float height = targetPosition.y;
            targetPosition -= CameraCache.Main.transform.position - MixedRealityPlayspace.Position;
            targetPosition.y = height;

            MixedRealityPlayspace.Position = targetPosition;
            MixedRealityPlayspace.RotateAround(
                        CameraCache.Main.transform.position, 
                        Vector3.up, 
                        targetRotation.y - CameraCache.Main.transform.eulerAngles.y);

            isProcessingTeleportRequest = false;

            // Raise complete event using the pointer and hot spot provided.
            RaiseTeleportComplete(eventData.Pointer, eventData.HotSpot);
        }
    }
}

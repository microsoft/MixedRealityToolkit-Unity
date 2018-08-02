// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Teleport;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.TeleportSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.SDK.Teleportation
{
    /// <summary>
    /// The Mixed Reality Toolkit's specific implementation of the <see cref="IMixedRealityTeleportSystem"/>
    /// </summary>
    public class MixedRealityTeleportManager : MixedRealityEventManager, IMixedRealityTeleportSystem
    {
        private TeleportEventData teleportEventData;

        #region IMixedRealityManager Implementation

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
                    if (!MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled)
                    {
                        new GameObject("Event System").AddComponent<EventSystem>();
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

        #endregion IMixedRealityManager Implementation

        #region IEventSystemManager Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            Debug.Assert(eventData != null);
            var baseInputEventData = ExecuteEvents.ValidateEventData<TeleportEventData>(eventData);
            Debug.Assert(baseInputEventData != null);
            Debug.Assert(!baseInputEventData.used);

            // Process all the event listeners
            for (int i = 0; i < EventListeners.Count; i++)
            {
                // Stop if we've used the input event data.
                if (baseInputEventData.used)
                {
                    break;
                }

                ExecuteEvents.Execute(EventListeners[i], baseInputEventData, eventHandler);
            }
        }

        /// <summary>
        /// Unregister a <see cref="GameObject"/> from listening to Teleport events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Register(GameObject listener)
        {
            if (EventListeners.Contains(listener)) { return; }
            EventListeners.Add(listener);
        }

        /// <summary>
        /// Unregister a <see cref="GameObject"/> from listening to Teleport events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Unregister(GameObject listener)
        {
            if (!EventListeners.Contains(listener)) { return; }
            EventListeners.Remove(listener);
        }

        #endregion IEventSystemManager Implementation

        /// <inheritdoc />
        public void RaiseTeleportRequest(IMixedRealityPointer pointer, ITeleportTarget target)
        {
            teleportEventData.Initialize(pointer, target);
        }

        /// <inheritdoc />
        public void RaiseTeleportStarted(IMixedRealityPointer pointer, ITeleportTarget target)
        {
            teleportEventData.Initialize(pointer, target);
        }

        /// <inheritdoc />
        public void RaiseTeleportComplete(IMixedRealityPointer pointer, ITeleportTarget target)
        {
            teleportEventData.Initialize(pointer, target);
        }

        /// <inheritdoc />
        public void RaiseTeleportCanceled(IMixedRealityPointer pointer, ITeleportTarget target)
        {
            teleportEventData.Initialize(pointer, target);
        }
    }
}

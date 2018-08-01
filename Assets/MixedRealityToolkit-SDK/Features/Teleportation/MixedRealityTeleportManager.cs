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

        /// <inheritdoc />
        public void RaiseTeleportRequest(IMixedRealityPointer pointer)
        {
        }

        /// <inheritdoc />
        public void RaiseTeleportStarted(IMixedRealityPointer pointer)
        {

        }

        /// <inheritdoc />
        public void RaiseTeleportComplete(IMixedRealityPointer pointer)
        {

        }

        /// <inheritdoc />
        public void RaiseTeleportCanceled(IMixedRealityPointer pointer)
        {

        }
    }
}

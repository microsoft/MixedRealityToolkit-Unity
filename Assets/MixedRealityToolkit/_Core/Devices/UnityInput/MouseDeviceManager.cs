// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Devices.UnityInput
{
    public class MouseDeviceManager : BaseDeviceManager, IMixedRealityComponent
    {
        public MouseDeviceManager(string name, uint priority) : base(name, priority) { }

        /// <summary>
        /// Current Mouse Controller.
        /// </summary>
        public MouseController Controller { get; private set; }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Input.mousePresent)
            {
                Disable();
                return;
            }

#if UNITY_EDITOR
            UnityEditor.EditorWindow.focusedWindow.ShowNotification(new GUIContent("Press \"ESC\" to regain mouse control"));
#endif

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            IMixedRealityInputSource mouseInputSource = null;

            MixedRealityRaycaster.DebugEnabled = true;

            if (MixedRealityOrchestrator.InputSystem != null)
            {
                var pointers = RequestPointers(new SystemType(typeof(MouseController)), Handedness.Any, true);
                mouseInputSource = MixedRealityOrchestrator.InputSystem.RequestNewGenericInputSource("Mouse Input", pointers);
            }

            Controller = new MouseController(TrackingState.NotApplicable, Handedness.Any, mouseInputSource);

            if (mouseInputSource != null)
            {
                for (int i = 0; i < mouseInputSource.Pointers.Length; i++)
                {
                    mouseInputSource.Pointers[i].Controller = Controller;
                }
            }

            Controller.SetupConfiguration(typeof(MouseController));
            MixedRealityOrchestrator.InputSystem?.RaiseSourceDetected(Controller.InputSource, Controller);
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (Input.mousePresent && Controller == null) { Enable(); }

            Controller?.Update();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (Controller != null)
            {
                MixedRealityOrchestrator.InputSystem?.RaiseSourceLost(Controller.InputSource, Controller);
            }
        }
    }
}
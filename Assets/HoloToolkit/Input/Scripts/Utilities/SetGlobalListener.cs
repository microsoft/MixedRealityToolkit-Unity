// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Register this game object on the InputManager as a global listener.
    /// </summary>
    public class SetGlobalListener : StartAwareBehaviour
    {
        protected override void OnEnableAfterStart()
        {
            base.OnEnableAfterStart();

            if (InputManager.IsInitialized)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }
        }

        protected override void OnDisableAfterStart()
        {
            if (InputManager.IsInitialized)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }

            base.OnDisableAfterStart();
        }

        private void OnDestroy()
        {
            if (InputManager.IsInitialized)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }
    }
}
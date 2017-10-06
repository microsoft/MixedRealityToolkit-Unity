//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.Receivers
{
    /// <summary>
    /// Simple receiver class for toggling a game object active or inactive.
    /// </summary>
    public class ToggleActiveReceiver : InteractionReceiver
    {
        /// <summary>
        /// When receiving the input down button toggle active/inactive all targets
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        protected override void InputDown(GameObject obj, InputEventData args)
        {
            if (Targets.Count > 0)
            {
                foreach(GameObject target in Targets)
                {
                    target.SetActive(!target.activeSelf);
                }
            }
        }
    }
}
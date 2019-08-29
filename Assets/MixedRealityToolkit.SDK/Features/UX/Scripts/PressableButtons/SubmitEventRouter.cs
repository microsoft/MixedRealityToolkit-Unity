// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This is a helper class to allow you to call OnSubmit() on a <see cref="UnityEngine.UI.Button"/> or other control derived from <see cref="ISubmitHandler"/> on this gameObject.
    /// It exposes a public function that can be bound in the Editor to a Unity Event.
    /// </summary>
    public class SubmitEventRouter : MonoBehaviour
    {
        public void Submit()
        {
            ExecuteEvents.Execute(this.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity.Examples.UX
{
    public class BoundingBoxRigActivityExample : MonoBehaviour, IBoundingBoxStateHandler
    {
        public void OnBoundingBoxRigActivated(BoundingBoxEventData eventData)
        {
            Debug.LogFormat("{0}'s bounding box rig is activated", eventData.BoundingBoxRiggedObject.name);
        }

        public void OnBoundingBoxRigDeactivated(BoundingBoxEventData eventData)
        {
            Debug.LogFormat("{0}'s bounding box rig is deactivated", eventData.BoundingBoxRiggedObject.name);
        }

        private void Start()
        {
            // We have to add a global listener in this case because the focused object is the rig's app bar
            // So, it will be the app bar which will receive the event, not this listener
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        private void OnDestroy()
        {
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }
    }
}
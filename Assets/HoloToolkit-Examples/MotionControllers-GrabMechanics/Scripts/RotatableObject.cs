// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using System.Collections;
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule.Examples.Grabbables
{
    /// <summary>
    /// ForceRotate inherits from BaseUsable because the object to be manipulated must first be
    /// pick up (grabbed) and is then "usable"
    /// </summary>
    public class RotatableObject : BaseUsable
    {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        private Vector3 touchPositionFromController = Vector3.zero;
#endif

        private BaseGrabbable baseGrabbable;

        protected override void OnEnable()
        {
            base.OnEnable();

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            InteractionManager.InteractionSourceUpdated += GetTouchPadPosition;
#endif

            if (baseGrabbable == null)
            {
                baseGrabbable = GetComponent<BaseGrabbable>();
            }
        }

        protected override void OnDisable()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            InteractionManager.InteractionSourceUpdated -= GetTouchPadPosition;
#endif

            base.OnDisable();
        }

        /// <summary>
        /// In the BaseUsable class that this class inherits from, UseStarted begins checking for usage
        /// after the object is grabbed/picked up
        /// </summary>
        protected override void UseStart()
        {
            if (baseGrabbable.GrabberPrimary != null)
            {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
                StartCoroutine(MakeRotate());
#endif
            }
        }

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        private IEnumerator MakeRotate()
        {
            while (UseState == UseStateEnum.Active && baseGrabbable.GrabberPrimary && touchPadPressed)
            {
                transform.Rotate(touchPositionFromController);
                yield return 0;
            }
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            yield return null;
        }

        private void GetTouchPadPosition(InteractionSourceUpdatedEventArgs obj)
        {
            if (baseGrabbable.GrabberPrimary != null)
            {
                Debug.Log(" obj.state.source.handedness =====" + obj.state.source.handedness + "   **** GrabberPriumary Handedness === " + baseGrabbable.GrabberPrimary.Handedness);
                if (obj.state.source.handedness == baseGrabbable.GrabberPrimary.Handedness)
                {
                    if (obj.state.touchpadTouched)
                    {
                        touchPositionFromController = obj.state.touchpadPosition;
                        touchPadPressed = true;
                    }
                    else
                    {
                        touchPadPressed = false;
                    }
                }
            }
        }

        private bool touchPadPressed;
#endif
    }
}

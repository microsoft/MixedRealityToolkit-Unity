using System.Collections;
using UnityEngine;
using UnityEngine.XR.WSA.Input;


namespace MRTK.Grabbables
{
    /// <summary>
    /// ForceRotate inherits from BaseUsable because the object to be manipulated must first be
    /// pick up (grabbed) and is then "usable"
    /// </summary>

    public class RotatableObject : BaseUsable
    {
        private Vector3 touchPositionFromController = Vector3.zero;

        protected override void OnEnable()
        {
            base.OnEnable();
            InteractionManager.InteractionSourceUpdated += GetTouchPadPosition;

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InteractionManager.InteractionSourceUpdated -= GetTouchPadPosition;

        }

        /// <summary>
        /// In the BaseUsable class that this class inherits from, UseStarted begins checking for usage
        /// after the object is grabbed/picked up
        /// </summary>
        /// <param name="obj"></param>
        /// 

        protected override void UseStart()
        {
            if (GetComponent<BaseGrabbable>().GrabberPrimary != null)
            {
                StartCoroutine(MakeRotate());
            }

        }

        private IEnumerator MakeRotate( )
        {
            while (UseState == UseStateEnum.Active && GetComponent<BaseGrabbable>().GrabberPrimary && touchPadPressed) {
                transform.Rotate(touchPositionFromController);
                yield return 0;
            }
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            yield return null;
        }

        private void GetTouchPadPosition(InteractionSourceUpdatedEventArgs obj)
        {
            if (GetComponent<BaseGrabbable>().GrabberPrimary != null)
            {
                Debug.Log( " obj.state.source.handedness =====" + obj.state.source.handedness+ "   **** GrabberPriumary Handedness === " + GetComponent<BaseGrabbable>().GrabberPrimary.Handedness);
                if (obj.state.source.handedness == GetComponent<BaseGrabbable>().GrabberPrimary.Handedness)
                {
                    if (obj.state.touchpadTouched)
                    {
                        touchPositionFromController = obj.state.touchpadPosition;
                        touchPadPressed = true;
                    } else
                    {
                        touchPadPressed = false;
                    }
                }
            }
        }

        private bool touchPadPressed;
    }
}
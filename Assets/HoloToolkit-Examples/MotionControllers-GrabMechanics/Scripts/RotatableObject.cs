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
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// In the BaseUsable class that this class inherits from, UseStarted begins checking for usage
        /// after the object is grabbed/picked up
        /// </summary>
        /// <param name="obj"></param>
        /// 

        protected void Start()
        {
        }

        protected override void UseStart()
        {
            StartCoroutine(MakeRotate( ));
            InteractionManager.InteractionSourceUpdated += GetTouchPadPosition;

        }

        protected override void UseEnd()
        {
            InteractionManager.InteractionSourceUpdated -= GetTouchPadPosition;

        }

        private IEnumerator MakeRotate( )
        {
            while (UseState == UseStateEnum.Active) {
                transform.Rotate(touchPositionFromController);
                yield return 0;
            }
            yield return null;
        }

        private void GetTouchPadPosition(InteractionSourceUpdatedEventArgs obj)
        {
            if (GetComponent<BaseGrabbable>().GrabberPrimary != null)
            {
                if (obj.state.source.handedness == GetComponent<BaseGrabbable>().GrabberPrimary.Handedness)
                {
                    if (obj.state.touchpadTouched)
                    {
                        touchPositionFromController = obj.state.touchpadPosition;
                    }
                }
            }
        }
    }
}
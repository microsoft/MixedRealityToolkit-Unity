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
        private Vector3 touchPositionFromController;

        protected override void OnEnable()
        {
            base.OnEnable();
            //InteractionManager.InteractionSourceUpdated += GetTouchPadPosition;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
           // InteractionManager.InteractionSourceUpdated -= GetTouchPadPosition;
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
            InteractionManager.InteractionSourceUpdated += GetTouchPadPosition;
            StartCoroutine(MakeRotate( ));
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
            if (obj.state.touchpadPressed)
            {
                Debug.Log("Event!~ " + obj.state.source.handedness);
            }
            
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
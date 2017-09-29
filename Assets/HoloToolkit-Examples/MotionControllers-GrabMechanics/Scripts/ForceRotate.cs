using System.Collections;
using UnityEngine;
using UnityEngine.XR.WSA.Input;


namespace MRTK.Grabbables
{
    /// <summary>
    /// ForceRotate inherits from BaseUsable because the object to be manipulated must first be
    /// pick up (grabbed) and is then "usable"
    /// </summary>

    public class ForceRotate : BaseUsable
    {
        /// <summary>
        /// In the BaseUsable class that this class inherits from, UseStarted begins checking for usage
        /// after the object is grabbed/picked up
        /// </summary>
        /// <param name="obj"></param>
        protected override void UseStart()
        {
            StartCoroutine(MakeRotate( ));
        }

        protected override void UseEnd()
        {
            // Nothing
        }

        private IEnumerator MakeRotate( )
        {
            while (UseState == UseStateEnum.Active) {                
                //transform.Rotate(obj.state.thumbstickPosition.x, obj.state.thumbstickPosition.y, 0.01f);
                yield return 0;
            }
            yield return null;
        }
    }
}
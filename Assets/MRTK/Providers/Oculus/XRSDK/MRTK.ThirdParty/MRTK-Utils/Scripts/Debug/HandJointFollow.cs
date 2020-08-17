//------------------------------------------------------------------------------ -
//MRTK - Quest
//https ://github.com/provencher/MRTK-Quest
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Eric Provencher
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace prvncher.MixedReality.Toolkit.Debug
{

    public class HandJointFollow : MonoBehaviour
    {
        [SerializeField]
        private Handedness trackedHandedness = Handedness.Left;

        [SerializeField]
        private TrackedHandJoint trackedJoint = TrackedHandJoint.Palm;

        void LateUpdate()
        {
            IMixedRealityHand hand = GetController(trackedHandedness) as IMixedRealityHand;
            if (hand == null || !hand.TryGetJoint(trackedJoint, out MixedRealityPose pose))
            {
                SetChildrenActive(false);
                return;
            }
            SetChildrenActive(true);
            transform.position = pose.Position;
            transform.rotation = pose.Rotation;
        }

        private void SetChildrenActive(bool isActive)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isActive);
            }
        }

        private static IMixedRealityController GetController(Handedness handedness)
        {
            foreach (IMixedRealityController c in CoreServices.InputSystem.DetectedControllers)
            {
                if (c.ControllerHandedness.IsMatch(handedness))
                {
                    return c;
                }
            }
            return null;
        }
    }

}
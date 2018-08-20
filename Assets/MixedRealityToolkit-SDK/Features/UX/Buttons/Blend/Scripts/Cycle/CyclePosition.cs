// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace Blend.Cycle
{
    /// <summary>
    /// updates the position of an object based on currently selected values from the array.
    /// Use MoveToPosition for easing... Auto detected
    /// </summary>
    public class CyclePosition : CycleArray<Vector3>
    {
        [Tooltip("use local position instead of position. Overrides MoveToPosition ToLocalPosition setting.")]
        public bool UseLocalPosition = false;

        private BlendPosition mTransition;

        protected override void Awake()
        {
            base.Awake();
            mTransition = TargetObject.GetComponent<BlendPosition>();
        }

        /// <summary>
        /// set the position
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            Vector3 item = Array[Index];

            // use MoveTo Position
            if (mTransition != null)
            {
                mTransition.ToLocalTransform = UseLocalPosition;
                mTransition.TargetValue = item;
                mTransition.Play();
            }
            else
            {
                if (UseLocalPosition)
                {
                    TargetObject.transform.localPosition = item;
                }
                else
                {
                    TargetObject.transform.position = item;
                }
                
            }
        }

    }
}

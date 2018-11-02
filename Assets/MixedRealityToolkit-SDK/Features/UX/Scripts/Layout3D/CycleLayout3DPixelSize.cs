// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Layout3D
{
    /// <summary>
    /// scales an object based on the selected value in the array
    /// Use with Layout3DPixelSize to build responsive layouts
    /// </summary>
    [RequireComponent(typeof(Layout3DPixelSize))]
    public class CycleLayout3DPixelSize : MonoBehaviour
    {
        /// <summary>
        /// A set of sizes for Layout3DPixelSize
        /// </summary>
        [Tooltip("List of sizes for the Layout3DPixelSize object to switch to")]
        public Vector3[] Sizes;
        
        /// <summary>
        /// The starting index of the cycle
        /// </summary>
        [Tooltip("The start index")]
        [SerializeField]
        private int Index;

        /// <summary>
        /// The animation time for size changes
        /// </summary>
        [Tooltip("The amount of time to animate size changes")]
        [SerializeField]
        private float EaseTime = 0.5f;

        /// <summary>
        /// An animation curve for animated size changes
        /// </summary>
        [Tooltip("An animation curve to add easing")]
        [SerializeField]
        private AnimationCurve EaseCurve;

        // the current pixel size
        private Vector3 currentSize;
        private Vector3 startSize;
        private Layout3DPixelSize pixelSize;
        private Easing easing;

        protected void Awake()
        {
            pixelSize = GetComponent<Layout3DPixelSize>();
            easing = new Easing();
            if (EaseCurve.keys == null || EaseCurve.keys.Length < 2)
            {
                easing.SetCurve(Easing.BasicEaseCurves.Linear);
            }
            else
            {
                easing.Curve = EaseCurve;
            }

            easing.LerpTime = EaseTime;
            easing.Enabled = true;
            easing.Stop();
            SetValues(Index, true);
        }

        /// <summary>
        /// Get the current index
        /// </summary>
        /// <returns></returns>
        public int GetIndex()
        {
            return Index;
        }

        /// <summary>
        /// Get the currenct size
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurrentSize()
        {
            return currentSize;
        }

        /// <summary>
        /// Advance the cycle
        /// </summary>
        public void Next()
        {
            SetIndex(Index + 1);
        }

        /// <summary>
        /// regress the cycle
        /// </summary>
        public void Previous()
        {
            SetIndex(Index - 1);
        }

        /// <summary>
        /// Set the scale value or animate size
        /// </summary>
        /// <param name="index"></param>
        public void SetIndex(int index)
        {
            SetValues(index, false);
        }

        private void SetValues(int index, bool force)
        {
            startSize = currentSize;

            if (index < 0)
            {
                index = Sizes.Length - 1;
            }
            else if (index >= Sizes.Length)
            {
                index = 0;
            }

            currentSize = Sizes[index];
            
            if (pixelSize != null)
            {
                if (force)
                {
                    pixelSize.SetSize(currentSize);
                }
                else if(easing != null)
                {
                    easing.Start();
                }
            }

            Index = index;
        }

        private void Update()
        {
            if(easing != null && easing.IsPlaying())
            {
                easing.OnUpdate();
                pixelSize.SetSize(Vector3.Lerp(startSize, currentSize, easing.GetCurved()));
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Interact
{

    /// <summary>
    /// Adds double-tap functionality
    /// </summary>
    public class InteractiveDoubleTap : Interactive
    {
        [Tooltip("Enable double-tap")]
        public bool DetectDoubleTap = true;
        
        public UnityEvent OnDoubleTap;
        
        protected float doubleTapTime = 0.3f;
        private Coroutine tapRoutine;

        /// <summary>
        /// Check for double-tap if the tap routine has not ended since the last click.
        /// </summary>
        protected override void HandleTaps()
        {
            if (!DetectDoubleTap)
            {
                OnClick.Invoke();
                return;
            }

            if (tapRoutine == null)
            {
                tapRoutine = StartCoroutine(TapTimer());
            }
            else
            {
                StopCoroutine(tapRoutine);
                tapRoutine = null;
                OnDoubleTap.Invoke();
            }
        }

        /// <summary>
        /// Creates a coroutine for checking doubletaps.
        /// </summary>
        /// <returns></returns>
        private IEnumerator TapTimer()
        {
            yield return new WaitForSeconds(doubleTapTime);
            OnClick.Invoke();
            tapRoutine = null;
        }

    }
}

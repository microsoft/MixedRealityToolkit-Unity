// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.UX.Tests
{
    /// <summary>
    /// Sample feedback for a hold timer
    /// </summary>
    public class InteractiveHoldTimer : MonoBehaviour
    {
        public Interactive Button;
        private Vector3 mStartScale;
        private void Start()
        {
            mStartScale = this.gameObject.transform.localScale;
        }

        // Update is called once per frame
        private void Update()
        {
            mStartScale.x = Button.GetHoldPercentage();
            this.gameObject.transform.localScale = mStartScale;
        }
    }
}

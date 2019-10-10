// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.UI
{
    public class HideAfterEnabled : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Number of seconds to wait before hiding the GameObject")]
        private float timeToHideLabel = 3f;

        private Coroutine hideCoroutine = null;

        private void OnEnable()
        {
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }

            hideCoroutine = StartCoroutine(HideAfterSeconds());
        }

        private IEnumerator HideAfterSeconds()
        {
            yield return new WaitForSeconds(timeToHideLabel);
            gameObject.SetActive(false);
        }
    }
}

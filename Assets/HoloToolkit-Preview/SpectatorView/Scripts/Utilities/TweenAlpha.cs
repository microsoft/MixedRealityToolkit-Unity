// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Utility for fading out marker
    /// </summary>
    public class TweenAlpha : MonoBehaviour
    {
        /// <summary>
        /// Alpha value to fade to
        /// </summary>
        [Tooltip("Alpha value to fade to")]
        [SerializeField]
        private float targetAlpha;
        /// <summary>
        /// Alpha value to fade to
        /// </summary>
        public float TargetAlpha
        {
            get
            {
                return targetAlpha;
            }

            set
            {
                targetAlpha = value;
            }
        }

        /// <summary>
        /// Time taken to fade from current alpha to TargetAlpha
        /// </summary>
        [Tooltip("Time taken to fade from current alpha to TargetAlpha")]
        [SerializeField]
        private float duration = 0.5f;
        /// <summary>
        /// Time taken to fade from current alpha to TargetAlpha
        /// </summary>
        public float Duration
        {
            get
            {
                return duration;
            }

            set
            {
                duration = value;
            }
        }

        /// <summary>
        /// Material to operate on
        /// </summary>
        private Material mat;

        private void Start()
        {
            if (mat == null)
            {
                mat = GetComponent<Renderer>().material;
            }
        }

        /// <summary>
        /// Start the alpha fade coroutine
        /// </summary>
        public void StartEffect()
        {
            StartCoroutine(LerpAlpha());
        }

        /// <summary>
        /// Fades alpha from current alpha to TargetAlpha in time Duration
        /// </summary>
        private IEnumerator LerpAlpha()
        {
            var elapsedTime = 0.0f;
            var currentA = mat.color.a;
            while (elapsedTime < Duration)
            {
                elapsedTime += Time.deltaTime;
                var a = Mathf.Lerp(currentA, TargetAlpha, (elapsedTime / Duration));
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, a);
                yield return null;
            }
        }
    }
}

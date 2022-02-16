// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Destroys the Game Object after the length of the Animator component.
    /// Attach this script to any game object with Animator component to destroy on animation complete.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Scripts/MRTK/SDK/DestroyOnAnimationComplete")]
    public class DestroyOnAnimationComplete : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Additional delay after the animation complete")]
        private float delay = 1.0f;
        public float Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        void Start()
        {
            Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
        }
    }
}

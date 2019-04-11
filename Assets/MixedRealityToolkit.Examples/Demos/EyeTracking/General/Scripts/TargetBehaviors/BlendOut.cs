// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// This script allows for dynamically blending out a target after it has been looked at for a certain amount of time. 
    /// </summary>
    public class BlendOut : MonoBehaviour
    {
        #region Serialized variables
        [Tooltip("Speed for blending out target.")]
        [SerializeField]
        private float BlendOutSpeed = 1.0f;

        [Tooltip("Minimal transparency between 0 and 1.")]
        [Range(0, 1)]
        [SerializeField]
        private float MinTransparency = 0.02f;

        [Tooltip("Transparency between 0 and 1 that will be set once the user looks at the target.")]
        [Range(0, 1)]
        [SerializeField]
        private float LookAtTransparency = 0.9f;

        [Tooltip("Idle transparency between 0 and 1 that will be set if the user looks away from the " +
            "target, but didn’t look at it long enough to be considered “fully engaged”. In this case, " +
            "it simply returns to an idle state.")]
        [Range(0, 1)]
        [SerializeField]
        private float IdleTransparency = 0.4f;

        [Tooltip("Boolean to decide whether to destroy the target once the blend out is complete.")]
        [SerializeField]
        private bool DestroyAfterBlendOut = false;

        [Tooltip("Depending on the materials applied to the notification, we need to check specific supported properties for blending.")]
        [SerializeField]
        string[] shaderPropsToCheckForBlending = new string[] { "_Color", "_FaceColor" };
        #endregion

        private GameObject target = null;
        private GameObject objectWithCollider = null;
        private bool fadeOut = false;
        private bool destroyIt = false;
        private bool wasDwelling = false;
        private float normalizedProgress;

        private void Start()
        {
            InitialSetup();
        }

        /// <summary>
        /// Making sure the relevant variables (target game object and collider) get assigned.
        /// </summary>
        private void InitialSetup()
        {
            if (target == null)
            {
                target = gameObject;
            }

            if (objectWithCollider == null)
            {
                Collider coll;
                coll = GetComponent<Collider>();
                if (coll == null)
                {
                    coll = GetComponentInChildren<Collider>();
                }

                if (coll != null)
                {
                    objectWithCollider = GetComponentInChildren<Collider>().gameObject;
                }
            }
        }

        /// <summary>
        /// Once the target is looked at, set it to its full "look at transparency" and let's prevent its destruction when being looked at.
        /// </summary>
        public void Engage()
        {
            destroyIt = false;
            ChangeTransparency(LookAtTransparency);
        }

        /// <summary>
        /// Once the user looks away from the hologram, determine whether the user has dwelled at it before. 
        /// If yes, start blending it out and destroy it if that option was selected. 
        /// If not, we can still blend it out a little, but keep it alive and visible.
        /// </summary>
        public void Disengage()
        {
            if (wasDwelling)
            {
                destroyIt = true;
            }

            fadeOut = true;
            wasDwelling = false;
            normalizedProgress = 1f;
        }

        public void DwellSucceeded()
        {
            Debug.Log("DwellSucceeded ");
            wasDwelling = true;
        }

        // Update is called once per frame
        public void Update()
        {
            // Continuously updates the blend out after disengaging
            SlowlyBlendOut();
        }

        private void SlowlyBlendOut()
        {
            if (fadeOut)
            {
                if (DestroyAfterBlendOut && destroyIt)
                {
                    // Fade out and destroy
                    if (normalizedProgress <= MinTransparency)
                    {
                        fadeOut = false;
                        Destroy(target);
                        return;
                    }
                }
                else
                {
                    // Fade out
                    if (normalizedProgress <= IdleTransparency)
                    {
                        fadeOut = false;
                        return;
                    }
                }

                // Otherwise, let's continue slowly blending the target out
                normalizedProgress = Mathf.Clamp(normalizedProgress - (BlendOutSpeed * Time.deltaTime), MinTransparency, 1);

                ChangeTransparency(normalizedProgress);
            }
        }

        private void ChangeTransparency(float normalizedProgress)
        {
            for (int i = 0; i < shaderPropsToCheckForBlending.Length; i++)
            {
                ChangeTransparency(normalizedProgress, shaderPropsToCheckForBlending[i]);
            }
        }

        private void ChangeTransparency(float normalizedProgress, string shaderProperty)
        {
            try
            {
                // Handle base renderer
                Renderer[] renderers = target.GetComponents<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].material.HasProperty(shaderProperty))
                    {
                        Materials_BlendOut(renderers[i].materials, normalizedProgress, shaderProperty);
                    }
                }

                // Handle renderer from children
                renderers = target.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i].material.HasProperty(shaderProperty))
                    {
                        Materials_BlendOut(renderers[i].materials, normalizedProgress, shaderProperty);
                    }
                }
            }
            catch (NullReferenceException)
            {
                // Just ignore; Usually happens after the game object already got destroyed, but the update sequence had already be started
            }
        }

        private void Materials_BlendOut(Material[] mats, float alpha, string shaderProperty)
        {
            for (int i = 0; i < mats.Length; i++)
            {
                // Simply get the current color and change its alpha value.
                if (mats[i] != null)
                {
                    Color c = mats[i].GetColor(shaderProperty);
                    mats[i].SetColor(shaderProperty, new Color(c.r, c.g, c.b, alpha));
                }
            }
        }
    }
}
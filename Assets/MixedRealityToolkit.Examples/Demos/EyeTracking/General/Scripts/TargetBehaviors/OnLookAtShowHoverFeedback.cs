// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Targeting
{
    /// <summary>
    /// Handles visual feedback for when a target is looked at, such as the option to highlight the looked at target or to show
    /// a visual anchor at the target's center. Different fade in and fade out options are also available.
    /// </summary>
    [RequireComponent(typeof(EyeTrackingTarget))]
    public class OnLookAtShowHoverFeedback : MonoBehaviour
    {
        // Overlay Feedback: Acts as a visual anchor at the target's center to fixate on.
        [Tooltip("If TRUE: Show a visual indicator at the target center when hovered.")]
        [SerializeField]
        private bool Overlay_UseIt = false;

        [Tooltip("Hover feedback: An instance of this will be displayed at the target center as visual indicator that the target is hovered.")]
        [SerializeField]
        private GameObject Overlay_GameObj = null;

        [Tooltip("Slowly fade overlay in starting from this initial transparency.")]
        [SerializeField]
        [Range(0, 1)]
        private float startTransparency = 0.0f;

        [Tooltip("Finish fade in of the overlay at this given transparency.")]
        [SerializeField]
        [Range(0, 1)]
        private float endTransparency = 1.0f;

        // Highlighting: The entire target is highlighted.
        [Tooltip("If TRUE: Highlight the target when hovered.")]
        [SerializeField]
        private bool Highlight_UseIt = false;

        [Tooltip("Color to use for tinting the target when hovered.")]
        [SerializeField]
        private Color Highlight_Color = Color.white;

        // Parameters for fading in and fading out the visual hover feedback.
        [Tooltip("Fade in start: Minimum hover time before the visual feedback starts fading in.")]
        [SerializeField]
        private int MinDwellTimeInMs = 50;

        [Tooltip("Fade in end: Max hover time indicating when the fade-in is finished. This can be used for slow fade ins.")]
        [SerializeField]
        private int MaxDwellTimeInMs = 2000;

        [Tooltip("Fade out start: Min look away time indicating when the fade-out starts. This is useful to prevent flickering feedback.")]
        [SerializeField]
        private int MinLookAwayTimeInMs = 50;

        [Tooltip("Fade out end: Max look away time indicating when the fade-out is finished.")]
        [SerializeField]
        private int MaxLookAwayTimeInMs = 500;

        [Tooltip("Modes for the type of fade in/out: On/off (boolean), linear blend, etc.")]
        [SerializeField]
        private HoverTransitionType BlendType = HoverTransitionType.LinearBlend;

        private enum HoverTransitionType
        {
            Boolean,
            LinearBlend,
            SlowInitialIncrease,
            FastInitialIncrease,
        }

        // Private variables
        private Color[] originalColors; // Internal variable to store the original base colors
        private Color[] originalEmissionColors; // Internal variable to store the original emission colors
        private DateTime cursorLeaveTime; // Time when the eye gaze ray stopped intersecting the object's hit collider
        private DateTime cursorEnterTime; // Time when the eye gaze ray first intersected with the object's hit collider
        private float normalizedInterest = 0; // Interest between 0 and 1. 0 == Not looked at -> Low interest. 1 == Dwelled -> High interest
        private bool highlightOn = false; // Indicates whether the object is currently highlighted
        private float lastIncreasedInterest = 0; // Remember last value when interest was increased for smooth blend out
        private EyeTrackingTarget eyeTarget = null;

        // Private methods
        private void Start()
        {
            // Init variables
            cursorEnterTime = DateTime.MaxValue;
            eyeTarget = this.GetComponent<EyeTrackingTarget>();
            eyeTarget.OnLookAtStart.AddListener(this.OnLookAtStart);
            eyeTarget.OnLookAway.AddListener(this.OnLookAtStop);

            // Store the original colors for later resetting the object to its original state after highlighting
            SaveOriginalColor();

            if (Overlay_GameObj != null)
            {
                Overlay_GameObj.SetActive(false);
            }
        }

        private void Update()
        {
            if ((eyeTarget.IsLookedAt) || (normalizedInterest > 0))
            {
                // Handle target confidence
                if (eyeTarget.IsLookedAt)
                {
                    // Increase interest
                    normalizedInterest = NormalizedInterest_Dwell();

                    // In case, the cursor moves outside the bounds next, let's remember this value to decrease from.
                    lastIncreasedInterest = normalizedInterest;
                }
                else
                {
                    // Decrease interest
                    normalizedInterest = Mathf.Clamp(lastIncreasedInterest - NormalizedDisinterest_LookAway(), 0, 1);
                }

                // Show feedback
                ShowFeedback(normalizedInterest);
            }

            // Once the user is not pointing at the target anymore, let's free some space and destroy the instance of the visual feedback.
            if ((!eyeTarget.IsLookedAt) && (normalizedInterest == 0) && (highlightOn))
            {
                DestroyLocalFeedback();
            }
        }

        /// <summary>
        /// Clean up once the target is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            DestroyLocalFeedback();
        }

        /// <summary>
        /// Returns a normalized "interest" value between 0 and 1 based on the current dwell time.
        /// </summary>
        /// <returns>Interest value between 0 and 1.</returns>
        private float NormalizedInterest_Dwell()
        {
            return Mathf.Clamp((float)((DwellTimeInMs - MinDwellTimeInMs) / (MaxDwellTimeInMs - MinDwellTimeInMs)), 0, 1);
        }

        /// <summary>
        /// Returns a normalized "disinterest" value between 0 and 1 based on the time the user has been looking away from the target.
        /// </summary>
        /// <returns>Disinterest value between 0 and 1.</returns>
        private float NormalizedDisinterest_LookAway()
        {
            return Mathf.Clamp((float)((LookAwayTimeInMs - MinLookAwayTimeInMs) / (MaxLookAwayTimeInMs - MinLookAwayTimeInMs)), 0, 1);
        }

        /// <summary>
        /// Destroy the overlay instance and disable the highlight state.
        /// </summary>
        private void DestroyLocalFeedback()
        {
            if (Overlay_GameObj != null)
            {
                Overlay_GameObj.SetActive(false);
            }
            highlightOn = false;
        }

        /// <summary>
        /// Shows different types of visual feedback based on given normalized interest level (0-no interest; 1-full interest). 
        /// </summary>
        private void ShowFeedback(float normalizedInterest)
        {
            if (highlightOn)
            {
                // Update interest based on the selected transition type
                normalizedInterest = TransitionAdjustedInterest(normalizedInterest);

                // Show visual overlay to indicate hover state
                ShowFeedback_Overlay(normalizedInterest);

                // Highlight the object itself
                if (Highlight_UseIt)
                {
                    ShowFeedback_Highlight(normalizedInterest);
                }
            }
            else
            {
                normalizedInterest = 0;
                ShowFeedback_Highlight(normalizedInterest);

                if (Overlay_GameObj != null)
                {
                    Overlay_GameObj.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Handles displaying a visual overlay to indicate the hover state.
        /// </summary>
        private void ShowFeedback_Overlay(float normalizedInterest)
        {
            if (Overlay_UseIt)
            {
                try
                {
                    // Change the transparency of the overlay to slowly blend it in/out
                    float newTransparency = normalizedInterest * (endTransparency - startTransparency) + startTransparency;

                    EyeTrackingDemoUtils.GameObject_ChangeTransparency(Overlay_GameObj, newTransparency);

                    // The overlay is only set active if the interest is between 0 and 1.
                    if ((Overlay_GameObj != null) && (normalizedInterest > 0) && (normalizedInterest <= 1))
                    {
                        Overlay_GameObj.SetActive(true);
                        return;
                    }
                }
                catch (Exception)
                {
                    // Just ignore; This sometimes happens after the game object already got destroyed, but the update sequence had already be started
                }
            }
        }

        /// <summary>
        /// Handles highlighting the target.
        /// </summary>
        private void ShowFeedback_Highlight(float normInterest)
        {
            if (Highlight_UseIt)
            {
                try
                {
                    // Handle base renderer
                    Renderer[] renderers = GetComponents<Renderer>();
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        Material[] mats = renderers[i].materials;
                        foreach (Material mat in mats)
                        {
                            // Blend the colors from the original to the highlight color based on the normalized interest
                            // Unity Standard Shaders
                            mat.EnableKeyword("_EmissionColor");
                            mat.SetColor("_EmissionColor", BlendColors(originalEmissionColors[i], Highlight_Color, normInterest));

                            // Mixed Reality Toolkit/Standard
                            mat.SetColor("_EmissiveColor", BlendColors(originalEmissionColors[i], Highlight_Color, normInterest));
                        }
                    }

                    // Handle renderer from children
                    renderers = GetComponentsInChildren<Renderer>();
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        Material[] mats = renderers[i].materials;
                        foreach (Material mat in mats)
                        {
                            // Unity Standard Shaders
                            mat.EnableKeyword("_EmissionColor");
                            mat.SetColor("_EmissionColor", BlendColors(originalEmissionColors[i], Highlight_Color, normInterest));

                            // Mixed Reality Toolkit/Standard
                            mat.SetColor("_EmissiveColor", BlendColors(originalEmissionColors[i], Highlight_Color, normInterest));
                        }
                    }
                }
                catch (Exception)
                {
                    // Just ignore; Usually happens after the game object already got destroyed, but the update sequence had already be started
                }
            }
        }

        /// <summary>
        /// Returns adjusted normalized interest based on the selected transition type.
        /// </summary>
        private float TransitionAdjustedInterest(float normalizedInterest)
        {
            float quadIncreasePower = 4;
            float logIncreasePower = 3;

            switch (BlendType)
            {
                case HoverTransitionType.Boolean:
                    if (normalizedInterest > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                case HoverTransitionType.LinearBlend:
                    return normalizedInterest;
                case HoverTransitionType.SlowInitialIncrease:
                    return (float)Math.Pow(normalizedInterest, quadIncreasePower);
                case HoverTransitionType.FastInitialIncrease:
                    if (normalizedInterest == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        double min = Math.Log(0.00001);
                        double max = Math.Log(1);
                        float val = (float)((Math.Log(normalizedInterest) - min) / (max - min));
                        return (float)Math.Pow(val, logIncreasePower);
                    }
                default:
                    return normalizedInterest;
            }
        }

        /// <summary>
        /// When the user looked away from the target last, amount of time the user looked away from this target in milliseconds.
        /// This means: Even if the user is looking at the target right now (HasFocus == true), do not return 0! This is to determine 
        /// whether to reset the feedback.
        /// </summary>
        private double LookAwayTimeInMs
        {
            get
            {
                return (DateTime.Now - cursorLeaveTime).TotalMilliseconds;
            }
        }

        /// <summary>
        /// Amount of time the user looked at this target in milliseconds.
        /// </summary>
        private double DwellTimeInMs
        {
            get
            {
                return ((!eyeTarget.IsLookedAt) ? 0 : (DateTime.Now - cursorEnterTime).TotalMilliseconds);
            }
        }

        #region Event handlers

        private void OnLookAtStop()
        {
            cursorLeaveTime = DateTime.Now;
        }

        private void OnLookAtStart()
        {
            // Reset dwell timer if necessary - If the user didn't look away from 
            // the target long enough, it's not considered as "new" entry.
            if ((LookAwayTimeInMs > MaxLookAwayTimeInMs) || (cursorEnterTime == DateTime.MaxValue))
            {
                cursorEnterTime = DateTime.Now;
            }

            ShowFeedback(normalizedInterest);

            highlightOn = (!highlightOn) ? true : highlightOn;
        }

        #endregion

        #region Handling color changes
        /// <summary>
        /// Stores the original base and emission colors.
        /// </summary>
        private void SaveOriginalColor()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            originalColors = GetColorsByProperty("_Color", renderers);
            originalEmissionColors = GetColorsByProperty("_EmissionColor", renderers);
        }

        /// <summary>
        /// Returns an array of colors with respect to a given array of renderers for a given color property, such as "_Color" or "_EmissionColor".
        /// For more information, see https://docs.unity3d.com/ScriptReference/Material.GetColor.html
        /// </summary>
        private Color[] GetColorsByProperty(string colorProperty, Renderer[] renderers)
        {
            Color[] saveColorsTo = new Color[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.HasProperty(colorProperty))
                {
                    renderers[i].material.EnableKeyword(colorProperty);
                    saveColorsTo[i] = renderers[i].material.GetColor(colorProperty);
                }
            }
            return saveColorsTo;
        }

        /// <summary>
        /// Returns a blended color based on given initial and final color and a normalized blend factor.
        /// </summary>
        /// <param name="colorStart">Initial color.</param>
        /// <param name="colorEnd">Final color once the blend is completed (blend factor = 1).</param>
        /// <param name="normalizedBlendFactor">Value between 0 (original color) and 1 (final color).</param>
        private Color BlendColors(Color colorStart, Color colorEnd, float normalizedBlendFactor)
        {
            Color c = colorStart;
            c.r = divBlendColor(colorStart.r, colorEnd.r, normalizedBlendFactor);
            c.g = divBlendColor(colorStart.g, colorEnd.g, normalizedBlendFactor);
            c.b = divBlendColor(colorStart.b, colorEnd.b, normalizedBlendFactor);
            c.a = divBlendColor(colorStart.a, colorEnd.a, normalizedBlendFactor);
            return c;
        }

        /// <summary>
        /// Returns an individual color channel that is blended between an original and final value based on normalized blend factor.
        /// </summary>
        private float divBlendColor(float startVal, float endVal, float normalizedBlendFactor)
        {
            return (startVal + (endVal - startVal) * normalizedBlendFactor);
        }

        #endregion
    }
}
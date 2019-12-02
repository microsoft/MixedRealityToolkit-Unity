// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Controls how the standard indeterminate loader moves and behaves over time.
    /// </summary>
    /// <remarks>
    /// This loader is calculated dynamically based on Sine and Cosine
    /// </remarks>
    [AddComponentMenu("Scripts/MRTK/SDK/LoaderController")]
    public class LoaderController : MonoBehaviour
    {

        /// <summary>
        /// Total strength of movement of the loading animation
        /// </summary>
        [SerializeField]
        [Tooltip("Total strength of movement of the loading animation")]
        private float amplitude = 0.05f;

        public float Amplitude
        {
            get => amplitude;
            set => amplitude = value;
        }

        /// <summary>
        /// How often <see cref="amplitude"/> happens
        /// </summary>
        [SerializeField]
        [Tooltip("How often 'amplitude' happens")]
        private float frequency = 3.0f;

        public float Frequency
        {
            get => frequency;
            set => frequency = value;
        }

        /// <summary>
        /// The base local position for the dots' parent
        /// </summary>
        [SerializeField]
        [Tooltip("The base local position for the dots' parent")]
        private float dotOffset = 0.05f;

        public float DotOffset
        {
            get => dotOffset;
            set => dotOffset = value;
        }

        /// <summary>
        /// The base scale unit for the dots' parent
        /// </summary>
        [SerializeField]
        [Tooltip("The base scale unit for the dots' parent")]
        private float dotSetScale = 0.06f;

        public float DotSetScale
        {
            get => dotSetScale;
            set => dotSetScale = value;
        }

        /// <summary>
        /// Use low frequency oscillation with the Sine calculation.
        /// </summary>
        [SerializeField]
        [Tooltip("Use low frequency oscillation with the Sine calculation.")]
        private bool lFOsin = false;

        public bool LFOsin
        {
            get => lFOsin;
            set => lFOsin = value;
        }

        /// <summary>
        /// Use low frequency oscillation with the Cosine calculation.
        /// </summary>
        [SerializeField]
        [Tooltip("Use low frequency oscillation with the Cosine calculation.")]
        private bool lFOcos = false;

        public bool LFOcos
        {
            get => lFOcos;
            set => lFOcos = value;
        }

        /// <summary>
        /// Low Frequency oscillation frequency 
        /// </summary>
        [SerializeField]
        [Tooltip("Low frequency oscillation frequency")]
        private float lFOfreq = 1.0f;

        public float LFOfreq
        {
            get => lFOfreq;
            set => lFOfreq = value;
        }

        /// <summary>
        /// Low Frequency oscillation amplitude 
        /// </summary>
        [SerializeField]
        [Tooltip("Low Frequency oscillation amplitude")]
        private float lFOamp = 0.1f;

        public float LFOamp
        {
            get => lFOamp;
            set => lFOamp = value;
        }

        /// <summary>
        /// Reverses dots' orbit rotation path
        /// </summary>
        [SerializeField]
        [Tooltip("Reverses dots' orbit rotation path")]
        private bool reverseOrbit = false;

        public bool ReverseOrbit
        {
            get => reverseOrbit;
            set => reverseOrbit = value;
        }

        /// <summary>
        /// Inverts dots' position in orbit
        /// </summary>
        [SerializeField]
        [Tooltip("Inverts dots' position in orbit")]
        private bool invertOrbitOffset = false;

        public bool InvertOrbitOffset
        {
            get => invertOrbitOffset;
            set => invertOrbitOffset = value;
        }

        /// <summary>
        /// Multiplier to dot's rotation calculation
        /// </summary>
        [SerializeField]
        [Tooltip("Multiplier to dot's rotation calculation")]
        private float dotSpinMultiplier = 0.3f;

        public float DotSpinMultiplier
        {
            get => dotSpinMultiplier;
            set => dotSpinMultiplier = value;
        }

        /// <summary>
        /// Multiplier to dot's scale calculation
        /// </summary>
        [SerializeField]
        [Tooltip("Multiplier to dot's scale calculation")]
        private float dotScaleMultipler = 0.0f;

        public float DotScaleMultipler
        {
            get => dotScaleMultipler;
            set => dotScaleMultipler = value;
        }

        /// <summary>
        /// When enabled, the dot scale uses Cosine to determine scale with including <see cref="dotSetScale"/>.
        /// Otherwise, it will use Sine.
        /// </summary>
        [SerializeField]
        [Tooltip("When enabled, the dot scale uses Cosine to determine scale with including 'dotSetScale'. Otherwise, it will use Sine.")]
        private bool sinCosSplitScale = false;

        public bool SinCosSplitScale
        {
            get => sinCosSplitScale;
            set => sinCosSplitScale = value;
        }

        /// <summary>
        /// Calculates the time cycle for the trigonometric functions
        /// </summary>
        private float Cycles
        {
            get
            {
                if (reverseOrbit)
                {
                    return (frequency < 0.0f) ? (Time.time / 0.000001f) * -1.0f : (Time.time / frequency) * -1.0f;
                }
                else
                {
                    return (frequency < 0.0f) ? Time.time / 0.000001f : Time.time / frequency;
                }
            }
        }

        private float degPerSec;

        private Vector3 parentNewPos = Vector3.zero;

        private Transform dot01;
        private Vector3 dot01NewPos = Vector3.zero;
        private Vector3 dot01NewScale = Vector3.zero;
        private Vector3 dot01NewRot = Vector3.zero;

        private Transform dot02;
        private Vector3 dot02NewPos = Vector3.zero;
        private Vector3 dot02NewScale = Vector3.zero;
        private Vector3 dot02NewRot = Vector3.zero;

        private const float tau = Mathf.PI * 2.0f;

        private void OnEnable()
        {
            if (dot01 == null)
            {
                dot01 = gameObject.transform.GetChild(0);
            }

            if (dot02 == null)
            {
                dot02 = gameObject.transform.GetChild(1);
            }
        }

        private void Update()
        {
            degPerSec = Time.deltaTime * 360f;

            AnimateParent();
            AnimateDotTransforms();
        }

        private void AnimateParent()
        {
            float cosX = Mathf.Cos(Cycles * tau) * amplitude;
            float sinY = Mathf.Sin(Cycles * tau) * amplitude;

            if (invertOrbitOffset == true)
            {
                cosX = -cosX;
                sinY = -sinY;
            }

            parentNewPos.Set(cosX, sinY, 0f);
            transform.localPosition = parentNewPos;

            if (lFOsin == true)
            {
                dotSpinMultiplier = Mathf.Sin(Time.time * lFOfreq) * lFOamp;
            }
            else if (lFOcos == true)
            {
                dotSpinMultiplier = Mathf.Cos(Time.time * lFOfreq) * lFOamp;
            }

            transform.Rotate(Vector3.forward * (degPerSec * dotSpinMultiplier));

        }

        private void AnimateDotTransforms()
        {
            //Set dot groups' scale
            float sinScaleCalc = dotSetScale + Mathf.Sin(Cycles * tau / 2) * dotScaleMultipler;
            float cosScaleCalc = dotSetScale + Mathf.Cos(Cycles * tau / 2) * dotScaleMultipler;

            if (sinCosSplitScale == true)
            {
                dot01NewPos.Set(cosScaleCalc, cosScaleCalc, cosScaleCalc);
            }
            else
            {
                dot01NewPos.Set(sinScaleCalc, sinScaleCalc, sinScaleCalc);
            }

            dot01.localScale = dot01NewPos;

            dot02NewPos.Set(sinScaleCalc, sinScaleCalc, sinScaleCalc);
            dot02.localScale = dot02NewPos;

            // Set dot groups' position Offset from Parent-Null Center
            dot01NewPos.Set(dotOffset, dotOffset, 0f);
            dot02NewPos.Set(-dotOffset, -dotOffset, 0f);

            dot01.transform.localPosition = dot01NewPos;
            dot02.transform.localPosition = dot02NewPos;
        }
    }
}
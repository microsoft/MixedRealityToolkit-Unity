// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// A color blending animation component, handles multiple materials
    /// </summary>
    public class ColorTransition : MonoBehaviour
    {
        [Tooltip("GameObject with the materials to be color blended - must support material.color")]
        public GameObject TargetObject;

        [Tooltip("Length of time to transition colors in seconds")]
        public float TransitionTime = 0.75f;

        [Tooltip("Use easing")]
        public bool SmoothTransition;

        /// <summary>
        /// Color and material data
        /// </summary>
        private struct ColorTransitionData
        {
            public Color EndColor;
            public Color StartColor;
            public float Percentage;
            public float Time;
            public float Count;
            public Material Material;
            public string Name;
        }

        // array of materials
        public Material[] Materials { get; set; }

        // list of data
        private List<ColorTransitionData> mData;
        
        private void Awake()
        {
            // set the tartget game object if not set already
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            // get the material array
            if (Materials == null)
            {
                Materials = TargetObject.GetComponent<Renderer>().materials;
            }

            // add materials to the ColorTransitionData list
            mData = new List<ColorTransitionData>();

            for (int i = 0; i < Materials.Length; ++i)
            {
                ColorTransitionData data = new ColorTransitionData();
                data.StartColor = Materials[i].color;
                data.Percentage = 0;
                data.Time = TransitionTime;
                data.Count = TransitionTime;
                data.Material = Materials[i];
                data.Name = Materials[i].name;

                int SpaceIndex = data.Name.IndexOf(" ");
                if (SpaceIndex > -1)
                {
                    data.Name = data.Name.Substring(0, SpaceIndex);
                }

                mData.Add(data);
            }
        }

        /// <summary>
        /// Fades the color of a material called by name
        /// </summary>
        /// <param name="color"></param>
        /// <param name="name"></param>
        public void StartTransition(Color color, string name = "")
        {

            for (int i = 0; i < mData.Count; ++i)
            {
                if (mData[i].Name == name || name == "")
                {
                    ColorTransitionData data = mData[i];
                    data.Count = 0;
                    data.Time = TransitionTime;
                    data.EndColor = color;
                    data.StartColor = data.Material.color;

                    mData[i] = data;
                }
            }
        }

        /// <summary>
        /// Returns the current blend of two colors using a percentage
        /// </summary>
        /// <param name="startColor"></param>
        /// <param name="endColor"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        private Color GetColorTransition(Color startColor, Color endColor, float percentage)
        {
            Color newColor = endColor;

            if (percentage < 1)
            {
                float smoothPercentage = percentage;
                if (SmoothTransition)
                {
                    smoothPercentage = -1 * 0.5f * (Mathf.Cos(Mathf.PI * percentage) - 1);
                }

                newColor = Color.LerpUnclamped(startColor, endColor, smoothPercentage);
            }

            return newColor;
        }

        /// <summary>
        /// apply the color to the material
        /// </summary>
        private void Update()
        {
            for (int i = 0; i < mData.Count; ++i)
            {
                ColorTransitionData data = mData[i];

                if (data.Count < data.Time)
                {
                    data.Count = Mathf.Clamp(data.Count + Time.deltaTime, 0, data.Time);
                    data.Material.color = GetColorTransition(data.StartColor, data.EndColor, data.Count / data.Time);
                    mData[i] = data;
                }

            }
        }
    }
}

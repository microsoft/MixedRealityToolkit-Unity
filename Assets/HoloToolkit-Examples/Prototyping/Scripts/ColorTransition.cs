// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HoloToolkit.Examples.Prototyping
{
    public class ColorTransition : MonoBehaviour
    {

        public GameObject TargetObject;
        public float TransitionTime = 0.75f;
        public bool SmoothTransition;

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

        public Material[] Materials { get; set; }
        private List<ColorTransitionData> mData;

        private void Awake()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            if (Materials == null)
            {
                Materials = TargetObject.GetComponent<Renderer>().materials;
            }

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

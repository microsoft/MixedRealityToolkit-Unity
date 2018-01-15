// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using UnityEngine;

namespace MixedRealityToolkit.Examples.GazeRuler
{
    /// <summary>
    /// provide a tip text of current measure mode
    /// </summary>
    public class ModeTip : Singleton<ModeTip>
    {
        private const string LineMode = "Line Mode";
        private const string PolygonMode = "Geometry Mode";
        private TextMesh text;
        private int fadeTime = 100;
        private Material material;

        private void Start()
        {
            text = GetComponent<TextMesh>();
            material = GetComponent<Renderer>().material;
            switch (MeasureManager.Instance.Mode)
            {
                case GeometryMode.Line:
                    text.text = LineMode;
                    break;
                default:
                    text.text = PolygonMode;
                    break;
            }
        }

        protected override void OnDestroy()
        {
            DestroyImmediate(material);
        }

        // Update is called once per frame
        private void Update()
        {
            if (gameObject.activeInHierarchy)
            {
                // if you want log the position of mode tip text, just uncomment it.
                // Debug.Log("pos: " + gameObject.transform.position);
                switch (MeasureManager.Instance.Mode)
                {
                    case GeometryMode.Line:
                        if (!text.text.Contains(LineMode))
                        {
                            text.text = LineMode;
                        }
                        break;
                    default:
                        if (!text.text.Contains(PolygonMode))
                        {
                            text.text = PolygonMode;
                        }
                        break;
                }

                fadeTime = 100;
                // fade tip text
                if (fadeTime == 0)
                {
                    var color = material.color;
                    fadeTime = 100;
                    color.a = 1f;
                    material.color = color;
                    gameObject.SetActive(false);
                }
                else
                {
                    var color = material.color;
                    color.a -= 0.01f;
                    material.color = color;
                    fadeTime--;
                }
            }
        }
    }
}
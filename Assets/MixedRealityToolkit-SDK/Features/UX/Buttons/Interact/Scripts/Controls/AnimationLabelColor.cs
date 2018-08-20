// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interact.Controls
{
    /// <summary>
    /// A component that exposes Text Color to Animation
    /// </summary>
    [ExecuteInEditMode]
    public class AnimationLabelColor : MonoBehaviour
    {
        public Color TextColor = new Color(0.25f, 0.25f,0.25f,1);
        public GameObject[] Targets;

        private Color lastColor;

        /// <summary>
        /// setting the color
        /// </summary>
        /// <param name="color"></param>
        private void SetColor(Color color)
        {
            if (Targets.Length < 1)
            {
                Targets = new GameObject[] { gameObject };
            }

            for (int i = 0; i < Targets.Length; i++)
            {
                Text text = GetComponent<Text>();
                TextMesh textMesh = GetComponent<TextMesh>();

                if (textMesh != null)
                {
                    textMesh.color = color;
                }
                else if (text != null)
                {
                    text.color = color;
                }
            }

            lastColor = color;
        }

        // Update is called once per frame
        private void Update()
        {
            if (TextColor != lastColor)
            {
                SetColor(TextColor);
            }
        }

    }
}

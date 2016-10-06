// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Simple Behaviour which calculates the average frames per second over a number of frames and shows the FPS in a referenced Text control.
    /// </summary>
    [RequireComponent(typeof(TextMesh))]
    public class FpsDisplay : MonoBehaviour
    {
        [Tooltip("Reference to Text UI control where the FPS should be displayed.")]
        [SerializeField]
        private TextMesh textMesh;

        [Tooltip("How many frames should we consider into our average calculation?")]
        [SerializeField]
        private int frameRange = 60;

        private int averageFps { get; set; }

        private int[] fpsBuffer;
        private int fpsBufferIndex;

        private static readonly string[] StringsFrom00To99 =
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
            "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
            "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99"
        };

        private void Update()
        {
            if (fpsBuffer == null || fpsBuffer.Length != frameRange || textMesh == null)
            {
                InitBuffer();
            }

            UpdateFrameBuffer();
            CalculateFps();

            UpdateTextDisplay(textMesh, averageFps);
        }

        private void InitBuffer()
        {
            textMesh = GetComponent<TextMesh>();

            if(frameRange <= 0)
            {
                frameRange = 1;
            }

            fpsBuffer = new int[frameRange];
            fpsBufferIndex = 0;
        }

        private void UpdateTextDisplay(TextMesh text, int fps)
        {
            text.text = StringsFrom00To99[Mathf.Clamp(fps, 0, 99)];
        }

        private void UpdateFrameBuffer()
        {
            fpsBuffer[fpsBufferIndex++] = (int)(1f/Time.unscaledDeltaTime);

            if(fpsBufferIndex >= frameRange)
            {
                fpsBufferIndex = 0;
            }
        }

        private void CalculateFps()
        {
            int sum = 0;

            for(int i = 0; i < frameRange; i++)
            {
                int fps = fpsBuffer[i];
                sum += fps;
            }

            averageFps = sum / frameRange;
        }
    }
}
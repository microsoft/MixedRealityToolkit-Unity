// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// Sets the text value of a TextMesh or UI Text object based on the selected value of the array
    /// </summary>
    public class CycleText : CycleArray<string>
    {
        private TextMesh mTextMesh;
        private Text mText;

        protected override void Awake()
        {
            base.Awake();

            mTextMesh = GetComponent<TextMesh>();
            mText = GetComponent<Text>();

            if (mTextMesh == null && mText == null)
            {
                Debug.LogError("TextMesh or Text is not set in CycleText!");
                Destroy(this);
            }
        }

        /// <summary>
        /// Set the text...
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            if (mTextMesh != null)
            {
                mTextMesh.text = Array[index];
            }

            if (mText != null)
            {
                mText.text = Array[index];
            }

        }
    }
}

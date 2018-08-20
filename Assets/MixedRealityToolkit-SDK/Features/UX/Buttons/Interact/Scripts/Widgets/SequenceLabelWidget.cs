// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interact.Widgets
{
    /// <summary>
    /// Looks for a SequenceCycleWidget and updates the label text based on the current index
    /// </summary>
    public class SequenceLabelWidget : MonoBehaviour
    {
        [Tooltip("the gameObject containing a TextMesh or Text component")]
        public GameObject Label;

        private Text mUIText;
        private TextMesh mTextMesh;
        private SequenceCycleWidget mCycleWidget;

        /// <summary>
        /// Get the TextMesh
        /// </summary>
        private void Awake()
        {
            if (Label == null)
            {
                Label = this.gameObject;
            }

            mTextMesh = Label.GetComponent<TextMesh>();
            mUIText = Label.GetComponent<Text>();

            if (mTextMesh == null && mUIText == null)
            {
                Debug.LogError("Textmesh or Text is not available for SequenceLabelWidget!");
                Destroy(this);
            }

            mCycleWidget = GetComponentInParent<SequenceCycleWidget>();
        }

        /// <summary>
        /// Set the text value
        /// </summary>
        /// <param name="state"></param>
        private void SetLabel()
        {
            if(mCycleWidget != null)
            {
                if (mTextMesh != null)
                {
                    mTextMesh.text = mCycleWidget.CurrentLabel;
                }
                else if (mUIText != null)
                {
                    mUIText.text = mCycleWidget.CurrentLabel;
                }
            }
        }

        private void Update()
        {
            SetLabel();
        }
    }
}

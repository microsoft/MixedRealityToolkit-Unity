// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Component to manage the visuals for a Mixed Reality Keyboard Preview window.
    /// </summary>
    public class MixedRealityKeyboardPreview : MonoBehaviour
    {
        [SerializeField, Tooltip("The Text Mesh Pro text field to display the preview text.")]
        private TextMeshPro previewText = null;

        /// <summary>
        /// The Text Mesh Pro text field to display the preview text.
        /// </summary>
        public TextMeshPro PreviewText
        {
            get { return previewText; }
            set
            {
                if (previewText != value)
                {
                    previewText = value;

                    if (previewText != null)
                    {
                        previewText.text = Text;

                        UpdateCaret();
                    }
                }
            }
        }

        [SerializeField, Tooltip("The transform to move based on the preview caret.")]
        private Transform previewCaret = null;

        /// <summary>
        /// The transform to move based on the preview caret.
        /// </summary>
        public Transform PreviewCaret
        {
            get { return previewCaret; }
            set
            {
                if (previewCaret != value)
                {
                    previewCaret = value;

                    if (previewCaret)
                    {
                        initialCaretLocation = previewCaret.position;

                        UpdateCaret();
                    }
                }
            }
        }

        private string text = string.Empty;

        /// <summary>
        /// The text to display in the preview.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                if (value != text)
                {
                    text = value;

                    if (PreviewText != null)
                    {
                        PreviewText.text = text;
                        PreviewText.ForceMeshUpdate();
                    }

                    UpdateCaret();
                }
            }
        }

        private int caretIndex = 0;

        /// <summary>
        /// Where the caret lies within the text.
        /// </summary>
        public int CaretIndex
        {
            get { return caretIndex; }
            set
            {
                if (value != caretIndex)
                {
                    caretIndex = value;

                    UpdateCaret();
                }
            }
        }

        /// <summary>
        /// Utility method which can be used to toggle if solvers update.
        /// </summary>
        public void ToggleSolvers()
        {
            var solverHandler = GetComponent<SolverHandler>();

            if (solverHandler != null)
            {
                solverHandler.UpdateSolvers = !solverHandler.UpdateSolvers;
            }
        }

        private Vector3 initialCaretLocation = Vector3.zero;

        #region MonoBehaviour Implementation

        private void Awake()
        {
            if (previewCaret != null)
            {
                initialCaretLocation = previewCaret.position;
            }
        }

        #endregion MonoBehaviour Implementation

        private void UpdateCaret()
        {
            caretIndex = Mathf.Clamp(caretIndex, 0, Text == null ? 0 : text.Length);

            if (previewCaret != null)
            {
                if (string.IsNullOrEmpty(text) || caretIndex == 0)
                {
                    previewCaret.transform.position = initialCaretLocation;
                }
                else
                {
                    Vector3 localPosition;

                    if (caretIndex == text.Length)
                    {
                        localPosition = PreviewText.textInfo.characterInfo[caretIndex - 1].topRight;
                    }
                    else
                    {
                        localPosition = PreviewText.textInfo.characterInfo[caretIndex].topLeft;
                    }

                    localPosition.y = 0.0f;
                    localPosition.z = 0.0f;

                    var position = PreviewText.transform.TransformPoint(localPosition);
                    previewCaret.transform.position = position;
                }
            }
        }
    }
}

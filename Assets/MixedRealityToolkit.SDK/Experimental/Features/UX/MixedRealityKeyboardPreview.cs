// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Component to manage the visuals for a Mixed Reality Keyboard Preview window.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/MixedRealityKeyboardPreview")]
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

        [SerializeField, Tooltip("The initial pitch when spawned.")]
        private float initialPitchOffsetDegrees = 5.0f;

        /// <summary>
        /// The initial pitch when spawned.
        /// </summary>
        public float InitialPitchOffsetDegrees
        {
            get { return initialPitchOffsetDegrees; }
            set
            {
                initialPitchOffsetDegrees = value;
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

                if (solverHandler.UpdateSolvers)
                {
                    ApplyShellSolverParameters();
                }
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

            ApplyShellSolverParameters();
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

        private void ApplyShellSolverParameters()
        {
            var solver = GetComponent<Follow>();

            if (solver != null)
            {
                // Position the keyboard in a comfortable place with a fixed pitch relative to the forward direction.
                var solverHandler = solver.GetComponent<SolverHandler>();

                if (solverHandler != null)
                {
                    var forward = solverHandler.TransformTarget != null ? solverHandler.TransformTarget.forward : Vector3.forward;
                    var forwardXZ = forward;
                    forwardXZ.y = 0.0f;

                    var pitchOffsetDegrees = Mathf.Acos(forwardXZ.magnitude / forward.magnitude) * Mathf.Rad2Deg;

                    if (forward.y < 0.0f)
                    {
                        // If the y component of the forward is negative that means the tracked transform is looking/
                        // pointing down. We want to negate our pitch offset to move the keyboard down in response.
                        pitchOffsetDegrees *= -1.0f;
                    }

                    // Initial carry pitch.
                    pitchOffsetDegrees += initialPitchOffsetDegrees;

                    solver.PitchOffset = pitchOffsetDegrees;
                }
            }
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using TMPro;
using UnityEngine;

#if MRTK_SPATIAL_PRESENT
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
#endif

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Component to manage the visuals for a Mixed Reality Keyboard Preview window.
    /// </summary>
    [AddComponentMenu("MRTK/UX/Keyboard Preview")]
    public class KeyboardPreview : MonoBehaviour
    {
        [SerializeField, Tooltip("The Text Mesh Pro text field to display the preview text.")]
        private TextMeshProUGUI previewText = null;

        /// <summary>
        /// The Text Mesh Pro text field to display the preview text.
        /// </summary>
        public TextMeshProUGUI PreviewText
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
        private RectTransform previewCaret = null;

        /// <summary>
        /// The transform to move based on the preview caret.
        /// </summary>
        public RectTransform PreviewCaret
        {
            get { return previewCaret; }
            set
            {
                if (previewCaret != value)
                {
                    previewCaret = value;

                    UpdateCaret();
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
#if MRTK_SPATIAL_PRESENT
            if (TryGetComponent(out SolverHandler solverHandler))
            {
                solverHandler.UpdateSolvers = !solverHandler.UpdateSolvers;

                if (solverHandler.UpdateSolvers)
                {
                    ApplyShellSolverParameters();
                }
            }
#endif
        }

        #region MonoBehaviour Implementation

        private void OnEnable()
        {
            StartCoroutine(BlinkCaret());
        }

        private void Start()
        {
            ApplyShellSolverParameters();
        }

        #endregion MonoBehaviour Implementation

        private void UpdateCaret()
        {
            caretIndex = Mathf.Clamp(caretIndex, 0, string.IsNullOrEmpty(text) ? 0 : text.Length);

            if (previewCaret != null)
            {
                if (caretIndex == 0)
                {
                    previewCaret.anchoredPosition = Vector2.zero;
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
                    localPosition.z = previewCaret.localPosition.z;

                    var position = PreviewText.transform.TransformPoint(localPosition);
                    previewCaret.position = position;
                }
            }
        }

        private IEnumerator BlinkCaret()
        {
            while (previewCaret != null)
            {
                previewCaret.gameObject.SetActive(!previewCaret.gameObject.activeSelf);

                // The default Window's text caret blinks every 530 milliseconds.
                const float blinkTime = 0.53f;
                yield return new WaitForSeconds(blinkTime);
            }
        }

        private void ApplyShellSolverParameters()
        {
#if MRTK_SPATIAL_PRESENT
            if (TryGetComponent(out Follow solver))
            {
                // Position the keyboard in a comfortable place with a fixed pitch relative to the forward direction.
                if (solver.TryGetComponent(out SolverHandler solverHandler))
                {
                    var forward = solverHandler.TransformTarget != null ? solverHandler.TransformTarget.forward : Vector3.forward;
                    var right = solverHandler.TransformTarget != null ? solverHandler.TransformTarget.right : Vector3.right;

                    // Calculate the initial view pitch.
                    var pitchOffsetDegrees = Vector3.SignedAngle(new Vector3(forward.x, 0.0f, forward.z), forward, right);

                    const float shellPitchOffset = 2.0f;
                    pitchOffsetDegrees += shellPitchOffset;

                    const float shellPitchMin = -20.0f;
                    const float shellPitchMax = 20.0f;
                    pitchOffsetDegrees = Mathf.Clamp(pitchOffsetDegrees, shellPitchMin, shellPitchMax);

                    solver.PitchOffset = pitchOffsetDegrees;
                    solver.SolverUpdate();
                }
            }
#endif
        }
    }
}

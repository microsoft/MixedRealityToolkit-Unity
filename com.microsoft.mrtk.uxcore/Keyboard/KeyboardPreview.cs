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
        private bool layoutInvalidated = true;

        [SerializeField, Tooltip("The Text Mesh Pro text field to display the preview text.")]
        private TMP_Text previewText = null;

        /// <summary>
        /// The Text Mesh Pro text field to display the preview text.
        /// </summary>
        public TMP_Text PreviewText
        {
            get { return previewText; }
            set
            {
                if (previewText != value)
                {
                    previewText = value;
                    if (previewText != null)
                    {
                        previewText.SetText(Text);
                    }
                    InvalidateLayout();
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
                    InvalidateLayout();
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
                    }

                    CaretIndex = Mathf.Clamp(CaretIndex, 0, Text.Length);
                    InvalidateLayout();
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
                value = Mathf.Clamp(value, 0, Text.Length);
                if (value != caretIndex)
                {
                    caretIndex = value;
                    InvalidateLayout();
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

        private void LateUpdate()
        {
            if (layoutInvalidated)
            {
                layoutInvalidated = false;
                UpdateLayout();
            }
            
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Force the <see cref="PreviewText"/> label and <see cref="PreviewCaret"/> transform to update their positions on the next update.
        /// </summary>
        public void InvalidateLayout()
        {
            layoutInvalidated = true;
        }


        private void UpdateLayout()
        {
            if (previewCaret == null || PreviewText == null) 
            {
                return;
            }

            var textInfo = PreviewText.GetTextInfo(Text);
            if (textInfo.lineCount == 0 || textInfo.characterCount == 0)
            {
                ResetCaret();
                ResetView();
                return;
            }

            TMP_CharacterInfo nextChar;
            if (CaretIndex == textInfo.characterCount)
            {
                TMP_CharacterInfo prevChar = textInfo.characterInfo[CaretIndex - 1];
                nextChar = prevChar;
                nextChar.character = '\0';
                nextChar.index++;
                if (PreviewText.isRightToLeftText)
                {
                    nextChar.origin = nextChar.topLeft.x + 3;
                    nextChar.topRight.x = nextChar.topLeft.x;
                    nextChar.bottomRight.x = nextChar.bottomLeft.x;
                }
                else
                {
                    nextChar.origin = nextChar.topRight.x + 3;
                    nextChar.topLeft.x = nextChar.topRight.x;
                    nextChar.bottomLeft.x = nextChar.bottomRight.x;
                }
            }
            else
            {
                nextChar = textInfo.characterInfo[CaretIndex];
            }

            if (nextChar.lineNumber > textInfo.lineCount)
            {
                ResetCaret();
                ResetView();
                return;
            }

            var lineInfo = textInfo.lineInfo[nextChar.lineNumber];
            ScrollView(ref textInfo, ref lineInfo, ref nextChar);
            UpdateCaret(ref textInfo, ref lineInfo, ref nextChar);
        }

        /// <summary>
        /// Move the caret transform immediately before the currently focused character.
        /// </summary>
        /// <remarks>
        /// If Right-To-Left text is enabled, this will adjust x-cordinates accordingly.
        /// </remarks>
        private void UpdateCaret(
            ref TMP_TextInfo textInfo,
            ref TMP_LineInfo lineInfo,
            ref TMP_CharacterInfo nextChar)
        {
            // get line info
            float focusedLineWidth = lineInfo.width;
            float focusedLineWidthHalf = focusedLineWidth / 2.0f;
            float focusedLineCurrentChar;
            float focusedLineStart;
            float focusedLineEnd;
            float caretWidthHalf = PreviewCaret.rect.width / 2.0f;

            if (textInfo.textComponent.isRightToLeftText)
            {
                focusedLineCurrentChar = nextChar.topRight.x + (focusedLineWidthHalf) + textInfo.textComponent.margin.x;
                focusedLineStart = focusedLineWidth + textInfo.textComponent.margin.x;
                focusedLineEnd = lineInfo.maxAdvance + focusedLineWidthHalf + textInfo.textComponent.margin.x - caretWidthHalf;

            }
            else
            {
                focusedLineCurrentChar = nextChar.topLeft.x + focusedLineWidthHalf + textInfo.textComponent.margin.x;
                focusedLineStart = textInfo.textComponent.margin.x;
                focusedLineEnd = lineInfo.maxAdvance + focusedLineWidthHalf + textInfo.textComponent.margin.x + caretWidthHalf;
            }

            float caretPositionX;
            float caretPoistionY = lineInfo.baseline - textInfo.lineInfo[0].baseline;

            // if at end of line, text info doesn't go to a new line info, so account for this.
            if (AtEmptyNewLine(ref textInfo, ref nextChar))
            {
                caretPositionX = focusedLineStart;
                caretPoistionY -= lineInfo.lineHeight;
            }
            else if (AtEndOfLine(ref lineInfo, ref nextChar))
            {
                caretPositionX = focusedLineEnd;
            }
            else
            {
                caretPositionX = focusedLineCurrentChar;
            }

            PreviewCaret.anchoredPosition = new Vector2(caretPositionX, caretPoistionY);  
        }

        /// <summary>
        /// Move the caret transform to the start of the text.
        /// </summary>
        /// <remarks>
        /// If Right-To-Left text is enabled, this will adjust x-cordinates accordingly.
        /// </remarks>
        private void ResetCaret()
        {
            if (PreviewText.isRightToLeftText)
            {
                PreviewCaret.anchoredPosition = new Vector2(PreviewText.rectTransform.rect.width - (PreviewCaret.rect.width / 2.0f) - PreviewText.margin.z, 0);
            }
            else
            {
                PreviewCaret.anchoredPosition = new Vector2(PreviewText.margin.x, 0);
            }
        }

        /// <summary>
        /// Move the text label transform so that the currently focus character is visible.
        /// </summary>
        /// <remarks>
        /// If Right-To-Left text is enabled, this will adjust x-cordinates accordingly.
        /// </remarks>
        private void ScrollView(
            ref TMP_TextInfo textInfo,
            ref TMP_LineInfo lineInfo,
            ref TMP_CharacterInfo nextChar)
        {
            // get line info
            float focusedLineWidth = lineInfo.width;
            float focusedLineLeft = focusedLineWidth / -2.0f;
            float focusedLineOffset = textInfo.lineInfo[0].baseline - lineInfo.baseline;

            // get char info
            float focusedCharacterLeft = nextChar.origin - focusedLineLeft;
            float focusedCharacterRight = nextChar.topRight.x - focusedLineLeft;

            float textPositionX = 0;
            float textPositionY = focusedLineOffset;

            // if at end of line, text info doesn't go to a new line info, so account for this.
            if (AtEmptyNewLine(ref textInfo, ref nextChar))
            {
                textPositionX = 0;
                textPositionY += lineInfo.lineHeight;
            }
            else if (focusedCharacterRight > focusedLineWidth)
            {
                textPositionX = focusedLineWidth - focusedCharacterRight;
            }
            else if (focusedCharacterLeft < 0)
            {
                textPositionX = -focusedCharacterLeft;
            }

            PreviewText.rectTransform.anchoredPosition = new Vector2(textPositionX, textPositionY);
        }

        /// <summary>
        /// Reset the text label transform to it's starting position.
        /// </summary>
        private void ResetView()
        {
            PreviewText.rectTransform.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Is the cursor at the start of new empty line, that is not the very first line.
        /// </summary>
        private bool AtEmptyNewLine(ref TMP_TextInfo textInfo, ref TMP_CharacterInfo nextChar)
        {
            return nextChar.index >= 0 &&
                nextChar.character == '\0' &&
                textInfo.characterInfo[nextChar.index - 1].character == '\n'; 
        }

        /// <summary>
        /// Is the cursor at the end of a line
        /// </summary>
        private bool AtEndOfLine(ref TMP_LineInfo lineInfo, ref TMP_CharacterInfo nextChar)
        {
            return lineInfo.characterCount == nextChar.index;
        }

        /// <summary>
        /// Continuously blink the caret gameobject until it is destroyed.
        /// </summary>
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

        /// <summary>
        /// If there is a follow solver and a solver handler, update their properties so that the KeyboardPreview transform
        /// moves ontop of the system's native's keyboard.
        /// </summary>
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

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

            // create a fake previous character if at the start
            TMP_CharacterInfo prevChar;
            if (CaretIndex == 0)
            {
                prevChar = default;
                prevChar.index = -1;
            }
            else
            {
                prevChar = textInfo.characterInfo[CaretIndex - 1];
            }

            // create a fake next character if at the end
            TMP_CharacterInfo nextChar;
            if (CaretIndex == textInfo.characterCount)
            {
                nextChar = prevChar;
                nextChar.character = '\0';
                nextChar.index++;
                nextChar.origin = nextChar.xAdvance;
                if (PreviewText.isRightToLeftText)
                {
                    nextChar.topRight.x = nextChar.topLeft.x;
                    nextChar.bottomRight.x = nextChar.bottomLeft.x;
                }
                else
                {
                    nextChar.topLeft.x = nextChar.topRight.x;
                    nextChar.bottomLeft.x = nextChar.bottomRight.x;
                }
            }
            else
            {
                nextChar = textInfo.characterInfo[CaretIndex];
            }

            if (prevChar.lineNumber > textInfo.lineCount)
            {
                ResetCaret();
                ResetView();
                return;
            }

            var lineInfo = textInfo.lineInfo[prevChar.lineNumber];
            ScrollView(textInfo, in lineInfo, in prevChar, in nextChar);
            UpdateCaret(textInfo, in lineInfo, in prevChar);
        }

        /// <summary>
        /// Move the caret transform immediately before the currently focused character.
        /// </summary>
        /// <remarks>
        /// If Right-To-Left text is enabled, this will adjust x-coordinate accordingly.
        /// </remarks>
        private void UpdateCaret(
            TMP_TextInfo textInfo,
            in TMP_LineInfo lineInfo,
            in TMP_CharacterInfo prevChar)
        {
            // get line info
            float focusedLineWidth = lineInfo.width;
            float focusedLineWidthHalf = focusedLineWidth / 2.0f;
            float focusedLineCurrentChar;
            float focusedLineStart;

            if (textInfo.textComponent.isRightToLeftText)
            {
                focusedLineCurrentChar = prevChar.xAdvance + (focusedLineWidthHalf) + textInfo.textComponent.margin.x;
                focusedLineStart = focusedLineWidth + textInfo.textComponent.margin.x;

            }
            else
            {
                focusedLineCurrentChar = prevChar.xAdvance + focusedLineWidthHalf + textInfo.textComponent.margin.x;
                focusedLineStart = textInfo.textComponent.margin.x;
            }

            float caretPositionX;
            float caretPositionY = lineInfo.baseline - textInfo.lineInfo[0].baseline;

            // if at end of line, text info doesn't go to a new line info, so account for this.
            if (AtEmptyNewLine(in lineInfo, in prevChar))
            {
                caretPositionX = focusedLineStart;
                caretPositionY -= lineInfo.lineHeight;
            }
            else if (AtStartOfLine(in lineInfo, in prevChar))
            {
                caretPositionX = focusedLineStart;
            }
            else
            {
                caretPositionX = focusedLineCurrentChar;
            }

            PreviewCaret.anchoredPosition = new Vector2(caretPositionX, caretPositionY);
        }

        /// <summary>
        /// Move the caret transform to the start of the text.
        /// </summary>
        /// <remarks>
        /// If Right-To-Left text is enabled, this will adjust x-coordinates accordingly.
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
        /// If Right-To-Left text is enabled, this will adjust x-coordinates accordingly.
        /// </remarks>
        private void ScrollView(
            TMP_TextInfo textInfo,
            in TMP_LineInfo lineInfo,
            in TMP_CharacterInfo prevChar,
            in TMP_CharacterInfo nextChar)
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
            if (AtEmptyNewLine(in lineInfo, in prevChar))
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
        private bool AtEmptyNewLine(in TMP_LineInfo lineInfo, in TMP_CharacterInfo prevChar)
        {
            return prevChar.character == '\n' && prevChar.index == lineInfo.lastCharacterIndex;
        }

        /// <summary>
        /// Is the cursor at a start of the line
        /// </summary>
        private bool AtStartOfLine(in TMP_LineInfo lineInfo, in TMP_CharacterInfo prevChar)
        {
            return lineInfo.characterCount == 0 ? true : lineInfo.firstCharacterIndex > prevChar.index;
        }

        /// <summary>
        /// Is the cursor at the end of a line
        /// </summary>
        private bool AtEndOfLine(in TMP_LineInfo lineInfo, in TMP_CharacterInfo prevChar)
        {
            return lineInfo.lastCharacterIndex <= prevChar.index;
        }

        /// <summary>
        /// Continuously blink the caret game object until it is destroyed.
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
        /// moves on top of the system's native's keyboard.
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

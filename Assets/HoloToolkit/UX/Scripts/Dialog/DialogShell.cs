// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using UnityEngine;
using HoloToolkit.UX.Buttons;
using System.Collections.Generic;
using HoloToolkit.Unity;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

namespace HoloToolkit.UX.Dialog
{
    /// <summary>
    /// Dialog that approximates the look of a Hololens shell dialog
    /// </summary>
    public class DialogShell : Dialog
    {
        [SerializeField]
        private int maxCharsPerLine = 45;

        [SerializeField]
        private TextMesh titleText;

        [SerializeField]
        private TextMesh messageText;

        [SerializeField]
        private GameObject[] oneButtonSet;

        [SerializeField]
        private GameObject[] twoButtonSet;

        private DialogButton buttonPressed;

        /// <summary>
        /// Defines the width of the textblock
        /// so that text is constrained to the Dialog.
        /// </summary>
        public int MaxCharsPerLine
        {
            get
            {
                return maxCharsPerLine;
            }

            set
            {
                maxCharsPerLine = value;
            }
        }

        /// <summary>
        /// Event handler when Editor gizmos are displayed
        /// </summary>
        protected void OnDrawGizmos()
        {
            if (messageText != null)
            {
                messageText.text = WordWrap(messageText.text, MaxCharsPerLine);
            }
        }

        /// <summary>
        /// Runs solver after Dialog is made to center it in view.
        /// </summary>
        protected override void FinalizeLayout()
        {
            SolverConstantViewSize solver = GetComponent<SolverConstantViewSize>();

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Optimize the content for immersive headset
            if (HolographicSettings.IsDisplayOpaque)
            {
                solver.TargetViewPercentV = 0.35f;
            }
#else
            solver.TargetViewPercentV = 0.35f;
#endif
        }

        /// <summary>
        /// Creates the buttons that are displayed on the dialog.
        /// </summary>
        protected override void GenerateButtons()
        {
            //Get List of ButtonTypes that should be created on Dialog
            List<DialogButtonType> buttonTypes = new List<DialogButtonType>();
            foreach (DialogButtonType buttonType in Enum.GetValues(typeof(DialogButtonType)))
            {
                if (buttonType == DialogButtonType.None)
                {
                    continue;
                }

                // If this button type flag is set
                if ((buttonType & result.Buttons) == buttonType)
                {
                    buttonTypes.Add(buttonType);
                }
            }

            oneButtonSet = new GameObject[1];
            twoButtonSet = new GameObject[2];

            //Activate/Deactivate buttons and set Titles
            foreach (Transform child in transform)
            {
                if (child.name == "ButtonParent")
                {
                    for (int childIndex = 0; childIndex < child.transform.childCount; ++childIndex)
                    {
                        Transform t = child.transform.GetChild(childIndex);
                        if (t != null)
                        {
                            DialogButton button = t.GetComponent<DialogButton>();
                            if (button != null)
                            {
                                button.ParentDialog = this;

                                if (button.name.Contains("ButtonOne") )
                                {
                                    button.gameObject.SetActive(buttonTypes.Count == 1);

                                    if (buttonTypes.Count == 1)
                                    {
                                        oneButtonSet[0] = button.gameObject;
                                        button.SetTitle(buttonTypes[0].ToString());
                                        button.ButtonTypeEnum = buttonTypes[0];
                                    }
                                }
                                else if (button.name.Contains("ButtonTwo"))
                                {
                                    button.gameObject.SetActive(buttonTypes.Count > 1);

                                    if (buttonTypes.Count > 1)
                                    {
                                        if (button.name.Contains("A"))
                                        {
                                            twoButtonSet[0] = button.gameObject;
                                            button.SetTitle(buttonTypes[0].ToString());
                                            button.ButtonTypeEnum = buttonTypes[0];
                                        }
                                        else if (button.name.Contains("B"))
                                        {
                                            twoButtonSet[1] = button.gameObject;
                                            button.SetTitle(buttonTypes[1].ToString());
                                            button.ButtonTypeEnum = buttonTypes[1];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Title and Text on the Dialog.
        /// </summary>
        protected override void SetTitleAndMessage()
        {
            foreach (Transform child in transform)
            {
                if (child != null && child.name == "TitleText")
                {
                    titleText = child.GetComponent<TextMesh>();
                }
                else if( child != null && child.name == "TitleMessage")
                {
                    messageText = child.GetComponent<TextMesh>();
                }
            }

            if (titleText != null)
            {
                titleText.text = Result.Title;
            }

            if (messageText != null)
            {
                messageText.text = WordWrap(Result.Message, MaxCharsPerLine);
            }
        }

        /// <summary>
        /// For the text to wordwrap. For handing long text.
        /// </summary>
        /// <param name="text">the string to be wrapped</param>
        /// <param name="maxCharsPerLine">the character count that defines a line</param>
        /// <returns>string with line breaks inserted</returns>
        public static string WordWrap(string text, int maxCharsPerLine)
        {
            int pos = 0;
            int next = 0;
            StringBuilder stringBuilder = new StringBuilder();

            if (maxCharsPerLine < 1)
            {
                return text;
            }
            
            for (pos = 0; pos < text.Length; pos = next)
            {
                int endOfLine = text.IndexOf(System.Environment.NewLine, pos);

                if (endOfLine == -1)
                {
                    next = endOfLine = text.Length;
                }
                else
                {
                    next = endOfLine + System.Environment.NewLine.Length;
                }

                if (endOfLine > pos)
                {
                    do
                    {
                        int len = endOfLine - pos;

                        if (len > maxCharsPerLine)
                            len = BreakLine(text, pos, maxCharsPerLine);

                        stringBuilder.Append(text, pos, len);
                        stringBuilder.Append(System.Environment.NewLine);

                        pos += len;

                        while (pos < endOfLine && Char.IsWhiteSpace(text[pos]))
                        {
                            pos++;
                        }

                    } while (endOfLine > pos);
                }
                else
                {
                    stringBuilder.Append(System.Environment.NewLine);
                }
            }

            return stringBuilder.ToString();
        }
        
        /// <summary>
        /// Method to linebreak text
        /// </summary>
        /// <param name="text">the string to have line break inserted</param>
        /// <param name="pos">the character index where linebreak insertion is desired</param>
        /// <param name="max">maximum character count before linebreak</param>
        /// <returns></returns>
        public static int BreakLine(string text, int pos, int max)
        {
            int i = max - 1;

            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }

            if (i < 0)
            {
                return max;
            }

            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }

            return i + 1;
        }

        /// <summary>
        /// Function to destroy the Dialog.
        /// </summary>
        public void DismissDialog()
        {
            State = DialogState.InputReceived;
        }
    }
}
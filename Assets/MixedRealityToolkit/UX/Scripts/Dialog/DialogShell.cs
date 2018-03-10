// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using UnityEngine;
using MixedRealityToolkit.Utilities.Solvers;
using MixedRealityToolkit.UX.Buttons;
using System.Collections.Generic;


#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif


namespace MixedRealityToolkit.UX.Dialog
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

        protected void OnDrawGizmos()
        {
            if (messageText != null)
            {
                messageText.text = WordWrap(messageText.text, MaxCharsPerLine);
            }
        }

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

        protected override void GenerateButtons()
        {
            //Get List of ButtonTypes that should be created on Dialog
            List<ButtonTypeEnum> buttonTypes = new List<ButtonTypeEnum>();
            foreach (ButtonTypeEnum buttonType in Enum.GetValues(typeof(ButtonTypeEnum)))
            {
                if (buttonType == ButtonTypeEnum.None)
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

        public static string WordWrap(string text, int maxCharsPerLine)
        {
            int pos = 0;
            int next = 0;
            StringBuilder stringBuilder = new StringBuilder();
            
            if (maxCharsPerLine < 1)
                return text;
            
            for (pos = 0; pos < text.Length; pos = next) {
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
        
        public static int BreakLine(string text, int pos, int max) {
            int i = max - 1;

            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i])) {
                i--;
            }

            if (i < 0) {
                return max;
            }

            while (i >= 0 && Char.IsWhiteSpace(text[pos + i])) {
                i--;
            }

            return i + 1;
        }

        public void DismissDialog()
        {
            State = StateEnum.InputReceived;
        }

    }
}
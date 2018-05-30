// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.Buttons;
using HoloToolkit.UX.Progress;
using UnityEngine;

namespace HoloToolkit.Examples.UX
{
    public class ProgressExampleLaunchButton : MonoBehaviour
    {
        [Header("Which Indicator style is desired?")]
        [SerializeField]
        private IndicatorStyleEnum indicatorStyle;

        [Header("Which Progress style is desired?")]
        [SerializeField]
        private ProgressStyleEnum progressStyle;

        /// <summary>
        /// Property that determines whether the indicator of the progress indicator
        /// is  None,  StaticIcon, AnimatedOrbs, or an instantiated Prefab.
        /// </summary>
        public IndicatorStyleEnum IndicatorStyle
        {
            get
            {
                return indicatorStyle;
            }

            set
            {
                indicatorStyle = value;
            }
        }

        /// <summary>
        /// Property indicating the Progress style:  None, Percentage, ProgressBar
        /// </summary>
        public ProgressStyleEnum ProgressStyle
        {
            get
            {
                return progressStyle;
            }

            set
            {
                progressStyle = value;
            }
        }

        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.OnButtonClicked += OnButtonClicked;
        }

        private void OnButtonClicked(GameObject obj)
        {
            ProgressExamples examples = Object.FindObjectOfType<ProgressExamples>();
            examples.LaunchProgress(indicatorStyle, progressStyle);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Examples.UX;
using MixedRealityToolkit.UX.Buttons;
using MixedRealityToolkit.UX.Progress;
using UnityEngine;

namespace MixedRealityToolkit.Examples.UX
{
    public class ProgressExampleLaunchButton : MonoBehaviour
    {
        [Header("Which Indicator style is desired?")]
        [SerializeField]
        private ProgressIndicator.IndicatorStyleEnum indicatorStyle;

        [Header("Which Progress style is desired?")]
        [SerializeField]
        private ProgressIndicator.ProgressStyleEnum progressStyle;

        public ProgressIndicator.IndicatorStyleEnum IndicatorStyle
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

        public ProgressIndicator.ProgressStyleEnum ProgressStyle
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

        private void OnEnable()
        {
            GetComponent<Button>().OnButtonClicked += OnButtonClicked;
        }

        private void OnButtonClicked(GameObject obj)
        {
            ProgressExamples examples = Object.FindObjectOfType<ProgressExamples>();
            examples.LaunchProgress(indicatorStyle, progressStyle);
        }
    }
}

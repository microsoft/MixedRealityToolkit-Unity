// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Buttons;
using MixedRealityToolkit.UX.Progress;
using UnityEngine;

namespace MixedRealityToolkit.Examples.UX
{
    public class ProgressExampleLaunchButton : MonoBehaviour
    {
        [Header("Which Indicator style is desired?")]
        [SerializeField]
        private IndicatorStyleEnum indicatorStyle = IndicatorStyleEnum.None;

        [Header("Which Progress style is desired?")]
        [SerializeField]
        private ProgressStyleEnum progressStyle = ProgressStyleEnum.None;

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

        private void OnEnable()
        {
            GetComponent<Button>().OnButtonClicked += OnButtonClicked;
        }

        private void OnButtonClicked(GameObject obj)
        {
            var examples = FindObjectOfType<ProgressExamples>();
            examples.LaunchProgress(indicatorStyle, progressStyle);
        }
    }
}

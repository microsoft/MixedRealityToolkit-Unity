// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Examples.UX;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons
{
    public class ProgressButton : MonoBehaviour
    {
        void OnEnable()
        {
            GetComponent<Button>().OnButtonClicked += OnButtonClicked;
        }

        private void OnButtonClicked(GameObject obj)
        {
            string na = this.gameObject.name;
            ProgressExamples examples = Object.FindObjectOfType<ProgressExamples>();
            examples.LaunchProgress(this.gameObject);
        }
    }
}

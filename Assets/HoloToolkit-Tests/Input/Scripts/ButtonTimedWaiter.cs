// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class ButtonTimedWaiter : MonoBehaviour
    {
        [SerializeField]
        private TestButton button = null;

        [SerializeField]
        private float TimeToWait = 1.0f;

        [SerializeField]
        private TextMesh textMesh = null;

        private void Awake()
        {
            button.Activated += OnButtonPressed;
        }

        private void OnDisable()
        {
            button.Activated -= OnButtonPressed;
        }

        private void OnButtonPressed(TestButton source)
        {
            InputManager.Instance.PushInputDisable();
            StartCoroutine(WaitForTime(TimeToWait));
        }

        IEnumerator WaitForTime(float timeToWait)
        {
            float currentTime = 0.0f;

            while (currentTime <= timeToWait)
            {
                currentTime += Time.deltaTime;
                textMesh.text = ((currentTime / timeToWait) * 100.0f).ToString("F0") + "%";
                yield return null;
            }

            InputManager.Instance.PopInputDisable();
            button.Selected = false;
            textMesh.text = "Wait";

            yield break;
        }
    }
}
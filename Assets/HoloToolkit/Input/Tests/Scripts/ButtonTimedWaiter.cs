//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class ButtonTimedWaiter : MonoBehaviour
    {
        [SerializeField]
        private Button button = null;

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

        private void OnButtonPressed(Button source)
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
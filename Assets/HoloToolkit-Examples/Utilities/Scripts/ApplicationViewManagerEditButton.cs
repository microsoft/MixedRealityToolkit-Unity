// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.Tests
{
    [RequireComponent(typeof(ApplicationViewManager))]
    public class ApplicationViewManagerEditButton : MonoBehaviour
    {
        public delegate void LaunchXmlView(string result);

        /// <summary>
        /// Event to subscribe to when a text result is returned from the XML view.
        /// </summary>
        public event LaunchXmlView OnResult;

        public Text Field;

        private ApplicationViewManager viewManager;

        private void Awake()
        {
            viewManager = gameObject.EnsureComponent<ApplicationViewManager>();
        }

        public void StartEdit()
        {
            StartCoroutine(OpenViewEdit());
        }

        private IEnumerator OpenViewEdit()
        {
            string result = string.Empty;

            yield return viewManager.OnLaunchXamlView<string>("TestPage", s => result = s);

            yield return new WaitUntil(() => !string.IsNullOrEmpty(result));

            if (OnResult != null)
            {
                OnResult(result);
            }

            if (Field != null)
            {
                Field.text = result;
            }
        }
    }
}

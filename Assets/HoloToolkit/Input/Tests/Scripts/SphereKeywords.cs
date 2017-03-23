// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Unity.InputModule.Tests
{
    [RequireComponent(typeof(Renderer))]
    public class SphereKeywords : MonoBehaviour, ISpeechHandler
    {
        private Material cachedMaterial;

        private void Awake()
        {
            cachedMaterial = GetComponent<Renderer>().material;
        }

        public void ChangeColor(string color)
        {
            switch (color.ToLower())
            {
                case "red":
                    cachedMaterial.SetColor("_Color", Color.red);
                    break;
                case "blue":
                    cachedMaterial.SetColor("_Color", Color.blue);
                    break;
                case "green":
                    cachedMaterial.SetColor("_Color", Color.green);
                    break;
            }
        }

        public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
        {
            ChangeColor(eventData.RecognizedText);
        }

        private void OnDestroy()
        {
            DestroyImmediate(cachedMaterial);
        }

		public void ScaleResponse()
		{
			StartCoroutine(ScaleResponseAnimation());
		}

		IEnumerator ScaleResponseAnimation()
		{

			while(transform.localScale.x < 0.4)
			{
				transform.localScale += Vector3.one * Time.deltaTime * 3;
				yield return null;
			}
			yield return new WaitForSeconds(0.1f);
			while (transform.localScale.x > 0.2f)
			{
				transform.localScale -= Vector3.one * Time.deltaTime * 3;
				yield return null;
			}
			transform.localScale = Vector3.one * 0.2f;
		}
    }
}
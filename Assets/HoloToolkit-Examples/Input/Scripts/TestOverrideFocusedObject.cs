// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TestOverrideFocusedObject : MonoBehaviour, IInputClickHandler
    {
        private InputManager inputManager;
        private TextMesh textMesh;

        //Use this for initialisation
        private void Start()
        {
            inputManager = InputManager.Instance;

            if (inputManager != null)
            {
                inputManager.OverrideFocusedObject = gameObject;
            }

            //Returns the object that matches the type of TextMesh
            textMesh = FindObjectOfType<TextMesh>();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (textMesh != null && inputManager != null)
            {
                textMesh.text = "Air tap worked and OverrideFocusedObject is null.";
                inputManager.OverrideFocusedObject = null;
            }
        }
    }
}
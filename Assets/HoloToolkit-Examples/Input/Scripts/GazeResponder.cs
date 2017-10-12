// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// This class implements IFocusable to respond to gaze changes.
    /// It highlights the object being gazed at.
    /// </summary>
    public class GazeResponder : MonoBehaviour, IFocusable
    {
        private Material[] defaultMaterials;

        private void Start()
        {
            defaultMaterials = GetComponent<Renderer>().materials;
        }

        public void OnFocusEnter()
        {
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                // Highlight the material when gaze enters using the shader property.
                defaultMaterials[i].SetFloat("_Highlight", .35f);
            }
        }

        public void OnFocusExit()
        {
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                // Remove highlight on material when gaze exits.
                defaultMaterials[i].SetFloat("_Highlight", 0f);
            }
        }

        private void OnDestroy()
        {
            foreach (var material in defaultMaterials)
            {
                Destroy(material);
            }
        }
    }
}
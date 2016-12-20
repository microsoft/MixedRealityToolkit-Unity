// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class SphereKeywords : MonoBehaviour
    {
        public void ChangeColor(string color)
        {
            switch(color.ToLower())
            {
                case "red":
                    ChangeColor(Color.red);
                    break;
                case "green":
                    ChangeColor(Color.green);
                    break;
                case "blue":
                    ChangeColor(Color.blue);
                    break;
            }
        }

        public void ResetAll()
        {
            ChangeChildrenColor(Color.white);
        }

        private void ChangeColor(Color color)
        {
            Renderer renderer = GetComponent<Renderer>();
            ChangeColor(renderer, color);
        }

        private void ChangeChildrenColor(Color color)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                ChangeColor(renderer, color);
            }
        }

        private static void ChangeColor(Renderer renderer, Color color)
        {
            renderer.material.color = color;
        }
    }
}
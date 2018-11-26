// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Examples.SpectatorView
{
    /// <summary>
    /// Network component that randomly changes the color of its Renderer every given time
    /// </summary>
    public class ColorChanger : NetworkBehaviour
    {
        /// <summary>
        /// Current color iteration of the object
        /// </summary>
        [SyncVar] private Color color;

        /// <summary>
        /// Material to operate on
        /// </summary>
        private Material mat;

        /// <summary>
        /// Counts the time between color changes
        /// </summary>
        private float timer;

        private void Start()
        {
            mat = GetComponent<Renderer>().material;
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            ChangeColor();
        }

        private void Update()
        {
            if (isServer)
            {
                if (timer > 3.0f)
                {
                    ChangeColor();
                    timer = 0f;
                }

                timer += Time.deltaTime;
            }

            mat.color = color;
        }

        /// <summary>
        /// Changes the renderer to a new random color
        /// </summary>
        private void ChangeColor()
        {
            color = UnityEngine.Random.ColorHSV();
            mat.color = color;
        }

        /// <summary>
        /// Destroys the changed material
        /// </summary>
        private void OnDestroy()
        {
            Destroy(mat);
        }
    }
}

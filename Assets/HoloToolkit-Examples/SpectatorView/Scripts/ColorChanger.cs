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
        /// Renderer to which the color is applied to
        /// </summary>
        private Renderer objectRenderer;

        /// <summary>
        /// Counts the time between color changes
        /// </summary>
        private float timer;

        private void Start()
        {
            objectRenderer = GetComponent<Renderer>();
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

            objectRenderer.material.color = color;
        }

        /// <summary>
        /// Changes the renderer to a new random color
        /// </summary>
        private void ChangeColor()
        {
            color = UnityEngine.Random.ColorHSV();
            objectRenderer.material.color = color;
        }
    }
}

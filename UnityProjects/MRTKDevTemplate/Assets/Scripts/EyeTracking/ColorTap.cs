// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    using UnityEngine.Serialization;
    using UnityEngine.XR.Interaction.Toolkit;

    [RequireComponent(typeof(Renderer))]
    public class ColorTap : MonoBehaviour
    {
        [SerializeField]
        private Color idleStateColor = Color.cyan;

        [SerializeField]
        private Color onHoverColor = Color.white;

        [SerializeField]
        private Color onSelectColor = Color.blue;

        private Material material;

        private void Awake()
        {
            material = GetComponent<Renderer>().material;
        }

        public void OnGazeHoverEntered()
        {
            material.color = onHoverColor;
        }

        public void OnGazeHoverExited()
        {
            material.color = idleStateColor;
        }

        public void OnGazePinchEntered()
        {
            material.color = onHoverColor;
        }

        public void OnGazePinchExited()
        {
            material.color = idleStateColor;
        }

        public void OnSelectEntered(SelectEnterEventArgs _)
        {
            material.color = onSelectColor;
        }

        public void OnSelectExited(SelectExitEventArgs _)
        {
            material.color = onHoverColor;
        }
    }
}

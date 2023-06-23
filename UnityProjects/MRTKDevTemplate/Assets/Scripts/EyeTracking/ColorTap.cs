// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    using UnityEngine.XR.Interaction.Toolkit;

    public class ColorTap : MonoBehaviour
    {
        [SerializeField]
        private Color _IdleStateColor = Color.cyan;

        [SerializeField]
        private Color _OnHoverColor = Color.white;

        [SerializeField]
        private Color _OnSelectColor = Color.blue;

        private Material _material;

        private void Awake()
        {
            _material = GetComponent<Renderer>().material;
        }

        public void OnGazeHoverEntered()
        {
            _material.color = _OnHoverColor;
        }

        public void OnGazeHoverExited()
        {
            _material.color = _IdleStateColor;
        }

        public void OnGazePinchEntered()
        {
            _material.color = _OnHoverColor;
        }

        public void OnGazePinchExited()
        {
            _material.color = _IdleStateColor;
        }

        public void OnSelectEntered(SelectEnterEventArgs args)
        {
            _material.color = _OnSelectColor;
        }

        public void OnSelectExited(SelectExitEventArgs args)
        {
            _material.color = _OnHoverColor;
        }
    }
}

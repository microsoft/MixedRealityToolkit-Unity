// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.InputModule
{
    public class RotateMinMax : MonoBehaviour
    {
        [SerializeField] private float _minAngle = 0;
        [SerializeField] private float _maxAngle = 0;
        [SerializeField] private float _step = 0;

        private void Update()
        {
            transform.Rotate(Vector3.up, _step);
            if (transform.localRotation.eulerAngles.y < _minAngle || transform.localRotation.eulerAngles.y > _maxAngle)
            {
                _step *= -1;
            }
        }
    }
}

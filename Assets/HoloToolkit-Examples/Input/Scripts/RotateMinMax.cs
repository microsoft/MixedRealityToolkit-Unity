// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class RotateMinMax : MonoBehaviour
    {
        [SerializeField] private float _minAngle;
        [SerializeField] private float _maxAngle;
        [SerializeField] private float _step;

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

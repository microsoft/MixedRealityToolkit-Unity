// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class ReflectionTestBehaviour : MonoBehaviour
    {
        public bool AwakeCalled { get; private set; }
        public bool StartCalled { get; private set; }
        public bool GenericPrivateMethodCalled { get; private set; }
        public bool GenericPublicMethodCalled { get; private set; }
        public int UpdateCallCount { get; private set; }

        private void Awake()
        {
            AwakeCalled = true;
        }

        private void Start()
        {
            StartCalled = true;
        }

        private void Update()
        {
            UpdateCallCount++;
        }

        private void GenericPrivateMethod()
        {
            GenericPrivateMethodCalled = true;
        }

        public void GenericPublicMethod()
        {
            GenericPublicMethodCalled = true;
        }
    }
}

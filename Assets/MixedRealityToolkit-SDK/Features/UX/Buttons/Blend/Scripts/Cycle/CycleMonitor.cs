// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blend.Cycle
{
    [RequireComponent(typeof(ICycle))]
    public class CycleMonitor : MonoBehaviour
    {

        public UnityEvent AdvanceComplete;

        private ICycle cycle;
        private bool isComplete = false;

        private void Awake()
        {
            cycle = GetComponent<ICycle>();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (cycle.Index == 0 && isComplete)
            {
                isComplete = false;
            }

            if (!isComplete && cycle.Index >= cycle.GetLastIndex())
            {
                isComplete = true;
                AdvanceComplete.Invoke();
            }
        }
    }
}

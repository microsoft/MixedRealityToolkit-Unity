// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blend.Cycle
{
    /// <summary>
    /// A way to automate ICycle objects
    /// </summary>
    public class CycleAnimator : MonoBehaviour
    {
        public ICycle[] CycleObjects;
        public float Gap = 0.3f;
        public bool Loop;
        public bool IsPlaying = false;
        public bool IsReverse = false;

        private float timer;

        private void Awake()
        {
            if (CycleObjects == null)
                CycleObjects = GetComponents<ICycle>();
        }

        public void StartPlaying()
        {
            timer = 0;
            IsPlaying = true;
        }

        private void MoveNext(bool reverse)
        {
            for (int i = 0; i < CycleObjects.Length; i++)
            {
                if (reverse)
                {
                    CycleObjects[i].MovePrevious();
                }
                else
                {
                    CycleObjects[i].MoveNext();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (IsPlaying)
            {
                if (timer < Gap)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    if (Loop)
                    {
                        timer = 0;
                    }

                    if (CycleObjects != null)
                    {
                        MoveNext(IsReverse);
                    }
                }
            }
        }
    }
}

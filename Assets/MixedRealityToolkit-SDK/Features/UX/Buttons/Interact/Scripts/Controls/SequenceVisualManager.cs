// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Interact.Widgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Controls
{
    /// <summary>
    /// A manager for laying out Sequence visual state elements (pips) that indicate the n of total states.
    /// </summary>
    public class SequenceVisualManager : MonoBehaviour
    {

        public GameObject VisualPrefab;
        public Vector3 Spacing = new Vector3(0.5f, 0, 0);

        private SequenceCycleWidget mCycleWidget;
        private int mCurrentIndex = -1;
        private List<SequenceVisualState> mVisualStates;

        /// <summary>
        /// Layout the states
        /// </summary>
        private void Awake()
        {
            mCycleWidget = GetComponentInParent<SequenceCycleWidget>();
            mVisualStates = new List<SequenceVisualState>();

            if (VisualPrefab != null && mCycleWidget != null)
            {
                int count = mCycleWidget.SequenceArray.Length;
                Vector3 startPosition = Spacing * count * 0.5f - Spacing * count + Spacing * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    GameObject spawn = GameObject.Instantiate(VisualPrefab);
                    spawn.transform.parent = transform;
                    spawn.transform.localPosition = startPosition + Spacing * i;
                    mVisualStates.Add(spawn.GetComponent<SequenceVisualState>());
                }
            }
        }

        /// <summary>
        /// update the states
        /// </summary>
        void Update()
        {
            if (mCycleWidget != null)
            {
                if (mCycleWidget.Index != mCurrentIndex)
                {
                    mCurrentIndex = mCycleWidget.Index;

                    for (int i = 0; i < mVisualStates.Count; i++)
                    {
                        mVisualStates[i].SetState(mCurrentIndex == i);
                    }
                }
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Interact.Widgets
{
    /// <summary>
    /// place on an Interactive to add sequence functionality
    /// Allows for setting the label and click event for each state of the sequence
    /// </summary>
    [RequireComponent(typeof(Interactive))]
    public class SequenceCycleWidget : InteractiveWidget
    {
        [System.Serializable]
        public struct SequenceInteractiveData
        {
            [Tooltip("Button label per state")]
            public string Label;
            [Tooltip("click event per state")]
            public UnityEvent OnSelectEvent;
        }

        [Tooltip("Array of SequenceInteractiveData to control the sequence states")]
        public SequenceInteractiveData[] SequenceArray;

        [Tooltip("Current label based on the index of the sequece")]
        public string CurrentLabel;

        [Tooltip("Current index, set to change the starting index")]
        public int Index;

        private Interactive mInteractive;

        private void Awake()
        {
            mInteractive = GetComponent<Interactive>();
            mInteractive.OnClick.AddListener(AdvanceSeqence);

            if (Index < SequenceArray.Length)
            {
                CurrentLabel = SequenceArray[Index].Label;
            }
        }

        /// <summary>
        /// move to the next item in the sequence array
        /// </summary>
        private void AdvanceSeqence()
        {
            if (Index < SequenceArray.Length - 1)
            {
                Index += 1;
            }
            else
            {
                Index = 0;
            }

            CurrentLabel = SequenceArray[Index].Label;
            SequenceArray[Index].OnSelectEvent.Invoke();

        }
    }
}

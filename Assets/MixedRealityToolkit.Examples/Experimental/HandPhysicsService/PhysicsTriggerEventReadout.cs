// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    /// <summary>
    /// Writes Collider Trigger from Articulated hands events to a TextMeshPro object
    /// </summary>
    public class PhysicsTriggerEventReadout : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("TextMeshPro object that will write the events")]
        private TextMeshPro textField;

        /// <summary>
        /// TextMeshPro object that will write the events 
        /// </summary>
        public TextMeshPro TextField
        {
            get { return textField; }
            set { textField = value; }
        }

        //private List<Object>

        private void OnEnable()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
//            Microsoft.MixedReality.Toolkit.Input.Jo
        }

        private void OnTriggerExit(Collider other)
        {

        }

        private void WriteText(string text)
        {
            if(textField)
            {
                textField.text = text;
            }
        }

        

    }
}
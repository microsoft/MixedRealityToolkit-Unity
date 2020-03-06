// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.HandPhysics.Examples
{
    /// <summary>
    /// Writes collider trigger from articulated hands events to a TextMeshPro object
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

        private List<JointKinematicBody> currentJoints = new List<JointKinematicBody>();

        private void OnTriggerEnter(Collider other)
        {
            JointKinematicBody joint = other.GetComponent<JointKinematicBody>();
            if (joint == null) { return; }

            currentJoints.Add(joint);
            WriteText();
        }

        private void OnTriggerExit(Collider other)
        {
            JointKinematicBody joint = other.GetComponent<JointKinematicBody>();
            if (joint == null) { return; }

            if(currentJoints.Contains(joint))
            {
                currentJoints.Remove(joint);
            }
            else
            {
                currentJoints.Add(joint);
                WriteText();
                currentJoints.Remove(joint);
            }
            if(currentJoints.Count <= 0)
            {
                WriteText(true);
            }
        }

        private void WriteText(bool clear = false)
        {
            if (textField == null) { return; }

            if (clear)
            {
                textField.text = "";
                return;
            }

            StringBuilder text = new StringBuilder();

            foreach (var joint in currentJoints)
            {
                text.Append(joint.name + " is touching. <br>");
            }
            textField.text = text + "<br>";
        }
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// Prints a message to Unity's console log
    /// </summary>
    public class DebugMessage : AudioNode
    {
        /// <summary>
        /// The text to print to Unity's console when the node is processed
        /// </summary>
        [SerializeField, Multiline]
        private string message = "";

        /// <summary>
        /// Print out the message and move on to the next node
        /// </summary>
        /// <param name="activeEvent"></param>
        public override void ProcessNode(ActiveEvent activeEvent)
        {
            Debug.Log(this.message, activeEvent.source);

            ProcessConnectedNode(0, activeEvent);
        }

#if UNITY_EDITOR
        
        /// <summary>
        /// EDITOR: Set the initial properties on the node
        /// </summary>
        /// <param name="position"></param>
        public override void InitializeNode(Vector2 position)
        {
            this.name = "Debug Message";
            this.nodeRect.height = 50;
            this.nodeRect.width = 250;
            this.nodeRect.position = position;
            AddInput();
            AddOutput();
        }

        /// <summary>
        /// EDITOR: Draw the text field for the node's message
        /// </summary>
        protected override void DrawProperties()
        {
            this.message = EditorGUILayout.TextField(this.message, EditorStyles.textArea);
        }

#endif
    }
}
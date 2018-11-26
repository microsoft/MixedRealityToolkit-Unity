// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// The input connector for an AudioNode
    /// </summary>
    public class AudioNodeInput : ScriptableObject
    {
        /// <summary>
        /// Perimeter of the connector object in the graph
        /// </summary>
        public Rect Window = new Rect(0, 0, ConnectorSize, ConnectorSize);
        /// <summary>
        /// The node that this connector is an input for
        /// </summary>
        [SerializeField]
        private AudioNode parentNode = null;
        /// <summary>
        /// All of the outputs that have connections to this input
        /// </summary>
        [SerializeField]
        private AudioNodeOutput[] connectedNodes = new AudioNodeOutput[0];

#if UNITY_EDITOR

        /// <summary>
        /// Whether the connector accepts more than one output to connect to it
        /// </summary>
        [SerializeField, HideInInspector]
        private bool forceSingleConnection = false;

#endif

        /// <summary>
        /// The size in pixels for the connector in the graph
        /// </summary>
        private const float ConnectorSize = 20;

        /// <summary>
        /// EDTIOR: The position of the center of the connector's Rect
        /// </summary>
        public Vector2 Center
        {
            get
            {
                Vector2 tempPos = this.Window.position;
                tempPos.x += ConnectorSize / 2;
                tempPos.y += ConnectorSize / 2;
                return tempPos;
            }
        }

        /// <summary>
        /// Public accessor for the outputs connected to this input
        /// </summary>
        public AudioNodeOutput[] ConnectedNodes
        {
            get { return this.connectedNodes; }
        }

        /// <summary>
        /// Public accessor for the node that this connector is an input for
        /// </summary>
        public AudioNode ParentNode
        {
            get { return this.parentNode; }
            set { this.parentNode = value; }
        }

#if UNITY_EDITOR

        /// <summary>
        /// EDITOR: Toggle whether this input can accept multiple connections or if a new connection will overwrite the previous one
        /// </summary>
        /// <param name="toggle"></param>
        public void SetSingleConnection(bool toggle)
        {
            this.forceSingleConnection = toggle;
        }

        /// <summary>
        /// EDITOR: Connect a new output to this input
        /// </summary>
        /// <param name="newOutput">The new output to connect</param>
        public void AddConnection(AudioNodeOutput newOutput)
        {
            if (this.forceSingleConnection)
            {
                AudioNodeOutput[] singleOutput = new AudioNodeOutput[1];
                singleOutput[0] = newOutput;
                this.connectedNodes = singleOutput;
                return;
            }

            for (int i = 0; i < this.connectedNodes.Length; i++)
            {
                if (this.connectedNodes[i] == newOutput)
                {
                    return;
                }
            }

            AudioNodeOutput[] newOutputs = new AudioNodeOutput[this.connectedNodes.Length + 1];
            this.connectedNodes.CopyTo(newOutputs, 0);
            newOutputs[newOutputs.Length - 1] = newOutput;
            this.connectedNodes = newOutputs;
            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// EDITOR: Sort the inputs in descending vertical order in the graph
        /// </summary>
        public void SortConnections()
        {
            List<AudioNodeOutput> updatedNodes = new List<AudioNodeOutput>();

            while (updatedNodes.Count < this.connectedNodes.Length)
            {
                AudioNode nextNode = this.connectedNodes[0].ParentNode;
                for (int i = 0; i < this.connectedNodes.Length; i++)
                {
                    AudioNode tempNode = this.connectedNodes[i].ParentNode;
                    if (updatedNodes.Contains(nextNode.Output) || (tempNode.NodeRect.y < nextNode.NodeRect.y && !updatedNodes.Contains(tempNode.Output)))
                    {
                        nextNode = tempNode;
                    }
                }
                updatedNodes.Add(nextNode.Output);
            }

            this.connectedNodes = updatedNodes.ToArray();
        }

        /// <summary>
        /// EDITOR: Clear an output connection
        /// </summary>
        /// <param name="outputToDelete">Output to disconnect from this input</param>
        public void RemoveConnection(AudioNodeOutput outputToDelete)
        {
            if (outputToDelete == null)
            {
                return;
            }

            List<AudioNodeOutput> updatedNodes = new List<AudioNodeOutput>();

            for (int i = this.connectedNodes.Length - 1; i >= 0; i--)
            {
                AudioNodeOutput tempOutput = this.connectedNodes[i];
                if (tempOutput != outputToDelete)
                {
                    updatedNodes.Add(tempOutput);
                }
            }

            this.connectedNodes = updatedNodes.ToArray();
        }

        /// <summary>
        /// EDITOR: Disconnect all output connections
        /// </summary>
        public void RemoveAllConnections()
        {
            this.connectedNodes = new AudioNodeOutput[0];
        }

#endif
    }
}
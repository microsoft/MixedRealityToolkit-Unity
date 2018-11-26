// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// A generic container for audio assets and containers in an AudioEvent
    /// </summary>
    [System.Serializable]
    public class AudioNode : ScriptableObject
    {
        /// <summary>
        /// The visual size of the node in the graph
        /// </summary>
        [SerializeField]
        protected Rect nodeRect = new Rect(0, 0, 200, 100);
        /// <summary>
        /// The left connector for the node that can be connected to an AudioNodeOutput
        /// </summary>
        [SerializeField]
        protected AudioNodeInput input;
        /// <summary>
        /// The right connector for the node that can be connected to an AudioNodeInput
        /// </summary>
        [SerializeField, HideInInspector]
        protected AudioNodeOutput output;

        /// <summary>
        /// The image used for node input and output connectors
        /// </summary>
        protected static Texture2D ConnectorTexture;
        /// <summary>
        /// The minimum possible value for an AudioSource's volume property
        /// </summary>
        protected const float Volume_Min = 0;
        /// <summary>
        /// The maximum possible value for an AudioSource's volume property
        /// </summary>
        protected const float Volume_Max = 1;
        /// <summary>
        /// The minimum possible value for an AudioSource's pitch property
        /// </summary>
        protected const float Pitch_Min = -3;
        /// <summary>
        /// The maximum possible value for an AudioSource's pitch property
        /// </summary>
        protected const float Pitch_Max = 3;

        /// <summary>
        /// Base function for node functionality when the AudioEvent is played
        /// </summary>
        /// <param name="activeEvent">The existing runtime instance of an event</param>
        public virtual void ProcessNode(ActiveEvent activeEvent)
        {
            return;
        }

        /// <summary>
        /// Get the connected node of index nodeNum and process it on the ActiveEvent
        /// </summary>
        /// <param name="nodeNum">The index of the connected node to process</param>
        /// <param name="activeEvent">The existing runtime instance of an AudioEvent</param>
        protected void ProcessConnectedNode(int nodeNum, ActiveEvent activeEvent)
        {
            if (this.input == null)
            {
                Debug.LogWarningFormat(activeEvent.source, "{0} does not have an input on node {1}", activeEvent, this.name);
                return;
            }

            if (nodeNum >= this.input.ConnectedNodes.Length)
            {
                Debug.LogWarningFormat(activeEvent.source, "{0} tried to access invalid connected node {1}", this.name, nodeNum);
                return;
            }

            this.input.ConnectedNodes[nodeNum].ParentNode.ProcessNode(activeEvent);
        }

        /// <summary>
        /// Reset runtime properties associated with the node
        /// </summary>
        public virtual void Reset()
        {

        }

#if UNITY_EDITOR

        /// <summary>
        /// EDITOR: Public accessor for the visual size of the node in the graph
        /// </summary>
        public Rect NodeRect
        {
            get { return this.nodeRect; }
            set { this.nodeRect = value; }
        }

        /// <summary>
        /// EDITOR: Public accessor for the left connector of the node
        /// </summary>
        public AudioNodeInput Input
        {
            get { return this.input; }
        }

        /// <summary>
        /// EDITOR: Public accessor for the right connector of the node
        /// </summary>
        public AudioNodeOutput Output
        {
            get { return this.output; }
        }

        /// <summary>
        /// EDITOR: Initialize required properties of the node when it is first created
        /// </summary>
        /// <param name="position"></param>
        public virtual void InitializeNode(Vector2 position)
        {
            this.name = "Blank Node";
            this.nodeRect.position = position;
        }

        /// <summary>
        /// EDITOR: Delete input and output connectors
        /// </summary>
        public virtual void DeleteConnections()
        {
            if (this.input != null)
            {
                ScriptableObject.DestroyImmediate(this.input, true);
            }
            if (this.output != null)
            {
                ScriptableObject.DestroyImmediate(this.output, true);
            }
        }

        /// <summary>
        /// EDITOR: Draw draggable GUI window in the graph
        /// </summary>
        /// <param name="id"></param>
        public virtual void DrawNode(int id)
        {
            this.nodeRect = GUI.Window(id, this.nodeRect, DrawWindow, this.name);
            DrawInput();
            DrawOutput();
        }

        /// <summary>
        /// EDITOR: Set the position of the node in the graph
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetPosition(Vector2 newPosition)
        {
            this.nodeRect.position = newPosition;
        }

        /// <summary>
        /// EDITOR: Add a left connector to the node when it is first created
        /// </summary>
        /// <param name="singleConnection"></param>
        protected void AddInput(bool singleConnection = false)
        {
            this.input = ScriptableObject.CreateInstance<AudioNodeInput>();
            AssetDatabase.AddObjectToAsset(this.input, this);
            this.input.name = this.name + "Input";
            this.input.ParentNode = this;
            this.input.SetSingleConnection(singleConnection);
        }

        /// <summary>
        /// EDITOR: Add a right connector to the node when it is first created
        /// </summary>
        protected void AddOutput()
        {
            this.output = ScriptableObject.CreateInstance<AudioNodeOutput>();
            AssetDatabase.AddObjectToAsset(this.output, this);
            this.output.name = this.name + "Output";
            this.output.ParentNode = this;
        }

        /// <summary>
        /// EDITOR: Draw the properties of the node in the graph and the drag window
        /// </summary>
        /// <param name="id"></param>
        protected void DrawWindow(int id)
        {
            DrawProperties();
            GUI.DragWindow();
        }

        /// <summary>
        /// EDITOR: Draw associated properties in the node draggable window
        /// </summary>
        protected virtual void DrawProperties() {}

        /// <summary>
        /// EDITOR: Draw the left connector on the node in the graph
        /// </summary>
        protected virtual void DrawInput()
        {
            if (this.input == null)
            {
                return;
            }

            Vector2 tempPos = new Vector2(this.nodeRect.x, this.nodeRect.y);
            tempPos.x -= this.input.Window.width;
            tempPos.y += (this.nodeRect.height / 2);
            this.input.Window.position = tempPos;

            if (ConnectorTexture == null)
            {
                ConnectorTexture = EditorGUIUtility.Load("icons/animationkeyframe.png") as Texture2D;
            }
            GUI.DrawTexture(this.input.Window, ConnectorTexture);

            for (int i = 0; i < this.input.ConnectedNodes.Length; i++)
            {
                AudioNodeOutput tempOutput = this.input.ConnectedNodes[i];
                DrawCurve(tempOutput.Center, this.input.Center);
            }
        }

        /// <summary>
        /// EDITOR: Draw the right connector on the node in the graph
        /// </summary>
        protected virtual void DrawOutput()
        {
            if (this.output == null)
            {
                return;
            }

            Vector2 tempPos = new Vector2(this.nodeRect.x, this.nodeRect.y);
            tempPos.x += this.nodeRect.width;
            tempPos.y += (this.nodeRect.height / 2);
            this.output.Window.position = tempPos;

            if (ConnectorTexture == null)
            {
                ConnectorTexture = EditorGUIUtility.Load("icons/animationkeyframe.png") as Texture2D;
            }
            GUI.DrawTexture(this.output.Window, ConnectorTexture);
        }

        /// <summary>
        /// EDITOR: Draw the line connecting two nodes using a Bezier curve
        /// </summary>
        /// <param name="start">Position of the input node</param>
        /// <param name="end">Position of the output node</param>
        public static void DrawCurve(Vector2 start, Vector2 end)
        {
            Vector3 startPosition = new Vector3(start.x, start.y);
            Vector3 endPosition = new Vector3(end.x, end.y);
            Vector3 startTangent = startPosition + (Vector3.right * 50);
            Vector3 endTangent = endPosition + (Vector3.left * 50);
            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Color.white, null, 2);
        }

#endif
    }
}
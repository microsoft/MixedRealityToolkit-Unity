using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
#endif

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// The logic, settings, and audio clips that define the playback of a sound
    /// </summary>
    public class AudioEvent : ScriptableObject
    {
        /// <summary>
        /// The maximum number of simultaneous instances of an event that can be played
        /// </summary>
        [SerializeField]
        private int instanceLimit = 0;
        
        /// <summary>
        /// The amount of time in seconds for the event to fade in from a volume of 0
        /// </summary>
        [SerializeField]
        private float fadeIn = 0;
        
        /// <summary>
        /// The amount of time in seconds for the event to fade out to a volume of 0
        /// If the event is not explicitly stopped, the fade out will start before the end of the audio file
        /// </summary>
        [SerializeField]
        private float fadeOut = 0;

        /// <summary>
        /// The group of events to stop when this event is played
        /// </summary>
        [SerializeField]
        private int group = 0;

        /// <summary>
        /// The nodes that determine the logic of an AudioEvent
        /// </summary>
        [SerializeField]
        private List<AudioNode> nodes = new List<AudioNode>();
        
        /// <summary>
        /// The final node in an AudioEvent that sets AudioSource properties
        /// </summary>
        [SerializeField]
        private AudioOutput output;
        
        /// <summary>
        /// The parameters that affect the ActiveEvent when it is playing
        /// </summary>
        [SerializeField]
        private List<EventParameter> parameters = new List<EventParameter>();

        /// <summary>
        /// The maximum number of simultaneous instances of an event that can be played
        /// </summary>
        public int InstanceLimit
        {
            get { return this.instanceLimit; }
            set { this.instanceLimit = value; }
        }

        /// <summary>
        /// Internal AudioManager use: accessor for the group this event belongs to
        /// </summary>
        public int Group
        {
            get { return this.group; }
            set { group = value; }
        }

        /// <summary>
        /// Internal AudioManager use: play the event using a pre-existing ActiveEvent
        /// </summary>
        /// <param name="activeEvent">The ActiveEvent for the AudioManager to update and track currently playing events</param>
        public void SetActiveEventProperties(ActiveEvent activeEvent)
        {
            this.output.ProcessNode(activeEvent);
        }

        /// <summary>
        /// Clear all nonserialized modifications the event
        /// </summary>
        public void Reset()
        {
            if (this.nodes == null)
            {
                return;
            }

            for (int i = 0; i < this.nodes.Count; i++)
            {
                this.nodes[i].Reset();
            }
        }

        #region EDITOR

#if UNITY_EDITOR

        /// <summary>
        /// EDITOR: Initialize the required components of a new AudioEvent when it is first created
        /// </summary>
        /// <param name="outputPos">The position of the Output node in the canvas</param>
        public void InitializeEvent(Vector2 outputPos)
        {
            AudioOutput tempNode = ScriptableObject.CreateInstance<AudioOutput>();
            AssetDatabase.AddObjectToAsset(tempNode, this);
            tempNode.InitializeNode(outputPos);
            this.output = tempNode;
            this.nodes.Add(tempNode);
            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// EDITOR: Add a created node to the event
        /// </summary>
        /// <param name="newNode">The node to add</param>
        public void AddNode(AudioNode newNode)
        {
            this.nodes.Add(newNode);
        }

        /// <summary>
        /// EDITOR: Destroy all nodes (including Output) and clear their connections
        /// </summary>
        public void DeleteNodes()
        {
            for (int i = 0; i < this.nodes.Count; i++)
            {
                this.nodes[i].DeleteConnections();
                ScriptableObject.DestroyImmediate(this.nodes[i], true);
            }
        }

        /// <summary>
        /// EDITOR: Remove and destroy a node (except Output)
        /// </summary>
        /// <param name="nodeToDelete">The node you wish to delete</param>
        public void DeleteNode(AudioNode nodeToDelete)
        {
            if (nodeToDelete == null)
            {
                Debug.LogWarning("Trying to remove null node!");
                return;
            }
            else if (nodeToDelete == this.output)
            {
                Debug.LogWarning("Trying to delete output node!");
                return;
            }

            for (int i = 0; i < this.nodes.Count; i++)
            {
                AudioNode tempNode = this.nodes[i];
                if (tempNode != nodeToDelete)
                {
                    if (tempNode.Input != null && nodeToDelete.Output != null)
                    {
                        tempNode.Input.RemoveConnection(nodeToDelete.Output);
                    }
                }
            }

            nodeToDelete.DeleteConnections();
            this.nodes.Remove(nodeToDelete);
            ScriptableObject.DestroyImmediate(nodeToDelete, true);
            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// EDITOR: Add an empty parameter
        /// </summary>
        public void AddParameter()
        {
            if (this.parameters == null)
            {
                this.parameters = new List<EventParameter>();
            }

            this.parameters.Add(new EventParameter());
        }

        /// <summary>
        /// EDITOR: Remove a parameter from the event
        /// </summary>
        /// <param name="parameterToDelete">The parameter you wish to remove</param>
        public void DeleteParameter(EventParameter parameterToDelete)
        {
            this.parameters.Remove(parameterToDelete);
        }

        /// <summary>
        /// EDITOR: Draw the parameters section of the event editor
        /// </summary>
        public void DrawParameters()
        {
            if (GUILayout.Button("Add Parameter"))
            {
                AddParameter();
            }

            if (this.parameters == null)
            {
                this.parameters = new List<EventParameter>();
            }

            for (int i = 0; i < this.parameters.Count; i++)
            {
                EventParameter tempParam = this.parameters[i];
                tempParam.parameter = EditorGUILayout.ObjectField(tempParam.parameter, typeof(AudioParameter), false) as AudioParameter;
                tempParam.responseCurve = EditorGUILayout.CurveField("Curve", tempParam.responseCurve);
                tempParam.paramType = (ParameterType)EditorGUILayout.EnumPopup("Property", tempParam.paramType);
                if (GUILayout.Button("Delete Parameter"))
                {
                    DeleteParameter(tempParam);
                }
                EditorGUILayout.Separator();
            }
        }

#endif

        /// <summary>
        /// Time in seconds for the event to fade in from a volume of 0
        /// </summary>
        public float FadeIn
        {
            get { return this.fadeIn; }
#if UNITY_EDITOR
            set { this.fadeIn = value; }
#endif
        }

        /// <summary>
        /// Time in seconds for the event to fade out
        /// </summary>
        public float FadeOut
        {
            get { return this.fadeOut; }
            set { this.fadeOut = value; }
        }

        /// <summary>
        /// Public accessor for the list of all nodes in the event
        /// </summary>
        public List<AudioNode> EditorNodes
        {
            get { return this.nodes; }
        }

        /// <summary>
        /// Public accessor for the Output node reference
        /// </summary>
        public AudioOutput Output
        {
            get { return this.output; }
            set { this.output = value; }
        }

        /// <summary>
        /// Public accessor for the list of parameters modifying the event at runtime
        /// </summary>
        public List<EventParameter> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    return new List<EventParameter>();
                }
                else
                {
                    return this.parameters;
                }
            }
        }

        #endregion
    }
}
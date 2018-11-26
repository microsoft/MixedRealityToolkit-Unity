// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    public class AudioGraph : EditorWindow
    {
        /// <summary>
        /// The AudioBank currently being edited
        /// </summary>
        private AudioBank audioBank;
        /// <summary>
        /// The AudioEvent currently being edited
        /// </summary>
        private AudioEvent selectedEvent;
        /// <summary>
        /// The AudioNode currently selected
        /// </summary>
        private AudioNode selectedNode;
        /// <summary>
        /// The node output being connected to an input
        /// </summary>
        private AudioNodeOutput selectedOutput;
        /// <summary>
        /// The rectangle defining the space where the list of events are drawn
        /// </summary>
        private Rect eventListRect = new Rect(0, 20, 200, 400);
        /// <summary>
        /// The rectangle defining the space where the event's properties are drawn
        /// </summary>
        private Rect eventPropertyRect = new Rect(200, 20, 240, 150);
        /// <summary>
        /// The rectangle defining the space where the parameters are drawn
        /// </summary>
        private Rect parameterListRect = new Rect(0, 20, 300, 400);
        /// <summary>
        /// Current position of the scroll box for the list of AudioEvents
        /// </summary>
        private Vector2 eventListScrollPosition = new Vector2();
        /// <summary>
        /// Current position of the scroll view for the list of the current AudioEvent's properties
        /// </summary>
        private Vector2 eventPropertiesScrollPosition = new Vector2();
        /// <summary>
        /// Current position of the scroll view for the list of parameters
        /// </summary>
        private Vector2 parameterListScrollPosition = new Vector2();
        /// <summary>
        /// The position of the mouse on the graph to calculate panning
        /// </summary>
        private Vector3 lastMousePos;
        /// <summary>
        /// The horizontal offset of the graph canvas in the window
        /// </summary>
        private float panX = 0;
        /// <summary>
        /// The verical offset of the graph canvas in the window
        /// </summary>
        private float panY = 0;
        /// <summary>
        /// Whether mouse movement should be used to calculate panning the graph
        /// </summary>
        private bool panGraph = false;
        /// <summary>
        /// Whether the graph has been panned since the right mouse button was clicked
        /// </summary>
        private bool hasPanned = false;
        /// <summary>
        /// Whether the right mouse button has been clicked
        /// </summary>
        private bool rightButtonClicked = false;
        /// <summary>
        /// The runtime event used to preview sounds in the graph
        /// </summary>
        private ActiveEvent previewEvent;
        /// <summary>
        /// Selection for which editor is currently being used
        /// </summary>
        private EditorTypes editorType = EditorTypes.Events;
        /// <summary>
        /// Names for the available editor types
        /// </summary>
        private readonly string[] editorTypeNames = { "Events", "Parameters" };
        /// <summary>
        /// The color to display a button for an event that is not currently being edited
        /// </summary>
        private Color unselectedButton = new Color(0.8f, 0.8f, 0.8f, 1);

        /// <summary>
        /// The size in pixels of the node canvas for the graph
        /// </summary>
        private const float CANVAS_SIZE = 20000;

        /// <summary>
        /// List of available editors for an AudioBank
        /// </summary>
        public enum EditorTypes
        {
            Events,
            Parameters
        }

        /// <summary>
        /// Display the graph window
        /// </summary>
        [MenuItem("Window/Audio Graph")]
        private static void OpenAudioGraph()
        {
            AudioGraph graph = GetWindow<AudioGraph>();
            graph.titleContent = new GUIContent("Audio Graph");
            graph.Show();
        }

        /// <summary>
        /// Display the graph window and automatically open an existing AudioBank
        /// </summary>
        /// <param name="bankToLoad"></param>
        public static void OpenAudioGraph(AudioBank bankToLoad)
        {
            AudioGraph graph = GetWindow<AudioGraph>();
            graph.audioBank = bankToLoad;
            graph.Show();
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            this.editorType = (EditorTypes)GUILayout.Toolbar((int)this.editorType, this.editorTypeNames, EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Actions", EditorStyles.toolbarDropDown))
            {
                GenericMenu newNodeMenu = new GenericMenu();
                newNodeMenu.AddItem(new GUIContent("Add Event"), false, AddEvent);
                newNodeMenu.AddItem(new GUIContent("Delete Event"), false, ConfirmDeleteEvent);
                newNodeMenu.AddItem(new GUIContent("Preview Event"), false, PreviewEvent);
                newNodeMenu.AddItem(new GUIContent("Stop Preview"), false, StopPreview);
                newNodeMenu.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            switch (this.editorType)
            {
                case EditorTypes.Events:
                    if (this.selectedEvent != null)
                    {
                        GetInput();

                        DrawEventNodes(this.selectedEvent);

                        DrawEventProperties(this.selectedEvent);
                    }
                    DrawEventList();
                    break;
                case EditorTypes.Parameters:
                    DrawParameterList();
                    break;
            }
        }

        #region Drawing

        /// <summary>
        /// Display the list of buttons to select an event
        /// </summary>
        private void DrawEventList()
        {
            this.eventListRect.height = this.position.height;
            GUILayout.BeginArea(this.eventListRect);
            this.eventListScrollPosition = EditorGUILayout.BeginScrollView(this.eventListScrollPosition);
            this.audioBank = EditorGUILayout.ObjectField(this.audioBank, typeof(AudioBank), false) as AudioBank;

            if (this.audioBank == null)
            {
                EditorGUILayout.EndScrollView();
                GUILayout.EndArea();
                return;
            }

            if (this.audioBank.EditorEvents != null)
            {
                for (int i = 0; i < this.audioBank.EditorEvents.Count; i++)
                {
                    AudioEvent tempEvent = this.audioBank.EditorEvents[i];
                    if (tempEvent == null)
                    {
                        continue;
                    }

                    if (this.selectedEvent == tempEvent)
                    {
                        GUI.color = Color.white;
                    }
                    else
                    {
                        GUI.color = this.unselectedButton;
                    }

                    if (GUILayout.Button(tempEvent.name))
                    {
                        SelectEvent(tempEvent);
                    }

                    GUI.color = Color.white;
                }
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Display the all of the parameters in the bank
        /// </summary>
        private void DrawParameterList()
        {
            this.parameterListRect.height = this.position.height / 2;
            GUILayout.BeginArea(this.parameterListRect);
            this.parameterListScrollPosition = EditorGUILayout.BeginScrollView(this.parameterListScrollPosition);

            if (this.audioBank == null)
            {
                EditorGUILayout.EndScrollView();
                GUILayout.EndArea();
                return;
            }

            if (GUILayout.Button("Add Parameter"))
            {
                this.audioBank.AddParameter();
            }

            if (this.audioBank.EditorParameters != null)
            {
                for (int i = 0; i < this.audioBank.EditorParameters.Count; i++)
                {
                    AudioParameter tempParameter = this.audioBank.EditorParameters[i];
                    if (tempParameter == null)
                    {
                        continue;
                    }

                    tempParameter.DrawParameterEditor();
                    if (GUILayout.Button("Delete Parameter"))
                    {
                        this.audioBank.DeleteParameter(tempParameter);
                    }
                    EditorGUILayout.Separator();
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Display the properties of the specified AudioEvent
        /// </summary>
        /// <param name="audioEvent">The event to display the properties for</param>
        private void DrawEventProperties(AudioEvent audioEvent)
        {
            GUILayout.BeginArea(this.eventPropertyRect);
            this.eventPropertiesScrollPosition = EditorGUILayout.BeginScrollView(this.eventPropertiesScrollPosition);
            audioEvent.name = EditorGUILayout.TextField("Event Name", audioEvent.name);
            audioEvent.InstanceLimit = EditorGUILayout.IntField("Instance limit", audioEvent.InstanceLimit);
            audioEvent.FadeIn = EditorGUILayout.FloatField("Fade In", audioEvent.FadeIn);
            audioEvent.FadeOut = EditorGUILayout.FloatField("Fade Out", audioEvent.FadeOut);
            audioEvent.Group = EditorGUILayout.IntField("Group", audioEvent.Group);

            audioEvent.DrawParameters();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Display the nodes for an AuidoEvent on the graph
        /// </summary>
        /// <param name="audioEvent">The audio event to display the nodes for</param>
        private void DrawEventNodes(AudioEvent audioEvent)
        {
            if (audioEvent == null)
            {
                return;
            }

            if (audioEvent.EditorNodes == null)
            {
                return;
            }

            GUI.BeginGroup(new Rect(this.panX, this.panY, CANVAS_SIZE, CANVAS_SIZE));
            BeginWindows();
            for (int i = 0; i < audioEvent.EditorNodes.Count; i++)
            {
                AudioNode currentNode = audioEvent.EditorNodes[i];
                currentNode.DrawNode(i);
            }
            EndWindows();
            GUI.EndGroup();
        }

        #endregion

        /// <summary>
        /// Create a new event and select it in the graph
        /// </summary>
        private void AddEvent()
        {
            AudioEvent newEvent = this.audioBank.AddEvent(new Vector2(CANVAS_SIZE / 2, CANVAS_SIZE / 2));
            SelectEvent(newEvent);
        }

        /// <summary>
        /// Display a confirmation dialog and delete the currently-selected event if confirmed
        /// </summary>
        private void ConfirmDeleteEvent()
        {
            if (EditorUtility.DisplayDialog("Confrim Event Deletion", "Delete event \"" + this.selectedEvent.name + "\"?", "Yes", "No"))
            {
                this.audioBank.DeleteEvent(this.selectedEvent);
            }
        }

        /// <summary>
        /// Select an event to display in the graph
        /// </summary>
        /// <param name="selection">The audio event to select and display in the graph</param>
        private void SelectEvent(AudioEvent selection)
        {
            this.selectedEvent = selection;
            Rect output = this.selectedEvent.Output.NodeRect;
            this.panX = -output.x + (this.position.width - output.width - 20);
            this.panY = -output.y + (this.position.height / 2);
        }

        /// <summary>
        /// Play the currently-selected event in the scene
        /// </summary>
        private void PreviewEvent()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Can't Preview Audio Event", "Editor must be in play mode to preview events", "OK");
                return;
            }

            if (this.previewEvent != null)
            {
                this.previewEvent.Stop();
            }

            GameObject tempEmitter = new GameObject("Preview_" + this.selectedEvent.name);
            this.previewEvent = AudioManager.PlayEvent(this.selectedEvent, tempEmitter);
            Destroy(tempEmitter, this.previewEvent.EstimatedRemainingTime);
        }

        /// <summary>
        /// Stop the currently-playing event that was previewed from the graph
        /// </summary>
        private void StopPreview()
        {
            if (this.previewEvent != null)
            {
                this.previewEvent.Stop();
            }
        }

        /// <summary>
        /// Process mouse clicks and call appropriate editor functions
        /// </summary>
        private void GetInput()
        {
            if (this.selectedEvent == null)
            {
                return;
            }

            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    this.selectedNode = GetNodeAtPosition(e.mousePosition);
                    Selection.activeObject = this.selectedNode;
                    if (e.button == 0)
                    {
                        HandleLeftClick(e);
                    }
                    else if (e.button == 1)
                    {
                        HandleRightClick(e);
                    }
                    break;
                case EventType.MouseUp:
                    HandleMouseUp(e);
                    break;
                default:
                    HandleMouseMovement(e);
                    break;
            }
        }

        #region Mouse

        /// <summary>
        /// Perform necessary actions for the left mouse button being pushed this frame
        /// </summary>
        /// <param name="e">The input event handled in Unity</param>
        private void HandleLeftClick(Event e)
        {
            this.rightButtonClicked = false;
            this.selectedOutput = GetOutputAtPosition(e.mousePosition);

            this.selectedNode = GetNodeAtPosition(e.mousePosition);
        }

        /// <summary>
        /// Perform necessary actions for the right mouse button being pushed this frame
        /// </summary>
        /// <param name="e">The input event handled by Unity</param>
        private void HandleRightClick(Event e)
        {
            this.rightButtonClicked = true;
            if (this.selectedOutput == null)
            {
                this.panGraph = true;
                this.lastMousePos = e.mousePosition;
            }
        }

        /// <summary>
        /// Perform necessary actions for the a mouse button being released this frame
        /// </summary>
        /// <param name="e">The input event handled by Unity</param>
        private void HandleMouseUp(Event e)
        {
            if (this.rightButtonClicked && !this.hasPanned)
            {
                this.selectedNode = GetNodeAtPosition(e.mousePosition);
                this.selectedOutput = GetOutputAtPosition(e.mousePosition);
                AudioNodeInput tempInput = GetInputAtPosition(e.mousePosition);

                if (tempInput != null)
                {
                    InputContextMenu(e.mousePosition);
                }
                else if (this.selectedOutput != null)
                {
                    OutputContextMenu(e.mousePosition);
                }
                else if (this.selectedNode == null)
                {
                    CanvasContextMenu(e.mousePosition);
                }
                else
                {
                    ModifyNodeContextMenu(e.mousePosition);
                }
            }
            else
            {
                if (this.selectedOutput != null)
                {
                    AudioNodeInput hoverInput = GetInputAtPosition(e.mousePosition);
                    if (hoverInput != null)
                    {
                        hoverInput.AddConnection(this.selectedOutput);
                    }
                }
            }

            this.panGraph = false;
            this.hasPanned = false;
            this.selectedOutput = null;
        }

        /// <summary>
        /// Perform necessary actions for the mouse moving while a button is held down
        /// </summary>
        /// <param name="e">The input event handled by Unity</param>
        private void HandleMouseMovement(Event e)
        {
            if (this.selectedOutput == null)
            {
                if (this.panGraph && this.selectedNode == null)
                {
                    if (Vector2.Distance(e.mousePosition, this.lastMousePos) > 0)
                    {
                        this.hasPanned = true;
                        this.panX += (e.mousePosition.x - this.lastMousePos.x);
                        this.panY += (e.mousePosition.y - this.lastMousePos.y);
                        this.lastMousePos = e.mousePosition;
                    }
                }

                return;
            }

            AudioNode.DrawCurve(ConvertToLocalPosition(this.selectedOutput.Center), e.mousePosition);
        }

        #endregion

        /// <summary>
        /// Create a context menu for a node's input connector
        /// </summary>
        /// <param name="position">The position on the graph to place the menu</param>
        private void InputContextMenu(Vector2 position)
        {
            GenericMenu newNodeMenu = new GenericMenu();
            newNodeMenu.AddItem(new GUIContent("Clear Connections"), false, ClearInput, position);
            newNodeMenu.AddItem(new GUIContent("Sort Connections"), false, SortInput, position);
            newNodeMenu.ShowAsContext();
        }

        /// <summary>
        /// Create a context menu for a node's output connector
        /// </summary>
        /// <param name="position">The position on the graph to place the menu</param>
        private void OutputContextMenu(Vector2 position)
        {
            GenericMenu newNodeMenu = new GenericMenu();
            newNodeMenu.AddItem(new GUIContent("Clear Connections"), false, ClearOutput, position);
            newNodeMenu.ShowAsContext();
        }

        /// <summary>
        /// Create a context menu on a blank space in the graph
        /// </summary>
        /// <param name="position">The position on the graph to place the menu</param>
        private void CanvasContextMenu(Vector2 position)
        {
            GenericMenu newNodeMenu = new GenericMenu();
            newNodeMenu.AddItem(new GUIContent("Add Audio File"), false, AddNodeAtPosition<AudioFile>, position);
            newNodeMenu.AddItem(new GUIContent("Add Voice File"), false, AddNodeAtPosition<VoiceFile>, position);
            newNodeMenu.AddItem(new GUIContent("Add Random Selector"), false, AddNodeAtPosition<RandomSelector>, position);
            newNodeMenu.AddItem(new GUIContent("Add Sequence Selector"), false, AddNodeAtPosition<SequenceSelector>, position);
            newNodeMenu.AddItem(new GUIContent("Add Language Selector"), false, AddNodeAtPosition<LanguageSelector>, position);
            newNodeMenu.AddItem(new GUIContent("Add Delay"), false, AddNodeAtPosition<Delay>, position);
            newNodeMenu.AddItem(new GUIContent("Add Debug Message"), false, AddNodeAtPosition<DebugMessage>, position);
            newNodeMenu.AddItem(new GUIContent("Set Output Position"), false, SetOutputPosition, position);
            newNodeMenu.ShowAsContext();
        }

        /// <summary>
        /// Create a context menu on a node
        /// </summary>
        /// <param name="position">The position on the graph to place the menu</param>
        private void ModifyNodeContextMenu(Vector2 position)
        {
            GenericMenu newNodeMenu = new GenericMenu();
            newNodeMenu.AddItem(new GUIContent("Delete Node"), false, RemoveNodeAtPosition, position);
            newNodeMenu.ShowAsContext();
        }

        #region Nodes

        /// <summary>
        /// Place the Output node at the specified position
        /// </summary>
        /// <param name="positionObject">The position at which to place the Output node</param>
        private void SetOutputPosition(object positionObject)
        {
            Vector2 newPosition = (Vector2)positionObject;
            newPosition = ConvertToGlobalPosition(newPosition);
            this.selectedEvent.Output.SetPosition(newPosition);
        }

        /// <summary>
        /// Remove a node from the current AudioEvent and delete it from the asset
        /// </summary>
        /// <param name="positionObject">The position of the object to delete</param>
        private void RemoveNodeAtPosition(object positionObject)
        {
            AudioNode tempNode = GetNodeAtPosition((Vector2)positionObject);
            this.selectedEvent.DeleteNode(tempNode);
            EditorUtility.SetDirty(this.selectedEvent);
        }

        /// <summary>
        /// Add a new node on the graph and add it to the current event
        /// </summary>
        /// <typeparam name="T">The AudioNode type to create</typeparam>
        /// <param name="positionObject">The position at which to place the new node</param>
        public void AddNodeAtPosition<T>(object positionObject) where T : AudioNode
        {
            Vector2 position = (Vector2)positionObject;
            T tempNode = ScriptableObject.CreateInstance<T>();
            AssetDatabase.AddObjectToAsset(tempNode, this.selectedEvent);
            tempNode.InitializeNode(ConvertToGlobalPosition(position));
            this.selectedEvent.AddNode(tempNode);
            EditorUtility.SetDirty(this.selectedEvent);
        }

        /// <summary>
        /// Remove all connections from an input connector
        /// </summary>
        /// <param name="positionObject">The position of the input connector</param>
        public void ClearInput(object positionObject)
        {
            AudioNodeInput tempInput = GetInputAtPosition((Vector2)positionObject);
            tempInput.RemoveAllConnections();
        }

        /// <summary>
        /// Arrange the order of connections for an input connector
        /// </summary>
        /// <param name="positionObject">The position of the input connector</param>
        public void SortInput(object positionObject)
        {
            AudioNodeInput tempInput = GetInputAtPosition((Vector2)positionObject);
            tempInput.SortConnections();
        }

        /// <summary>
        /// Remove all connections from an output connector
        /// </summary>
        /// <param name="positionObject"></param>
        public void ClearOutput(object positionObject)
        {
            AudioNodeOutput tempOutput = GetOutputAtPosition((Vector2)positionObject);
            for (int i = 0; i < this.selectedEvent.EditorNodes.Count; i++)
            {
                AudioNode tempNode = this.selectedEvent.EditorNodes[i];
                if (tempNode.Input != null)
                {
                    tempNode.Input.RemoveConnection(tempOutput);
                }
            }
        }

        /// <summary>
        /// Find a node that overlaps a position on the graph
        /// </summary>
        /// <param name="position">The position on the graph to check against the nodes</param>
        /// <returns>The first node found that occupies the specified position or null</returns>
        private AudioNode GetNodeAtPosition(Vector2 position)
        {
            if (this.selectedEvent == null)
            {
                return null;
            }

            position = ConvertToGlobalPosition(position);

            for (int i = 0; i < this.selectedEvent.EditorNodes.Count; i++)
            {
                AudioNode tempNode = this.selectedEvent.EditorNodes[i];
                if (tempNode.NodeRect.Contains(position))
                {
                    return tempNode;
                }
            }

            return null;
        }

        /// <summary>
        /// Find an input connector that overlaps the position on the graph
        /// </summary>
        /// <param name="position">The position on the graph to test against all input connectors</param>
        /// <returns>The first input connector found that occupies the specified position or null</returns>
        private AudioNodeInput GetInputAtPosition(Vector2 position)
        {
            if (this.selectedEvent == null)
            {
                return null;
            }

            position = ConvertToGlobalPosition(position);

            for (int i = 0; i < this.selectedEvent.EditorNodes.Count; i++)
            {
                AudioNode tempNode = this.selectedEvent.EditorNodes[i];
                if (tempNode.Input != null && tempNode.Input.Window.Contains(position))
                {
                    return tempNode.Input;
                }
            }

            return null;
        }

        /// <summary>
        /// Find an output connector that overlaps the position on the graph
        /// </summary>
        /// <param name="position">The position on the graph to test against all output connectors</param>
        /// <returns>The first output connector found that occupies the specified position or null</returns>
        private AudioNodeOutput GetOutputAtPosition(Vector2 position)
        {
            position = ConvertToGlobalPosition(position);
            if (this.selectedEvent == null)
            {
                Debug.LogWarning("Tried to get output with no selected event");
                return null;
            }

            for (int i = 0; i < this.selectedEvent.EditorNodes.Count; i++)
            {
                AudioNode tempNode = this.selectedEvent.EditorNodes[i];
                
                if (tempNode.Output != null && tempNode.Output.Window.Contains(position))
                {
                    return tempNode.Output;
                }
            }

            return null;
        }

        /// <summary>
        /// Convert a global graph position to the local GUI position
        /// </summary>
        /// <param name="inputPosition">The graph position before panning is applied</param>
        /// <returns>The local GUI position after panning is applied</returns>
        public Vector2 ConvertToLocalPosition(Vector2 inputPosition)
        {
            inputPosition.x += this.panX;
            inputPosition.y += this.panY;
            return inputPosition;
        }

        /// <summary>
        /// Convert a local GUI position to the global position on the graph
        /// </summary>
        /// <param name="inputPosition">The local GUI position after panning is applied</param>
        /// <returns>The graph position before panning is applied</returns>
        public Vector2 ConvertToGlobalPosition(Vector2 inputPosition)
        {
            inputPosition.x -= this.panX;
            inputPosition.y -= this.panY;
            return inputPosition;
        }

        #endregion
    }
}
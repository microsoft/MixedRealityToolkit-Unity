// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.UX;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


namespace HoloToolkit.Unity.InputModule.Utilities.Interactions
{
    /// <summary>
    /// This script allows for an object to be movable, scalable, and rotatable with one or two hands. 
    /// You may also configure the script on only enable certain manipulations. The script works with 
    /// both HoloLens' gesture input and immersive headset's motion controller input.
    /// See Assets/HoloToolkit-Examples/Input/Readme/README_TwoHandManipulationTest.md
    /// for instructions on how to use the script.
    /// </summary>
    public class TwoHandManipulatable : MonoBehaviour, IInputHandler, ISourceStateHandler
    {
        [SerializeField]
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform = null;

        [SerializeField]
        [Tooltip("To visualize the object bounding box, drop the HoloToolKit/UX/Prefabs/BoundingBoxes/BoundingBoxBasic.prefab here. This is optional.")]
        private BoundingBox boundingBoxPrefab = null;

        /// <summary>
        /// Reference to the Prefab from which clone is instantiated.
        /// </summary>
        public BoundingBox BoundingBoxPrefab
        {
            set
            {
                boundingBoxPrefab = value;
            }

            get
            {
                return boundingBoxPrefab;
            }
        }

        [SerializeField]
        [Tooltip("What manipulation will two hands perform?")]
        private ManipulationMode manipulationMode = ManipulationMode.Scale;

        [SerializeField]
        [Tooltip("Constrain rotation along an axis")]
        private AxisConstraint constraintOnRotation = AxisConstraint.None;

        [SerializeField]
        [Tooltip("If true, grabbing the object with one hand will initiate movement.")]
        private bool oneHandMovement = true;

        /// <summary>
        /// private properties that store transform information.
        /// </summary>
        private BoundingBox boundingBoxInstance;
        private ManipulationMode currentState;
        private TwoHandMoveLogic m_moveLogic;
        private TwoHandScaleLogic m_scaleLogic;
        private TwoHandRotateLogic m_rotateLogic;

        /// <summary>
        /// Maps input id -> position of hand
        /// </summary>
        private readonly Dictionary<uint, Vector3> m_handsPressedLocationsMap = new Dictionary<uint, Vector3>();

        /// <summary>
        /// Maps input id -> input source. Then obtain position of input source using currentInputSource.TryGetGripPosition(currentInputSourceId, out inputPosition);
        /// </summary>
        private readonly Dictionary<uint, IInputSource> m_handsPressedInputSourceMap = new Dictionary<uint, IInputSource>();

        /// <summary>
        /// Property that turns on and off the Visibility of the BoundingBox cloned from the BoundingBoxPrefab reference.
        /// </summary>
        private bool ShowBoundingBox
        {
            set
            {
                if (boundingBoxPrefab != null)
                {
                    if (boundingBoxInstance == null)
                    {
                        // Instantiate Bounding Box from the Prefab
                        boundingBoxInstance = Instantiate(boundingBoxPrefab) as BoundingBox;
                    }

                    if (value)
                    {
                        boundingBoxInstance.Target = gameObject;
                        boundingBoxInstance.gameObject.SetActive(true);
                    }
                    else
                    {
                        boundingBoxInstance.Target = null;
                        boundingBoxInstance.gameObject.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// SetManipulationMode
        /// </summary>
        public void SetManipulationMode(ManipulationMode mode)
        {
            manipulationMode = mode;
        }

        /// <summary>
        /// Private Methods
        /// </summary>
        private void Awake()
        {
            m_moveLogic = new TwoHandMoveLogic();
            m_rotateLogic = new TwoHandRotateLogic(constraintOnRotation);
            m_scaleLogic = new TwoHandScaleLogic();
        }

        private void Start()
        {
            if (hostTransform == null)
            {
                hostTransform = transform;
            }
        }

        private void Update()
        {
            //Update positions of all hands
            foreach (var key in m_handsPressedInputSourceMap.Keys)
            {
                var inputSource = m_handsPressedInputSourceMap[key];
                Vector3 inputPosition;
                if (inputSource.TryGetGripPosition(key, out inputPosition))
                {
                    m_handsPressedLocationsMap[key] = inputPosition;
                }
            }

            if (currentState != ManipulationMode.None)
            {
                UpdateStateMachine();
            }
        }

        private Vector3 GetInputPosition(InputEventData eventData)
        {
            Vector3 result;
            eventData.InputSource.TryGetGripPosition(eventData.SourceId, out result);
            return result;
        }

        private void RemoveSourceIdFromHandMap(uint sourceId)
        {
            if (m_handsPressedLocationsMap.ContainsKey(sourceId))
            {
                m_handsPressedLocationsMap.Remove(sourceId);
            }

            if (m_handsPressedInputSourceMap.ContainsKey(sourceId))
            {
                m_handsPressedInputSourceMap.Remove(sourceId);
            }
        }

        /// <summary>
        /// /// Event Handler receives input from inputSource
        /// </summary>
        public void OnInputDown(InputEventData eventData)
        {
            // Add to hand map
            m_handsPressedLocationsMap[eventData.SourceId] = GetInputPosition(eventData);
            m_handsPressedInputSourceMap[eventData.SourceId] = eventData.InputSource;
            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// Event Handler receives input from inputSource
        /// </summary>
        public void OnInputUp(InputEventData eventData)
        {
            RemoveSourceIdFromHandMap(eventData.SourceId);
            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// OnSourceDetected Event Handler
        /// </summary>
        public void OnSourceDetected(SourceStateEventData eventData) { }

        /// <summary>
        /// OnSourceLost
        /// </summary>
        public void OnSourceLost(SourceStateEventData eventData)
        {
            RemoveSourceIdFromHandMap(eventData.SourceId);
            UpdateStateMachine();
            eventData.Use();
        }

        /// <summary>
        /// private Event Handlers
        /// </summary>
        private void UpdateStateMachine()
        {
            var handsPressedCount = m_handsPressedLocationsMap.Count;
            ManipulationMode newState = currentState;

            switch (currentState)
            {
                case ManipulationMode.None:
                case ManipulationMode.Move:
                    if (handsPressedCount == 0)
                    {
                        newState = ManipulationMode.None;
                    }
                    else if (handsPressedCount == 1 && oneHandMovement)
                    {
                        newState = ManipulationMode.Move;
                    }
                    else if (handsPressedCount > 1)
                    {
                        newState = manipulationMode;
                    }
                    break;
                case ManipulationMode.Scale:
                case ManipulationMode.Rotate:
                case ManipulationMode.MoveAndScale:
                case ManipulationMode.MoveAndRotate:
                case ManipulationMode.RotateAndScale:
                case ManipulationMode.MoveScaleAndRotate:
                    // TODO: if < 2, make this go to start state ('drop it')
                    if (handsPressedCount == 0)
                    {
                        newState = ManipulationMode.None;
                    }
                    else if (handsPressedCount == 1)
                    {
                        newState = ManipulationMode.Move;
                    }
                    break;
            }

            InvokeStateUpdateFunctions(currentState, newState);
            currentState = newState;
        }

        private void InvokeStateUpdateFunctions(ManipulationMode oldState, ManipulationMode newState)
        {
            if (newState != oldState)
            {
                switch (newState)
                {
                    case ManipulationMode.None:
                        OnManipulationEnded();
                        break;
                    case ManipulationMode.Move:
                        OnOneHandMoveStarted();
                        break;
                    case ManipulationMode.Scale:
                    case ManipulationMode.Rotate:
                    case ManipulationMode.MoveAndScale:
                    case ManipulationMode.MoveAndRotate:
                    case ManipulationMode.RotateAndScale:
                    case ManipulationMode.MoveScaleAndRotate:
                        OnTwoHandManipulationStarted(newState);
                        break;
                }

                switch (oldState)
                {
                    case ManipulationMode.None:
                        OnManipulationStarted();
                        break;
                    case ManipulationMode.Move:
                        break;
                    case ManipulationMode.Scale:
                    case ManipulationMode.Rotate:
                    case ManipulationMode.MoveAndScale:
                    case ManipulationMode.MoveAndRotate:
                    case ManipulationMode.RotateAndScale:
                    case ManipulationMode.MoveScaleAndRotate:
                        OnTwoHandManipulationEnded();
                        break;
                }
            }
            else
            {
                switch (newState)
                {
                    case ManipulationMode.None:
                        break;
                    case ManipulationMode.Move:
                        OnOneHandMoveUpdated();
                        break;
                    case ManipulationMode.Scale:
                    case ManipulationMode.Rotate:
                    case ManipulationMode.MoveAndScale:
                    case ManipulationMode.MoveAndRotate:
                    case ManipulationMode.RotateAndScale:
                    case ManipulationMode.MoveScaleAndRotate:
                        OnTwoHandManipulationUpdated();
                        break;
                }
            }
        }

        private void OnTwoHandManipulationUpdated()
        {
#if UNITY_2017_2_OR_NEWER
            var targetRotation = hostTransform.rotation;
            var targetPosition = hostTransform.position;
            var targetScale = hostTransform.localScale;

            if ((currentState & ManipulationMode.Move) > 0)
            {
                targetPosition = m_moveLogic.Update(GetHandsCentroid(), targetPosition);
            }

            if ((currentState & ManipulationMode.Rotate) > 0)
            {
                targetRotation = m_rotateLogic.Update(m_handsPressedLocationsMap, hostTransform, targetRotation);
            }

            if ((currentState & ManipulationMode.Scale) > 0)
            {
                targetScale = m_scaleLogic.UpdateMap(m_handsPressedLocationsMap);
            }

            hostTransform.position = targetPosition;
            hostTransform.rotation = targetRotation;
            hostTransform.localScale = targetScale;
#endif // UNITY_2017_2_OR_NEWER
        }

        private void OnOneHandMoveUpdated()
        {
            var targetPosition = m_moveLogic.Update(m_handsPressedLocationsMap.Values.First(), hostTransform.position);

            hostTransform.position = targetPosition;
        }

        private void OnTwoHandManipulationEnded()
        {
#if UNITY_2017_2_OR_NEWER
            // This implementation currently does nothing
#endif // UNITY_2017_2_OR_NEWER
        }

        private Vector3 GetHandsCentroid()
        {
            Vector3 result = m_handsPressedLocationsMap.Values.Aggregate(Vector3.zero, (current, state) => current + state);
            return result / m_handsPressedLocationsMap.Count;
        }

        private void OnTwoHandManipulationStarted(ManipulationMode newState)
        {
#if UNITY_2017_2_OR_NEWER
            if ((newState & ManipulationMode.Rotate) > 0)
            {
                m_rotateLogic.Setup(m_handsPressedLocationsMap, hostTransform);
            }

            if ((newState & ManipulationMode.Move) > 0)
            {
                m_moveLogic.Setup(GetHandsCentroid(), hostTransform);
            }

            if ((newState & ManipulationMode.Scale) > 0)
            {
                m_scaleLogic.Setup(m_handsPressedLocationsMap, hostTransform);
            }
#endif // UNITY_2017_2_OR_NEWER
        }

        private void OnOneHandMoveStarted()
        {
            Assert.IsTrue(m_handsPressedLocationsMap.Count == 1);

            m_moveLogic.Setup(m_handsPressedLocationsMap.Values.First(), hostTransform);
        }

        private void OnManipulationStarted()
        {
            InputManager.Instance.PushModalInputHandler(gameObject);

            //Show Bounding Box visual on manipulation interaction
            ShowBoundingBox = true;
        }

        private void OnManipulationEnded()
        {
            InputManager.Instance.PopModalInputHandler();

            //Hide Bounding Box visual on release
            ShowBoundingBox = false;
        }
    }
}

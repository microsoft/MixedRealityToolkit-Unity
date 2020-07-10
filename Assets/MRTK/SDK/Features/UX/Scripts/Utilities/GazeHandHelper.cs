// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// This class must be instantiated by a script that implements the <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealitySourceStateHandler"/>,
    /// <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler"/> and <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler{T}"/>.
    /// 
    /// ***It must receive EventData arguments from OnInputDown(), OnInputUp(), OnInputChanged() and OnSourceLost().***
    /// 
    /// This class manages the states of input necessary to calculate a proper grab position
    /// The eventData received on inputdown has the point on the target that was hit by the gaze;
    /// the mixedrealitypose - eventdata received on input changed contains the handposition in eventdata.inputdata.position
    /// It also contains useful retrieval functions.
    /// </summary>
    [System.Obsolete("This component is no longer supported", true)]
    public class GazeHandHelper
    {
        #region Private Variables
        private readonly Dictionary<uint, bool> positionAvailableMap = new Dictionary<uint, bool>();
        private readonly Dictionary<uint, IMixedRealityInputSource> handSourceMap = new Dictionary<uint, IMixedRealityInputSource>();
        private readonly Dictionary<uint, Vector3> handStartPositionMap = new Dictionary<uint, Vector3>();
        private readonly Dictionary<uint, Vector3> handPositionMap = new Dictionary<uint, Vector3>();
        private readonly Dictionary<uint, Vector3> gazePointMap = new Dictionary<uint, Vector3>();
        #endregion Private Variables

        #region Public Methods

        /// <summary>
        /// This function must be called from the OnInputDown handler in a script implementing the <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler{T}"/>.
        /// </summary>
        /// <param name="eventData">The InputEventData argument 'eventData' is passed through to GazeHandHelper</param>
        public void AddSource(InputEventData eventData)
        {
            IMixedRealityInputSource source = eventData.InputSource;
            if (source != null && IsInDictionary(source.SourceId) == false && source.Pointers != null && source.Pointers.Length > 0)
            {
                handSourceMap.Add(source.SourceId, source);
                gazePointMap.Add(source.SourceId, source.Pointers[0].Position);
                handStartPositionMap.Add(source.SourceId, Vector3.zero);
                handPositionMap.Add(source.SourceId, Vector3.zero);
                positionAvailableMap.Add(source.SourceId, false);
            }
        }

        /// <summary>
        /// This function must be called from the OnInputUp handler in a script implementing the <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler{T}"/>.
        /// </summary>
        /// <param name="eventData">he InputEventData argument 'eventData' is passed through to GazeHandHelper</param>
        public void RemoveSource(InputEventData eventData)
        {
            uint sourceId = eventData.SourceId;
            handPositionMap.Remove(sourceId);
            handSourceMap.Remove(sourceId);
            gazePointMap.Remove(sourceId);
            handStartPositionMap.Remove(sourceId);
            positionAvailableMap.Remove(sourceId);
        }

        /// <summary>
        /// This function must be called from the OnSourceLost handler in a script implementing the IMixedRealitySourceStateHandler interface.
        /// </summary>
        public void RemoveSource(SourceStateEventData eventData)
        {
            uint sourceId = eventData.SourceId;
            handPositionMap.Remove(sourceId);
            handSourceMap.Remove(sourceId);
            gazePointMap.Remove(sourceId);
            handStartPositionMap.Remove(sourceId);
            positionAvailableMap.Remove(sourceId);
        }

        /// <summary>
        /// This function must be called from the OnInputChanged handler in a script implementing the <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputHandler{T}"/>.
        /// </summary>
        public void UpdateSource(InputEventData<MixedRealityPose> eventData)
        {
            uint id = eventData.SourceId;
            Vector3 handPosition = eventData.InputData.Position;

            if (IsInDictionary(id) == true)
            {
                if (handStartPositionMap[id] == Vector3.zero)
                {
                    handStartPositionMap[id] = handPosition;
                    positionAvailableMap[id] = true;
                }

                if (true == TryGetPointerPosition(id, out Vector3 currentGazePoint))
                {
                    handPositionMap[id] = handPosition + (currentGazePoint - gazePointMap[id]);
                }
            }
        }

        /// <summary>
        /// This function returns the number of active hands.
        /// </summary>
        public int GetActiveHandCount()
        {
            int count = 0;
            foreach (uint key in positionAvailableMap.Keys)
            {
                if (positionAvailableMap[key] == true)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// This function gets the average of the positions of all active hands.
        /// </summary>
        /// <returns>Vector3 representing the average position</returns>
        public Vector3 GetHandsCentroid()
        {
            if (true == TryGetHandsCentroid(out Vector3 centroid))
            {
                return centroid;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// This function gets an array of all active hand positions
        /// </summary>
        /// <returns>enumerable of Vector3</returns>
        public IEnumerable<Vector3> GetAllHandPositions()
        {
            foreach (uint key in positionAvailableMap.Keys)
            {
                if (positionAvailableMap[key] == true)
                {
                    yield return handPositionMap[key];
                }
            }
            yield break;
        }

        /// <summary>
        /// This function retrieves the position of the first active hand.
        /// </summary>
        /// <returns>Vector3 representing position</returns>
        public Vector3 GetFirstHand()
        {
            foreach (Vector3 hand in GetAllHandPositions())
                return hand;

            return Vector3.zero;
        }

        /// <summary>
        /// This function retrieves a reference to the Dictionary that maps hand positions to sourceIds.
        /// This return value is NOT filtered for whether the hands are active. User should check first
        /// using GetActiveHandCount().
        /// </summary>
        /// <returns>Dictionary with uint Keys mapping to Vector3 positions</returns>
        public Dictionary<uint, Vector3> GetHandPositionsDictionary()
        {
            return handPositionMap;
        }
        #endregion  Public Methods

        #region Safe TryGet Style Public Methods
        /// <summary>
        /// TryGet style function to return HandPosition of a certain handedness if available.
        /// </summary>
        /// <param name="handedness">asks for left or right hand or either</param>
        /// <param name="position">out value that gets filled with a Vector3 representing position</param>
        /// <returns>true or false- whether the hand existed</returns>
        public bool TryGetHandPosition(Handedness handedness, out Vector3 position)
        {
            foreach (uint key in positionAvailableMap.Keys)
            {
                if (positionAvailableMap[key] == true)
                {
                    if (handSourceMap[key].Pointers != null && handSourceMap[key].Pointers.Length > 0 && handSourceMap[key].Pointers[0].Controller.ControllerHandedness == handedness)
                    {
                        position = handPositionMap[key];
                        return true;
                    }
                }
            }

            position = Vector3.zero;
            return false;
        }

        /// <summary>
        /// TryGet style function to return HandPosition of a certain sourceId if available.
        /// </summary>
        /// <param name="id">asks for the hand position associated with a certain IMixedRealityInputSource id</param>
        /// <param name="handPosition">out value that gets filled with a Vector3 representing position</param>
        /// <returns>true or false- whether the hand existed</returns>
        public bool TryGetHandPosition(uint id, out Vector3 handPosition)
        {
            if (IsInDictionary(id) == true && positionAvailableMap[id] == true)
            {
                handPosition = handPositionMap[id];
                return true;
            }

            handPosition = Vector3.zero;
            return false;
        }

        /// <summary>
        /// TryGet style function to get the average of all active hand positions.
        /// </summary>
        /// <param name="handsCentroid">out value filled with Vector3 representing average of hand positions</param>
        /// <returns>true if there were any active hands; false if there were no active hands</returns>
        public bool TryGetHandsCentroid(out Vector3 handsCentroid)
        {
            if (GetActiveHandCount() > 0)
            {
                Vector3 agg = Vector3.zero;
                int activeCount = 0;
                foreach (uint key in handPositionMap.Keys)
                {
                    if (positionAvailableMap[key] == true)
                    {
                        agg += handPositionMap[key];
                        activeCount++;
                    }
                }

                if (activeCount > 0)
                {
                    handsCentroid = agg / (float)activeCount;
                    return true;
                }
            }

            handsCentroid = Vector3.zero;
            return false;
        }
        #endregion Safe TryGet Style Public Methods

        #region Private Methods
        private bool IsInDictionary(uint id)
        {
            return handSourceMap.ContainsKey(id);
        }
        private bool GetInitialHandPosition(uint id, out Vector3 initialHandPosition)
        {
            if (IsInDictionary(id) == true)
            {
                initialHandPosition = handStartPositionMap[id];
                return true;
            }

            initialHandPosition = Vector3.zero;
            return false;
        }
        private bool GetInitialGazePosition(uint id, out Vector3 initialGazePosition)
        {
            if (IsInDictionary(id) == true)
            {
                initialGazePosition = gazePointMap[id];
                return true;
            }

            initialGazePosition = Vector3.zero;
            return false;
        }
        private bool TryGetPointerPosition(uint id, out Vector3 position)
        {
            IMixedRealityInputSource source = handSourceMap[id];
            if (source != null && source.Pointers != null && source.Pointers.Length > 0)
            {
                position = source.Pointers[0].Position;
                return true;
            }

            position = Vector3.zero;
            return false;
        }
        #endregion Private Methods
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    [AddComponentMenu("Scripts/MRTK/Examples/TriggerZonePlaceObjsWithin")]
    public class TriggerZonePlaceObjsWithin : MonoBehaviour
    {
        [Tooltip("Array of referenced game objects that are supposed to be placed within the collider of this target.).")]
        [SerializeField]
        private GameObject[] _objsToPlaceHere = null;

        [Tooltip("Color of this object when 'idle' - waiting on the correct targets to be placed.")]
        [SerializeField]
        private Color _statusColorIdle = Color.blue;

        [Tooltip("Color of this object once all requested targets have been placed within the trigger zone.")]
        [SerializeField]
        private Color _statusColorAchieved = Color.green;

        [Tooltip("Optional audio clip to be played once all requested objects have been correctly placed.")]
        [SerializeField]
        private AudioClip _audioFxSuccess = null;

        private AudioSource _audioSource;
        private List<string> _currCollidingObjs; // List of game objects that are currently colliding with this target.
        private Color? _originalColor = null; // Store the original color of the target in case we want to use it later.
        private bool _prevAllObjsInZone = false; // Were all requested objects correctly placed in the last run already?
        private bool _justDroppedTarget = false;
        private bool _justTriggered = false;

        private void Start()
        {
            // Init our list of currently colliding game objects
            _currCollidingObjs = new List<string>();

            _audioSource = GetComponent<AudioSource>();

            // Set default color
            EyeTrackingDemoUtils.GameObject_ChangeColor(gameObject, _statusColorIdle, ref _originalColor, false);
        }

        #region Handle collision detection
        /// <summary>
        /// If a new collider enters our trigger zone, let's add it to the list of currently colliding targets.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!_currCollidingObjs.Contains(other.gameObject.name))
            {
                _justTriggered = true;
                _currCollidingObjs.Add(other.gameObject.name);
                CheckForCompletion();
            }
        }

        /// <summary>
        /// If an already enrolled collider leaves our trigger zone, let's remove it from the list of currently colliding targets.
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            if (_currCollidingObjs.Contains(other.gameObject.name))
            {
                _justTriggered = true;
                _currCollidingObjs.Remove(other.gameObject.name);
                CheckForCompletion();
            }
        }
        #endregion

        /// <summary>
        /// This additional method is required as the OnTriggerEnter method sometimes is triggered later than the OnRelease events. 
        /// Hence, we're noting whether a release (via hands, voice, etc.) has happened and then wait for the OnTriggerEnter method
        /// to call for the CheckForCompletion to determine if all our specified targets are in the destination zone.
        /// </summary>
        private void CheckForCompletion()
        {
            if (_justDroppedTarget && _justTriggered)
            {
                AreWeThereYet();
                _justDroppedTarget = false;
                _justTriggered = false;
            }
        }

        /// <summary>
        /// Public method that can be called externally to indicate that one of the targets has been dropped somewhere.
        /// </summary>
        public void TargetDropped()
        {
            _justDroppedTarget = true;
            CheckForCompletion();
        }

        /// <summary>
        /// If a new target entered the trigger zone and one of our desired targets recently has been dropped, 
        /// the AreWeThereYet method is called to check if all desired targets are in the destination zone.
        /// </summary>
        private void AreWeThereYet()
        {
            bool areAllObjsInZone = true;

            // Check that each requested target is within the trigger zone.
            // Note, this does not check whether additional items have been placed in our trigger zone.
            for (int i = 0; i < _objsToPlaceHere.Length; i++)
            {
                if (_objsToPlaceHere[i] != null && !_currCollidingObjs.Contains(_objsToPlaceHere[i].name))
                {
                    areAllObjsInZone = false;
                    break;
                }
            }

            // Only change color if the status has changed 
            if (areAllObjsInZone && !_prevAllObjsInZone)
            {
                EyeTrackingDemoUtils.GameObject_ChangeColor(gameObject, _statusColorAchieved, ref _originalColor, false);
                _audioSource.PlayOneShot(_audioFxSuccess);
                
            }
            else if (!areAllObjsInZone && _prevAllObjsInZone)
            {
                EyeTrackingDemoUtils.GameObject_ChangeColor(gameObject, _statusColorIdle, ref _originalColor, false);
            }

            _prevAllObjsInZone = areAllObjsInZone;
        }
    }
}

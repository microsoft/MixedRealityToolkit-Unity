// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    public class TriggerZonePlaceObjsWithin : MonoBehaviour
    {
        [Tooltip("Array of referenced game objects that are supposed to be placed within the collider of this target.).")]
        [SerializeField]
        private GameObject[] ObjsToPlaceHere = null;

        [Tooltip("Color of this object when 'idle' - waiting on the correct targets to be placed.")]
        [SerializeField]
        private Color statusColor_idle = Color.blue;

        [Tooltip("Color of this object once all requested targets have been placed within the trigger zone.")]
        [SerializeField]
        private Color statusColor_achieved = Color.green;

        [Tooltip("Optional audio clip to be played once all requested objects have been correctly placed.")]
        [SerializeField]
        private AudioClip AudioFx_Success = null;

        private List<string> currCollidingObjs; // List of game objects that are currently colliding with this target.
        private Color? originalColor = null; // Store the original color of the target in case we want to use it later.
        private bool prevAllObjsInZone = false; // Were all requested objects correctly placed in the last run already?
        private bool justDroppedTarget = false;
        private bool justTriggered = false;

        private void Start()
        {
            // Init our list of currently colliding game objects
            currCollidingObjs = new List<string>();

            // Set default color
            EyeTrackingDemoUtils.GameObject_ChangeColor(gameObject, statusColor_idle, ref originalColor, false);
        }

        #region Handle collision detection
        /// <summary>
        /// If a new collider enters our trigger zone, let's add it to the list of currently colliding targets.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!currCollidingObjs.Contains(other.gameObject.name))
            {
                justTriggered = true;
                currCollidingObjs.Add(other.gameObject.name);
                CheckForCompletion();
            }
        }

        /// <summary>
        /// If an already enrolled collider leaves our trigger zone, let's remove it from the list of currently colliding targets.
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            if (currCollidingObjs.Contains(other.gameObject.name))
            {
                justTriggered = true;
                currCollidingObjs.Remove(other.gameObject.name);
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
            if (justDroppedTarget && justTriggered)
            {
                AreWeThereYet();
                justDroppedTarget = false;
                justTriggered = false;
            }
        }

        /// <summary>
        /// Public method that can be called externally to indicate that one of the targets has been dropped somewhere.
        /// </summary>
        public void TargetDropped()
        {
            justDroppedTarget = true;
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
            for (int i = 0; i < ObjsToPlaceHere.Length; i++)
            {
                if ((ObjsToPlaceHere[i] != null) && (!currCollidingObjs.Contains(ObjsToPlaceHere[i].name)))
                {
                    areAllObjsInZone = false;
                    break;
                }
            }

            // Only change color if the status has changed 
            if (areAllObjsInZone && !prevAllObjsInZone)
            {
                EyeTrackingDemoUtils.GameObject_ChangeColor(gameObject, statusColor_achieved, ref originalColor, false);
                AudioFeedbackPlayer.Instance.PlaySound(AudioFx_Success);
            }
            else if (!areAllObjsInZone && prevAllObjsInZone)
            {
                EyeTrackingDemoUtils.GameObject_ChangeColor(gameObject, statusColor_idle, ref originalColor, false);
            }

            prevAllObjsInZone = areAllObjsInZone;
        }
    }
}
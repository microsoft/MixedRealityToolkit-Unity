// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

#if UNITY_2017_2_OR_NEWER
using System.Collections;
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Use this script on GameObjects you wish to be aligned in certain ways depending on the application space type.
    /// For example, if you want to place an object at the height of the user's head in a room scale application, check alignWithHeadHeight.
    /// In a stationary scale application, this is equivalent to placing the object at a height of 0.
    /// You can also specify specific locations to place the object based on space type.
    /// 
    /// This script runs once, on GameObject creation.
    /// </summary>
    public class MixedRealitySceneContent : MonoBehaviour
    {
        public enum AlignmentType
        {
            AlignWithExperienceScale,
            AlignWithHeadHeight
        }

        [SerializeField]
        [Tooltip("Select this if the container should be placed in front of the head on app launch in a room scale app.")]
        public AlignmentType alignmentType = AlignmentType.AlignWithExperienceScale;

        private Vector3 contentPosition = Vector3.zero;

        [Tooltip("Optional container object reference. If null, this script will move the object it's attached to.")]
        private Transform containerObject = null;

        private void Awake()
        {
            if (containerObject == null)
            {
                containerObject = transform;
            }

            // Init the content height on non-XR platforms
            StartCoroutine(InitializeSceneContentWithDelay());
        }

        // Not waiting a frame often caused the camera's position to be incorrect at this point. This seems like a Unity bug.
        private IEnumerator InitializeSceneContentWithDelay()
        {
            yield return null;
            InitializeSceneContent();
        }


        // <summary>
        // bool used to track whether this content has been initialized yet.
        // </summary>
        private bool contentInitialized = false;

        /// <summary>
        /// Initializes the scene content based on the experience settings
        /// Should only be called once.
        /// </summary>
        public void InitializeSceneContent()
        {
            if (contentInitialized || containerObject == null)
            {
                return;
            }

            if (alignmentType == AlignmentType.AlignWithExperienceScale)
            {
                bool experienceAdjustedByXRDevice =
#if UNITY_2019_3_OR_NEWER
                    XRSubsystemHelpers.InputSubsystem != null && !XRSubsystemHelpers.InputSubsystem.GetTrackingOriginMode().HasFlag(TrackingOriginModeFlags.Unknown);
#else
                    XRDevice.isPresent && XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale;
#endif // UNITY_2019_3_OR_NEWER

                // The scene content will be adjusted upwards if the target experience scale is set to room or world scale
                // AND if we are either in editor (!XRDevicePresent) or we are on an XR device that will adjust the camera's height
                if ((MixedRealityToolkit.Instance.ActiveProfile.ExperienceSettingsProfile.TargetExperienceScale == ExperienceScale.Room ||
                    MixedRealityToolkit.Instance.ActiveProfile.ExperienceSettingsProfile.TargetExperienceScale == ExperienceScale.World) &&
                    (!DeviceUtility.IsPresent || experienceAdjustedByXRDevice))
                {
                    contentPosition.x = containerObject.position.x;
                    contentPosition.y = containerObject.position.y + MixedRealityToolkit.Instance.ActiveProfile.ExperienceSettingsProfile.ContentOffset;
                    contentPosition.z = containerObject.position.z;

                    containerObject.position = contentPosition;
                }
                else
                {
                    contentPosition = Vector3.zero;
                    containerObject.position = contentPosition;
                }
            }

            if (alignmentType == AlignmentType.AlignWithHeadHeight)
            {
                contentPosition.x = containerObject.position.x;
                contentPosition.y = containerObject.position.y + CameraCache.Main.transform.position.y;
                contentPosition.z = containerObject.position.z;

                containerObject.position = contentPosition;
            }

            contentInitialized = true;
        }
    }
}

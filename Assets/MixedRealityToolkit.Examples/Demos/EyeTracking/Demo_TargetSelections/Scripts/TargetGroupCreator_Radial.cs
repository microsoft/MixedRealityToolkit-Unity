// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Handles the creation of a group of targets based on a list of given templates.
    /// </summary>
    public class TargetGroupCreator_Radial : MonoBehaviour
    {
        #region Variables
        [Tooltip("Target templates from which the group of targets will be created.")]
        [SerializeField]
        private GameObject[] templates = null;

        [Tooltip("The size of targets in visual angle.")]
        [SerializeField]
        private Vector3 targetSizeInVisAngle = new Vector3(1f, 1f, 0.01f);

        [Tooltip("If true, the target sizes will be continuously adjusted to keep the size in visual angle constant.")]
        [SerializeField]
        private bool keepVisAngleSizeConstant = true;

        [Tooltip("If true, the targets will keep facing the user.")]
        [SerializeField]
        private bool keepTargetsFacingTheCam = true;

        [Tooltip("If true, the templates will be hidden on startup.")]
        [SerializeField]
        private bool hideTemplatesOnStartup = true;

        [Tooltip("Number of targets per ring.")]
        [SerializeField]
        private int radialLayout_nTargets = 8;

        [Tooltip("An array of radii for the concentric rings.")]
        [SerializeField]
        private float[] radialLayout_radiusInVisAngle = new float[] { 2, 3 };

        [Tooltip("If true, show a target also at the center of the rings.")]
        [SerializeField]
        private bool showTargetAtGroupCenter = false;

        private Vector3 targetSizeInMeters;
        private List<GameObject> instantiatedTargets;
        #endregion

        private void Start()
        {
            // Hide the template game objects
            if (hideTemplatesOnStartup)
                HideTemplates();

            // Instantiate targets in a radial layout
            CreateNewTargets_RadialLayout();
        }

        private void Update()
        {
            if (keepVisAngleSizeConstant)
                KeepConstantVisAngleTargetSize();

            if (keepTargetsFacingTheCam)
                KeepFacingTheCamera();
        }

        /// <summary>
        /// Hide the provided templates on start up.
        /// </summary>
        private void HideTemplates()
        {
            if (templates != null)
            {
                foreach (GameObject tobj in templates)
                {
                    tobj.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Creates a group of instantiated targets in a radial layout.
        /// </summary>
        private void CreateNewTargets_RadialLayout()
        {
            instantiatedTargets = new List<GameObject>();

            // Set target size
            float dist = Vector3.Distance(CameraCache.Main.transform.position, transform.position);
            targetSizeInMeters = EyeTrackingDemoUtils.VisAngleInDegreesToMeters(targetSizeInVisAngle, dist);

            // Let's store the local rotation and reset it for now to rotate it later with all children
            Vector3 rotate = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(Vector3.zero);

            // Let's make sure we have some templates to work with
            if ((templates != null) && (templates.Length > 0))
            {
                // Show a target at the center of the target group
                if (showTargetAtGroupCenter)
                    InstantiateRadialLayoutedTarget(0, dist, 0);

                // Instantiate and place the remaining targets
                // Create different number of rings based on the amount of given radii
                for (int ir = 0; ir < radialLayout_radiusInVisAngle.Length; ir++)
                {
                    // Per ring create a given number of targets
                    for (int it = 0; it < radialLayout_nTargets; it++)
                    {
                        InstantiateRadialLayoutedTarget(radialLayout_radiusInVisAngle[ir], dist, it);
                    }
                }

                // Rotate the target family
                transform.Rotate(rotate);
            }
        }

        /// <summary>
        /// List of all instantiated targets.
        /// </summary>
        internal GameObject[] InstantiatedObjects
        {
            get
            {
                if (instantiatedTargets == null)
                {
                    return null;
                }
                return instantiatedTargets.ToArray();
            }
        }

        /// <summary>
        /// Returns a random template.
        /// </summary>
        /// <returns></returns>
        private GameObject GetRandomTemplate()
        {
            int num = Random.Range(0, templates.Length);
            return templates[num];
        }

        /// <summary>
        /// Instantiates a new target and transforms it according to the given parameters.
        /// </summary>
        /// <param name="radius">Radius of the ring in which the target is placed in.</param>
        /// <param name="viewingDist">The distance between the target group and the camera.</param>
        /// <param name="iTarget">The index of the target within the current ring.</param>
        private void InstantiateRadialLayoutedTarget(float radius, float viewingDist, int iTarget)
        {
            GameObject _target = Instantiate(GetRandomTemplate());

            // Position
            float xnew = transform.position.x + EyeTrackingDemoUtils.VisAngleInDegreesToMeters(radius, viewingDist) * Mathf.Cos(Mathf.Deg2Rad * iTarget * 360 / radialLayout_nTargets);
            float ynew = transform.position.y + EyeTrackingDemoUtils.VisAngleInDegreesToMeters(radius, viewingDist) * Mathf.Sin(Mathf.Deg2Rad * iTarget * 360 / radialLayout_nTargets);
            _target.transform.localPosition = new Vector3(xnew, ynew, transform.position.z);

            // Scale
            float dist2 = Vector3.Distance(CameraCache.Main.transform.position, _target.transform.position);
            Vector3 tmpTargetSizeInMeters = EyeTrackingDemoUtils.VisAngleInDegreesToMeters(targetSizeInVisAngle, dist2);
            _target.transform.localScale = tmpTargetSizeInMeters;

            // Name it
            _target.name = string.Format("target_r{0}_t{1}", radius, iTarget);

            // Assign parent
            _target.transform.SetParent(transform);

            _target.gameObject.SetActive(true);

            // Add it to our list of instantiated targets
            instantiatedTargets.Add(_target);
        }

        /// <summary>
        /// Keeps the target size in visual angle constant.
        /// </summary>
        private void KeepConstantVisAngleTargetSize()
        {
            // Note: We could improve performance by checking the delta camera movement. If below thresh -> Don't update.
            foreach (GameObject gobj in instantiatedTargets)
            {
                if (gobj != null)
                {
                    float distObjToCam = Vector3.Distance(CameraCache.Main.transform.position, gobj.transform.position);
                    gobj.transform.localScale = EyeTrackingDemoUtils.VisAngleInDegreesToMeters(targetSizeInVisAngle, distObjToCam);
                }
            }
        }

        /// <summary>
        /// Keeps the entire target family facing the camera.
        /// </summary>
        private void KeepFacingTheCamera()
        {
            transform.LookAt(CameraCache.Main.transform.position);
        }
    }
}
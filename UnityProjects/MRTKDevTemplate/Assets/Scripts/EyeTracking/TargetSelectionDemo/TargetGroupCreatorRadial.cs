// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Handles the creation of a group of targets based on a list of given templates.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/TargetGroupCreatorRadial")]
    public class TargetGroupCreatorRadial : MonoBehaviour
    {
        #region Variables
        [Tooltip("Target templates from which the group of targets will be created.")]
        [SerializeField]
        private GameObject[] targetTemplates = null;

        [Tooltip("The size of targets in visual angle.")]
        [SerializeField]
        private Vector3 targetSizeInVisualAngle = new Vector3(1f, 1f, 0.01f);

        [Tooltip("If true, the target sizes will be continuously adjusted to keep the size in visual angle constant.")]
        [SerializeField]
        private bool keepVisualAngleSizeConstant = true;

        [Tooltip("If true, the targets will keep facing the user.")]
        [SerializeField]
        private bool keepTargetsFacingTheCamera = true;

        [Tooltip("If true, the templates will be hidden on startup.")]
        [SerializeField]
        private bool hideTemplatesOnStartup = true;

        [Tooltip("Number of targets per ring.")]
        [SerializeField]
        private int radialLayoutNumTargets = 8;

        [Tooltip("An array of radii for the concentric rings.")]
        [SerializeField]
        private float[] radialLayoutRadiusInVisualAngle = { 2f, 3f };

        [Tooltip("If true, show a target also at the center of the rings.")]
        [SerializeField]
        private bool showTargetAtGroupCenter = false;

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
            if (keepVisualAngleSizeConstant)
                KeepConstantVisAngleTargetSize();

            if (keepTargetsFacingTheCamera)
                KeepFacingTheCamera();
        }

        /// <summary>
        /// Hide the provided templates on start up.
        /// </summary>
        private void HideTemplates()
        {
            if (targetTemplates != null)
            {
                foreach (GameObject template in targetTemplates)
                {
                    template.SetActive(false);
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
            float dist = Vector3.Distance(Camera.main.transform.position, transform.position);

            // Let's store the local rotation and reset it for now to rotate it later with all children
            Vector3 rotate = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(Vector3.zero);

            // Let's make sure we have some templates to work with
            if (targetTemplates?.Length > 0)
            {
                // Show a target at the center of the target group
                if (showTargetAtGroupCenter)
                    InstantiateRadialLayoutedTarget(0f, dist, 0);

                // Instantiate and place the remaining targets
                // Create different number of rings based on the amount of given radii
                for (int ir = 0; ir < radialLayoutRadiusInVisualAngle.Length; ir++)
                {
                    // Per ring create a given number of targets
                    for (int it = 0; it < radialLayoutNumTargets; it++)
                    {
                        InstantiateRadialLayoutedTarget(radialLayoutRadiusInVisualAngle[ir], dist, it);
                    }
                }

                // Rotate the target family
                transform.Rotate(rotate);
            }
        }

        /// <summary>
        /// Returns a random template.
        /// </summary>
        private GameObject GetRandomTemplate()
        {
            int num = Random.Range(0, targetTemplates.Length);
            return targetTemplates[num];
        }

        /// <summary>
        /// Instantiates a new target and transforms it according to the given parameters.
        /// </summary>
        /// <param name="radius">Radius of the ring in which the target is placed in.</param>
        /// <param name="viewingDist">The distance between the target group and the camera.</param>
        /// <param name="iTarget">The index of the target within the current ring.</param>
        private void InstantiateRadialLayoutedTarget(float radius, float viewingDist, int iTarget)
        {
            GameObject target = Instantiate(GetRandomTemplate());

            // Position
            float xPosition = transform.position.x + EyeTrackingUtilities.VisAngleInDegreesToMeters(radius, viewingDist) * Mathf.Cos(iTarget * 360 * Mathf.Deg2Rad / radialLayoutNumTargets);
            float yPosition = transform.position.y + EyeTrackingUtilities.VisAngleInDegreesToMeters(radius, viewingDist) * Mathf.Sin(iTarget * 360 * Mathf.Deg2Rad / radialLayoutNumTargets);
            target.transform.localPosition = new Vector3(xPosition, yPosition, transform.position.z);

            // Scale
            float dist2 = Vector3.Distance(Camera.main.transform.position, target.transform.position);
            Vector3 tmpTargetSizeInMeters = EyeTrackingUtilities.VisAngleInDegreesToMeters(targetSizeInVisualAngle, dist2);
            target.transform.localScale = tmpTargetSizeInMeters;

            // Name it
            target.name = $"target_r{radius}_t{iTarget}";

            // Assign parent
            target.transform.SetParent(transform);

            target.SetActive(true);

            // Add it to our list of instantiated targets
            instantiatedTargets.Add(target);
        }

        /// <summary>
        /// Keeps the target size in visual angle constant.
        /// </summary>
        private void KeepConstantVisAngleTargetSize()
        {
            // Note: We could improve performance by checking the delta camera movement. If below thresh -> Don't update.
            foreach (GameObject target in instantiatedTargets)
            {
                if (target != null)
                {
                    float distObjToCam = Vector3.Distance(Camera.main.transform.position, target.transform.position);
                    target.transform.localScale = EyeTrackingUtilities.VisAngleInDegreesToMeters(targetSizeInVisualAngle, distObjToCam);
                }
            }
        }

        /// <summary>
        /// Keeps the entire target family facing the camera.
        /// </summary>
        private void KeepFacingTheCamera()
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}

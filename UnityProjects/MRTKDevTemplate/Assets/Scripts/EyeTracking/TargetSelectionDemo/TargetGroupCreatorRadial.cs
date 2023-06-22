// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
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
        private GameObject[] _targetTemplates = null;

        [Tooltip("The size of targets in visual angle.")]
        [SerializeField]
        private Vector3 _targetSizeInVisAngle = new Vector3(1f, 1f, 0.01f);

        [Tooltip("If true, the target sizes will be continuously adjusted to keep the size in visual angle constant.")]
        [SerializeField]
        private bool _keepVisAngleSizeConstant = true;

        [Tooltip("If true, the targets will keep facing the user.")]
        [SerializeField]
        private bool _keepTargetsFacingTheCam = true;

        [Tooltip("If true, the templates will be hidden on startup.")]
        [SerializeField]
        private bool _hideTemplatesOnStartup = true;

        [Tooltip("Number of targets per ring.")]
        [SerializeField]
        private int _radialLayoutNumTargets = 8;

        [Tooltip("An array of radii for the concentric rings.")]
        [SerializeField]
        private float[] _radialLayoutRadiusInVisAngle = new float[] { 2, 3 };

        [Tooltip("If true, show a target also at the center of the rings.")]
        [SerializeField]
        private bool _showTargetAtGroupCenter = false;

        private List<GameObject> _instantiatedTargets;
        #endregion

        private void Start()
        {
            // Hide the template game objects
            if (_hideTemplatesOnStartup)
                HideTemplates();

            // Instantiate targets in a radial layout
            CreateNewTargets_RadialLayout();
        }

        private void Update()
        {
            if (_keepVisAngleSizeConstant)
                KeepConstantVisAngleTargetSize();

            if (_keepTargetsFacingTheCam)
                KeepFacingTheCamera();
        }

        /// <summary>
        /// Hide the provided templates on start up.
        /// </summary>
        private void HideTemplates()
        {
            if (_targetTemplates != null)
            {
                foreach (GameObject tobj in _targetTemplates)
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
            _instantiatedTargets = new List<GameObject>();

            // Set target size
            float dist = Vector3.Distance(Camera.main.transform.position, transform.position);

            // Let's store the local rotation and reset it for now to rotate it later with all children
            Vector3 rotate = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(Vector3.zero);

            // Let's make sure we have some templates to work with
            if (_targetTemplates?.Length > 0)
            {
                // Show a target at the center of the target group
                if (_showTargetAtGroupCenter)
                    InstantiateRadialLayoutedTarget(0, dist, 0);

                // Instantiate and place the remaining targets
                // Create different number of rings based on the amount of given radii
                for (int ir = 0; ir < _radialLayoutRadiusInVisAngle.Length; ir++)
                {
                    // Per ring create a given number of targets
                    for (int it = 0; it < _radialLayoutNumTargets; it++)
                    {
                        InstantiateRadialLayoutedTarget(_radialLayoutRadiusInVisAngle[ir], dist, it);
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
                return _instantiatedTargets?.ToArray();
            }
        }

        /// <summary>
        /// Returns a random template.
        /// </summary>
        private GameObject GetRandomTemplate()
        {
            int num = Random.Range(0, _targetTemplates.Length);
            return _targetTemplates[num];
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
            float xnew = transform.position.x + EyeTrackingDemoUtils.VisAngleInDegreesToMeters(radius, viewingDist) * Mathf.Cos(iTarget * 360 * Mathf.Deg2Rad / _radialLayoutNumTargets);
            float ynew = transform.position.y + EyeTrackingDemoUtils.VisAngleInDegreesToMeters(radius, viewingDist) * Mathf.Sin(iTarget * 360 * Mathf.Deg2Rad / _radialLayoutNumTargets);
            target.transform.localPosition = new Vector3(xnew, ynew, transform.position.z);

            // Scale
            float dist2 = Vector3.Distance(Camera.main.transform.position, target.transform.position);
            Vector3 tmpTargetSizeInMeters = EyeTrackingDemoUtils.VisAngleInDegreesToMeters(_targetSizeInVisAngle, dist2);
            target.transform.localScale = tmpTargetSizeInMeters;

            // Name it
            target.name = $"target_r{radius}_t{iTarget}";

            // Assign parent
            target.transform.SetParent(transform);

            target.SetActive(true);

            // Add it to our list of instantiated targets
            _instantiatedTargets.Add(target);
        }

        /// <summary>
        /// Keeps the target size in visual angle constant.
        /// </summary>
        private void KeepConstantVisAngleTargetSize()
        {
            // Note: We could improve performance by checking the delta camera movement. If below thresh -> Don't update.
            foreach (GameObject gobj in _instantiatedTargets)
            {
                if (gobj != null)
                {
                    float distObjToCam = Vector3.Distance(Camera.main.transform.position, gobj.transform.position);
                    gobj.transform.localScale = EyeTrackingDemoUtils.VisAngleInDegreesToMeters(_targetSizeInVisAngle, distObjToCam);
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

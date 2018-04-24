// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System;
using MixedRealityToolkit.UX.AppBarControl;
using UnityEngine;

namespace MixedRealityToolkit.UX.BoundingBoxes
{
    public class BoundingBoxRig : MonoBehaviour
    {
        /// <summary>
        /// Fired when the rig is activated.
        /// </summary>
        public event Action Activated;

        /// <summary>
        /// Fired when the rig is deactivated.
        /// </summary>
        public event Action Deactivated;

        [Header("Flattening")]
        [SerializeField]
        [Tooltip("Choose this option if Rig is to be applied to a 2D object.")]
        private BoundingBox.FlattenModeEnum flattenedAxis = BoundingBox.FlattenModeEnum.DoNotFlatten;

        [Header("Customization Settings")]
        [SerializeField]
        private Material scaleHandleMaterial = null;

        [SerializeField]
        private Material rotateHandleMaterial = null;

        [SerializeField]
        private Material interactingMaterial = null;

        [Header("Behavior")]
        [SerializeField]
        private float scaleRate = 1.0f;

        [SerializeField]
        [Tooltip("This is the maximum scale that one grab can accomplish.")]
        private float maxScale = 2.0f;

        [SerializeField]
        private BoundingBoxGizmoHandle.RotationType rotationType = BoundingBoxGizmoHandle.RotationType.objectCoordinates;

        [SerializeField]
        private BoundingBoxGizmoHandle.HandMotionType handMotionToRotate = BoundingBoxGizmoHandle.HandMotionType.handRotatesToRotateObject;

        [Header("Preset Components")]
        [SerializeField]
        [Tooltip("To visualize the object bounding box, drop the MixedRealityToolkit/UX/Prefabs/BoundingBoxes/BoundingBoxBasic.prefab here.")]
        private BoundingBox boundingBoxPrefab = null;

        [SerializeField]
        [Tooltip("AppBar prefab.")]
        private AppBar appBarPrefab = null;

        private BoundingBox boxInstance = null;

        private GameObject objectToBound = null;

        private AppBar appBarInstance = null;

        private GameObject[] rotateHandles = null;

        private GameObject[] cornerHandles = null;

        private List<Vector3> handleCentroids = null;

        private BoundingBoxGizmoHandle[] rigScaleGizmoHandles = null;

        private BoundingBoxGizmoHandle[] rigRotateGizmoHandles = null;

        private bool showRig = false;

        private readonly Vector3 scaleHandleSize = new Vector3(0.04f, 0.04f, 0.04f);
        private readonly Vector3 rotateHandleSize = new Vector3(0.04f, 0.04f, 0.04f);
        private readonly Vector3 upperLeftFrontVector = new Vector3(0.5f, 0.5f, 0.5f);
        private readonly Vector3 upperLeftBackVector = new Vector3(0.5f, 0.5f, -0.5f);
        private readonly Vector3 lowerLeftFrontVector = new Vector3(0.5f, -0.5f, 0.5f);
        private readonly Vector3 lowerLeftBackVector = new Vector3(0.5f, -0.5f, -0.5f);
        private readonly Vector3 upperRightFrontVector = new Vector3(-0.5f, 0.5f, 0.5f);
        private readonly Vector3 upperRightBackVector = new Vector3(-0.5f, 0.5f, -0.5f);
        private readonly Vector3 lowerRightFrontVector = new Vector3(-0.5f, -0.5f, 0.5f);
        private readonly Vector3 lowerRightBackVector = new Vector3(-0.5f, -0.5f, -0.5f);

        private bool destroying = false;

        public BoundingBox BoundingBoxPrefab
        {
            get
            {
                return boundingBoxPrefab;
            }

            set
            {
                boundingBoxPrefab = value;
            }
        }

        public Material ScaleHandleMaterial
        {
            get
            {
                return scaleHandleMaterial;
            }

            set
            {
                scaleHandleMaterial = value;
            }
        }

        public Material RotateHandleMaterial
        {
            get
            {
                return rotateHandleMaterial;
            }

            set
            {
                rotateHandleMaterial = value;
            }
        }

        public Material InteractingMaterial
        {
            get
            {
                return interactingMaterial;
            }

            set
            {
                interactingMaterial = value;
            }
        }

        private bool ShowRig
        {
            get
            {
                return showRig;
            }
            set
            {
                if (destroying) { return; }

                if (value)
                {
                    UpdateBoundsPoints();
                    UpdateHandles();
                }

                if (boxInstance != null)
                {
                    boxInstance.IsVisible = value;
                }

                if (cornerHandles != null && rotateHandles != null)
                {
                    for (var i = 0; i < cornerHandles.Length; i++)
                    {
                        cornerHandles[i].SetActive(value);
                    }

                    for (var i = 0; i < rotateHandles.Length; i++)
                    {
                        rotateHandles[i].SetActive(value);
                    }
                }

                showRig = value;
            }
        }

        public void Activate()
        {
            ShowRig = true;
        }

        public void Deactivate()
        {
            ShowRig = false;
        }

        public void FocusOnHandle(GameObject handle)
        {
            if (handle != null)
            {
                for (int i = 0; i < rotateHandles.Length; ++i)
                {
                    rotateHandles[i].SetActive(rotateHandles[i].gameObject == handle);
                }
                for (int i = 0; i < cornerHandles.Length; ++i)
                {
                    cornerHandles[i].SetActive(cornerHandles[i].gameObject == handle);
                }
            }
            else
            {
                for (int i = 0; i < rotateHandles.Length; ++i)
                {
                    rotateHandles[i].SetActive(true);
                }
                for (int i = 0; i < cornerHandles.Length; ++i)
                {
                    cornerHandles[i].SetActive(true);
                }
            }
        }

        private void Start()
        {
            objectToBound = gameObject;

            boxInstance = Instantiate(BoundingBoxPrefab);
            boxInstance.Target = objectToBound;
            boxInstance.FlattenPreference = flattenedAxis;

            BuildRig();

            appBarInstance = Instantiate(appBarPrefab);
            appBarInstance.BoundingBox = boxInstance;

            boxInstance.IsVisible = false;
        }

        private void Update()
        {
            if (destroying == false && ShowRig)
            {
                UpdateBoundsPoints();
                UpdateHandles();
            }
        }

        private void OnDestroy()
        {
            destroying = true;
            ShowRig = false;
            ClearHandles();
        }

        private void UpdateBoundsPoints()
        {
            handleCentroids = GetBounds();
        }

        private void UpdateCornerHandles()
        {
            if (handleCentroids != null)
            {
                GetBounds();
            }

            Debug.Assert(handleCentroids != null);

            if (cornerHandles == null)
            {
                cornerHandles = new GameObject[handleCentroids.Count];
                rigScaleGizmoHandles = new BoundingBoxGizmoHandle[handleCentroids.Count];
                for (int i = 0; i < cornerHandles.Length; ++i)
                {
                    cornerHandles[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cornerHandles[i].GetComponent<Renderer>().material = scaleHandleMaterial;
                    cornerHandles[i].transform.localScale = scaleHandleSize;
                    cornerHandles[i].name = "Corner " + i.ToString();
                    rigScaleGizmoHandles[i] = cornerHandles[i].AddComponent<BoundingBoxGizmoHandle>();
                    rigScaleGizmoHandles[i].Rig = this;
                    rigScaleGizmoHandles[i].ScaleRate = scaleRate;
                    rigScaleGizmoHandles[i].MaxScale = maxScale;
                    rigScaleGizmoHandles[i].TransformToAffect = objectToBound.transform;
                    rigScaleGizmoHandles[i].Axis = BoundingBoxGizmoHandle.AxisToAffect.Y;
                    rigScaleGizmoHandles[i].AffineType = BoundingBoxGizmoHandle.TransformType.Scale;
                }
            }

            for (int i = 0; i < cornerHandles.Length; ++i)
            {
                cornerHandles[i].transform.position = handleCentroids[i];
                cornerHandles[i].transform.localRotation = objectToBound.transform.rotation;
            }
        }

        private void UpdateRotateHandles()
        {
            if (handleCentroids != null)
            {
                GetBounds();
            }

            if (rotateHandles == null)
            {
                rotateHandles = new GameObject[12];
                rigRotateGizmoHandles = new BoundingBoxGizmoHandle[12];
                for (int i = 0; i < rotateHandles.Length; ++i)
                {
                    rotateHandles[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    rotateHandles[i].GetComponent<Renderer>().material = rotateHandleMaterial;
                    rotateHandles[i].transform.localScale = rotateHandleSize;
                    rotateHandles[i].name = "Middle " + i.ToString();
                    rigRotateGizmoHandles[i] = rotateHandles[i].AddComponent<BoundingBoxGizmoHandle>();
                    rigRotateGizmoHandles[i].Rig = this;
                    rigRotateGizmoHandles[i].HandMotionForRotation = handMotionToRotate;
                    rigRotateGizmoHandles[i].RotationCoordinateSystem = rotationType;
                    rigRotateGizmoHandles[i].TransformToAffect = objectToBound.transform;
                    rigRotateGizmoHandles[i].AffineType = BoundingBoxGizmoHandle.TransformType.Rotation;

                }

                //set axis to affect
                rigRotateGizmoHandles[0].Axis = BoundingBoxGizmoHandle.AxisToAffect.Y;
                rigRotateGizmoHandles[1].Axis = BoundingBoxGizmoHandle.AxisToAffect.Y;
                rigRotateGizmoHandles[2].Axis = BoundingBoxGizmoHandle.AxisToAffect.Y;
                rigRotateGizmoHandles[3].Axis = BoundingBoxGizmoHandle.AxisToAffect.Y;

                rigRotateGizmoHandles[4].Axis = BoundingBoxGizmoHandle.AxisToAffect.Z;
                rigRotateGizmoHandles[5].Axis = BoundingBoxGizmoHandle.AxisToAffect.Z;
                rigRotateGizmoHandles[6].Axis = BoundingBoxGizmoHandle.AxisToAffect.Z;
                rigRotateGizmoHandles[7].Axis = BoundingBoxGizmoHandle.AxisToAffect.Z;

                rigRotateGizmoHandles[8].Axis = BoundingBoxGizmoHandle.AxisToAffect.X;
                rigRotateGizmoHandles[9].Axis = BoundingBoxGizmoHandle.AxisToAffect.X;
                rigRotateGizmoHandles[10].Axis = BoundingBoxGizmoHandle.AxisToAffect.X;
                rigRotateGizmoHandles[11].Axis = BoundingBoxGizmoHandle.AxisToAffect.X;

                //set lefthandedness
                rigRotateGizmoHandles[0].IsLeftHandedRotation = false;
                rigRotateGizmoHandles[1].IsLeftHandedRotation = false;
                rigRotateGizmoHandles[2].IsLeftHandedRotation = false;
                rigRotateGizmoHandles[3].IsLeftHandedRotation = false;

                rigRotateGizmoHandles[4].IsLeftHandedRotation = false;
                rigRotateGizmoHandles[5].IsLeftHandedRotation = false;
                rigRotateGizmoHandles[6].IsLeftHandedRotation = true;
                rigRotateGizmoHandles[7].IsLeftHandedRotation = true;

                rigRotateGizmoHandles[8].IsLeftHandedRotation = false;
                rigRotateGizmoHandles[9].IsLeftHandedRotation = true;
                rigRotateGizmoHandles[10].IsLeftHandedRotation = false;
                rigRotateGizmoHandles[11].IsLeftHandedRotation = true;
            }

            Debug.Assert(handleCentroids != null);

            rotateHandles[0].transform.localPosition = (handleCentroids[2] + handleCentroids[0]) * 0.5f;
            rotateHandles[1].transform.localPosition = (handleCentroids[3] + handleCentroids[1]) * 0.5f;
            rotateHandles[2].transform.localPosition = (handleCentroids[6] + handleCentroids[4]) * 0.5f;
            rotateHandles[3].transform.localPosition = (handleCentroids[7] + handleCentroids[5]) * 0.5f;
            rotateHandles[4].transform.localPosition = (handleCentroids[0] + handleCentroids[1]) * 0.5f;
            rotateHandles[5].transform.localPosition = (handleCentroids[2] + handleCentroids[3]) * 0.5f;
            rotateHandles[6].transform.localPosition = (handleCentroids[4] + handleCentroids[5]) * 0.5f;
            rotateHandles[7].transform.localPosition = (handleCentroids[6] + handleCentroids[7]) * 0.5f;
            rotateHandles[8].transform.localPosition = (handleCentroids[0] + handleCentroids[4]) * 0.5f;
            rotateHandles[9].transform.localPosition = (handleCentroids[1] + handleCentroids[5]) * 0.5f;
            rotateHandles[10].transform.localPosition = (handleCentroids[2] + handleCentroids[6]) * 0.5f;
            rotateHandles[11].transform.localPosition = (handleCentroids[3] + handleCentroids[7]) * 0.5f;
        }

        private void UpdateHandles()
        {
            UpdateCornerHandles();
            UpdateRotateHandles();
        }

        private void ClearCornerHandles()
        {
            if (cornerHandles != null)
            {
                for (int i = 0; i < cornerHandles.Length; ++i)
                {
                    Destroy(cornerHandles[i]);
                }

                cornerHandles = null;
                handleCentroids = null;
            }

            cornerHandles = null;
            handleCentroids = null;
        }

        private void ClearRotateHandles()
        {
            if (rotateHandles != null && rotateHandles.Length > 0 && rotateHandles[0] != null)
            {
                for (int i = 0; i < rotateHandles.Length; ++i)
                {
                    if (rotateHandles[i] != null)
                    {
                        Destroy(rotateHandles[i]);
                        rotateHandles[i] = null;
                    }
                }
            }

            rotateHandles = null;
        }

        private void ClearHandles()
        {
            ClearCornerHandles();
            ClearRotateHandles();
        }

        private void BuildRig()
        {
            Vector3 scale = objectToBound.transform.localScale;

            var rig = new GameObject { name = "center" };
            rig.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            rig.transform.localScale = new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z);

            var upperLeftFront = new GameObject { name = "upperleftfront" };
            upperLeftFront.transform.SetPositionAndRotation(upperLeftFrontVector, Quaternion.identity);
            upperLeftFront.transform.localScale = Vector3.one;
            upperLeftFront.transform.parent = rig.transform;

            var upperLeftBack = new GameObject { name = "upperleftback" };
            upperLeftBack.transform.SetPositionAndRotation(upperLeftBackVector, Quaternion.identity);
            upperLeftBack.transform.localScale = Vector3.one;
            upperLeftBack.transform.parent = rig.transform;

            var lowerLeftFront = new GameObject { name = "lowerleftfront" };
            lowerLeftFront.transform.SetPositionAndRotation(lowerLeftFrontVector, Quaternion.identity);
            lowerLeftFront.transform.localScale = Vector3.one;
            lowerLeftFront.transform.parent = rig.transform;

            var lowerLeftBack = new GameObject { name = "lowerleftback" };
            lowerLeftBack.transform.SetPositionAndRotation(lowerLeftBackVector, Quaternion.identity);
            lowerLeftBack.transform.localScale = Vector3.one;
            lowerLeftBack.transform.parent = rig.transform;

            var upperRightFront = new GameObject { name = "upperrightfront" };
            upperRightFront.transform.SetPositionAndRotation(upperRightFrontVector, Quaternion.identity);
            upperRightFront.transform.localScale = Vector3.one;
            upperRightFront.transform.parent = rig.transform;

            var upperRightBack = new GameObject { name = "upperrightback" };
            upperRightBack.transform.SetPositionAndRotation(upperRightBackVector, Quaternion.identity);
            upperRightBack.transform.localScale = Vector3.one;
            upperRightBack.transform.parent = rig.transform;

            var lowerRightFront = new GameObject { name = "lowerrightfront" };
            lowerRightFront.transform.SetPositionAndRotation(lowerRightFrontVector, Quaternion.identity);
            lowerRightFront.transform.localScale = Vector3.one;
            lowerRightFront.transform.parent = rig.transform;

            var lowerRightBack = new GameObject { name = "lowerrightback" };
            lowerRightBack.transform.SetPositionAndRotation(lowerRightBackVector, Quaternion.identity);
            lowerRightBack.transform.localScale = Vector3.one;
            lowerRightBack.transform.parent = rig.transform;
        }

        private List<Vector3> GetBounds()
        {
            if (objectToBound != null)
            {
                var bounds = new List<Vector3>();
                var mask = new LayerMask();

                GameObject clone = GameObject.Instantiate(boxInstance.gameObject);
                clone.transform.localRotation = Quaternion.identity;
                clone.transform.position = Vector3.zero;
                BoundingBox.GetMeshFilterBoundsPoints(clone, bounds, mask);
                Destroy(clone);
                Matrix4x4 m = Matrix4x4.Rotate(objectToBound.transform.rotation);
                for (int i = 0; i < bounds.Count; ++i)
                {
                    bounds[i] = m.MultiplyPoint(bounds[i]);
                    bounds[i] += boxInstance.TargetBoundsCenter;
                }

                return bounds;
            }

            return null;
        }
    }
}
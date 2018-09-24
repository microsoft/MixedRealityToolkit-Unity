// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Constructs the scale and rotate gizmo handles for the Bounding Box 
    /// </summary>
    public class BoundingBoxRig : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Enter adjust mode and show handles in default.")]
        private bool activateInDefault = false;

        [Header("Flattening")]
        [SerializeField]
        [Tooltip("Choose this option if Rig is to be applied to a 2D object.")]
        private FlattenModeEnum flattenedAxis = default(FlattenModeEnum);

        [Header("Customization Settings")]
        [SerializeField]
        private Material scaleHandleMaterial;
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

        [SerializeField]
        private Material rotateHandleMaterial;
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

        [SerializeField]
        private Material interactingMaterial;
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

        [SerializeField]
        [Tooltip("This is the maximum scale that one grab can accomplish.")]
        private float maxScale = 2.0f;

        public int RecalculateCount = 0;

        [Header("Preset Components")]
        [SerializeField]
        [Tooltip("To visualize the object bounding box, drop the MixedRealityToolkit/UX/Prefabs/BoundingBoxes/BoundingBoxBasic.prefab here.")]
        private BoundingBox boundingBoxPrefab;

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

        private BoundingBox boxInstance;

        private GameObject objectToBound;

        private GameObject[] rotateHandles;

        private GameObject[] cornerHandles;

        private List<Vector3> handleCentroids;

        private GameObject transformRig;

        private BoundingBoxGizmoHandle[] rigScaleGizmoHandles;

        private BoundingBoxGizmoHandle[] rigRotateGizmoHandles;

        private bool showRig = false;

        private Vector3 scaleHandleSize = new Vector3(0.04f, 0.04f, 0.04f);

        private Vector3 rotateHandleSize = new Vector3(0.04f, 0.04f, 0.04f);

        private bool destroying = false;

        private void Start()
        {
            objectToBound = this.gameObject;

            boxInstance = Instantiate(BoundingBoxPrefab) as BoundingBox;
            boxInstance.Target = objectToBound;
            boxInstance.FlattenPreference = flattenedAxis;
            boxInstance.ManualUpdateActive = true;

            BuildRig();

            boxInstance.IsVisible = false;

            if(activateInDefault == true)
            {
                Activate();
            }
        }

        private void Update()
        {
            if (destroying == false && ShowRig)
            {
                if (RecalculateCount > 0)
                {
                    boxInstance.ManualUpdate();
                    UpdateBoundsPoints();
                    UpdateHandles();
                    RecalculateCount--;
                }
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

        private void CreateHandles()
        {
            ClearHandles();
            UpdateHandles();
            ParentHandles();
            UpdateHandles();
        }

        private void UpdateCornerHandles()
        {
            if (handleCentroids != null)
            {
                GetBounds();
            }

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
                    rigScaleGizmoHandles[i].MaxScale = maxScale;
                    rigScaleGizmoHandles[i].TransformToAffect = objectToBound.transform;
                    rigScaleGizmoHandles[i].Axis = BoundingBoxGizmoHandleAxisToAffect.Y;
                    rigScaleGizmoHandles[i].AffineType = BoundingBoxGizmoHandleTransformType.Scale;
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
                    rigRotateGizmoHandles[i].TransformToAffect = objectToBound.transform;
                    rigRotateGizmoHandles[i].AffineType = BoundingBoxGizmoHandleTransformType.Rotation;
                   
                }

                //set axis to affect
                rigRotateGizmoHandles[0].Axis = BoundingBoxGizmoHandleAxisToAffect.Y;
                rigRotateGizmoHandles[1].Axis = BoundingBoxGizmoHandleAxisToAffect.Y;
                rigRotateGizmoHandles[2].Axis = BoundingBoxGizmoHandleAxisToAffect.Y;
                rigRotateGizmoHandles[3].Axis = BoundingBoxGizmoHandleAxisToAffect.Y;

                rigRotateGizmoHandles[4].Axis = BoundingBoxGizmoHandleAxisToAffect.Z;
                rigRotateGizmoHandles[5].Axis = BoundingBoxGizmoHandleAxisToAffect.Z;
                rigRotateGizmoHandles[6].Axis = BoundingBoxGizmoHandleAxisToAffect.Z;
                rigRotateGizmoHandles[7].Axis = BoundingBoxGizmoHandleAxisToAffect.Z;

                rigRotateGizmoHandles[8].Axis  = BoundingBoxGizmoHandleAxisToAffect.X;
                rigRotateGizmoHandles[9].Axis  = BoundingBoxGizmoHandleAxisToAffect.X;
                rigRotateGizmoHandles[10].Axis = BoundingBoxGizmoHandleAxisToAffect.X;
                rigRotateGizmoHandles[11].Axis = BoundingBoxGizmoHandleAxisToAffect.X;
            }

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

        private void ParentHandles()
        {
            transformRig.transform.position = boxInstance.transform.position;
            transformRig.transform.rotation = boxInstance.transform.rotation;

            Vector3 invScale = objectToBound.transform.localScale;

            transformRig.transform.localScale = new Vector3(0.5f / invScale.x, 0.5f / invScale.y, 0.5f / invScale.z);
            transformRig.transform.parent = objectToBound.transform;
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
                    GameObject.Destroy(cornerHandles[i]);
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

        private GameObject BuildRig()
        {
            Vector3 scale = objectToBound.transform.localScale;

            GameObject rig = new GameObject();
            rig.name = "center";
            rig.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            rig.transform.localScale = new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z);

            GameObject upperLeftFront = new GameObject();
            upperLeftFront.name = "upperleftfront";
            upperLeftFront.transform.SetPositionAndRotation(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity);
            upperLeftFront.transform.localScale = new Vector3(1, 1, 1);
            upperLeftFront.transform.parent = rig.transform;

            GameObject upperLeftBack = new GameObject();
            upperLeftBack.name = "upperleftback";
            upperLeftBack.transform.SetPositionAndRotation(new Vector3(0.5f, 0.5f, -0.5f), Quaternion.identity);
            upperLeftBack.transform.localScale = new Vector3(1, 1, 1);
            upperLeftBack.transform.parent = rig.transform;

            GameObject lowerLeftFront = new GameObject();
            lowerLeftFront.name = "lowerleftfront";
            lowerLeftFront.transform.SetPositionAndRotation(new Vector3(0.5f, -0.5f, 0.5f), Quaternion.identity);
            lowerLeftFront.transform.localScale = new Vector3(1, 1, 1);
            lowerLeftFront.transform.parent = rig.transform;

            GameObject lowerLeftBack = new GameObject();
            lowerLeftBack.name = "lowerleftback";
            lowerLeftBack.transform.SetPositionAndRotation(new Vector3(0.5f, -0.5f, -0.5f), Quaternion.identity);
            lowerLeftBack.transform.localScale = new Vector3(1, 1, 1);
            lowerLeftBack.transform.parent = rig.transform;

            GameObject upperRightFront = new GameObject();
            upperRightFront.name = "upperrightfront";
            upperRightFront.transform.SetPositionAndRotation(new Vector3(-0.5f, 0.5f, 0.5f), Quaternion.identity);
            upperRightFront.transform.localScale = new Vector3(1, 1, 1);
            upperRightFront.transform.parent = rig.transform;

            GameObject upperRightBack = new GameObject();
            upperRightBack.name = "upperrightback";
            upperRightBack.transform.SetPositionAndRotation(new Vector3(-0.5f, 0.5f, -0.5f), Quaternion.identity);
            upperRightBack.transform.localScale = new Vector3(1, 1, 1);
            upperRightBack.transform.parent = rig.transform;

            GameObject lowerRightFront = new GameObject();
            lowerRightFront.name = "lowerrightfront";
            lowerRightFront.transform.SetPositionAndRotation(new Vector3(-0.5f, -0.5f, 0.5f), Quaternion.identity);
            lowerRightFront.transform.localScale = new Vector3(1, 1, 1);
            lowerRightFront.transform.parent = rig.transform;

            GameObject lowerRightBack = new GameObject();
            lowerRightBack.name = "lowerrightback";
            lowerRightBack.transform.SetPositionAndRotation(new Vector3(-0.5f, -0.5f, -0.5f), Quaternion.identity);
            lowerRightBack.transform.localScale = new Vector3(1, 1, 1);
            lowerRightBack.transform.parent = rig.transform;

            transformRig = rig;

            return rig;
        }

        private FlattenModeEnum GetBestAxisToFlatten()
        {
            int index = handleCentroids.Count - 8;
            float width = (handleCentroids[index + 0] - handleCentroids[index + 4]).magnitude;
            float height = (handleCentroids[index + 0] - handleCentroids[index + 2]).magnitude;
            float depth = (handleCentroids[index + 0] - handleCentroids[index + 1]).magnitude;

            if (width < height && width < depth)
            {
                return FlattenModeEnum.FlattenX;
            }
            else if (height < width && height < depth)
            {
                return FlattenModeEnum.FlattenY;
            }
            else if (depth < height && depth < width)
            {
                return FlattenModeEnum.FlattenZ;
            }

            return FlattenModeEnum.DoNotFlatten;
        }

        private bool ShowRig
        {
            get
            {
                return showRig;
            }
            set
            {
                if (destroying == false)
                {
                    if (value == true)
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
                        foreach (GameObject handle in cornerHandles)
                        {
                            handle.SetActive(value);
                        }
                        foreach (GameObject handle in rotateHandles)
                        {
                            handle.SetActive(value);
                        }
                    }

                    showRig = value;
                }
            }
        }

        #region Public Methods
        /// <summary>
        /// This function turns on BoundingBox Rig
        /// </summary>
        public void Activate()
        {
            ShowRig = true;
            boxInstance.ManualUpdate();
            RecalculateCount = 3;
        }

        /// <summary>
        /// This function turns off BoundingBox Rig
        /// </summary>
        public void Deactivate()
        {
            ShowRig = false;
            RecalculateCount = 0;
        }

        /// <summary>
        /// calculates the centroid of the rig corner points
        /// </summary>
        public Vector3 RigCentroid
        {
            get
            {
                if (handleCentroids.Count > 0)
                {
                    Vector3 centroid = Vector3.zero;
                    for (int i = 0; i < handleCentroids.Count; ++i)
                    {
                        centroid += handleCentroids[i];
                    }
                    centroid *= 1.0f / (float)handleCentroids.Count;
                    return centroid;
                }
                return Vector3.zero;
            }
        }

        /// <summary>
        /// This function tells the rig that a handle has focus
        /// </summary>
        /// <param name="handle"></param>
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

        //this function retrieves the boundingbox bounds as a set of 3D points
        public List<Vector3> GetBounds()
        {
            if (objectToBound != null)
            {
                List<Vector3> bounds = new List<Vector3>();
                LayerMask mask = new LayerMask();

                GameObject clone = GameObject.Instantiate(boxInstance.gameObject);
                clone.transform.localRotation = Quaternion.identity;
                clone.transform.position = Vector3.zero;
                BoundingBox.GetMeshFilterBoundsPoints(clone, bounds, mask);
                Vector3 centroid = boxInstance.TargetBoundsCenter;
                GameObject.Destroy(clone);
#if UNITY_2017_1_OR_NEWER
                Matrix4x4 m = Matrix4x4.Rotate(objectToBound.transform.rotation);
                for (int i = 0; i < bounds.Count; ++i)
                {
                    bounds[i] = m.MultiplyPoint(bounds[i]);
                    bounds[i] += boxInstance.TargetBoundsCenter;
                }
#endif // UNITY_2017_1_OR_NEWER
                return bounds;
            }

            return null;
        }

        /// <summary>
        /// This function gets the rig transform position
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRigPosition()
        {
            if (boxInstance != null)
            {
                return boxInstance.transform.position;
            }
            return Vector3.zero;
        }
        #endregion
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class BaseHandVisualizer : MonoBehaviour, IMixedRealityHandVisualizer, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler, IMixedRealityHandMeshHandler
    {
        public virtual Handedness Handedness { get; set; }

        public GameObject GameObjectProxy => gameObject;

        public IMixedRealityController Controller { get; set; }

        protected readonly Dictionary<TrackedHandJoint, Transform> joints = new Dictionary<TrackedHandJoint, Transform>();
        protected MeshFilter handMeshFilter;

        // This member stores the last set of hand mesh vertices, to avoid using
        // handMeshFilter.mesh.vertices, which does a copy of the vertices.
        private Vector3[] lastHandMeshVertices;

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        private void OnEnable()
        {
            InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
            InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
            InputSystem?.RegisterHandler<IMixedRealityHandMeshHandler>(this);
        }

        private void OnDisable()
        {
            InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
            InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
            InputSystem?.UnregisterHandler<IMixedRealityHandMeshHandler>(this);
        }

        private void OnDestroy()
        {
            foreach (var joint in joints)
            {
                Destroy(joint.Value.gameObject);
            }

            if (handMeshFilter != null)
            {
                Destroy(handMeshFilter.gameObject);
                handMeshFilter = null;
            }
        }

        public bool TryGetJointTransform(TrackedHandJoint joint, out Transform jointTransform)
        {
            if (joints == null)
            {
                jointTransform = null;
                return false;
            }

            if (joints.TryGetValue(joint, out jointTransform))
            {
                return true;
            }

            jointTransform = null;
            return false;
        }

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            if (Controller?.InputSource.SourceId == eventData.SourceId)
            {
                Destroy(gameObject);
            }
        }

        void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            if (eventData.InputSource.SourceId != Controller.InputSource.SourceId)
            {
                return;
            }
            Debug.Assert(eventData.Handedness == Controller.ControllerHandedness);

            MixedRealityHandTrackingProfile handTrackingProfile = InputSystem?.InputSystemProfile.HandTrackingProfile;
            if (handTrackingProfile != null && !handTrackingProfile.EnableHandJointVisualization)
            {
                // clear existing joint GameObjects / meshes
                foreach (var joint in joints)
                {
                    Destroy(joint.Value.gameObject);
                }

                joints.Clear();
                return;
            }

            foreach (TrackedHandJoint handJoint in eventData.InputData.Keys)
            {
                Transform jointTransform;
                if (joints.TryGetValue(handJoint, out jointTransform))
                {
                    jointTransform.position = eventData.InputData[handJoint].Position;
                    jointTransform.rotation = eventData.InputData[handJoint].Rotation;
                }
                else
                {
                    GameObject prefab;
                    if (handJoint == TrackedHandJoint.None)
                    {
                        // No visible mesh for the "None" joint
                        prefab = null;
                    }
                    else if (handJoint == TrackedHandJoint.Palm)
                    {
                        prefab = InputSystem.InputSystemProfile.HandTrackingProfile.PalmJointPrefab;
                    }
                    else if (handJoint == TrackedHandJoint.IndexTip)
                    {
                        prefab = InputSystem.InputSystemProfile.HandTrackingProfile.FingerTipPrefab;
                    }
                    else
                    {
                        prefab = InputSystem.InputSystemProfile.HandTrackingProfile.JointPrefab;
                    }

                    GameObject jointObject;
                    if (prefab != null)
                    {
                        jointObject = Instantiate(prefab);
                    }
                    else
                    {
                        jointObject = new GameObject();
                    }

                    jointObject.name = handJoint.ToString() + " Proxy Transform";
                    jointObject.transform.position = eventData.InputData[handJoint].Position;
                    jointObject.transform.rotation = eventData.InputData[handJoint].Rotation;
                    jointObject.transform.parent = transform;

                    joints.Add(handJoint, jointObject.transform);
                }
            }
        }

        public void OnHandMeshUpdated(InputEventData<HandMeshInfo> eventData)
        {
            if (eventData.Handedness != Controller?.ControllerHandedness)
            {
                return;
            }

            if (handMeshFilter == null &&
                InputSystem?.InputSystemProfile?.HandTrackingProfile?.HandMeshPrefab != null)
            {
                handMeshFilter = Instantiate(InputSystem.InputSystemProfile.HandTrackingProfile.HandMeshPrefab).GetComponent<MeshFilter>();
                lastHandMeshVertices = handMeshFilter.mesh.vertices;
            }

            if (handMeshFilter != null)
            {
                Mesh mesh = handMeshFilter.mesh;

                bool meshChanged = false;
                // On some platforms, mesh length counts may change as the hand mesh is updated.
                // In order to update the vertices when the array sizes change, the mesh
                // must be cleared per instructions here:
                // https://docs.unity3d.com/ScriptReference/Mesh.html
                if (lastHandMeshVertices != null &&
                    lastHandMeshVertices.Length != 0 &&
                    lastHandMeshVertices.Length != eventData.InputData.vertices?.Length)
                {
                    meshChanged = true;
                    mesh.Clear();
                }

                mesh.vertices = eventData.InputData.vertices;
                mesh.normals = eventData.InputData.normals;
                mesh.triangles = eventData.InputData.triangles;
                lastHandMeshVertices = eventData.InputData.vertices;

                if (eventData.InputData.uvs != null && eventData.InputData.uvs.Length > 0)
                {
                    mesh.uv = eventData.InputData.uvs;
                }

                if (meshChanged)
                {
                    mesh.RecalculateBounds();
                }

                handMeshFilter.transform.position = eventData.InputData.position;
                handMeshFilter.transform.rotation = eventData.InputData.rotation;
            }
        }
    }
}
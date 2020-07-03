// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [AddComponentMenu("Scripts/MRTK/Core/BaseHandVisualizer")]
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

        private void OnEnable()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandMeshHandler>(this);
        }

        private void OnDisable()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandMeshHandler>(this);
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
            var inputSystem = CoreServices.InputSystem;

            if (eventData.InputSource.SourceId != Controller.InputSource.SourceId)
            {
                return;
            }
            Debug.Assert(eventData.Handedness == Controller.ControllerHandedness);

            MixedRealityHandTrackingProfile handTrackingProfile = inputSystem?.InputSystemProfile.HandTrackingProfile;
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
                        prefab = inputSystem.InputSystemProfile.HandTrackingProfile.PalmJointPrefab;
                    }
                    else if (handJoint == TrackedHandJoint.IndexTip)
                    {
                        prefab = inputSystem.InputSystemProfile.HandTrackingProfile.FingerTipPrefab;
                    }
                    else
                    {
                        prefab = inputSystem.InputSystemProfile.HandTrackingProfile.JointPrefab;
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

            bool newMesh = handMeshFilter == null;

            if (newMesh &&
                CoreServices.InputSystem?.InputSystemProfile != null &&
                CoreServices.InputSystem.InputSystemProfile.HandTrackingProfile != null &&
                CoreServices.InputSystem.InputSystemProfile.HandTrackingProfile.HandMeshPrefab != null)
            {
                handMeshFilter = Instantiate(CoreServices.InputSystem.InputSystemProfile.HandTrackingProfile.HandMeshPrefab).GetComponent<MeshFilter>();
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
                if ((lastHandMeshVertices == null && eventData.InputData.vertices != null) ||
                    (lastHandMeshVertices != null &&
                    lastHandMeshVertices.Length != 0 &&
                    lastHandMeshVertices.Length != eventData.InputData.vertices?.Length))
                {
                    meshChanged = true;
                    mesh.Clear();
                }

                mesh.vertices = eventData.InputData.vertices;
                mesh.normals = eventData.InputData.normals;
                lastHandMeshVertices = eventData.InputData.vertices;

                if (newMesh || meshChanged)
                {
                    mesh.triangles = eventData.InputData.triangles;

                    if (eventData.InputData.uvs?.Length > 0)
                    {
                        mesh.uv = eventData.InputData.uvs;
                    }
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
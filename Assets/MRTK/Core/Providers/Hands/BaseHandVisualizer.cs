// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
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

        // This member stores the last count of hand mesh vertices, to avoid using
        // handMeshFilter.mesh.vertices, which does a copy of the vertices.
        private int lastHandMeshVerticesCount = 0;

        private void OnEnable()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandMeshHandler>(this);
        }

        private void Start()
        {
            if (Controller != null)
            {
                Handedness = Controller.ControllerHandedness;
            }
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

        private static readonly ProfilerMarker OnHandJointsUpdatedPerfMarker = new ProfilerMarker("[MRTK] BaseHandVisualizer.OnHandJointsUpdated");

        void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            using (OnHandJointsUpdatedPerfMarker.Auto())
            {
                if (eventData.InputSource.SourceId != Controller.InputSource.SourceId)
                {
                    return;
                }
                Debug.Assert(eventData.Handedness == Controller.ControllerHandedness);

                IMixedRealityInputSystem inputSystem = CoreServices.InputSystem;
                MixedRealityHandTrackingProfile handTrackingProfile = inputSystem?.InputSystemProfile != null ? inputSystem.InputSystemProfile.HandTrackingProfile : null;
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
                        if (handJoint == TrackedHandJoint.None || handTrackingProfile == null)
                        {
                            // No visible mesh for the "None" joint
                            prefab = null;
                        }
                        else if (handJoint == TrackedHandJoint.Palm)
                        {
                            prefab = handTrackingProfile.PalmJointPrefab;
                        }
                        else if (handJoint == TrackedHandJoint.IndexTip)
                        {
                            prefab = handTrackingProfile.FingerTipPrefab;
                        }
                        else
                        {
                            prefab = handTrackingProfile.JointPrefab;
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
        }

        private static readonly ProfilerMarker OnHandMeshUpdatedPerfMarker = new ProfilerMarker("[MRTK] BaseHandVisualizer.OnHandMeshUpdated");

        public void OnHandMeshUpdated(InputEventData<HandMeshInfo> eventData)
        {
            using (OnHandMeshUpdatedPerfMarker.Auto())
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
                    lastHandMeshVerticesCount = handMeshFilter.mesh.vertices.Length;
                }

                if (handMeshFilter != null)
                {
                    Mesh mesh = handMeshFilter.mesh;

                    bool meshChanged = false;
                    // On some platforms, mesh length counts may change as the hand mesh is updated.
                    // In order to update the vertices when the array sizes change, the mesh
                    // must be cleared per instructions here:
                    // https://docs.unity3d.com/ScriptReference/Mesh.html
                    if (lastHandMeshVerticesCount != eventData.InputData.vertices?.Length)
                    {
                        meshChanged = true;
                        mesh.Clear();
                    }

                    mesh.vertices = eventData.InputData.vertices;
                    mesh.normals = eventData.InputData.normals;
                    lastHandMeshVerticesCount = eventData.InputData.vertices != null ? eventData.InputData.vertices.Length : 0;

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
}

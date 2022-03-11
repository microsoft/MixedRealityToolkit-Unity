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

        [System.Obsolete("This has been replaced with jointsArray with TrackedHandJoint as the int index.", true)]
        protected readonly Dictionary<TrackedHandJoint, Transform> joints = new Dictionary<TrackedHandJoint, Transform>();

        protected readonly Transform[] jointsArray = new Transform[ArticulatedHandPose.JointCount];
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
            foreach (Transform joint in jointsArray)
            {
                if (joint != null)
                {
                    Destroy(joint.gameObject);
                }
            }

            if (handMeshFilter != null)
            {
                Destroy(handMeshFilter.gameObject);
                handMeshFilter = null;
            }
        }

        public bool TryGetJointTransform(TrackedHandJoint joint, out Transform jointTransform)
        {
            if (jointsArray == null)
            {
                jointTransform = null;
                return false;
            }

            jointTransform = jointsArray[(int)joint];
            return jointTransform != null;
        }

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            // We must check if either this or gameObject equate to null because this callback may be triggered after
            // the object has been destroyed. Although event handlers are unregistered in OnDisable(), this may in fact
            // be postponed (see BaseEventSystem.UnregisterHandler()).
            if (this.IsNotNull() && gameObject != null && Controller?.InputSource.SourceId == eventData.SourceId)
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
                    foreach (Transform joint in jointsArray)
                    {
                        if (joint != null)
                        {
                            Destroy(joint.gameObject);
                        }
                    }

                    return;
                }

                // This starts at 1 to skip over TrackedHandJoint.None.
                for (int i = 1; i < ArticulatedHandPose.JointCount; i++)
                {
                    TrackedHandJoint handJoint = (TrackedHandJoint)i;
                    MixedRealityPose handJointPose = eventData.InputData[handJoint];
                    Transform jointTransform = jointsArray[i];

                    if (jointTransform != null)
                    {
                        jointTransform.SetPositionAndRotation(handJointPose.Position, handJointPose.Rotation);
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
                        jointObject.transform.SetPositionAndRotation(handJointPose.Position, handJointPose.Rotation);
                        jointObject.transform.parent = transform;

                        jointsArray[i] = jointObject.transform;
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

                    handMeshFilter.transform.SetPositionAndRotation(eventData.InputData.position, eventData.InputData.rotation);
                }
            }
        }
    }
}

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

        protected IMixedRealityHand MixedRealityHand { get; private set; } = null;
        protected Transform[] JointsArray { get; private set; } = new Transform[ArticulatedHandPose.JointCount];
        protected MeshFilter handMeshFilter;

        // This member stores the last count of hand mesh vertices, to avoid using
        // handMeshFilter.mesh.vertices, which does a copy of the vertices.
        private int lastHandMeshVerticesCount = 0;

        private bool handJointsUpdated = false;
        private HandMeshInfo lastHandMeshInfo = null;

        private void OnEnable()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandMeshHandler>(this);
        }

        protected virtual void Start()
        {
            if (Controller != null)
            {
                Handedness = Controller.ControllerHandedness;
                MixedRealityHand = Controller as IMixedRealityHand;
            }
        }

        protected virtual void Update()
        {
            UpdateHandJoints();
            UpdateHandMesh();
        }

        private void OnDisable()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandMeshHandler>(this);
        }

        private void OnDestroy()
        {
            foreach (Transform joint in JointsArray)
            {
                if (joint != null)
                {
                    Destroy(joint.gameObject);
                }

                JointsArray = System.Array.Empty<Transform>();
            }

            if (handMeshFilter != null)
            {
                Destroy(handMeshFilter.gameObject);
                handMeshFilter = null;
            }
        }

        [System.Obsolete("Use HandJointUtils.TryGetJointPose instead of this")]
        public bool TryGetJointTransform(TrackedHandJoint joint, out Transform jointTransform)
        {
            if (JointsArray == null)
            {
                jointTransform = null;
                return false;
            }

            jointTransform = JointsArray[(int)joint];
            return jointTransform != null;
        }

        /// <inheritdoc/>
        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            using (OnHandJointsUpdatedPerfMarker.Auto())
            {
                if (eventData.InputSource.SourceId != Controller.InputSource.SourceId
                    || eventData.Handedness != Controller.ControllerHandedness)
                {
                    return;
                }

                handJointsUpdated = true;
                eventData.Use();
            }
        }

        private static readonly ProfilerMarker OnHandMeshUpdatedPerfMarker = new ProfilerMarker("[MRTK] BaseHandVisualizer.OnHandMeshUpdated");

        /// <inheritdoc/>
        void IMixedRealityHandMeshHandler.OnHandMeshUpdated(InputEventData<HandMeshInfo> eventData)
        {
            using (OnHandMeshUpdatedPerfMarker.Auto())
            {
                if (eventData.InputSource.SourceId != Controller.InputSource.SourceId
                    || eventData.Handedness != Controller.ControllerHandedness)
                {
                    return;
                }

                lastHandMeshInfo = eventData.InputData;
                eventData.Use();
            }
        }

        private static readonly ProfilerMarker UpdateHandJointsPerfMarker = new ProfilerMarker("[MRTK] BaseHandVisualizer.UpdateHandJoints");

        protected virtual bool UpdateHandJoints()
        {
            using (UpdateHandJointsPerfMarker.Auto())
            {
                if (!handJointsUpdated || MixedRealityHand.IsNull())
                {
                    return false;
                }

                IMixedRealityInputSystem inputSystem = CoreServices.InputSystem;
                MixedRealityHandTrackingProfile handTrackingProfile = inputSystem?.InputSystemProfile != null ? inputSystem.InputSystemProfile.HandTrackingProfile : null;
                if (handTrackingProfile != null && !handTrackingProfile.EnableHandJointVisualization)
                {
                    // clear existing joint GameObjects / meshes
                    foreach (Transform joint in JointsArray)
                    {
                        if (joint != null)
                        {
                            Destroy(joint.gameObject);
                        }
                    }

                    JointsArray = System.Array.Empty<Transform>();

                    // Even though the base class isn't handling joint visualization, we still received new joints.
                    // Return true here in case any derived classes want to update.
                    return true;
                }

                if (JointsArray.Length != ArticulatedHandPose.JointCount)
                {
                    JointsArray = new Transform[ArticulatedHandPose.JointCount];
                }

                // This starts at 1 to skip over TrackedHandJoint.None.
                for (int i = 1; i < ArticulatedHandPose.JointCount; i++)
                {
                    TrackedHandJoint handJoint = (TrackedHandJoint)i;

                    // Skip this hand joint if the event data doesn't have an entry for it
                    if (!MixedRealityHand.TryGetJoint(handJoint, out MixedRealityPose handJointPose))
                    {
                        continue;
                    }

                    Transform jointTransform = JointsArray[i];
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

                        JointsArray[i] = jointObject.transform;
                    }
                }

                handJointsUpdated = false;
                return true;
            }
        }

        private static readonly ProfilerMarker UpdateHandMeshPerfMarker = new ProfilerMarker("[MRTK] BaseHandVisualizer.UpdateHandMesh");

        protected virtual void UpdateHandMesh()
        {
            using (UpdateHandMeshPerfMarker.Auto())
            {
                if (lastHandMeshInfo == null)
                {
                    return;
                }

                bool newMesh = handMeshFilter == null;

                IMixedRealityInputSystem inputSystem = CoreServices.InputSystem;
                MixedRealityHandTrackingProfile handTrackingProfile = inputSystem?.InputSystemProfile != null ? inputSystem.InputSystemProfile.HandTrackingProfile : null;
                if (newMesh &&  handTrackingProfile != null)
                {
                    // Create the hand mesh in the scene and assign the proper material to it
                    if(handTrackingProfile.SystemHandMeshMaterial.IsNotNull())
                    {
                        handMeshFilter = new GameObject("System Hand Mesh").EnsureComponent<MeshFilter>();
                        handMeshFilter.EnsureComponent<MeshRenderer>().material = handTrackingProfile.SystemHandMeshMaterial;
                    }
#pragma warning disable 0618
                    else if (handTrackingProfile.HandMeshPrefab.IsNotNull())
                    {
                        handMeshFilter = Instantiate(handTrackingProfile.HandMeshPrefab).GetComponent<MeshFilter>();
                    }
#pragma warning restore 0618

                    // Initialize the hand mesh if we generated it successfully
                    if (handMeshFilter != null)
                    {
                        lastHandMeshVerticesCount = handMeshFilter.mesh.vertices.Length;
                        handMeshFilter.transform.parent = transform;
                    }
                }

                if (handMeshFilter != null)
                {
                    Mesh mesh = handMeshFilter.mesh;

                    bool meshChanged = false;
                    // On some platforms, mesh length counts may change as the hand mesh is updated.
                    // In order to update the vertices when the array sizes change, the mesh
                    // must be cleared per instructions here:
                    // https://docs.unity3d.com/ScriptReference/Mesh.html
                    if (lastHandMeshVerticesCount != lastHandMeshInfo.vertices?.Length)
                    {
                        meshChanged = true;
                        mesh.Clear();
                    }

                    mesh.vertices = lastHandMeshInfo.vertices;
                    mesh.normals = lastHandMeshInfo.normals;
                    lastHandMeshVerticesCount = lastHandMeshInfo.vertices != null ? lastHandMeshInfo.vertices.Length : 0;

                    if (newMesh || meshChanged)
                    {
                        mesh.triangles = lastHandMeshInfo.triangles;

                        if (lastHandMeshInfo.uvs?.Length > 0)
                        {
                            mesh.uv = lastHandMeshInfo.uvs;
                        }
                    }

                    if (meshChanged)
                    {
                        mesh.RecalculateBounds();
                    }

                    handMeshFilter.transform.SetPositionAndRotation(lastHandMeshInfo.position, lastHandMeshInfo.rotation);
                }
            }
        }
    }
}

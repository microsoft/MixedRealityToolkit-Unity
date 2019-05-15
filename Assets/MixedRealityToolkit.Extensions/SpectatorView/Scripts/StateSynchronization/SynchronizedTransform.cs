// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// A <see cref="ISynchronizedComponent"/> specifically for transforms.
    /// </summary>
    public class SynchronizedTransform : SynchronizedComponent<SynchronizedTransformService, SynchronizedTransformChangeType>
    {
        public const string SpectatorViewHiddenTag = "SpectatorViewHidden";
        private short id;

        private bool previousIsActive = true;
        private string previousName;
        private int previousLayer;
        private short previousParentId = NullTransformId;
        private Vector3 previousPosition;
        private Quaternion previousRotation;
        private Vector3 previousScale;

        private string cachedName;
        private Transform cachedTransform;
        private RectTransform cachedRectTransform;
        private GameObject cachedGameObject;
        private SynchronizedTransform cachedParentTransform;
        private bool isCachedParentTransformValid;
        private bool isCachedPerformanceParametersValid;

        private SynchronizationPerformanceParameters cachedPerformanceParameters = null;
        private SynchronizedRectTransform rectCache = null;

        private bool needsComponentCheck = true;
        private bool isIdInitialized = false;
        private bool? isHidden;

        private Transform CachedTransform
        {
            get
            {
                if (cachedTransform == null)
                {
                    cachedTransform = this.transform;
                }

                return cachedTransform;
            }
        }

        private RectTransform CachedRectTransform => cachedRectTransform ?? (cachedRectTransform = GetComponent<RectTransform>());

        protected override bool UpdateWhenDisabled => true;

        /// <summary>
        /// Returns an id that can be used for initialization checks
        /// </summary>
        public static short NullTransformId
        {
            get { return -1; }
        }

        /// <summary>
        /// Returns the unique id for the synchronized transform
        /// </summary>
        public short Id
        {
            get { return id; }
        }

        private SynchronizedTransform CachedParentTransform
        {
            get
            {
                if (!isCachedParentTransformValid)
                {
                    if (CachedTransform.parent == null)
                    {
                        cachedParentTransform = null;
                    }
                    else
                    {
                        cachedParentTransform = CachedTransform.parent.GetComponent<SynchronizedTransform>();
                    }

                    isCachedParentTransformValid = true;
                }

                return cachedParentTransform;
            }
        }

        internal SynchronizationPerformanceParameters PerformanceParameters
        {
            get
            {
                if (!isCachedPerformanceParametersValid)
                {
                    cachedPerformanceParameters = GetComponentInParent<SynchronizationPerformanceParameters>();
                    if (cachedPerformanceParameters == null && DefaultSynchronizationPerformanceParameters.IsInitialized && DefaultSynchronizationPerformanceParameters.Instance != null)
                    {
                        cachedPerformanceParameters = DefaultSynchronizationPerformanceParameters.Instance;
                    }
                    if (cachedPerformanceParameters == null)
                    {
                        cachedPerformanceParameters = SynchronizationPerformanceParameters.CreateEmpty();
                    }

                    isCachedPerformanceParametersValid = true;
                }

                return cachedPerformanceParameters;
            }
        }

        private bool IsHidden
        {
            get
            {
                if (isHidden == null)
                {
                    isHidden = (tag == SpectatorViewHiddenTag);
                }
                return isHidden.Value;
            }
        }

        /// <summary>
        /// SocketEndpoint connections that are currently blocked
        /// </summary>
        public ISet<SocketEndpoint> BlockedConnections
        {
            get;
        } = new HashSet<SocketEndpoint>();

        /// <summary>
        /// Returns true if the provided endpoint should receive a transform update, otherwise false
        /// </summary>
        /// <param name="endpoint">network connection to potentially send transform update</param>
        /// <returns>True if the provided endpoint should receive a transform update, otherwise false</returns>
        public bool ShouldSendTransformInHierarchy(SocketEndpoint endpoint)
        {
            if (BlockedConnections.Contains(endpoint))
            {
                return false;
            }

            if (IsHidden)
            {
                return false;
            }

            if (CachedParentTransform != null)
            {
                return CachedParentTransform.ShouldSendTransformInHierarchy(endpoint);
            }
            else
            {
                return true;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            EnsureIDAndParentTransformInitialized();

            UpdateTransformHierarchy();
        }

        private void EnsureIDAndParentTransformInitialized()
        {
            if (!isIdInitialized)
            {
                synchronizedTransform = this;
                cachedTransform = transform;
                cachedGameObject = gameObject;
                id = SynchronizedSceneManager.Instance.GetNewTransformId();

                SynchronizedSceneManager.Instance.AssignMirror(this.cachedGameObject, this.id);

                AttachSynchronizedTransformControllers();

                if (CachedParentTransform != null)
                {
                    CachedParentTransform.EnsureIDAndParentTransformInitialized();
                }

                isIdInitialized = true;
            }
        }

        private void OnTransformParentChanged()
        {
            isCachedParentTransformValid = false;
        }

        private void OnTransformChildrenChanged()
        {
            UpdateTransformHierarchy();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (SynchronizedSceneManager.IsInitialized && SynchronizedSceneManager.Instance != null)
            {
                SynchronizedSceneManager.Instance.RemoveMirror(this.id);
            }
        }

        /// <summary>
        /// Returns true if the provided change type contains the specified flag, otherwise false.
        /// </summary>
        /// <param name="changeType">Change type</param>
        /// <param name="flag">flag to check for</param>
        /// <returns>Returns true if the provided change type contains the specified flag, otherwise false.</returns>
        public static bool HasFlag(SynchronizedTransformChangeType changeType, SynchronizedTransformChangeType flag)
        {
            return (changeType & flag) == flag;
        }

        /// <summary>
        /// Parent transform's id, returns NullTransformId if no parent transform exists
        /// </summary>
        public short ParentId
        {
            get
            {
                SynchronizedTransform parentTransform = CachedParentTransform;
                if (parentTransform == null)
                {
                    return NullTransformId;
                }
                else
                {
                    return parentTransform.Id;
                }
            }
        }

        /// <summary>
        /// Associated game object's name
        /// </summary>
        public string CachedName
        {
            get
            {
                if (cachedName == null)
                {
                    cachedName = this.cachedGameObject.name;
                }

                return cachedName;
            }
        }

        /// <summary>
        /// Associated game object's layer
        /// </summary>
        public int Layer
        {
            get
            {
                return this.cachedGameObject.layer;
            }
        }

        protected override bool ShouldUpdateFrame(SocketEndpoint endpoint)
        {
            return base.ShouldUpdateFrame(endpoint) && ShouldSendTransformInHierarchy(endpoint);
        }

        protected override void BeginUpdatingFrame(SocketEndpointConnectionDelta connectionDelta)
        {
            // Messages for hierarchies need to be sent from root to leaf to ensure
            // that Canvas construction on the other end happens in the correct order.
            CachedParentTransform?.OnFrameCompleted(connectionDelta);
        }

        protected override void EndUpdatingFrame()
        {
            UpdateSynchronizedComponents();
        }

        protected override bool HasChanges(SynchronizedTransformChangeType changeFlags)
        {
            return changeFlags != 0;
        }

        protected override void SendComponentCreation(IEnumerable<SocketEndpoint> newConnections)
        {
            // Don't send component creations for transforms
        }

        protected override SynchronizedTransformChangeType CalculateDeltaChanges()
        {
            SynchronizedTransformChangeType changeType = 0;
            short newParentId = ParentId;
            Vector3 newPosition;
            Quaternion newRotation;
            Vector3 newScale;
            int newLayer = Layer;
            GetLocalPose(out newPosition, out newRotation, out newScale);
            bool newIsActive = SynchronizedIsActive;

            if (previousName != CachedName)
            {
                previousName = CachedName;
                changeType |= SynchronizedTransformChangeType.Name;
            }
            if (previousLayer != newLayer)
            {
                previousLayer = newLayer;
                changeType |= SynchronizedTransformChangeType.Layer;
            }
            if (previousParentId != newParentId)
            {
                previousParentId = newParentId;
                changeType |= SynchronizedTransformChangeType.Parent;
            }
            if (previousPosition != newPosition)
            {
                previousPosition = newPosition;
                changeType |= SynchronizedTransformChangeType.Position;
            }
            if (previousRotation != newRotation)
            {
                previousRotation = newRotation;
                changeType |= SynchronizedTransformChangeType.Rotation;
            }
            if (previousScale != newScale)
            {
                previousScale = newScale;
                changeType |= SynchronizedTransformChangeType.Scale;
            }
            if (previousIsActive != newIsActive)
            {
                previousIsActive = newIsActive;
                changeType |= SynchronizedTransformChangeType.IsActive;
            }

            RectTransform rectTrans = CachedRectTransform;
            if (rectTrans && rectCache == null)
            {
                rectCache = new SynchronizedRectTransform();
            }
            else if (rectTrans == null && rectCache != null)
            {
                rectCache = null;
            }

            if (rectCache != null && rectCache.UpdateChange(rectTrans))
            {
                changeType |= SynchronizedTransformChangeType.RectTransform;
            }

            return changeType;
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            SynchronizedTransformChangeType changeType =
                SynchronizedTransformChangeType.IsActive |
                SynchronizedTransformChangeType.Name |
                SynchronizedTransformChangeType.Layer |
                SynchronizedTransformChangeType.Parent |
                SynchronizedTransformChangeType.Position |
                SynchronizedTransformChangeType.Rotation |
                SynchronizedTransformChangeType.Scale;

            if (rectCache != null)
            {
                changeType |= SynchronizedTransformChangeType.RectTransform;
            }

            SendDeltaChanges(endpoints, changeType);
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, SynchronizedTransformChangeType changeFlags)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                SynchronizedTransformService.Instance.WriteHeader(message, this);

                message.Write(this.Id);
                message.Write((byte)changeFlags);

                if (HasFlag(changeFlags, SynchronizedTransformChangeType.Name))
                {
                    message.Write(this.CachedName);
                }
                if (HasFlag(changeFlags, SynchronizedTransformChangeType.Layer))
                {
                    message.Write(previousLayer);
                }
                if (HasFlag(changeFlags, SynchronizedTransformChangeType.Parent))
                {
                    message.Write(this.ParentId);
                    message.Write(this.transform.GetSiblingIndex());
                }
                if (HasFlag(changeFlags, SynchronizedTransformChangeType.Position))
                {
                    message.Write(previousPosition);
                }
                if (HasFlag(changeFlags, SynchronizedTransformChangeType.Rotation))
                {
                    message.Write(previousRotation);
                }
                if (HasFlag(changeFlags, SynchronizedTransformChangeType.Scale))
                {
                    message.Write(previousScale);
                }
                if (HasFlag(changeFlags, SynchronizedTransformChangeType.IsActive))
                {
                    message.Write(this.SynchronizedIsActive);
                }
                if (HasFlag(changeFlags, SynchronizedTransformChangeType.RectTransform))
                {
                    RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();
                    SynchronizedRectTransform.Send(rectTrans, message);
                }

                message.Flush();
                SynchronizedSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
            }
        }

        private void AttachSynchronizedTransformControllers()
        {
            foreach (SynchronizedComponentDefinition componentDefinition in SynchronizedSceneManager.Instance.SynchronizedComponentDefinitions)
            {
                if (componentDefinition.IsSynchronizedTransformController)
                {
                    bool changesDetected;
                    componentDefinition.EnsureSynchronized(cachedGameObject, out changesDetected);
                }
            }
        }

        private void UpdateSynchronizedComponents()
        {
            SynchronizationPerformanceParameters synchronizationPerformanceParameters = PerformanceParameters;
            if ((synchronizationPerformanceParameters.CheckForSynchronizedComponents == SynchronizationPerformanceParameters.PollingFrequency.UpdateOnceOnStart && needsComponentCheck) ||
                synchronizationPerformanceParameters.CheckForSynchronizedComponents == SynchronizationPerformanceParameters.PollingFrequency.UpdateContinuously)
            {
                using (SynchronizationPerformanceMonitor.Instance.MeasureScope(SynchronizationPerformanceFeature.GameObjectComponentCheck))
                {
                    bool anyChangesDetected = false;
                    foreach (SynchronizedComponentDefinition componentDefinition in SynchronizedSceneManager.Instance.SynchronizedComponentDefinitions)
                    {
                        if (!componentDefinition.IsSynchronizedTransformController)
                        {
                            bool changesDetected;
                            componentDefinition.EnsureSynchronized(cachedGameObject, out changesDetected);
                            anyChangesDetected |= changesDetected;
                        }
                    }

                    if (anyChangesDetected)
                    {
                        needsComponentCheck = true;
                    }
                    else
                    {
                        needsComponentCheck = false;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the associated game object is active in the scene, otherwise false
        /// </summary>
        public bool SynchronizedIsActive
        {
            get
            {
                if (ParentId == NullTransformId)
                {
                    return cachedGameObject.activeInHierarchy;
                }
                else
                {
                    return cachedGameObject.activeSelf;
                }
            }
        }

        private void GetLocalPose(out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale)
        {
            if (ParentId == NullTransformId)
            {
                localPosition = CachedTransform.position;
                localRotation = CachedTransform.rotation;
                localScale = CachedTransform.lossyScale;
            }
            else
            {
                localPosition = CachedTransform.localPosition;
                localRotation = CachedTransform.localRotation;
                localScale = CachedTransform.localScale;
            }
        }

        private void UpdateTransformHierarchy()
        {
            EnsureChildTransformsAreSynchronized();
        }

        /// <summary>
        /// Writes child transforms hierarchy to network message
        /// </summary>
        /// <param name="message"></param>
        public void WriteChildHierarchyTree(BinaryWriter message)
        {
            int childCount = CachedTransform.childCount;
            message.Write(childCount);
            for (int i = 0; i < childCount; i++)
            {
                SynchronizedTransform childTransform = CachedTransform.GetChild(i).GetComponent<SynchronizedTransform>();
                message.Write(childTransform.Id);
                message.Write(childTransform.CachedName);

                childTransform.WriteChildHierarchyTree(message);
            }
        }

        private void EnsureChildTransformsAreSynchronized()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                ComponentExtensions.EnsureComponent<SynchronizedTransform>(this.transform.GetChild(i).gameObject);
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// A <see cref="IComponentBroadcaster"/> specifically for transforms.
    /// </summary>
    public class TransformBroadcaster : ComponentBroadcaster<TransformBroadcasterService, TransformBroadcasterChangeType>
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
        private TransformBroadcaster cachedParentTransform;
        private bool isCachedParentTransformValid;
        private bool isCachedPerformanceParametersValid;

        private StateSynchronizationPerformanceParameters cachedPerformanceParameters = null;
        private RectTransformBroadcaster rectCache = null;

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
        /// Returns the unique id for the broadcasted transform
        /// </summary>
        public short Id
        {
            get { return id; }
        }

        private TransformBroadcaster CachedParentTransform
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
                        cachedParentTransform = CachedTransform.parent.GetComponent<TransformBroadcaster>();
                    }

                    isCachedParentTransformValid = true;
                }

                return cachedParentTransform;
            }
        }

        internal StateSynchronizationPerformanceParameters PerformanceParameters
        {
            get
            {
                if (!isCachedPerformanceParametersValid)
                {
                    cachedPerformanceParameters = GetComponentInParent<StateSynchronizationPerformanceParameters>();
                    if (cachedPerformanceParameters == null && DefaultStateSynchronizationPerformanceParameters.IsInitialized && DefaultStateSynchronizationPerformanceParameters.Instance != null)
                    {
                        cachedPerformanceParameters = DefaultStateSynchronizationPerformanceParameters.Instance;
                    }
                    if (cachedPerformanceParameters == null)
                    {
                        cachedPerformanceParameters = StateSynchronizationPerformanceParameters.CreateEmpty();
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
                transformBroadcaster = this;
                cachedTransform = transform;
                cachedGameObject = gameObject;
                id = StateSynchronizationSceneManager.Instance.GetNewTransformId();

                StateSynchronizationSceneManager.Instance.AssignMirror(this.cachedGameObject, this.id);

                AttachTransformBroadcasterControllers();

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

            if (StateSynchronizationSceneManager.IsInitialized && StateSynchronizationSceneManager.Instance != null)
            {
                StateSynchronizationSceneManager.Instance.RemoveMirror(this.id);
            }
        }

        /// <summary>
        /// Returns true if the provided change type contains the specified flag, otherwise false.
        /// </summary>
        /// <param name="changeType">Change type</param>
        /// <param name="flag">flag to check for</param>
        /// <returns>Returns true if the provided change type contains the specified flag, otherwise false.</returns>
        public static bool HasFlag(TransformBroadcasterChangeType changeType, TransformBroadcasterChangeType flag)
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
                TransformBroadcaster parentTransform = CachedParentTransform;
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
            UpdateComponentBroadcasters();
        }

        protected override bool HasChanges(TransformBroadcasterChangeType changeFlags)
        {
            return changeFlags != 0;
        }

        protected override void SendComponentCreation(IEnumerable<SocketEndpoint> newConnections)
        {
            // Don't send component creations for transforms
        }

        protected override TransformBroadcasterChangeType CalculateDeltaChanges()
        {
            TransformBroadcasterChangeType changeType = 0;
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
                changeType |= TransformBroadcasterChangeType.Name;
            }
            if (previousLayer != newLayer)
            {
                previousLayer = newLayer;
                changeType |= TransformBroadcasterChangeType.Layer;
            }
            if (previousParentId != newParentId)
            {
                previousParentId = newParentId;
                changeType |= TransformBroadcasterChangeType.Parent;
            }
            if (previousPosition != newPosition)
            {
                previousPosition = newPosition;
                changeType |= TransformBroadcasterChangeType.Position;
            }
            if (previousRotation != newRotation)
            {
                previousRotation = newRotation;
                changeType |= TransformBroadcasterChangeType.Rotation;
            }
            if (previousScale != newScale)
            {
                previousScale = newScale;
                changeType |= TransformBroadcasterChangeType.Scale;
            }
            if (previousIsActive != newIsActive)
            {
                previousIsActive = newIsActive;
                changeType |= TransformBroadcasterChangeType.IsActive;
            }

            RectTransform rectTrans = CachedRectTransform;
            if (rectTrans && rectCache == null)
            {
                rectCache = new RectTransformBroadcaster();
            }
            else if (rectTrans == null && rectCache != null)
            {
                rectCache = null;
            }

            if (rectCache != null && rectCache.UpdateChange(rectTrans))
            {
                changeType |= TransformBroadcasterChangeType.RectTransform;
            }

            return changeType;
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            TransformBroadcasterChangeType changeType =
                TransformBroadcasterChangeType.IsActive |
                TransformBroadcasterChangeType.Name |
                TransformBroadcasterChangeType.Layer |
                TransformBroadcasterChangeType.Parent |
                TransformBroadcasterChangeType.Position |
                TransformBroadcasterChangeType.Rotation |
                TransformBroadcasterChangeType.Scale;

            if (rectCache != null)
            {
                changeType |= TransformBroadcasterChangeType.RectTransform;
            }

            SendDeltaChanges(endpoints, changeType);
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, TransformBroadcasterChangeType changeFlags)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                TransformBroadcasterService.Instance.WriteHeader(message, this);

                message.Write(this.Id);
                message.Write((byte)changeFlags);

                if (HasFlag(changeFlags, TransformBroadcasterChangeType.Name))
                {
                    message.Write(this.CachedName);
                }
                if (HasFlag(changeFlags, TransformBroadcasterChangeType.Layer))
                {
                    message.Write(previousLayer);
                }
                if (HasFlag(changeFlags, TransformBroadcasterChangeType.Parent))
                {
                    message.Write(this.ParentId);
                    message.Write(this.transform.GetSiblingIndex());
                }
                if (HasFlag(changeFlags, TransformBroadcasterChangeType.Position))
                {
                    message.Write(previousPosition);
                }
                if (HasFlag(changeFlags, TransformBroadcasterChangeType.Rotation))
                {
                    message.Write(previousRotation);
                }
                if (HasFlag(changeFlags, TransformBroadcasterChangeType.Scale))
                {
                    message.Write(previousScale);
                }
                if (HasFlag(changeFlags, TransformBroadcasterChangeType.IsActive))
                {
                    message.Write(this.SynchronizedIsActive);
                }
                if (HasFlag(changeFlags, TransformBroadcasterChangeType.RectTransform))
                {
                    RectTransform rectTrans = this.gameObject.GetComponent<RectTransform>();
                    RectTransformBroadcaster.Send(rectTrans, message);
                }

                message.Flush();
                StateSynchronizationSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
            }
        }

        private void AttachTransformBroadcasterControllers()
        {
            foreach (ComponentBroadcasterDefinition componentDefinition in StateSynchronizationSceneManager.Instance.ComponentBroadcasterDefinitions)
            {
                if (componentDefinition.IsTransformBroadcasterController)
                {
                    bool changesDetected;
                    componentDefinition.EnsureComponentBroadcastersCreated(cachedGameObject, out changesDetected);
                }
            }
        }

        private void UpdateComponentBroadcasters()
        {
            StateSynchronizationPerformanceParameters StateSynchronizationPerformanceParameters = PerformanceParameters;
            if ((StateSynchronizationPerformanceParameters.CheckForComponentBroadcasters == StateSynchronizationPerformanceParameters.PollingFrequency.UpdateOnceOnStart && needsComponentCheck) ||
                StateSynchronizationPerformanceParameters.CheckForComponentBroadcasters == StateSynchronizationPerformanceParameters.PollingFrequency.UpdateContinuously)
            {
                using (StateSynchronizationPerformanceMonitor.Instance.MeasureScope(StateSynchronizationPerformanceFeature.GameObjectComponentCheck))
                {
                    bool anyChangesDetected = false;
                    foreach (ComponentBroadcasterDefinition componentDefinition in StateSynchronizationSceneManager.Instance.ComponentBroadcasterDefinitions)
                    {
                        if (!componentDefinition.IsTransformBroadcasterController)
                        {
                            bool changesDetected;
                            componentDefinition.EnsureComponentBroadcastersCreated(cachedGameObject, out changesDetected);
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
                TransformBroadcaster childTransform = CachedTransform.GetChild(i).GetComponent<TransformBroadcaster>();
                message.Write(childTransform.Id);
                message.Write(childTransform.CachedName);

                childTransform.WriteChildHierarchyTree(message);
            }
        }

        private void EnsureChildTransformsAreSynchronized()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                ComponentExtensions.EnsureComponent<TransformBroadcaster>(this.transform.GetChild(i).gameObject);
            }
        }
    }
}

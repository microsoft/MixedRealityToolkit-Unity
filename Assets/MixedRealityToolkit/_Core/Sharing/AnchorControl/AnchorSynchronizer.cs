using Pixie.AppSystems;
using Pixie.Core;
using Pixie.DeviceControl;
using Pixie.StateControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.AnchorControl
{
    public class AnchorSynchronizer : MonoBehaviourSharingApp, ISharingAppObject, IAnchorSynchronizerClient, IAnchorSynchronizerServer
    {
        public const bool ApplyAlignmentRotation = true;
        public const bool ApplyAlignmentScale = false;
        public const bool RotateOnYAxisOnly = true;

        public AnchorSyncStateEnum State { get { return state; } }
        
        [SerializeField]
        private AnchorSyncStateEnum state = AnchorSyncStateEnum.Stopped;
        private Dictionary<short, Transform> sharedAnchorTransformLookup = new Dictionary<short, Transform>();
        private Dictionary<short, Transform> userAnchorTransformLookup = new Dictionary<short, Transform>();
        private List<Transform> sharedAnchorTransformList = new List<Transform>();
        private List<Transform> userAnchorTransformList = new List<Transform>();
        private Transform sceneRootTransform;

        public void CreateAnchorStates(IAnchorDefinitions definitions, IAppStateReadWrite appState)
        {
            switch (state)
            {
                case AnchorSyncStateEnum.Stopped:
                    break;

                default:
                    throw new Exception("Can't do this when state is " + state);
            }

            state = AnchorSyncStateEnum.CreatingSharedAnchors;
            StartCoroutine(CreateAnchorStatesInternal(definitions, appState));
        }

        private IEnumerator CreateAnchorStatesInternal(IAnchorDefinitions definitions, IAppStateReadWrite appState)
        {
            definitions.FetchDefinitions();

            while (!definitions.Ready)
            {
                // Wait for definitions to be fetched
                yield return null;
            }

            sbyte sharedAnchorNum = 1;
            sbyte userAnchorNum = 1;

            sceneRootTransform = new GameObject("Scene Root Anchor").transform;
            sceneRootTransform.parent = transform;

            // Create a shared anchor state for each anchor ID supplied
            foreach (AnchorDefinition definition in definitions.Definitions)
            {
                // Create a single shared anchor to be referenced by all
                // This anchor state was defined once on startup
                SharedAnchorState sharedAnchor = new SharedAnchorState(sharedAnchorNum);
                sharedAnchor.AnchorID = definition.ID;
                sharedAnchor.Position = definition.Position;
                sharedAnchor.Rotation = definition.Rotation;
                appState.AddState<SharedAnchorState>(sharedAnchor);

                // Create transforms that we can use for shared / user anchor calculations
                // Store both by shared anchor ID
                Transform sharedAnchorTransform = new GameObject("Shared Anchor " + sharedAnchor.AnchorID).transform;
                Transform userAnchorTransform = new GameObject("User Anchor " + sharedAnchor.AnchorID).transform;
                sharedAnchorTransform.parent = transform;
                userAnchorTransform.parent = transform;

                sharedAnchorTransform.position = sharedAnchor.Position;
                sharedAnchorTransform.eulerAngles = sharedAnchor.Rotation;
                userAnchorTransform.position = sharedAnchor.Position;
                userAnchorTransform.eulerAngles = sharedAnchor.Rotation;

                sharedAnchorTransformLookup.Add(sharedAnchorNum, sharedAnchorTransform);
                userAnchorTransformLookup.Add(sharedAnchorNum, userAnchorTransform);
                sharedAnchorTransformList.Add(sharedAnchorTransform);
                userAnchorTransformList.Add(userAnchorTransform);

                foreach (UserSlot user in appState.GetStates<UserSlot>())
                {
                    // Create a user anchor for each user - this is the user's own anchor state, retrieved individually
                    UserAnchorState userAnchor = new UserAnchorState(userAnchorNum);
                    // Set its target to the shared anchor state num
                    userAnchor.TargetNum = sharedAnchorNum;
                    // Set its user num to the current user
                    userAnchor.UserNum = user.UserID;
                    userAnchor.State = UserAnchorState.StateEnum.Unknown;
                    appState.AddState<UserAnchorState>(userAnchor);
                    userAnchorNum++;

                    yield return null;
                }

                sharedAnchorNum++;
            }

            // Create an alignment state for each user - this is where we will store the result of our alignment operation
            foreach (UserSlot user in appState.GetStates<UserSlot>())
            {
                AlignmentState alignmentState = new AlignmentState(user.UserID);
                appState.AddState<AlignmentState>(alignmentState);
            }

            appState.Flush<AlignmentState>();
            appState.Flush<SharedAnchorState>();
            appState.Flush<UserAnchorState>();

            Debug.Log("Finished creating anchor states internal");

            state = AnchorSyncStateEnum.Synchronizing;
        }

        public void UpdateUserAnchorStates(short userID, IAnchorMatrixSource anchorMatrixSource, IAppStateReadWrite appState)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    state = AnchorSyncStateEnum.Synchronizing;
                    break;

                case AppRoleEnum.Host:
                    break;

                default:
                    throw new Exception("Shouldn't call this function when app role is " + AppRole);
            }

            switch (state)
            {
                case AnchorSyncStateEnum.Synchronizing:
                    break;

                default:
                    throw new Exception("Can't do this when state is " + state);
            }

            foreach (UserAnchorState userAnchor in appState.GetStates<UserAnchorState>())
            {
                if (userAnchor.UserNum != userID)
                    continue;

                // Get the target anchor this user anchor is linked to
                SharedAnchorState targetAnchor = appState.GetState<SharedAnchorState>(userAnchor.TargetNum);
                Transform userAnchorTransform = GetOrCreateUserAnchorTransform(userAnchor.TargetNum);

                Matrix4x4 matrix;
                if (anchorMatrixSource.GetAnchorMatrix(targetAnchor.AnchorID, out matrix))
                {
                    // This anchor is known!
                    // Apply the matrix to the user anchor
                    UserAnchorState newUserAnchor = userAnchor;
                    newUserAnchor.State = UserAnchorState.StateEnum.Known;
                    newUserAnchor.Position = matrix.ExtractPosition();
                    newUserAnchor.Rotation = matrix.ExtractRotation().eulerAngles;
                    // Save the state
                    appState.SetState<UserAnchorState>(newUserAnchor);
                }
                else
                {
                    // Anchor is unknown
                    // By default anchors are unknown and typically won't revert back to unknown once found
                    // But just in case, set the state to unknown
                    // If there's no difference app state will filter out the change anyway
                    UserAnchorState newUserAnchor = userAnchor;
                    newUserAnchor.State = UserAnchorState.StateEnum.Unknown;
                    appState.SetState<UserAnchorState>(newUserAnchor);
                }
            }
        }

        private Transform GetOrCreateUserAnchorTransform(short targetNum)
        {
            Transform anchorTransform;
            if (!userAnchorTransformLookup.TryGetValue(targetNum, out anchorTransform))
            {
                anchorTransform = new GameObject("User Anchor " + targetNum).transform;
                anchorTransform.parent = transform;
                userAnchorTransformLookup.Add(targetNum, anchorTransform);
            }
            return anchorTransform;
        }

        public void UpdateAlignmentStates(IAppStateReadWrite appState)
        {
            switch (state)
            {
                case AnchorSyncStateEnum.Synchronizing:
                    break;

                default:
                    throw new Exception("Can't update alignment states in state " + state);
            }

            foreach (AlignmentState alignmentState in appState.GetStates<AlignmentState>())
            {
                int anchorIndex = 0;
                foreach (UserAnchorState userAnchor in appState.GetStates<UserAnchorState>())
                {
                    // If this anchor isn't associated with this alignment state, skip it
                    if (userAnchor.UserNum != alignmentState.UserNum)
                        continue;

                    // Otherwise copy transform info to user anchor transform
                    userAnchorTransformList[anchorIndex].position = userAnchor.Position;
                    userAnchorTransformList[anchorIndex].eulerAngles = userAnchor.Rotation;
                    anchorIndex++;
                }

                // Now that we've updated our user anchors, do the alignment
                AlignmentUtility.Align(
                    sceneRootTransform, 
                    sharedAnchorTransformList, 
                    userAnchorTransformList, 
                    ApplyAlignmentRotation,
                    ApplyAlignmentScale, 
                    RotateOnYAxisOnly);

                // Copy the root transform's new alignment to the alignment state
                AlignmentState newAlignmentState = alignmentState;
                newAlignmentState.Position = sceneRootTransform.position;
                newAlignmentState.EulerAngles = sceneRootTransform.eulerAngles;

                // Apply the new alignment state
                appState.SetState<AlignmentState>(newAlignmentState);
            }
        }
    }
}
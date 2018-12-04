using System;
using System.Collections.Generic;
using Pixie.DeviceControl;
using Pixie.DeviceControl.Users;
using Pixie.StateControl;
using UnityEngine;

namespace Pixie.AnchorControl
{
    public class UserAlignment : MonoBehaviour, IUserAlignment
    {
        private Transform worldHelper;
        private Transform localHelper;

        public void SetAlignedWorldTransforms(IUserView users, IAppStateReadWrite appState)
        {
            CreateTransformHelper();

            // Go through each of the local transforms in the app state
            // These represent the player's raw positions in local space

            foreach (LocalTransformState localTransform in appState.GetStates<LocalTransformState>())
            {
                // If the user doesn't exist yet or isn't initialized, continue
                IUserObject userObject = null;
                if (!users.GetUserObject(localTransform.UserID, out userObject) || !userObject.IsAssigned)
                    continue;

                IUserTransforms userTransforms = (IUserTransforms)userObject.gameObject.GetComponent(typeof(IUserTransforms));
                if (userTransforms == null)
                    throw new Exception("User states was null for user " + userObject.UserID);

                // Get the user's alignment state
                // This shows how we need to transform the local position / rotation
                // so that it matches the 'true' world origin, based on anchor positions
                AlignmentState alignment = appState.GetState<AlignmentState>(userObject.UserID);
                worldHelper.position = alignment.Position;
                worldHelper.eulerAngles = alignment.EulerAngles;

                // Now apply the local pos / rotation to our local helper
                // This represents the transformed position
                localHelper.localPosition = localTransform.LocalPosition;
                localHelper.localEulerAngles = localTransform.LocalEulerAngles;

                // Apply this transformed position to the world transform state
                WorldTransformState worldTransform = appState.GetState<WorldTransformState>(localTransform.ItemID);
                worldTransform.Position = localHelper.position;
                worldTransform.EulerAngles = localHelper.eulerAngles;

                // Set the state
                appState.SetState<WorldTransformState>(worldTransform);
            }
        }

        public void AlignUserObjects(IEnumerable<IUserObject> userObjects, IAppStateReadOnly appState)
        {
            if (worldHelper == null)
                CreateTransformHelper();

            foreach (IUserObject userObject in userObjects)
            {
                if (!userObject.IsAssigned)
                    continue;

                // Get the alignment state
                AlignmentState alignment = appState.GetState<AlignmentState>(userObject.UserID);
                // Apply the alignment to the user object's scene transform
                userObject.SceneAlignment.position = alignment.Position;
                userObject.SceneAlignment.eulerAngles = alignment.EulerAngles;
                // Any local transformations applied to transforms under this scene transform
                // will now be aligned to the shared state
            }
        }

        private void CreateTransformHelper()
        {
            if (worldHelper == null)
            {
                worldHelper = new GameObject("UserStateAlignment Helper (world)").transform;
                worldHelper.parent = transform;
            }

            if (localHelper == null)
            {
                localHelper = new GameObject("UserStateAlignment Helper (local)").transform;
                localHelper.parent = worldHelper;
            }
        }
    }
}
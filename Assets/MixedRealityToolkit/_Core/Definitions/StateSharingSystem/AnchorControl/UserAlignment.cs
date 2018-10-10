using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AnchorControl
{
    public class UserAlignment : MonoBehaviour, IUserAlignment
    {
        private Transform transformHelper;

        public void AlignUserStates(IEnumerable<IUserObject> userObjects, IAppStateReadWrite appState)
        {
            if (transformHelper == null)
                CreateTransformHelper();

            foreach (IUserObject userObject in userObjects)
            {
                IUserStates userStates = (IUserStates)userObject.gameObject.GetComponent(typeof(IUserStates));
                if (userStates == null)
                    throw new Exception("User states was null for user " + userObject.UserNum);

                // Get the local user state from the user object
                // This represents their local position / rotation
                UserState userState = userStates.UserState;
                // Get the user's alignment state
                // This shows how we need to transform the local position / rotation
                // so that it matches the 'true' world origin, based on anchor positions
                AlignmentState alignment = appState.GetState<AlignmentState>(userObject.UserNum);
                transformHelper.position = alignment.Position;
                transformHelper.eulerAngles = alignment.Rotation;

                userState.HeadPos = transformHelper.TransformPoint(userState.HeadPos);
                userState.HeadDir = transformHelper.TransformDirection(userState.HeadDir);

                if (appState.StateExists<HandState>(userObject.UserNum))
                {
                    HandState handState = userStates.HandState;
                    handState.LHandPos = transformHelper.TransformPoint(handState.LHandPos);
                    handState.RHandPos = transformHelper.TransformPoint(handState.RHandPos);
                    handState.LHandDir = transformHelper.TransformDirection(handState.LHandDir);
                    handState.RHandDir = transformHelper.TransformDirection(handState.RHandDir);
                    appState.SetState<HandState>(handState);
                }
                
                // Copy the transformed user state into the app state
                appState.SetState<UserState>(userState);
            }
        }

        public void AlignUserObjects(IEnumerable<IUserObject> userObjects, IAppStateReadOnly appState)
        {
            if (transformHelper == null)
                CreateTransformHelper();

            foreach (IUserObject userObject in userObjects)
            {
                // Get the alignment state
                AlignmentState alignment = appState.GetState<AlignmentState>(userObject.UserNum);
                // Apply the alignment to the user object's scene transform
                userObject.SceneAlignment.position = alignment.Position;
                userObject.SceneAlignment.eulerAngles = alignment.Rotation;
            }
        }        

        private void CreateTransformHelper()
        {
            transformHelper = new GameObject("UserStateAlignment Helper").transform;
            transformHelper.parent = transform;
        }
    }
}
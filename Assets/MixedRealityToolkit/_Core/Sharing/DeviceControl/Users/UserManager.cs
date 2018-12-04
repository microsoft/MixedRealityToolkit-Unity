using Pixie.Core;
using Pixie.Initialization;
using Pixie.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.DeviceControl.Users
{
    public class UserManager : MonoBehaviourSharingApp, IUserManager
    {
        public bool UsersDefined
        {
            get
            {
                if (!initialized)
                    return false;

                return appState.Initialized && appState.GetNumStates<UserSlot>() > 0;
            }
        }

        public bool AllSlotsFilled
        {
            get
            {
                if (!initialized)
                    return false;

                foreach (UserSlot slot in appState.GetStates<UserSlot>())
                {
                    if (slot.FillState == UserSlot.FillStateEnum.Empty)
                        return false;
                }
                return true;
            }
        }

        public IEnumerable<IUserObject> UserObjects { get { return userObjects.Values; } }

        public bool LocalUserAssigned
        {
            get
            {
                switch (AppRole)
                {
                    case AppRoleEnum.Server:
                        return false;

                    default:
                        if (UsersDefined)
                        {
                            RefreshLocalUserObject();
                            return localUserObject != null;
                        }
                        return false;
                }
            }
        }

        public short LocalUserID
        {
            get
            {
                switch (AppRole)
                {
                    case AppRoleEnum.Server:
                        return -1;

                    default:
                        return (localUserObject == null) ? (short)-1 : localUserObject.UserID;
                }
            }
        }

        public IUserObject LocalUserObject
        {
            get
            {
                return localUserObject;
            }
        }
                
        private Dictionary<short, IUserObject> userObjects = new Dictionary<short, IUserObject>();
        private IUserObject localUserObject;
        private ISystemPrefabPool prefabs;
        private IAppStateReadWrite appState;
        private IDeviceSource devices;
        private bool initialized;

        public void CheckForUserObjects()
        {
            foreach (UserSlot userSlot in appState.GetStates<UserSlot>())
            {
                IUserObject userObject = null;
                if (!userObjects.TryGetValue(userSlot.UserID, out userObject))
                {
                    GameObject userObjectGo = prefabs.InstantiateUser();
                    userObject = (IUserObject)userObjectGo.GetComponent(typeof(IUserObject));

                    if (userObject == null)
                        throw new Exception("User object prefab must have component of type " + typeof(IUserObject));

                    userObject.AssignUser(userSlot);
                    userObjects.Add(userSlot.UserID, userObject);
                }
            }
        }

        public void GenerateUserStates(IEnumerable<UserDefinition> userDefinitions)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    throw new Exception("This function shouldn't be executed on client.");

                default:
                    break;
            }

            short userID = 1;

            foreach (UserDefinition userDefinition in userDefinitions)
            {
                Debug.Log("Adding user slot: " + userDefinition.Role + ", " + userDefinition.Team);
                UserSlot userSlot = new UserSlot(
                    userID,
                    userDefinition.Role,
                    userDefinition.Team,
                    userDefinition.DeviceType,
                    userDefinition.Transforms,
                    userDefinition.DeviceRoles);

                appState.AddState<UserSlot>(userSlot);
                userID++;
            }

            // Get any user state generators in the scene and let them do their thing
            // These will likely be app state source, but they may also be other scripts
            List<IUserStateGenerator> userStateGenerators = new List<IUserStateGenerator>();
            ComponentFinder.FindAllInScenes<IUserStateGenerator>(userStateGenerators, ComponentFinder.SearchTypeEnum.Recursive);

            Debug.Log("Found " + userStateGenerators.Count + " user state generators");

            // Sort the list by execution order
            userStateGenerators.Sort(delegate (IUserStateGenerator usg1, IUserStateGenerator usg2) { return usg1.ExecutionOrder.CompareTo(usg2.ExecutionOrder); });
            // Call for all slots
            foreach (UserSlot userSlot in appState.GetStates<UserSlot>())
            {
                foreach (IUserStateGenerator usg in userStateGenerators)
                {
                    Debug.Log("Generating user states for slot " + userSlot.UserID);
                    usg.GenerateUserStates(userSlot, appState);
                }
            }
        }

        public void ClearUserSlot(short userID)
        {
            UserSlot userSlot = appState.GetState<UserSlot>(userID);
            userSlot.FillState = UserSlot.FillStateEnum.Empty;
            appState.SetState<UserSlot>(userSlot);
        }
        
        public bool SetSlotFillState(short userID, UserSlot.FillStateEnum fillState)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    throw new Exception("This function shouldn't be executed on client.");

                default:
                    break;
            }

            UserSlot slot = appState.GetState<UserSlot>(userID);
            slot.FillState = fillState;
            appState.SetState<UserSlot>(slot);
            return true;
        }

        public bool GetUserObject(short userID, out IUserObject userObject)
        {
            return userObjects.TryGetValue(userID, out userObject);
        }

        private void RefreshLocalUserObject()
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    break;

                default:
                    throw new Exception("This function should only be called on server.");
            }

            localUserObject = null;
            if (devices.LocalDeviceConnected)
            {
                // Get the device ID associated with this client
                short deviceID = devices.LocalDeviceID;

                // Figure out which user this device is assigned to

                // It's possible for the local device to have connected without a state existing in the app state yet
                // We try to avoid situations where you can't rely on app state parity - this is a rare exception
                if (!appState.StateExists<UserDeviceState>(deviceID))
                    return;

                UserDeviceState deviceState = appState.GetState<UserDeviceState>(deviceID);

                // If the device hasn't been assigned to anything, un-assign all local users
                if (!deviceState.IsAssigned)
                {
                    foreach (IUserObject userObject in userObjects.Values)
                        userObject.AssignLocalUser(false);

                    return;
                }

                // Update all user objects
                // local user object may change depending on where device is assigned
                // so it's necessary to go through each one
                foreach (IUserObject userObject in userObjects.Values)
                    userObject.AssignLocalUser(userObject.UserID == deviceState.UserID);

                // Try to get a user object for that device's user ID
                // If the user object hasn't been assigned yet, that's fine
                if (!GetUserObject(deviceState.UserID, out localUserObject))
                    Debug.Log("User object doesn't exist");
            }
            else
            {
                // Un-assign all local users
                foreach (IUserObject userObject in userObjects.Values)
                    userObject.AssignLocalUser(false);
            }
        }

        public override void OnAppInitialize()
        {
            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);
            ComponentFinder.FindInScenes<ISystemPrefabPool>(out prefabs);
            ComponentFinder.FindInScenes<IDeviceSource>(out devices);

            initialized = true;
        }

        private void Update()
        {
            if (!initialized)
                return;

            CheckForUserObjects();
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(UserManager))]
        public class UserManagerEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                UserManager um = (UserManager)target;

                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField("Users:");

                foreach (IUserObject user in um.userObjects.Values)
                {
                    if (um.LocalUserAssigned && user.IsLocalUser)
                    {
                        GUI.color = Color.green;
                    }
                    else
                    {
                        GUI.color = Color.white;
                    }
                    GUILayout.Button(user.name, UnityEditor.EditorStyles.miniButton);
                }

                UnityEditor.EditorGUILayout.EndVertical();
            }
        }
#endif
    }
}

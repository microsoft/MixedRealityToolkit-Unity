using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public class UserManager : MixedRealityManager, IMixedRealityManager, IUserManagerServer, IStatePipeOutputSource, IUserSlots
    {
        public string Name { get { return "UserManager"; } }

        public uint Priority { get { return 0; } }

        public AppRoleEnum AppRole { get; set; }

        public bool AllSlotsFilled
        {
            get
            {
                foreach (UserSlot slot in appState.GetStates<UserSlot>())
                {
                    if (slot.FillState == UserSlot.FillStateEnum.Empty)
                        return false;
                }
                return true;
            }
        }

        public bool AllUsersInitialized
        {
            get
            {
                foreach (UserSlot slot in appState.GetStates<UserSlot>())
                {
                    if (slot.FillState == UserSlot.FillStateEnum.Empty)
                        return false;

                    IUserObject userObject = null;
                    if (!GetUserObject(slot.ItemNum, out userObject) || !userObject.HasRole)
                        return false;
                }

                return true;
            }
        }

        public IEnumerable<IUserObject> UserObjects { get { return userObjects; } }

        public bool LocalUserSpawned
        {
            get
            {
                switch (AppRole)
                {
                    case AppRoleEnum.Server:
                        return false;

                    default:
                        return localUserObject != null;
                }
            }
        }

        public sbyte LocalUserNum
        {
            get
            {
                switch (AppRole)
                {
                    case AppRoleEnum.Server:
                        return -1;

                    default:
                        return (localUserObject == null) ? (sbyte)-1 : localUserObject.UserNum;
                }
            }
        }

        public ILocalUserObject LocalUserObject
        {
            get
            {
                return localUserObject;
            }
        }

        public IEnumerable<IStatePipeOutput> StatePipeOutputs
        {
            get
            {
                foreach (IUserObject userObject in userObjects)
                {
                    if (userObject != null)
                        yield return userObject.StatePipeOutput;
                }
                yield break;
            }
        }

        private float cleanSlotsInterval = 5f;
        private float nextCleanSlotsTime;
        private List<IUserObject> userObjects = new List<IUserObject>();
        private ILocalUserObject localUserObject;
        private ITimeHandler timeHandler;
        private IAppStateReadWrite appState;
        private List<UserSlot> dirtyUserSlots = new List<UserSlot>();

        public void GenerateUserStates(int numSessions, IEnumerable<StandInSetting> slotTypes)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    throw new Exception("This function should only be executed on a server or host.");
                default:
                    break;
            }

            if (numSessions <= 0)
                throw new Exception("Attempting to generate users with 0 num sessions. Num sessions must be at least 1.");

            sbyte userNum = 1;

            for (sbyte sessionNum = 0; sessionNum < numSessions; sessionNum++)
            {
                foreach (StandInSetting slotType in slotTypes)
                {
                    Debug.Log("Adding user slot: " + slotType.Role + ", " + slotType.Team);
                    UserSlot userSlot = new UserSlot(userNum, sessionNum, slotType.Role, slotType.Team);
                    appState.AddState<UserSlot>(userSlot);
                    userNum++;
                }
            }

            // Get any user state generators in the scene and let them do their thing
            // These will likely be app state source, but they may also be other scripts
            List<IUserStateGenerator> userStateGenerators = new List<IUserStateGenerator>();
            SceneScraper.FindAllInScenes<IUserStateGenerator>(userStateGenerators);
            // Sort the list by execution order
            userStateGenerators.Sort(delegate (IUserStateGenerator usg1, IUserStateGenerator usg2) { return usg1.ExecutionOrder.CompareTo(usg2.ExecutionOrder); });
            // Call for all slots
            foreach (UserSlot userSlot in appState.GetStates<UserSlot>())
            {
                foreach (IUserStateGenerator usg in userStateGenerators)
                {
                    usg.GenerateUserStates(userSlot, appState);
                }
            }
        }

        public void ClearUserSlot(sbyte slotNum)
        {
            UserSlot userSlot = appState.GetState<UserSlot>(slotNum);
            userSlot.FillState = UserSlot.FillStateEnum.Empty;
            appState.SetState<UserSlot>(userSlot);
        }

        public void AddAssignedUserObject(IUserObject userObject)
        {
            if (!userObjects.Contains(userObject))
                userObjects.Add(userObject);

            if (userObject.isLocalPlayer)
            {
                Debug.Log("Local player assigned!");
                localUserObject = userObject as ILocalUserObject;
            }
        }

        public void RevokeAssignment(IUserObject userObject)
        {
            if (userObjects.Contains(userObject))
                userObjects.Remove(userObject);

            if (userObject.isLocalPlayer)
            {
                localUserObject = null;
            }
        }

        public void Update()
        {
            // Remove all destroyed user objects
            // Assign action handler for those that aren't destroyed
            for (int i = userObjects.Count - 1; i >= 0; i--)
            {
                if (userObjects[i] == null || userObjects[i].IsDestroyed)
                {
                    userObjects.RemoveAt(i);
                }
            }
        }

        private void CleanUserSlots()
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    throw new Exception("This function should only be executed on a server or host.");
                default:
                    break;
            }

            dirtyUserSlots.Clear();

            foreach (UserSlot userSlot in appState.GetStates<UserSlot>())
            {
                switch (userSlot.FillState)
                {
                    case UserSlot.FillStateEnum.Filled:
                        IUserObject userObject = null;
                        if (!GetUserObject(userSlot.ItemNum, out userObject))
                        {
                            UserSlot dirtyUserSlot = userSlot;
                            dirtyUserSlot.FillState = UserSlot.FillStateEnum.Empty;
                            dirtyUserSlots.Add(dirtyUserSlot);
                            Debug.Log("Couldn't find user object for slot " + userSlot.ItemNum + " - setting to empty");
                        }
                        break;

                    default:
                        break;
                }
            }

            for (int i = 0; i < dirtyUserSlots.Count; i++)
            {
                appState.SetState<UserSlot>(dirtyUserSlots[i]);
            }
        }

        public bool TryAssignUserToSlot(IUserObject userObject, sbyte slotNum, UserDeviceEnum userDevice, UserSlot.FillStateEnum state = UserSlot.FillStateEnum.Filled)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    throw new Exception("This function should only be executed on a server or host.");
                default:
                    break;
            }

            if (userObject.HasRole)
                throw new Exception("Can't reserve user slot for initialized user.");

            UserSlot slot = appState.GetState<UserSlot>(slotNum);

            if (slot.FillState != UserSlot.FillStateEnum.Empty)
            {
                Debug.Log("Slot exists but state is " + slot.FillState);
                return false;
            }

            Debug.Log("Filled slot " + slot.UserRole + " " + slot.ItemNum + " on request");
            slot.FillState = state;
            appState.SetState<UserSlot>(slot);
            // Initialize our user object and add to our user object list
            userObject.AssignUserRole(slot.UserRole, slot.UserTeam, userDevice, slotNum);
            userObjects.Add(userObject);
            return true;
        }

        public bool SetSlotIgnored(sbyte slotNum, bool ignore)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    throw new Exception("This function should only be executed on a server or host.");
                default:
                    break;
            }

            UserSlot slot = appState.GetState<UserSlot>(slotNum);
            slot.FillState = ignore ? UserSlot.FillStateEnum.Ignore : UserSlot.FillStateEnum.Empty;
            appState.SetState<UserSlot>(slot);
            return true;
        }

        public bool GetUserObject(short userNum, out IUserObject userObject)
        {
            userObject = null;

            for (int i = userObjects.Count - 1; i >= 0; i--)
            {
                if (userObjects[i] == null || userObjects[i].IsDestroyed)
                {
                    userObjects.RemoveAt(i);
                    continue;
                }

                if (userObjects[i].UserNum == userNum)
                {
                    userObject = userObjects[i];
                    return true;
                }
            }

            return false;
        }

        public void OnSharingStart()
        {
            SceneScraper.FindInScenes<IAppStateReadWrite>(out appState);
        }

        public void OnStateInitialized() { }

        public void OnSharingStop() { }

        public bool TryAssignDeviceToSlot(IUserDevice device, sbyte slotNum)
        {
            IUserObject userObject = (IUserObject)device.gameObject.GetComponent(typeof(IUserObject));

            if (userObject == null)
                throw new Exception("Couldn't get IUserDevice from " + device.name);

            return TryAssignUserToSlot(userObject, slotNum, device.DeviceType);
        }

        public void RevokeAssignment(IUserDevice device, sbyte slotNum)
        {
            IUserObject userObject = (IUserObject)device.gameObject.GetComponent(typeof(IUserObject));

            if (userObject == null)
                throw new Exception("Couldn't get IUserDevice from " + device.name);

            if (!userObject.HasRole)
                throw new Exception("Can't revoke role from user object - has no role!");

            UserSlot slot = appState.GetState<UserSlot>(slotNum);

            if (slot.FillState != UserSlot.FillStateEnum.Filled)
                throw new Exception("Can't revoke assignment - slot is not filled!");

            userObject.RevokeUserRole();

            slot.FillState = UserSlot.FillStateEnum.Empty;
            appState.SetState<UserSlot>(slot);
        }

        public void Initialize() { }

        public void Reset() { }

        public void Enable() { }

        public void Disable() { }

        public void Destroy() { }

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

                foreach (IUserObject user in um.userObjects)
                {
                    if (user.isLocalPlayer)
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

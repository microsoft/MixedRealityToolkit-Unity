using Pixie.Core;
using Pixie.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.DeviceControl.Users
{
    public class AppStateDevicesAndUsers : MonoBehaviour, IAppStateSource, IUserStateGenerator
    {
        public IEnumerable<Type> StateTypes { get { return new Type[] {
            typeof(UserSlot),
            typeof (UserDeviceState),
            typeof(LocalTransformState),
            typeof(WorldTransformState)
        }; } }

        public int ExecutionOrder
        {
            get { return 2; }
        }

        [SerializeField]
        private int numConcurrentSessions = 1;
        private short transformNum;

        public void GenerateRequiredStates(IAppStateReadWrite appState) { }

        public void GenerateUserStates(UserSlot slot, IAppStateReadWrite appState)
        {
            Debug.Log("Generating transform user states for slot " + slot.UserID);

            foreach (TransformTypeEnum transformType in slot.Transforms)
            {
                appState.AddState<LocalTransformState>(new LocalTransformState(transformNum, transformType, slot.UserID));
                appState.AddState<WorldTransformState>(new WorldTransformState(transformNum, transformType, slot.UserID));
                transformNum++;
            }
        }
    }
}
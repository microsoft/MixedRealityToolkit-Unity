using Newtonsoft.Json;
using Pixie.Core;
using Pixie.Initialization;
using Pixie.StateControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.DeviceControl
{
    public class LocalDevicePrefs : MonoBehaviourSharingApp, ILocalDevicePrefs
    {
        public const string DevicePrefsKeyPrefix = "Pixie.LocalDevicePrefs.";
        
        private IDeviceSource deviceSources;
        private IDeviceAssigner deviceAssigner;
        private IAppStateReadWrite appState;

        public void ClearPrefs()
        {
            string prefsKey = GetPrefsKey(DeviceType);

            if (PlayerPrefs.HasKey(prefsKey))
                PlayerPrefs.DeleteKey(prefsKey);
        }

        public void SavePrefs()
        {
            if (!deviceSources.LocalDeviceConnected)
                return;

            if (!appState.StateExists<UserDeviceState>(deviceSources.LocalDeviceID))
                return;

            UserDeviceState deviceState = appState.GetState<UserDeviceState>(deviceSources.LocalDeviceID);

            if (deviceState.UserID < 0)
                return;

            UserSlot userSlot = appState.GetState<UserSlot>(deviceState.UserID);

            // We've got a valid configuration
            // Create a new preference state and save it
            LocalDevicePrefsState prefs = new LocalDevicePrefsState();
            prefs.UserID = deviceState.UserID;
            prefs.DeviceRole = userSlot.DeviceRoles[deviceState.DeviceRoleIndex];

            SaveAssignment(prefs);
        }

        public bool GetSavedPrefs(out LocalDevicePrefsState prefs)
        {
            prefs = default(LocalDevicePrefsState);

            string devicePrefsKey = GetPrefsKey(DeviceType);
            if (PlayerPrefs.HasKey(devicePrefsKey))
            {
                string devicePrefsString = PlayerPrefs.GetString(devicePrefsKey);
                prefs = JsonConvert.DeserializeObject<LocalDevicePrefsState>(devicePrefsString);
                return true;
            }

            return false;
        }

        public override void OnAppInitialize()
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    break;

                default:
                    throw new Exception("This component is not intended for use on " + AppRole);
            }

            ComponentFinder.FindInScenes<IDeviceSource>(out deviceSources);
            ComponentFinder.FindInScenes<IDeviceAssigner>(out deviceAssigner);
            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);

            deviceSources.OnDeviceConnected += OnDeviceConnected;
        }

        private void OnDeviceConnected(short deviceID, string deviceName, bool isLocalDevice, Dictionary<string, string> properties)
        {
            if (isLocalDevice)
            {
                // Our local device has connected!
                LocalDevicePrefsState prefs;
                if (GetSavedPrefs(out prefs))
                {
                    // We don't have a saved assignemnt
                    // So we'll wait for one and save it to prefs
                    StartCoroutine(SuggestAssignment(deviceID, prefs));
                }
                else
                {
                    // We have a saved assignment
                    // We'll wait for the device state to appear
                    // then requirest that assignment again
                    StartCoroutine(WaitForAssignment(deviceID));
                }
            }
        }

        private IEnumerator WaitForAssignment(short deviceID)
        {
            // Wait until a device state with this device ID shows up in our app state
            while (!appState.StateExists<UserDeviceState>(deviceID))
                yield return new WaitForSeconds(0.5f);

            UserDeviceState deviceState = appState.GetState<UserDeviceState>(deviceID);
            
            while (deviceState.UserID < 0)
            {
                deviceState = appState.GetState<UserDeviceState>(deviceID);
                yield return new WaitForSeconds(0.5f);
            }

            SavePrefs();

            // All done!
            yield break;
        }

        private IEnumerator SuggestAssignment(short deviceID, LocalDevicePrefsState prefs)
        {
            // Wait until a device state with this device ID shows up in our app state
            while (!appState.StateExists<UserDeviceState>(deviceID))
                yield return new WaitForSeconds(0.5f);

            while (!appState.StateExists<UserSlot>(prefs.UserID))
                yield return new WaitForSeconds(0.5f);

            deviceAssigner.TryAssignDevice(deviceID, prefs.UserID, prefs.DeviceRole);
            yield break;
        }

        private void SaveAssignment(LocalDevicePrefsState prefs)
        {
            string devicePrefsKey = GetPrefsKey(DeviceType);

            string devicePrefsString = JsonConvert.SerializeObject(prefs);
            PlayerPrefs.SetString(devicePrefsKey, devicePrefsString);
        }

        private static string GetPrefsKey(DeviceTypeEnum deviceType)
        {
            return DevicePrefsKeyPrefix + deviceType.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(LocalDevicePrefs))]
        public class LocalDevicePrefsEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                LocalDevicePrefs ldf = (LocalDevicePrefs)target;

                if (ldf.deviceSources == null || !ldf.deviceSources.LocalDeviceConnected)
                    return;
               
                LocalDevicePrefsState prefs;
                if (ldf.GetSavedPrefs(out prefs))
                {
                    UnityEditor.EditorGUILayout.LabelField(LocalDevicePrefs.GetPrefsKey(ldf.DeviceType), UnityEditor.EditorStyles.miniLabel);
                    UnityEditor.EditorGUILayout.LabelField("UserID: " + prefs.UserID);
                    UnityEditor.EditorGUILayout.LabelField("DeviceRole: " + prefs.DeviceRole);
                }
                else
                {
                    UnityEditor.EditorGUILayout.LabelField("(No saved prefs exist)");
                }

                if (GUILayout.Button("Save Assignment"))
                    ldf.SavePrefs();

                if (GUILayout.Button("Clear Prefs"))
                    ldf.ClearPrefs();
            }
        }
#endif
    }
}
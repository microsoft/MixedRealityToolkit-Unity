using Pixie.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.DeviceControl
{
    [ExecuteInEditMode]
    public class LightingControl : MonoBehaviour, ILightingControl
    {
        [SerializeField]
        private LightingControlSetting[] settings;
        [SerializeField]
        private int settingIndex = 0;
        [SerializeField]
        private Transform skyboxTransform;
        private Light sunLight;
        private List<Light> directionalLights = new List<Light>();
        private LightingControlSetting setting;

        public void SetDevice(DeviceTypeEnum device)
        {
            for (int i = 0; i < settings.Length; i++)
            {
                if (settings[i].Device == device)
                {
                    settingIndex = i;
                    UpdateLighting();
                    return;
                }
            }

            Debug.LogWarning("No setting found for device type " + device);
        }

        private void OnEnable()
        {
            directionalLights.Clear();
            gameObject.GetComponentsInChildren<Light>(true, directionalLights);
        }

        private void Start()
        {
            UpdateLighting();
        }

        private void Update()
        {
            UpdateSkybox();

            if (Application.isPlaying)
                return;

            UpdateLighting();
        }

        private void UpdateSkybox()
        {
            if (settings.Length == 0)
                return;

            if (settingIndex < 0 || settingIndex >= settings.Length)
                settingIndex = 0;

            setting = settings[settingIndex];

            // In the editor we update the main camera directly
            // In the app we let UserCamera / AppCamera handle the camera settings
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.clearFlags = setting.CameraClearFlags;
                mainCamera.backgroundColor = setting.CameraClearColor;
                skyboxTransform.position = mainCamera.transform.position;
            }
        }

        private void UpdateLighting()
        {
            if (settings.Length == 0)
                return;

            if (settingIndex < 0 || settingIndex >= settings.Length)
                settingIndex = 0;

            setting = settings[settingIndex];

            int numLights = 0;
            foreach (LightingControlSetting.DirectionalLight lightSetting in setting.DirectionalLights)
            {
                if (directionalLights.Count - 1 < numLights)
                {
                    GameObject newLightGo = new GameObject("Directional Light " + numLights);
                    newLightGo.transform.parent = transform;
                    Light newLight = newLightGo.AddComponent<Light>();
                    directionalLights.Add(newLight);
                }

                Light currentLight = directionalLights[numLights];
                currentLight.gameObject.SetActive(true);
                currentLight.intensity = lightSetting.Intensity;
                currentLight.transform.eulerAngles = lightSetting.Rotation;
                currentLight.transform.position = Vector3.up * 2 + (Vector3.right * numLights);
                currentLight.type = LightType.Directional;
                currentLight.color = lightSetting.Color;
                currentLight.shadows = lightSetting.Shadows;

                numLights++;
            }

            if (directionalLights.Count > 0)
            {
                sunLight = directionalLights[0];

                for (int i = numLights; i < directionalLights.Count; i++)
                {
                    directionalLights[i].gameObject.SetActive(false);
                }
            }
            else
            {
                sunLight = null;
            }

            RenderSettings.ambientEquatorColor = setting.AmbientEquatorColor;
            RenderSettings.ambientGroundColor = setting.AmbientGroundColor;
            RenderSettings.ambientSkyColor = setting.AmbientSkyColor;
            RenderSettings.ambientIntensity = setting.AmbientIntensity;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.defaultReflectionMode = setting.ReflectionMode;
            RenderSettings.reflectionIntensity = setting.ReflectionIntensity;
            RenderSettings.customReflection = setting.ReflectionCubeMap;
            RenderSettings.skybox = setting.SkyboxMaterial;
            RenderSettings.sun = sunLight;

            RenderSettings.reflectionBounces = 0;
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(LightingControl))]
        public class LightingControlEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                LightingControl lc = (LightingControl)target;

                if (lc.settings.Length == 0)
                {
                    UnityEditor.EditorGUILayout.LabelField("No settings added. You need to add some settings to the array.");
                    base.OnInspectorGUI();
                    return;
                }

                List<string> settingOptions = new List<string>();
                foreach (LightingControlSetting setting in lc.settings)
                {
                    if (setting == null)
                    {
                        UnityEditor.EditorGUILayout.LabelField("Null setting found in array.");
                        base.OnInspectorGUI();
                        return;
                    }

                    settingOptions.Add(setting.name);
                }

                lc.settingIndex = UnityEditor.EditorGUILayout.Popup("Setting", lc.settingIndex, settingOptions.ToArray());

                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.Editor settingEditor = UnityEditor.Editor.CreateEditor(lc.settings[lc.settingIndex]);
                settingEditor.OnInspectorGUI();
                UnityEditor.EditorGUI.indentLevel--;
                UnityEditor.EditorGUILayout.EndVertical();

                base.OnInspectorGUI();

                UnityEditor.EditorUtility.SetDirty(lc.settings[lc.settingIndex]);
                UnityEditor.EditorUtility.SetDirty(target);
            }
        }
#endif
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl
{
    [CreateAssetMenu(fileName = "LightingControlSettings", menuName = "Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem/LightingControlSettings")]
    public class LightingControlSetting : ScriptableObject
    {
        public UserDeviceEnum Device { get { return device; } }
        public CameraClearFlags CameraClearFlags { get { return cameraClearFlags; } }
        public Color CameraClearColor { get { return cameraClearColor; } }
        public Color AmbientSkyColor { get { return ambientSkyColor; } }
        public Color AmbientEquatorColor { get { return ambientEquatorColor; } }
        public Color AmbientGroundColor { get { return ambientGroundColor; } }
        public float AmbientIntensity { get { return ambientIntensity; } }
        public Material SkyboxMaterial { get { return skyboxMaterial; } }
        public Cubemap ReflectionCubeMap { get { return reflectionCubeMap; } }
        public DefaultReflectionMode ReflectionMode { get { return reflectionMode; } }
        public IEnumerable<DirectionalLight> DirectionalLights { get { return directionalLights; } }
        public float ReflectionIntensity { get { return reflectionIntensity; } }

        [Serializable]
        public struct DirectionalLight
        {
            public Vector3 Rotation;
            [ColorUsage(false, true)]
            public Color Color;
            [Range(0, 8)]
            public float Intensity;
            public LightShadows Shadows;
        }

        [Header("Device Type")]
        private UserDeviceEnum device = UserDeviceEnum.Unknown;
        [Header("Camera Settings")]
        [SerializeField]
        private CameraClearFlags cameraClearFlags = CameraClearFlags.SolidColor;
        [SerializeField]
        private Color cameraClearColor = Color.black;
        [Header("Ambient Lighting")]
        [ColorUsage(false, true)]
        [SerializeField]
        private Color ambientSkyColor = Color.gray;
        [ColorUsage(false, true)]
        [SerializeField]
        private Color ambientEquatorColor = Color.gray;
        [ColorUsage(false, true)]
        [SerializeField]
        private Color ambientGroundColor = Color.gray;
        [Range(0, 1)]
        [SerializeField]
        private float ambientIntensity = 1;
        [Header("Skybox / Reflection settings")]
        [SerializeField]
        private Material skyboxMaterial;
        [SerializeField]
        private Cubemap reflectionCubeMap;
        [SerializeField]
        [Range(0f, 1f)]
        private float reflectionIntensity = 1f;
        [SerializeField]
        private DefaultReflectionMode reflectionMode = DefaultReflectionMode.Custom;
        [Header("Directional Lights (first light is used as sunlight)")]
        [SerializeField]
        private DirectionalLight[] directionalLights;
    }
}
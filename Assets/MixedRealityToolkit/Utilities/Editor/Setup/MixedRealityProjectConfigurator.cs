// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MixedRealityProjectConfigurator
{
    private const int SpatialAwarenessDefaultLayer = 31;

    public enum Configurations
    {
        LatestScriptingRuntime = 1,
        ForceTextSerialization,
        VisibleMetaFiles,
        VirtualRealitySupported,
        SinglePassInstancing,
        SpatialAwarenessLayer,

        // WSA Capabilities
        SpatialPerceptionCapability,
        MicrophoneCapability,
        InternetClientCapability,
        EyeTrackingCapability,
    };

    private static Dictionary<Configurations, Func<bool>> ConfigurationGetters = new Dictionary<Configurations, Func<bool>>()
    {
        { Configurations.LatestScriptingRuntime,  () => { return IsLatestScriptingRuntime(); } },
        { Configurations.ForceTextSerialization,  () => { return IsForceTextSerialization(); } },
        { Configurations.VisibleMetaFiles,  () => { return IsVisibleMetaFiles(); } },
        { Configurations.VirtualRealitySupported,  () => { return PlayerSettings.virtualRealitySupported; } },
        { Configurations.SinglePassInstancing,  () => { return MixedRealityOptimizeUtils.IsSinglePassInstanced(); } },
        { Configurations.SpatialAwarenessLayer,  () => { return HasSpatialAwarenessLayer(); } },

        { Configurations.SpatialPerceptionCapability,  () => { return PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.SpatialPerception); } },
        { Configurations.MicrophoneCapability,  () => { return PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.Microphone); } },
        { Configurations.InternetClientCapability,  () => { return PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.InternetClient); } },
    };

    private static Dictionary<Configurations, Action> ConfigurationSetters = new Dictionary<Configurations, Action>()
    {
        { Configurations.LatestScriptingRuntime,  () => { SetLatestScriptingRuntime(); } },
        { Configurations.ForceTextSerialization,  () => { SetForceTextSerialization(); } },
        { Configurations.VisibleMetaFiles,  () => { SetVisibleMetaFiles(); } },
        { Configurations.VirtualRealitySupported,  () => { PlayerSettings.virtualRealitySupported = true; } },
        { Configurations.SinglePassInstancing,  () => { MixedRealityOptimizeUtils.SetSinglePassInstanced(); } },
        { Configurations.SpatialAwarenessLayer,  () => { SetSpatialAwarenessLayer(); } },

        { Configurations.SpatialPerceptionCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, true); } },
        { Configurations.MicrophoneCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.Microphone, true); } },
        { Configurations.InternetClientCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true); } },
    };

    public static bool IsConfigured(Configurations config)
    {
        if (ConfigurationGetters.ContainsKey(config))
        {
            return ConfigurationGetters[config].Invoke();
        }

        return false;
    }

    public static void Configure(Configurations config)
    {
        if (ConfigurationSetters.ContainsKey(config))
        {
            ConfigurationSetters[config].Invoke();
        }
    }

    public static bool IsProjectConfigured()
    {
        foreach (var getter in ConfigurationGetters)
        {
            if (!getter.Value.Invoke())
            {
                return false;
            }
        }

        return true;
    }

    public static void ConfigureProject(HashSet<Configurations> filter = null)
    {
        if (filter == null)
        {
            foreach (var setter in ConfigurationSetters)
            {
                setter.Value.Invoke();
            }
        }
        else
        {
            foreach (var key in filter)
            {
                if (ConfigurationSetters.ContainsKey(key))
                {
                    ConfigurationSetters[key].Invoke();
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    public static bool IsLatestScriptingRuntime()
    {
        return PlayerSettings.scriptingRuntimeVersion == ScriptingRuntimeVersion.Latest;
    }

    public static void SetLatestScriptingRuntime()
    {
        PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
    }

    public static bool IsForceTextSerialization()
    {
        return EditorSettings.serializationMode == SerializationMode.ForceText;
    }

    public static void SetForceTextSerialization()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
    }

    public static bool IsVisibleMetaFiles()
    {
        return EditorSettings.externalVersionControl.Equals("Visible Meta Files");
    }

    public static void SetVisibleMetaFiles()
    {
        EditorSettings.externalVersionControl = "Visible Meta Files";
    }

    public static bool HasSpatialAwarenessLayer()
    {
        return !string.IsNullOrEmpty(LayerMask.LayerToName(SpatialAwarenessDefaultLayer));
    }

    public static void SetSpatialAwarenessLayer()
    {
        if (!HasSpatialAwarenessLayer())
        {
            if (EditorLayerExtensions.SetupLayer(SpatialAwarenessDefaultLayer, "Spatial Awareness"))
            {
                Debug.LogWarning(string.Format($"Can't modify project layers. It's possible the format of the layers and tags data has changed in this version of Unity. Set layer {SpatialAwarenessDefaultLayer} to \"Spatial Awareness\" manually via Project Settings > Tags and Layers window."));
            }
        }
    }

    /// <summary>
    /// Discover and set the appropriate XR Settings for the current build target.
    /// </summary>
    public static void ApplyXRSettings()
    {
        BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        List<string> targetSDKs = new List<string>();
        foreach (string sdk in PlayerSettings.GetAvailableVirtualRealitySDKs(targetGroup))
        {
            if (sdk.Contains("OpenVR") || sdk.Contains("Windows"))
            {
                targetSDKs.Add(sdk);
            }
        }

        if (targetSDKs.Count != 0)
        {
            PlayerSettings.SetVirtualRealitySDKs(targetGroup, targetSDKs.ToArray());
            PlayerSettings.SetVirtualRealitySupported(targetGroup, true);
        }
    }

}

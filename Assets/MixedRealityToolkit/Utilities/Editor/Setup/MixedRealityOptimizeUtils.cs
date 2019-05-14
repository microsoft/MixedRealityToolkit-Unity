using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class MixedRealityOptimizeUtils
{

    public static void SetDepthBufferSharing(bool enableDepthBuffer, bool set16BitDepthBuffer)
    {
        PlayerSettings.VROculus.sharedDepthBuffer = enableDepthBuffer;

#if UNITY_2019
                PlayerSettings.VRWindowsMixedReality.depthBufferSharingEnabled = enableDepthBuffer;

                PlayerSettings.VRWindowsMixedReality.depthBufferFormat = set16BitDepthBuffer ? 
                    PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat16Bit :
                    PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat24Bit;
#else
        var playerSettings = GetSettingsObject("PlayerSettings");

            MixedRealityOptimizeUtils.ChangeProperty(playerSettings, 
                "vrSettings.hololens.depthBufferSharingEnabled", 
                property => property.boolValue = enableDepthBuffer);

            MixedRealityOptimizeUtils.ChangeProperty(playerSettings, 
                "vrSettings.hololens.depthFormat", 
                property => property.intValue = set16BitDepthBuffer ? 0 : 1);
#endif
    }

    public static void ChangeProperty(SerializedObject target, string name, Action<SerializedProperty> changer)
    {
        var prop = target.FindProperty(name);
        if (prop != null)
        {
            changer(prop);
            target.ApplyModifiedProperties();
        }
        else Debug.LogError("property not found: " + name);
    }

    public static SerializedObject GetSettingsObject(string className)
    {
        var settings = Unsupported.GetSerializedAssetInterfaceSingleton(className);
        return new SerializedObject(settings);
    }

    public static SerializedObject GetLighmapSettings()
    {
        var getLightmapSettingsMethod = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic);
        var lightmapSettings = getLightmapSettingsMethod.Invoke(null, null) as UnityEngine.Object;
        return new SerializedObject(lightmapSettings);
    }

}

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEditor;

public class BaseMixedRealityToolkitRuntimePlatformConfigurationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
{
    protected static string[] runtimePlatformNames;
    protected static Type[] runtimePlatformTypes;
    protected static int[] runtimePlatformMasks;

    protected void GatherSupportedPlatforms(SerializedProperty serializedProperty)
    {
        runtimePlatformTypes = IPlatformSupportExtension.GetSupportedPlatformTypes();
        runtimePlatformNames = IPlatformSupportExtension.GetSupportedPlatformNames();

        runtimePlatformMasks = new int[serializedProperty.arraySize];
        SerializedProperty supportedPlatformsArray;
        string platformName;
        for (int i = 0; i < serializedProperty.arraySize; i++)
        {
            supportedPlatformsArray = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("runtimePlatform");

            for (int j = 0; j < runtimePlatformTypes.Length; j++)
            {
                platformName = SystemType.GetReference(runtimePlatformTypes[j]);
                for (int k = 0; k < supportedPlatformsArray.arraySize; k++)
                {
                    if (platformName.Equals(supportedPlatformsArray.GetArrayElementAtIndex(k).FindPropertyRelative("reference").stringValue))
                    {
                        runtimePlatformMasks[i] |= 1 << j;
                    }
                }
            }
        }
    }

    protected void ApplyMaskToProperty(SerializedProperty runtimePlatform, int runtimePlatformBitMask)
    {
        runtimePlatform.arraySize = MathExtensions.CountBits(runtimePlatformBitMask);
        int arrayIndex = 0;
        for (int i = 0; i < runtimePlatformTypes.Length; i++)
        {
            if ((runtimePlatformBitMask & 1 << i) != 0)
            {
                runtimePlatform.GetArrayElementAtIndex(arrayIndex++).FindPropertyRelative("reference").stringValue = SystemType.GetReference(runtimePlatformTypes[i]);
            }
        }
    }

    protected void RenderSupportedPlatforms(SerializedProperty runtimePlatform, int index, UnityEngine.GUIContent runtimePlatformContent = null)
    {
         runtimePlatformMasks[index] = EditorGUILayout.MaskField(runtimePlatformContent, runtimePlatformMasks[index], runtimePlatformNames);
        ApplyMaskToProperty(runtimePlatform, runtimePlatformMasks[index]);
    }
}

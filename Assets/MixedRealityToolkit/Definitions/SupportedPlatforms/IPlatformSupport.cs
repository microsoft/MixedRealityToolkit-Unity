using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IPlatformSupport
{
    bool Check();

#if UNITY_EDITOR
    bool Check(BuildTarget buildTarget);
#endif
    bool Check(RuntimePlatform runtimePlatform);
}

public static class IPlatformSupportExtension
{
    public static bool IsPlatformSupported(this IPlatformSupport[] platformSupports)
    {
        foreach (var item in platformSupports)
        {
            if (item.Check())
                return true;
        }

        return false;
    }

    public static IPlatformSupport[] Convert(this SystemType[] systemTypes)
    {
        var supportedPlatforms = new IPlatformSupport[systemTypes.Length];

        for (int i = 0; i < systemTypes.Length; i++)
        {
            supportedPlatforms[i] = Activator.CreateInstance(systemTypes[i]) as IPlatformSupport;
        }

        return supportedPlatforms;
    }

    public static SystemType[] Convert(this IPlatformSupport[] supportedPlatforms)
    {
        var systemTypes = new SystemType[supportedPlatforms.Length];

        for (int i = 0; i < supportedPlatforms.Length; i++)
        {
            systemTypes[i] = new SystemType(supportedPlatforms[i].GetType());
        }

        return systemTypes;
    }
}

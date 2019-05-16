using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IPlatformSupport
{
    bool IsThisEditorOrRuntimePlatform();

    bool IsThisTheCurrentRuntimePlatform(RuntimePlatform runtimePlatform);
}

public static class IPlatformSupportExtension
{
    public static bool IsPlatformSupported(this IPlatformSupport[] platformSupports)
    {
        if (platformSupports == null)
        {
            return true;
        }

        foreach (var item in platformSupports)
        {
            if (item.IsThisEditorOrRuntimePlatform())
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

    public static Type[] GetSupportedPlatformTypes()
    {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where typeof(IPlatformSupport).IsAssignableFrom(type)
                where type != typeof(IPlatformSupport)
                orderby type.Name
                select type).ToArray();
    }

    public static string[] GetSupportedPlatformNames()
    {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where typeof(IPlatformSupport).IsAssignableFrom(type)
                where type != typeof(IPlatformSupport)
                orderby type.Name
                select type.Name).ToArray();
    }
}

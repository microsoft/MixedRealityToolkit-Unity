// ----------------------------------------------------------------------------
// <copyright file="PhotonConverter.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Script to convert old RPC attributes into new RPC attributes.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 || UNITY_5_4_OR_NEWER
#define UNITY_MIN_5_3
#endif


using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PhotonConverter : Photon.MonoBehaviour
{
    public static List<string> GetScriptsInFolder(string folder)
    {
        List<string> scripts = new List<string>();

        try
        {
            scripts.AddRange(Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories));
            scripts.AddRange(Directory.GetFiles(folder, "*.js", SearchOption.AllDirectories));
            scripts.AddRange(Directory.GetFiles(folder, "*.boo", SearchOption.AllDirectories));
        }
        catch (System.Exception ex)
        {
            Debug.Log("Getting script list from folder " + folder + " failed. Exception:\n" + ex.ToString());
        }

        return scripts;
    }

    ///  default path: "Assets"
    public static void ConvertRpcAttribute(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }

        List<string> scripts = GetScriptsInFolder(path);
        foreach (string file in scripts)
        {
            string text = File.ReadAllText(file);
            string textCopy = text;
            if (file.EndsWith("PhotonConverter.cs"))
            {
                continue;
            }

            text = text.Replace("[RPC]", "[PunRPC]");
            text = text.Replace("@RPC", "@PunRPC");

            if (!text.Equals(textCopy))
            {
                File.WriteAllText(file, text);
                Debug.Log("Converted RPC to PunRPC in: " + file);
            }
        }
    }
}
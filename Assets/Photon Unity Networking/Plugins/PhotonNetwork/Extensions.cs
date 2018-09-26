// ----------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Provides some helpful methods and extensions for Hashtables, etc.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using SupportClassPun = ExitGames.Client.Photon.SupportClass;


/// <summary>
/// This static class defines some useful extension methods for several existing classes (e.g. Vector3, float and others).
/// </summary>
public static class Extensions
{

    public static Dictionary<MethodInfo, ParameterInfo[]> parametersOfMethods = new Dictionary<MethodInfo, ParameterInfo[]>();
    public static ParameterInfo[] GetCachedParemeters(this MethodInfo mo)
    {
        ParameterInfo[] result;
        bool cached= parametersOfMethods.TryGetValue(mo, out result);

        if (!cached)
        {
            result =  mo.GetParameters();
            parametersOfMethods[mo] = result;
        }

        return result;
    }

    public static PhotonView[] GetPhotonViewsInChildren(this UnityEngine.GameObject go)
    {
        return go.GetComponentsInChildren<PhotonView>(true) as PhotonView[];
    }

    public static PhotonView GetPhotonView(this UnityEngine.GameObject go)
    {
        return go.GetComponent<PhotonView>() as PhotonView;
    }

    /// <summary>compares the squared magnitude of target - second to given float value</summary>
    public static bool AlmostEquals(this Vector3 target, Vector3 second, float sqrMagnitudePrecision)
    {
        return (target - second).sqrMagnitude < sqrMagnitudePrecision;  // TODO: inline vector methods to optimize?
    }

    /// <summary>compares the squared magnitude of target - second to given float value</summary>
    public static bool AlmostEquals(this Vector2 target, Vector2 second, float sqrMagnitudePrecision)
    {
        return (target - second).sqrMagnitude < sqrMagnitudePrecision;  // TODO: inline vector methods to optimize?
    }

    /// <summary>compares the angle between target and second to given float value</summary>
    public static bool AlmostEquals(this Quaternion target, Quaternion second, float maxAngle)
    {
        return Quaternion.Angle(target, second) < maxAngle;
    }

    /// <summary>compares two floats and returns true of their difference is less than floatDiff</summary>
    public static bool AlmostEquals(this float target, float second, float floatDiff)
    {
        return Mathf.Abs(target - second) < floatDiff;
    }

    /// <summary>
    /// Merges all keys from addHash into the target. Adds new keys and updates the values of existing keys in target.
    /// </summary>
    /// <param name="target">The IDictionary to update.</param>
    /// <param name="addHash">The IDictionary containing data to merge into target.</param>
    public static void Merge(this IDictionary target, IDictionary addHash)
    {
        if (addHash == null || target.Equals(addHash))
        {
            return;
        }

        foreach (object key in addHash.Keys)
        {
            target[key] = addHash[key];
        }
    }

    /// <summary>
    /// Merges keys of type string to target Hashtable.
    /// </summary>
    /// <remarks>
    /// Does not remove keys from target (so non-string keys CAN be in target if they were before).
    /// </remarks>
    /// <param name="target">The target IDicitionary passed in plus all string-typed keys from the addHash.</param>
    /// <param name="addHash">A IDictionary that should be merged partly into target to update it.</param>
    public static void MergeStringKeys(this IDictionary target, IDictionary addHash)
    {
        if (addHash == null || target.Equals(addHash))
        {
            return;
        }

        foreach (object key in addHash.Keys)
        {
            // only merge keys of type string
            if (key is string)
            {
                target[key] = addHash[key];
            }
        }
    }

    /// <summary>
    /// Returns a string-representation of the IDictionary's content, inlcuding type-information.
    /// Note: This might turn out a "heavy-duty" call if used frequently but it's usfuly to debug Dictionary or Hashtable content.
    /// </summary>
    /// <param name="origin">Some Dictionary or Hashtable.</param>
    /// <returns>String of the content of the IDictionary.</returns>
    public static string ToStringFull(this IDictionary origin)
    {
        return SupportClassPun.DictionaryToString(origin, false);
    }

    /// <summary>
    /// This method copies all string-typed keys of the original into a new Hashtable.
    /// </summary>
    /// <remarks>
    /// Does not recurse (!) into hashes that might be values in the root-hash.
    /// This does not modify the original.
    /// </remarks>
    /// <param name="original">The original IDictonary to get string-typed keys from.</param>
    /// <returns>New Hashtable containing only string-typed keys of the original.</returns>
    public static Hashtable StripToStringKeys(this IDictionary original)
    {
        Hashtable target = new Hashtable();
        if (original != null)
        {
            foreach (object key in original.Keys)
            {
                if (key is string)
                {
                    target[key] = original[key];
                }
            }
        }

        return target;
    }

    /// <summary>
    /// This removes all key-value pairs that have a null-reference as value.
    /// Photon properties are removed by setting their value to null.
    /// Changes the original passed IDictionary!
    /// </summary>
    /// <param name="original">The IDictionary to strip of keys with null-values.</param>
    public static void StripKeysWithNullValues(this IDictionary original)
    {
        object[] keys = new object[original.Count];
        //original.Keys.CopyTo(keys, 0);                // todo: figure out which platform didn't support this
        int i = 0;
        foreach (object k in original.Keys)
        {
            keys[i++] = k;
        }

        for (int index = 0; index < keys.Length; index++)
        {
            var key = keys[index];
            if (original[key] == null)
            {
                original.Remove(key);
            }
        }
    }

    /// <summary>
    /// Checks if a particular integer value is in an int-array.
    /// </summary>
    /// <remarks>This might be useful to look up if a particular actorNumber is in the list of players of a room.</remarks>
    /// <param name="target">The array of ints to check.</param>
    /// <param name="nr">The number to lookup in target.</param>
    /// <returns>True if nr was found in target.</returns>
    public static bool Contains(this int[] target, int nr)
    {
        if (target == null)
        {
            return false;
        }

        for (int index = 0; index < target.Length; index++)
        {
            if (target[index] == nr)
            {
                return true;
            }
        }

        return false;
    }
}


/// <summary>Small number of extension methods that make it easier for PUN to work cross-Unity-versions.</summary>
public static class GameObjectExtensions
{
    /// <summary>Unity-version-independent replacement for active GO property.</summary>
    /// <returns>Unity 3.5: active. Any newer Unity: activeInHierarchy.</returns>
    public static bool GetActive(this GameObject target)
    {
        #if UNITY_3_5
        return target.active;
        #else
        return target.activeInHierarchy;
        #endif
    }

    #if UNITY_3_5
    /// <summary>Unity-version-independent setter for active and SetActive().</summary>
    public static void SetActive(this GameObject target, bool value)
    {
        target.active = value;
    }
    #endif
}

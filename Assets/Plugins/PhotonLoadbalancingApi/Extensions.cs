// ----------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Provides some helpful methods and extensions for Hashtables, etc.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_1 || UNITY_6_0
#define UNITY
#endif

namespace ExitGames.Client.Photon.LoadBalancing
{
    using System.Collections;

    //#if UNITY_EDITOR || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    //#endif


    public static class Extensions
    {
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
        /// This method copies all string-typed keys of the original into a new Hashtable.
        /// </summary>
        /// <remarks>
        /// Does not recurse (!) into hashes that might be values in the root-hash.
        /// This does not modify the original.
        /// </remarks>
        /// <param name="original">The original IDictonary to get string-typed keys from.</param>
        /// <returns>New Hashtable containing parts ot fht original.</returns>
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
            original.Keys.CopyTo(keys, 0);

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
}
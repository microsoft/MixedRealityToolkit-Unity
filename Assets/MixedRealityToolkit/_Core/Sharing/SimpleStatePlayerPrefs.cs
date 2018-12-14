using System;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Sharing;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Services.Sharing
{
    /// <summary>
    /// A local simulation of state sharing
    /// Uses JSON to store and retrieve serialized objects from PlayerPrefs
    /// Useful for testing state sharing without a connection
    /// </summary>
    public class SimpleStatePlayerPrefs : BaseService, ISimpleState
    {
        public override void Initialize()
        {
            Name = "SimpleStatePlayerPrefs";
        }

        public int SimulatedLatency { get; set; }

        public async Task<bool> HasKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Key cannot be empty.");

            await Task.Delay(SimulatedLatency);

            return PlayerPrefs.HasKey(key);
        }
        
        public async Task<T> GetState<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Key cannot be empty.");

            await Task.Delay(SimulatedLatency);

            if (PlayerPrefs.HasKey(key))
            {
                try
                {
                    string jsonString = PlayerPrefs.GetString(key);
                    T deserializedObject = JsonUtility.FromJson<T>(jsonString);
                    return deserializedObject;
                } 
                catch (Exception e)
                {
                    Debug.LogError("Error when attempting to deserialize object from key " + key);
                }
            }
            else
            {
                Debug.LogError("Key " + key + " does not exist in state.");
            }

            return default(T);
        }

        public async Task SetState<T>(string key, T state)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Key cannot be empty.");

            await Task.Delay(SimulatedLatency);

            string serializedObject = JsonUtility.ToJson(state);
            PlayerPrefs.SetString(key, serializedObject);
        }
    }
}
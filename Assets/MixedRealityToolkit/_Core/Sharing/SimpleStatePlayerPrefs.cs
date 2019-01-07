using System;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Sharing;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async.AwaitYieldInstructions;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
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
        /// <inheritdoc />
        public override string Name { get { return "SimpleStatePlayerPrefs"; } }

        public bool IsConnected { get; set; }

        public int SimulatedLatency { get; set; }

        public async Task<bool> HasKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Key cannot be empty.");

            await Task.Delay(SimulatedLatency);

            await new WaitForUpdate();

            return PlayerPrefs.HasKey(key);
        }
        
        public async Task<T> GetState<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Key cannot be empty.");

            await Task.Delay(SimulatedLatency);

            await new WaitForUpdate();

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

            await new WaitForUpdate();

            string serializedObject = string.Empty;
            try
            {
                serializedObject = JsonUtility.ToJson(state);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to serialize object of type " + typeof(T) + ": " + e.ToString());
                return;
            }

            PlayerPrefs.SetString(key, serializedObject);
        }
    }
}
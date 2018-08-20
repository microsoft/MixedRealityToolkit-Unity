// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blend
{
    [System.Serializable]
    public struct BlendCollectiontData
    {
        public BlendInstance Blend;
        public BlendInstanceProperties InstanceProperties;
    }

    public abstract class BlendCollection
    {
        protected List<BlendCollectiontData> BlendData = new List<BlendCollectiontData>();
        public bool SetStartToTarget = false;
        public bool IsPlaying = false;

        protected bool completed = true;

        protected MonoBehaviour hostMonoBehavior;

        protected Coroutine ticker;
        protected bool tickerActive;

        public BlendCollection(MonoBehaviour host)
        {
            hostMonoBehavior = host;

            if (ticker != null)
            {
                hostMonoBehavior.StopCoroutine(ticker);
                ticker = null;
            }

            ticker = hostMonoBehavior.StartCoroutine(StartTicker());
        }

        /// <summary>
        /// make sure we clean up the coroutine on destroy
        /// </summary>
        public virtual void Destroy()
        {
            if (ticker != null)
            {
                hostMonoBehavior.StopCoroutine(ticker);
                ticker = null;
            }
        }

        /// <summary>
        /// start the ticker based on the host monoBehavior
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartTicker()
        {
            bool active = true;
            while (active)
            {
                yield return new WaitUntil(() => active);
                Update();
            }

            ticker = null;
        }

        // for the interface to enforce a status value
        public virtual bool GetIsPlaying()
        {
            return IsPlaying;
        }
        
        public virtual void UpdateData(List<BlendCollectiontData> data)
        {
            BlendData = data;
        }

        /// <summary>
        /// Create the BlendInstances and make sure all the properties are valid;
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual BlendCollectiontData ValidateProperties(BlendCollectiontData data)
        {
            BlendInstance instance = data.Blend;

            if (data.Blend == null)
            {
                instance = new BlendInstance();
            }

            data.InstanceProperties = instance.Init(data.InstanceProperties);
            data.Blend = instance;
            return data;
        }

        /// <summary>
        /// Start the animation
        /// </summary>
        public virtual void Play()
        {
            for (int i = 0; i < BlendData.Count; i++)
            {
                BlendData[i] = SetStartValues(i, BlendData[i]);
                BlendData[i].Blend.Play();
            }
        }

        /// <summary>
        /// Set the trandform to the cached starting value
        /// </summary>
        public virtual void ResetTransform()
        {
            StartQueue();

            for (int i = 0; i < BlendData.Count; i++)
            {
                BlendData[i].Blend.ResetBlend();
                QueueValue(i, BlendData[i]);
            }

            ApplyValues();
        }

        public virtual void ResetTransitionValues()
        {
            for (int i = 0; i < BlendData.Count; i++)
            {
                BlendData[i].Blend.ResetBlend();
            }
        }

        /// <summary>
        /// reverse the transition - go back
        /// </summary>
        public virtual void Reverse(bool relitiveStart = false)
        {
            for (int i = 0; i < BlendData.Count; i++)
            {
                BlendData[i].Blend.Reverse(relitiveStart);
            }
        }

        /// <summary>
        /// Stop the animation
        /// </summary>
        public virtual void Stop()
        {
            for (int i = 0; i < BlendData.Count; i++)
            {
                BlendData[i].Blend.Stop();
            }
        }

        public virtual void AddBlendData(BlendCollectiontData data)
        {
            BlendData.Add(data);
        }

        public virtual void RemoveBlendData(BlendCollectiontData data)
        {
            BlendData.Remove(data);
        }

        public virtual void RemoveBlendDataAt(int index)
        {
            BlendData.RemoveAt(index);
        }

        /// <summary>
        /// Set the Blend properties of a specific Blend Instance
        /// </summary>
        public virtual void SetBlendDataByIndex(int index, BlendCollectiontData data)
        {
            if (BlendData.Count > index && index >= 0)
            {
                BlendData[index] = data;
            }
        }

        /// <summary>
        /// The Update and queue has started.
        /// </summary>
        public abstract void StartQueue();

        /// <summary>
        /// apply the updated values all at once
        /// </summary>
        public abstract void ApplyValues();

        /// <summary>
        /// get the start transforms or values, current values
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract BlendCollectiontData SetStartValues(int index, BlendCollectiontData data);

        /// <summary>
        /// set up the transforms
        /// </summary>
        /// <param name="data"></param>
        public abstract void QueueValue(int index, BlendCollectiontData data);

        public virtual void Lerp(float percent)
        {
            StartQueue();

            for (int i = 0; i < BlendData.Count; i++)
            {
                BlendData[i].Blend.Lerp(percent, BlendData[i].InstanceProperties);
                QueueValue(i, BlendData[i]);
            }

            ApplyValues();
        }

        /// <summary>
        /// Animate
        /// </summary>
        public virtual void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            StartQueue();
            bool updated = false;
            completed = false;
            for (int i = 0; i < BlendData.Count; i++)
            {
                if (BlendData[i].Blend.IsPlaying)
                {
                    BlendData[i].Blend.Update(Time.deltaTime, BlendData[i].InstanceProperties);
                    QueueValue(i, BlendData[i]);

                    updated = true;

                    if (!completed)
                    {
                        completed = BlendData[i].Blend.IsCompleted;
                    }
                }
            }

            IsPlaying = updated;

            if (updated)
            {
                ApplyValues();
            }
        }
    }
}

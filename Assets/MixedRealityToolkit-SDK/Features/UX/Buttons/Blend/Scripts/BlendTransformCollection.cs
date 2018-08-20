// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blend
{
    [System.Serializable]
    public struct BlendQuaternions
    {
        public Quaternion TargetValue;
        public Quaternion StartValue;
    }

    [System.Serializable]
    public struct BlendVectors
    {
        public Vector3 TargetValue;
        public Vector3 StartValue;
    }

    [System.Serializable]
    public enum TransformTypes { Position, Scale, Rotation, Quaternion}

    [System.Serializable]
    public struct TransformProperties
    {
        public TransformTypes Type;
        public bool ToLocalTransform;
    }

    [System.Serializable]
    public struct BlendTransformData
    {
        public TransformProperties TransformProperties;
        public BlendInstance Blend;
        public BlendVectors VectorValues;
        public BlendFloats FloatValues;
        public BlendQuaternions QuaternionValues;
        public BlendInstanceProperties InstanceProperties;
    }

    public class BlendTransformCollection : BlendCollection
    {
        protected List<BlendTransformData> BlendTransformData = new List<BlendTransformData>();

        public static Vector4 QuaternionToVector4(Quaternion rotation)
        {
            return new Vector4(rotation.x, rotation.y, rotation.z, rotation.w);
        }

        public static Quaternion Vector4ToQuaternion(Vector4 vector)
        {
            return new Quaternion(vector.x, vector.y, vector.z, vector.w);
        }

        public BlendTransformCollection(MonoBehaviour host) : base(host)
        {
            // constructor
        }

        /// <summary>
        /// conform the custom data to the BlendCollectionData
        /// </summary>
        /// <param name="data"></param>
        public virtual void UpdateData(List<BlendTransformData> data)
        {
            BlendData = new List<BlendCollectiontData>();
            
            data = RefreshData(data);

            BlendTransformData = data;
            base.UpdateData(BlendData);
            

        }

        /// <summary>
        /// update the data values to make sure everything is up to date
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<BlendTransformData> RefreshData(List<BlendTransformData> data)
        {
            BlendData = new List<BlendCollectiontData>();

            for (int i = 0; i < data.Count; i++)
            {
                BlendTransformData transformData = data[i];

                BlendCollectiontData collectionData = SetupBlendData(transformData);

                BlendData.Add(collectionData);

                transformData.Blend = collectionData.Blend;
                transformData.InstanceProperties = collectionData.InstanceProperties;

                data[i] = transformData;
            }

            return data;
        }

        /// <summary>
        /// set up the basic blend collection data values
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private BlendCollectiontData SetupBlendData(BlendTransformData data)
        {
            BlendCollectiontData collectionData = new BlendCollectiontData();
            collectionData.InstanceProperties = data.InstanceProperties;
            collectionData.Blend = data.Blend;
			
			// We set to zero if Blends are disabled, so we need to make sure Validate does not set it back to 1.
            collectionData.InstanceProperties.LerpTime = data.InstanceProperties.LerpTime;

            collectionData = ValidateProperties(collectionData);
            return collectionData;
        }
        
        /// <summary>
        /// The Update and queue has started.
        /// </summary>
        public override void StartQueue()
        {
            // not needed in this type of collection
        }

        /// <summary>
        /// apply the updated values all at once
        /// </summary>
        public override void ApplyValues()
        {
            // values already updated, not used in this type of collection
        }

        public override BlendCollectiontData SetStartValues(int index, BlendCollectiontData data)
        {
            
            BlendTransformData transformData = BlendTransformData[index];
            Transform transform = hostMonoBehavior.gameObject.transform;
            bool toLocal = transformData.TransformProperties.ToLocalTransform;

            switch (transformData.TransformProperties.Type)
            {
                case TransformTypes.Position:
                    transformData.VectorValues.StartValue = toLocal ? transform.localPosition: transform.position;
                    break;
                case TransformTypes.Scale:
                    transformData.VectorValues.StartValue = transform.localScale;
                    break;
                case TransformTypes.Rotation:
                    transformData.VectorValues.StartValue = toLocal ? transform.localRotation.eulerAngles : transform.rotation.eulerAngles;
                    break;
                case TransformTypes.Quaternion:
                    transformData.QuaternionValues.StartValue = toLocal ? transform.localRotation : transform.rotation;
                    break;
                default:
                    break;
            }

            BlendTransformData[index] = transformData;
            
            return data;
        }

        public override void QueueValue(int index, BlendCollectiontData data)
        {
            BlendTransformData transformData = BlendTransformData[index];

            Vector3 vector = Vector3.zero;
            Quaternion quaternion = Quaternion.identity;

            if (transformData.TransformProperties.Type == TransformTypes.Quaternion)
            {
                quaternion = Quaternion.Lerp(transformData.QuaternionValues.StartValue, transformData.QuaternionValues.TargetValue, data.Blend.GetLerpValue());
            }
            else
            {
                vector = Vector3.Lerp(transformData.VectorValues.StartValue, transformData.VectorValues.TargetValue, data.Blend.GetLerpValue());
            }

            Transform transform = hostMonoBehavior.gameObject.transform;
            bool toLocal = transformData.TransformProperties.ToLocalTransform;

            switch (transformData.TransformProperties.Type)
            {
                case TransformTypes.Position:
                    if (toLocal)
                    {
                        transform.localPosition = vector;
                    }
                    else
                    {
                        transform.position = vector;
                    }
                    break;
                case TransformTypes.Scale:
                    transform.localScale = vector;
                    break;
                case TransformTypes.Rotation:
                    if (toLocal)
                    {
                        transform.localRotation = Quaternion.Euler(vector);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(vector);
                    }
                    break;
                case TransformTypes.Quaternion:
                    if (toLocal)
                    {
                        transform.localRotation = quaternion;
                    }
                    else
                    {
                        transform.rotation = quaternion;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

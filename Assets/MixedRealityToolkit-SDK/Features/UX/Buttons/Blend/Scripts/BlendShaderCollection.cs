// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blend
{
    [System.Serializable]
    public class BlendValueData<T>
    {
        public T TargetValue;
        [HideInInspector]
        public T StartValue;
    }

    [System.Serializable]
    public struct BlendColors
    {
        public Color TargetValue;
        public Color StartValue;
    }

    [System.Serializable]
    public struct BlendFloats
    {
        public float TargetValue;
        public float StartValue;
    }

    [System.Serializable]
    public struct BlendShaderData
    {
        public ShaderProperties ShaderProperty;
        public BlendInstance Blend;
        public BlendColors ColorValues;
        public BlendFloats FloatValues;
        public BlendInstanceProperties InstanceProperties;
    }

    /// <summary>
    /// Handles animating multiple shader propertiesto reduce the amount of MaterialPropertyBlock updates per frame
    /// </summary>
    public class BlendShaderCollection : BlendCollection
    {
        protected List<BlendShaderData> BlendShaderData = new List<BlendShaderData>();
        private MaterialPropertyBlock materialBlock;

        public BlendShaderCollection(MonoBehaviour host) : base(host)
        {
            // constructor
        }

        public virtual void UpdateData(List<BlendShaderData> data)
        {
            BlendData = new List<BlendCollectiontData>();

            if (materialBlock == null)
            {
                List<string> floatProps = new List<string>();
                List<string> colorProps = new List<string>();

                for (int i = 0; i < data.Count; i++)
                {
                    BlendShaderData shaderData = data[i];

                    if (data[i].ShaderProperty.Type == ShaderPropertyType.Color)
                    {
                        colorProps.Add(data[i].ShaderProperty.Name);
                        if (SetStartToTarget)
                        {
                            shaderData.ColorValues.StartValue = shaderData.ColorValues.TargetValue;
                        }
                    }
                    else
                    {
                        floatProps.Add(data[i].ShaderProperty.Name);
                        if (SetStartToTarget)
                        {
                            shaderData.FloatValues.StartValue = shaderData.FloatValues.TargetValue;
                        }
                    }

                    BlendCollectiontData collectionData = SetupBlendData(shaderData);

                    BlendData.Add(collectionData);

                    shaderData.Blend = collectionData.Blend;
                    shaderData.InstanceProperties = collectionData.InstanceProperties;

                    data[i] = shaderData;
                }
                
                materialBlock = ColorAbstraction.GetValidPropertyBlock(hostMonoBehavior.gameObject, colorProps.ToArray(), floatProps.ToArray());

                if (!SetStartToTarget)
                {
                    data = RefreshData(data);
                }
            }
            else
            {
                data = RefreshData(data);
            }

            BlendShaderData = data;
            base.UpdateData(BlendData);

            SetPropertyBlock();

        }

        private List<BlendShaderData> RefreshData(List<BlendShaderData> data)
        {
            BlendData = new List<BlendCollectiontData>();

            for (int i = 0; i < data.Count; i++)
            {
                BlendShaderData shaderData = data[i];

                BlendCollectiontData collectionData = SetupBlendData(shaderData);

                BlendData.Add(collectionData);

                shaderData.Blend = collectionData.Blend;
                shaderData.InstanceProperties = collectionData.InstanceProperties;

                data[i] = shaderData;
            }

            return data;
        }

        /// <summary>
        /// set up the collection data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private BlendCollectiontData SetupBlendData(BlendShaderData data)
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
        /// a color abstraction that colors materials and text objects
        /// </summary>
        /// <returns></returns>
        private void SetPropertyBlock()
        {
            if (materialBlock == null)
            {
                materialBlock = new MaterialPropertyBlock();
            }

            Renderer renderer = hostMonoBehavior.gameObject.GetComponent<Renderer>();
            renderer.GetPropertyBlock(materialBlock);
        }

        /// <summary>
        /// Set the Blend properties of a specific Blend instance by property id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public void SetBlendPropertiesById(ShaderProperties shaderProps, BlendShaderData data)
        {
            for (int i = 0; i < BlendShaderData.Count; i++)
            {
                if (BlendShaderData[i].ShaderProperty.Name == shaderProps.Name)
                {
                    BlendShaderData[i] = data;
                    break;
                }
            }
        }

        /// <summary>
        /// Set the color shader property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void SetColorValue(string property, Color value)
        {
            StartQueue();
            materialBlock.SetColor(property, value);
            ApplyValues();
        }

        /// <summary>
        /// set the float shader property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void SetFloatValue(string property, float value)
        {
            StartQueue();
            materialBlock.SetFloat(property, value);
            ApplyValues();
        }

        /// <summary>
        /// The Update and queue has started.
        /// </summary>
        public override void StartQueue()
        {
            SetPropertyBlock();
        }

        /// <summary>
        /// apply the updated values all at once
        /// </summary>
        public override void ApplyValues()
        {
            hostMonoBehavior.gameObject.GetComponent<Renderer>().SetPropertyBlock(materialBlock);
        }

        public override BlendCollectiontData SetStartValues(int index, BlendCollectiontData data)
        {
            BlendShaderData shaderData = BlendShaderData[index];

            if (shaderData.ShaderProperty.Type == ShaderPropertyType.Color)
            {
                shaderData.ColorValues.StartValue = materialBlock.GetVector(shaderData.ShaderProperty.Name);
            }
            else
            {
                shaderData.FloatValues.StartValue = materialBlock.GetFloat(shaderData.ShaderProperty.Name);
            }

            BlendShaderData[index] = shaderData;

            return data;
        }

        public override void QueueValue(int index, BlendCollectiontData data)
        {
            BlendShaderData shaderData = BlendShaderData[index];

            if (shaderData.ShaderProperty.Type == ShaderPropertyType.Color)
            {
                Color color = Color.Lerp(shaderData.ColorValues.StartValue, shaderData.ColorValues.TargetValue, data.Blend.GetLerpValue());
                materialBlock.SetColor(shaderData.ShaderProperty.Name, color);
            }
            else
            {
                float targetValue = shaderData.FloatValues.TargetValue;
                float startValue = shaderData.FloatValues.StartValue;
                float value = (targetValue - startValue) * data.Blend.GetLerpValue() + startValue;
                materialBlock.SetFloat(shaderData.ShaderProperty.Name, value);
            }
        }
    }
}

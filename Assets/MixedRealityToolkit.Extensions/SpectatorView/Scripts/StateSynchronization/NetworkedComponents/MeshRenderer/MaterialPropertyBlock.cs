// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityMaterialPropertyBlock = UnityEngine.MaterialPropertyBlock;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class MaterialPropertyBlock
    {
        private PropertyData propertyData;

        public UnityMaterialPropertyBlock UnityPropertyBlock
        {
            get { return propertyData.UnityPropertyBlock; }
        }

        public void SetData(PropertyData data)
        {
            propertyData = data;
        }

        public bool isEmpty
        {
            get { return propertyData.isEmpty; }
        }

        public void Clear()
        {
            propertyData.Clear();
        }

        #region Get and set
        public Color GetColor(int nameID) { return propertyData.GetValue<Color>(nameID, propertyData.UnityPropertyBlock.GetColor); }
        public Color GetColor(string name) { return GetColor(Shader.PropertyToID(name)); }
        public float GetFloat(int nameID) { return propertyData.GetValue<float>(nameID, propertyData.UnityPropertyBlock.GetFloat); }
        public float GetFloat(string name) { return GetFloat(Shader.PropertyToID(name)); }
        public float[] GetFloatArray(int nameID) { return propertyData.GetValue<float[]>(nameID, propertyData.UnityPropertyBlock.GetFloatArray); }
        public float[] GetFloatArray(string name) { return GetFloatArray(Shader.PropertyToID(name)); }
        public void GetFloatArray(int nameID, List<float> values) { values.Clear(); values.AddRange(GetFloatArray(nameID)); }
        public void GetFloatArray(string name, List<float> values) { GetFloatArray(Shader.PropertyToID(name), values); }
        public Matrix4x4 GetMatrix(int nameID) { return propertyData.GetValue<Matrix4x4>(nameID, propertyData.UnityPropertyBlock.GetMatrix); }
        public Matrix4x4 GetMatrix(string name) { return GetMatrix(Shader.PropertyToID(name)); }
        public void GetMatrixArray(int nameID, List<Matrix4x4> values) { values.Clear(); values.AddRange(GetMatrixArray(nameID)); }
        public void GetMatrixArray(string name, List<Matrix4x4> values) { GetMatrixArray(Shader.PropertyToID(name), values); }
        public Matrix4x4[] GetMatrixArray(int nameID) { return propertyData.GetValue<Matrix4x4[]>(nameID, propertyData.UnityPropertyBlock.GetMatrixArray); }
        public Matrix4x4[] GetMatrixArray(string name) { return GetMatrixArray(Shader.PropertyToID(name)); }
        public Texture GetTexture(string name) { return GetTexture(Shader.PropertyToID(name)); }
        public Texture GetTexture(int nameID) { return propertyData.GetValue<Texture>(nameID, propertyData.UnityPropertyBlock.GetTexture); }
        public Vector4 GetVector(string name) { return GetVector(Shader.PropertyToID(name)); }
        public Vector4 GetVector(int nameID) { return propertyData.GetValue<Vector4>(nameID, propertyData.UnityPropertyBlock.GetVector); }
        public void GetVectorArray(string name, List<Vector4> values) { GetVectorArray(Shader.PropertyToID(name), values); }
        public void GetVectorArray(int nameID, List<Vector4> values) { values.Clear(); values.AddRange(GetVectorArray(nameID)); }
        public Vector4[] GetVectorArray(string name) { return GetVectorArray(Shader.PropertyToID(name)); }
        public Vector4[] GetVectorArray(int nameID) { return propertyData.GetValue<Vector4[]>(nameID, propertyData.UnityPropertyBlock.GetVectorArray); }
        public void SetBuffer(string name, ComputeBuffer value) { SetBuffer(Shader.PropertyToID(name), value); }
        public void SetBuffer(int nameID, ComputeBuffer value) { propertyData.SetValue(nameID, value, propertyData.UnityPropertyBlock.SetBuffer); }
        public void SetColor(string name, Color value) { SetColor(Shader.PropertyToID(name), value); }
        public void SetColor(int nameID, Color value) { propertyData.SetValue(nameID, value, propertyData.UnityPropertyBlock.SetColor); }
        public void SetFloat(string name, float value) { SetFloat(Shader.PropertyToID(name), value); }
        public void SetFloat(int nameID, float value) { propertyData.SetValue(nameID, value, propertyData.UnityPropertyBlock.SetFloat); }
        public void SetFloatArray(string name, List<float> values) { SetFloatArray(Shader.PropertyToID(name), values); }
        public void SetFloatArray(string name, float[] values) { SetFloatArray(Shader.PropertyToID(name), values); }
        public void SetFloatArray(int nameID, List<float> values) { SetFloatArray(nameID, values.ToArray()); }
        public void SetFloatArray(int nameID, float[] values) { propertyData.SetValue(nameID, values, propertyData.UnityPropertyBlock.SetFloatArray); }
        public void SetMatrix(int nameID, Matrix4x4 value) { propertyData.SetValue(nameID, value, propertyData.UnityPropertyBlock.SetMatrix); }
        public void SetMatrix(string name, Matrix4x4 value) { SetMatrix(Shader.PropertyToID(name), value); }
        public void SetMatrixArray(string name, List<Matrix4x4> values) { SetMatrixArray(Shader.PropertyToID(name), values); }
        public void SetMatrixArray(int nameID, List<Matrix4x4> values) { SetMatrixArray(nameID, values.ToArray()); }
        public void SetMatrixArray(string name, Matrix4x4[] values) { SetMatrixArray(Shader.PropertyToID(name), values); }
        public void SetMatrixArray(int nameID, Matrix4x4[] values) { propertyData.SetValue(nameID, values, propertyData.UnityPropertyBlock.SetMatrixArray); }
        public void SetTexture(int nameID, Texture value) { propertyData.SetValue(nameID, value, propertyData.UnityPropertyBlock.SetTexture); }
        public void SetTexture(string name, Texture value) { SetTexture(Shader.PropertyToID(name), value); }
        public void SetVector(int nameID, Vector4 value) { propertyData.SetValue(nameID, value, propertyData.UnityPropertyBlock.SetVector); }
        public void SetVector(string name, Vector4 value) { SetVector(Shader.PropertyToID(name), value); }
        public void SetVectorArray(string name, List<Vector4> values) { SetVectorArray(Shader.PropertyToID(name), values); }
        public void SetVectorArray(int nameID, List<Vector4> values) { SetVectorArray(nameID, values.ToArray()); }
        public void SetVectorArray(string name, Vector4[] values) { SetVectorArray(Shader.PropertyToID(name), values); }
        public void SetVectorArray(int nameID, Vector4[] values) { propertyData.SetValue(nameID, values, propertyData.UnityPropertyBlock.SetVectorArray); }
        #endregion

        public class PropertyData
        {
            private readonly Renderer renderer;
            private readonly UnityMaterialPropertyBlock unityPropertyBlock = new UnityMaterialPropertyBlock();
            private readonly HashSet<int> locallySetProperties = new HashSet<int>();
            private readonly HashSet<int> externallySetProperties = new HashSet<int>();

            public UnityMaterialPropertyBlock UnityPropertyBlock
            {
                get { return unityPropertyBlock; }
            }

            public bool isEmpty
            {
                get { return unityPropertyBlock.isEmpty; }
            }

            public PropertyData(Renderer renderer)
            {
                this.renderer = renderer;
            }

            public void Update()
            {
                renderer.GetPropertyBlock(unityPropertyBlock);
            }

            public void Clear()
            {
                unityPropertyBlock.Clear();
                locallySetProperties.Clear();
            }

            public Color GetColor(int propertyId) { return GetValue(propertyId, unityPropertyBlock.GetColor); }
            public float GetFloat(int propertyId) { return GetValue(propertyId, unityPropertyBlock.GetFloat); }
            public Texture GetTexture(int propertyId) { return GetValue(propertyId, unityPropertyBlock.GetTexture); }
            public Vector4 GetVector(int propertyId) { return GetValue(propertyId, unityPropertyBlock.GetVector); }
            public Matrix4x4 GetMatrix(int propertyId) { return GetValue(propertyId, unityPropertyBlock.GetMatrix); }

            public T GetValue<T>(int propertyId, Func<int, T> unityPropertyGetter)
            {
                T value = unityPropertyGetter(propertyId);
                UpdateAndGetHasValue(propertyId, _ => value);
                return value;
            }

            public void SetValue<T>(int propertyId, T value, Action<int, T> unityPropertySetter)
            {
                locallySetProperties.Add(propertyId);
                unityPropertySetter(propertyId, value);
            }

            public bool HasValue(int propertyId, MaterialPropertyType materialPropertyType)
            {
                switch (materialPropertyType)
                {
                    case MaterialPropertyType.Color:
                        return HasValue(propertyId, unityPropertyBlock.GetColor);
                    case MaterialPropertyType.Float:
                    case MaterialPropertyType.Range:
                        return HasValue(propertyId, unityPropertyBlock.GetFloat);
                    case MaterialPropertyType.Matrix:
                        return HasValue(propertyId, unityPropertyBlock.GetMatrix);
                    case MaterialPropertyType.Texture:
                        return HasValue(propertyId, unityPropertyBlock.GetTexture);
                    case MaterialPropertyType.Vector:
                        return HasValue(propertyId, unityPropertyBlock.GetVector);
                    case MaterialPropertyType.RenderQueue:
                    case MaterialPropertyType.ShaderKeywords:
                        return false;
                    default:
                        throw new InvalidOperationException();
                }
            }

            private bool HasValue<T>(int propertyId, Func<int, T> unityPropertyGetter)
            {
                return UpdateAndGetHasValue(propertyId, unityPropertyGetter);
            }

            private bool UpdateAndGetHasValue<T>(int propertyId, Func<int, T> unityPropertyGetter)
            {
                if (locallySetProperties.Contains(propertyId) || externallySetProperties.Contains(propertyId))
                {
                    return true;
                }

                T value = unityPropertyGetter(propertyId);
                if (!Equals(value, default(T)))
                {
                    externallySetProperties.Add(propertyId);
                    return true;
                }

                return false;
            }
        }
    }

    internal static class MaterialPropertyBlockExtensions
    {
        private static readonly ConditionalWeakTable<Renderer, MaterialPropertyBlock.PropertyData> propertyBlockTable = new ConditionalWeakTable<Renderer, MaterialPropertyBlock.PropertyData>();

        public static void GetPropertyBlock(this Renderer renderer, MaterialPropertyBlock propertyBlock)
        {
            var data = propertyBlockTable.GetValue(renderer, r => new MaterialPropertyBlock.PropertyData(r));
            data.Update();
            propertyBlock.SetData(data);
        }

        public static void UpdateCachedPropertyBlock(this Renderer renderer)
        {
            var data = propertyBlockTable.GetValue(renderer, r => new MaterialPropertyBlock.PropertyData(r));
            data.Update();
        }

        public static bool TryGetPropertyBlockData(this Renderer renderer, out MaterialPropertyBlock.PropertyData propertyBlock)
        {
            return propertyBlockTable.TryGetValue(renderer, out propertyBlock);
        }

        public static void SetPropertyBlock(this Renderer renderer, MaterialPropertyBlock propertyBlock)
        {
            renderer.SetPropertyBlock(propertyBlock.UnityPropertyBlock);
        }
    }
}

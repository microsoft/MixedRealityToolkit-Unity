// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    internal class MaterialPropertyAsset
    {
        public Shader shader;

        [SerializeField]
        private string shaderName;
        public string propertyName;
        public MaterialPropertyType propertyType;
        private int? propertyId;

        public Shader Shader
        {
            get { return shader; }
            set
            {
                shader = value;
                shaderName = shader?.name;
            }
        }

        public string ShaderName
        {
            get { return shaderName; }
        }

        private int PropertyID
        {
            get
            {
                if (propertyId == null)
                {
                    propertyId = Shader.PropertyToID(propertyName);
                }
                return propertyId.Value;
            }
        }

        public object GetValue(Renderer renderer, Material material)
        {
            MaterialPropertyBlock.PropertyData data;
            if (renderer != null && renderer.TryGetPropertyBlockData(out data) && data.HasValue(PropertyID, propertyType))
            {
                switch (propertyType)
                {
                    case MaterialPropertyType.Color:
                        return data.GetColor(PropertyID);
                    case MaterialPropertyType.Float:
                    case MaterialPropertyType.Range:
                        return data.GetFloat(PropertyID);
                    case MaterialPropertyType.Texture:
                        return data.GetTexture(PropertyID);
                    case MaterialPropertyType.Vector:
                        return data.GetVector(PropertyID);
                    case MaterialPropertyType.Matrix:
                        return data.GetMatrix(PropertyID);
                }
            }

            switch (propertyType)
            {
                case MaterialPropertyType.Color:
                    return material.GetColor(PropertyID);
                case MaterialPropertyType.Float:
                case MaterialPropertyType.Range:
                    return material.GetFloat(PropertyID);
                case MaterialPropertyType.Texture:
                    return material.GetTexture(PropertyID);
                case MaterialPropertyType.Vector:
                    return material.GetVector(PropertyID);
                case MaterialPropertyType.Matrix:
                    return material.GetMatrix(PropertyID);
                case MaterialPropertyType.RenderQueue:
                    return material.renderQueue;
                case MaterialPropertyType.ShaderKeywords:
                    return material.shaderKeywords;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Write(BinaryWriter message, Renderer renderer, Material material)
        {
            message.Write(propertyName);
            message.Write((byte)propertyType);

            switch (propertyType)
            {
                case MaterialPropertyType.Color:
                    message.Write((Color)GetValue(renderer, material));
                    break;
                case MaterialPropertyType.Float:
                case MaterialPropertyType.Range:
                    message.Write((float)GetValue(renderer, material));
                    break;
                case MaterialPropertyType.Texture:
                    {
                        Texture texture = (Texture)GetValue(renderer, material);
                        if (AssetService.Instance.TrySerializeTexture(message, texture))
                        {
                            message.Write(material.GetTextureScale(PropertyID));
                            message.Write(material.GetTextureOffset(PropertyID));
                        }
                    }
                    break;
                case MaterialPropertyType.Vector:
                    message.Write((Vector4)GetValue(renderer, material));
                    break;
                case MaterialPropertyType.Matrix:
                    message.Write((Matrix4x4)GetValue(renderer, material));
                    break;
                case MaterialPropertyType.RenderQueue:
                    message.Write((int)GetValue(renderer, material));
                    break;
                case MaterialPropertyType.ShaderKeywords:
                    {
                        string[] shaderKeywords = (string[])GetValue(renderer, material);
                        message.Write(shaderKeywords != null);
                        if (shaderKeywords != null)
                        {
                            message.Write(shaderKeywords.Length);
                            for (int i = 0; i < shaderKeywords.Length; i++)
                            {
                                message.Write(shaderKeywords[i]);
                            }
                        }
                    }
                    break;
            }
        }

        public static void Read(BinaryReader message, Material[] materials, int materialIndex)
        {
            string propertyName = message.ReadString();
            MaterialPropertyType propertyType = (MaterialPropertyType)message.ReadByte();
            Material mat = materials[materialIndex];
            DefaultStateSynchronizationPerformanceParameters.Instance?.NotifyMaterialMutated(mat, propertyName);
            switch (propertyType)
            {
                case MaterialPropertyType.Color:
                    mat.SetColor(propertyName, message.ReadColor());
                    break;
                case MaterialPropertyType.Float:
                case MaterialPropertyType.Range:
                    mat.SetFloat(propertyName, message.ReadSingle());
                    break;
                case MaterialPropertyType.Texture:
                    {
                        Texture texture;
                        if (AssetService.Instance.TryDeserializeTexture(message, out texture))
                        {
                            mat.SetTexture(propertyName, texture);
                            mat.SetTextureScale(propertyName, message.ReadVector2());
                            mat.SetTextureOffset(propertyName, message.ReadVector2());
                        }
                    }
                    break;
                case MaterialPropertyType.Vector:
                    mat.SetVector(propertyName, message.ReadVector4());
                    break;
                case MaterialPropertyType.Matrix:
                    mat.SetMatrix(propertyName, message.ReadMatrix4x4());
                    break;
                case MaterialPropertyType.RenderQueue:
                    mat.renderQueue = message.ReadInt32();
                    break;
                case MaterialPropertyType.ShaderKeywords:
                    {
                        bool isNotNull = message.ReadBoolean();
                        if (isNotNull)
                        {
                            int length = message.ReadInt32();
                            string[] shaderKeywords = new string[length];
                            for (int i = 0; i < length; i++)
                            {
                                shaderKeywords[i] = message.ReadString();
                            }
                            mat.shaderKeywords = shaderKeywords;
                        }
                        else
                        {
                            mat.shaderKeywords = null;
                        }
                    }
                    break;
            }
        }

        public static Material[] ReadMaterials(BinaryReader message, Material[] existingMaterials)
        {
            int materialCount = message.ReadInt32();
            Material[] materials = new Material[materialCount];
            for (int i = 0; i < materialCount; i++)
            {
                string shaderName = message.ReadString();
                if (shaderName != string.Empty)
                {
                    Material material;
                    if (existingMaterials != null && i < existingMaterials.Length)
                    {
                        material = existingMaterials[i];
                        material.name = message.ReadString();

                        if (material.shader.name != shaderName)
                        {
                            Shader shader = Shader.Find(shaderName);
                            if (shader == null)
                            {
                                Debug.Log("Couldn't find shader with name " + shaderName);
                                shader = Shader.Find("Standard");
                            }
                            material.shader = shader;
                        }
                    }
                    else
                    {
                        Shader shader = Shader.Find(shaderName);
                        if (shader == null)
                        {
                            Debug.Log("Couldn't find shader with name " + shaderName);
                            shader = Shader.Find("Standard");
                        }

                        material = new Material(shader);
                        material.name = message.ReadString();
                    }

                    materials[i] = material;

                    int materialPropertyCount = message.ReadInt32();
                    for (int j = 0; j < materialPropertyCount; j++)
                    {
                        MaterialPropertyAsset.Read(message, materials, i);
                    }
                }
            }
            return materials;
        }

    }
}

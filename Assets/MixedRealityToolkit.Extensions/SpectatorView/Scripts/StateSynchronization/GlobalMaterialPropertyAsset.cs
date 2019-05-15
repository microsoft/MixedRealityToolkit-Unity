// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    internal class GlobalMaterialPropertyAsset
    {
        public string propertyName;
        public MaterialPropertyType propertyType;

        public object GetValue()
        {
            switch (propertyType)
            {
                case MaterialPropertyType.Color:
                    return Shader.GetGlobalColor(propertyName);
                case MaterialPropertyType.Float:
                case MaterialPropertyType.Range:
                    return Shader.GetGlobalFloat(propertyName);
                case MaterialPropertyType.Texture:
                    return Shader.GetGlobalTexture(propertyName);
                case MaterialPropertyType.Vector:
                    return Shader.GetGlobalVector(propertyName);
                case MaterialPropertyType.Matrix:
                    return Shader.GetGlobalMatrix(propertyName);
                default:
                    throw new NotImplementedException();
            }
        }

        public void Write(BinaryWriter message)
        {
            message.Write(propertyName);
            message.Write((byte)propertyType);

            switch (propertyType)
            {
                case MaterialPropertyType.Color:
                    message.Write(Shader.GetGlobalColor(propertyName));
                    break;
                case MaterialPropertyType.Float:
                case MaterialPropertyType.Range:
                    message.Write(Shader.GetGlobalFloat(propertyName));
                    break;
                case MaterialPropertyType.Texture:
                    {
                        Texture texture = Shader.GetGlobalTexture(propertyName);
                        AssetService.Instance.TrySerializeTexture(message, texture);
                    }
                    break;
                case MaterialPropertyType.Vector:
                    message.Write(Shader.GetGlobalVector(propertyName));
                    break;
                case MaterialPropertyType.Matrix:
                    message.Write(Shader.GetGlobalMatrix(propertyName));
                    break;
            }
        }

        public static void Read(BinaryReader message)
        {
            string propertyName = message.ReadString();
            MaterialPropertyType propertyType = (MaterialPropertyType)message.ReadByte();
            switch (propertyType)
            {
                case MaterialPropertyType.Color:
                    Shader.SetGlobalColor(propertyName, message.ReadColor());
                    break;
                case MaterialPropertyType.Float:
                case MaterialPropertyType.Range:
                    Shader.SetGlobalFloat(propertyName, message.ReadSingle());
                    break;
                case MaterialPropertyType.Texture:
                    {
                        Texture texture;
                        if (AssetService.Instance.TryDeserializeTexture(message, out texture))
                        {
                            Shader.SetGlobalTexture(propertyName, texture);
                        }
                    }
                    break;
                case MaterialPropertyType.Vector:
                    Shader.SetGlobalVector(propertyName, message.ReadVector4());
                    break;
                case MaterialPropertyType.Matrix:
                    Shader.SetGlobalMatrix(propertyName, message.ReadMatrix4x4());
                    break;
            }
        }
    }
}
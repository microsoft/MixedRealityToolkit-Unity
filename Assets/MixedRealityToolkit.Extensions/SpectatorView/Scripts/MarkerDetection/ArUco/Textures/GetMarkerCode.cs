// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.MarkerDetection
{
    public class GetMarkerCode : MonoBehaviour
    {
        [SerializeField, TextArea] string _codes;
        [SerializeField] Texture2D[] _textures;

        // Use this for initialization
        void Start()
        {

        }

        [ContextMenu("Codes")]
        void CheckTextureCodes()
        {
            string result = "{";
            for (int i = 0; i < _textures.Length; i++)
            {
                result += "\"" + GetCode(_textures[i]) + "\"";
                if (i != _textures.Length - 1)
                    result += ",\n";
            }
            _codes = result + "}";
        }

        string GetCode(Texture2D aTex)
        {
            string result = "";
            int chunk = aTex.width / 8;

            for (int y = 5; y >= 0; y--)
            {
                for (int x = 0; x < 6; x++)
                {
                    Color c = aTex.GetPixel(
                        (x + 1) * chunk + chunk / 2,
                        (y + 1) * chunk + chunk / 2);

                    result += c.r > 0.5f ? "1" : "0";
                }
            }
            return result;
        }
    }

}

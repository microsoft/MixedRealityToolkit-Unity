// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Configures volume variables in shader
    /// </summary>
    public class VolumeController : MonoBehaviour
    {
        public VolumeInformation VolumeInfo;

        public Int3 VolumeSize { get; private set; }
        public Int3 VolumeSizePow2 { get; private set; }

        private bool isSetup;
        
        void OnEnable()
        {
            EnsureSetup();
        }

        void Update()
        {
            Shader.SetGlobalMatrix("_WorldToVolume", this.transform.worldToLocalMatrix);

            Shader.SetGlobalVector("_VolBufferSize", new Vector4(VolumeSize.x, VolumeSize.y, VolumeSize.z));

            Shader.SetGlobalVector("_VolTextureFactor", new Vector4((float)VolumeSize.x / VolumeSizePow2.x,
                                                                    (float)VolumeSize.y / VolumeSizePow2.y,
                                                                    (float)VolumeSize.z / VolumeSizePow2.z,
                                                                    0.0f));
        }

        public void EnsureSetup()
        {
            if (isSetup)
            {
                return;
            }

            isSetup = true;

            VolumeSize = this.VolumeInfo.Size;
            VolumeSizePow2 = MathExtensions.PowerOfTwoGreaterThanOrEqualTo(VolumeSize);

            Debug.Log("Volume Controller Configured! " + this.VolumeSizePow2);
        }
    }
}
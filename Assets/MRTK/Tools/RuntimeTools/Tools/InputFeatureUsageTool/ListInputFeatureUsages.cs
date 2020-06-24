// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Tools.Runtime
{
    /// <summary>
    /// Displays all active controllers with all available feature usages and their current state.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Tools/ListInputFeatureUsages")]
    public class ListInputFeatureUsages : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Used for displaying all detected input source names.")]
        private TextMesh listInputDevicesTextMesh = null;

        [SerializeField]
        [Tooltip("Used for displaying data from input.")]
        private TextMesh[] displayFeatureUsagesTextMeshes = null;

        private readonly List<InputDevice> inputDevices = new List<InputDevice>();
        private readonly List<InputFeatureUsage> featureUsages = new List<InputFeatureUsage>();

        private void Update()
        {
            if (listInputDevicesTextMesh == null || displayFeatureUsagesTextMeshes.Length == 0)
            {
                return;
            }

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, inputDevices);

            listInputDevicesTextMesh.text = $"Detected {inputDevices.Count} input source{(inputDevices.Count > 1 ? "s:" : inputDevices.Count != 0 ? ":" : "s")}\n";

            for (int i = 0; i < displayFeatureUsagesTextMeshes.Length; i++)
            {
                TextMesh textMesh = displayFeatureUsagesTextMeshes[i];
                if (textMesh == null)
                {
                    continue;
                }

                if (i >= inputDevices.Count && textMesh.text != string.Empty)
                {
                    textMesh.text = string.Empty;
                    continue;
                }

                InputDevice inputDevice = inputDevices[i];

                listInputDevicesTextMesh.text += $"{inputDevice.name}\n";
                textMesh.text = $"{inputDevice.name}\n";

                if (inputDevice.TryGetFeatureUsages(featureUsages))
                {
                    foreach (InputFeatureUsage inputFeatureUsage in featureUsages)
                    {
                        textMesh.text += $"{inputFeatureUsage.name}";

                        if (inputFeatureUsage.type.Equals(typeof(bool)))
                        {
                            if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<bool>(inputFeatureUsage.name), out bool data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(uint)))
                        {
                            if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<uint>(inputFeatureUsage.name), out uint data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(float)))
                        {
                            if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<float>(inputFeatureUsage.name), out float data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(Vector2)))
                        {
                            if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector2>(inputFeatureUsage.name), out Vector2 data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(InputTrackingState)))
                        {
                            if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<InputTrackingState>(inputFeatureUsage.name), out InputTrackingState data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(Vector3)))
                        {
                            if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector3>(inputFeatureUsage.name), out Vector3 data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(Quaternion)))
                        {
                            if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<Quaternion>(inputFeatureUsage.name), out Quaternion data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else
                        {
                            textMesh.text += $"\n";
                        }
                    }
                }
            }
        }
    }
}

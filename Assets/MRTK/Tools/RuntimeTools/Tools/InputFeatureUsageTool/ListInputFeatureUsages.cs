// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR;
#endif // UNITY_2019_3_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Tools.Runtime
{
    /// <summary>
    /// Displays all active controllers with all available feature usages and their current state.
    /// </summary>
    /// <remarks>Only works on Unity 2019.3 or newer.</remarks>
    [AddComponentMenu("Scripts/MRTK/Tools/ListInputFeatureUsages")]
    public class ListInputFeatureUsages : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Used for displaying all detected input source names.")]
        private TextMesh listInputDevicesTextMesh = null;

        [SerializeField]
        [Tooltip("Used for displaying all detected input source names.")]
        private GridObjectCollection gridObjectCollection = null;

        [SerializeField]
        [Tooltip("Used for displaying data from input.")]
        private GameObject displayFeatureUsagesPrefab = null;

#if UNITY_2019_3_OR_NEWER
        private readonly List<InputDevice> leftInputDevices = new List<InputDevice>();
        private readonly List<InputDevice> rightInputDevices = new List<InputDevice>();
        private readonly List<InputFeatureUsage> featureUsages = new List<InputFeatureUsage>();
        private readonly List<TextMesh> displayFeatureUsagesTextMeshes = new List<TextMesh>();

        private const float BackingPanelMargin = 0.05f;
        private const float BackingPanelEntryHeight = 0.03f;
#endif // UNITY_2019_3_OR_NEWER

        private void Update()
        {
            if (listInputDevicesTextMesh == null || gridObjectCollection == null || displayFeatureUsagesPrefab == null)
            {
                return;
            }

#if UNITY_2019_3_OR_NEWER
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, leftInputDevices);
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, rightInputDevices);

            List<InputDevice> inputDevices = leftInputDevices.Union(rightInputDevices).ToList();
            int sourceCount = inputDevices.Count;

            listInputDevicesTextMesh.text = $"Detected {sourceCount} input source{(sourceCount > 1 ? "s:" : sourceCount != 0 ? ":" : "s")}\n";

            bool collectionNeedsUpdating = false;

            for (int i = displayFeatureUsagesTextMeshes.Count; i < sourceCount; i++)
            {
                displayFeatureUsagesTextMeshes.Add(Instantiate(displayFeatureUsagesPrefab, gameObject.transform).GetComponentInChildren<TextMesh>());
                collectionNeedsUpdating = true;
            }

            for (int i = 0; i < displayFeatureUsagesTextMeshes.Count; i++)
            {
                TextMesh textMesh = displayFeatureUsagesTextMeshes[i];
                if (textMesh == null)
                {
                    continue;
                }

                if (i >= sourceCount)
                {
                    if (textMesh.transform.parent.gameObject.activeSelf)
                    {
                        textMesh.transform.parent.gameObject.SetActive(false);
                        collectionNeedsUpdating = true;
                    }
                    continue;
                }

                if (!textMesh.transform.parent.gameObject.activeSelf)
                {
                    textMesh.transform.parent.gameObject.SetActive(true);
                    collectionNeedsUpdating = true;
                }

                InputDevice inputDevice = inputDevices[i];

                bool displayNeedsResizing = !textMesh.text.StartsWith(inputDevice.name);

                listInputDevicesTextMesh.text += $"{inputDevice.name} | {inputDevice.manufacturer}\n";
                textMesh.text = $"{inputDevice.name}\n\n";

                if (inputDevice.TryGetFeatureUsages(featureUsages))
                {
                    foreach (InputFeatureUsage inputFeatureUsage in featureUsages)
                    {
                        if (displayNeedsResizing)
                        {
                            // The first child in the text panel GameObject must be the backing panel
                            Transform backingPanel = textMesh.gameObject.transform.parent.GetChild(0);
                            // The additional 2 added to featureUsages.Count represents the source name and empty new line before the usages are listed
                            float backingPanelHeight = BackingPanelMargin + (BackingPanelEntryHeight * (featureUsages.Count + 2)) + BackingPanelMargin;
                            backingPanel.localScale = new Vector3(backingPanel.localScale.x, backingPanelHeight, backingPanel.localScale.z);
                        }

                        textMesh.text += $"{inputFeatureUsage.name}";

                        if (inputFeatureUsage.type.Equals(typeof(bool)))
                        {
                            if (inputDevice.TryGetFeatureValue(inputFeatureUsage.As<bool>(), out bool data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(uint)))
                        {
                            if (inputDevice.TryGetFeatureValue(inputFeatureUsage.As<uint>(), out uint data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(float)))
                        {
                            if (inputDevice.TryGetFeatureValue(inputFeatureUsage.As<float>(), out float data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(Vector2)))
                        {
                            if (inputDevice.TryGetFeatureValue(inputFeatureUsage.As<Vector2>(), out Vector2 data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(Vector3)))
                        {
                            if (inputDevice.TryGetFeatureValue(inputFeatureUsage.As<Vector3>(), out Vector3 data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(Quaternion)))
                        {
                            if (inputDevice.TryGetFeatureValue(inputFeatureUsage.As<Quaternion>(), out Quaternion data))
                            {
                                textMesh.text += $": {data}\n";
                            }
                        }
                        else if (inputFeatureUsage.type.Equals(typeof(InputTrackingState)))
                        {
                            if (inputDevice.TryGetFeatureValue(inputFeatureUsage.As<InputTrackingState>(), out InputTrackingState data))
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

            if (collectionNeedsUpdating)
            {
                gridObjectCollection.UpdateCollection();
            }
#else
            listInputDevicesTextMesh.text = $"This feature is only supported on Unity 2019.3 or newer.";
#endif // UNITY_2019_3_OR_NEWER
        }
    }
}

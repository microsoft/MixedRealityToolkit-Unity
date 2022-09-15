// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.GraphicsTools;
using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.accessibility",
        DisplayName = "MRTK Accessibility Subsystem",
        Author = "Microsoft",
        ProviderType = typeof(AccessibilityProvider),
        SubsystemTypeOverride = typeof(MRTKAccessibilitySubsystem),
        ConfigType = typeof(AccessibilitySubsystemConfig))]
    public class MRTKAccessibilitySubsystem : AccessibilitySubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<MRTKAccessibilitySubsystem, AccessibilitySubsystemCinfo>();

            if (!AccessibilitySubsystem.Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        [Preserve]
        class AccessibilityProvider : Provider
        {
            #region IAccessibilitySubsystem implementation

            protected AccessibilitySubsystemConfig Config { get; }

            public AccessibilityProvider() : base()
            {
                describableObjects.Add(ObjectClassification.People, new List<GameObject>());
                describableObjects.Add(ObjectClassification.Places, new List<GameObject>());
                describableObjects.Add(ObjectClassification.Things, new List<GameObject>());
                describableObjects.Add(ObjectClassification.UserInterface, new List<GameObject>());
                describableObjects.Add(ObjectClassification.Background, new List<GameObject>());

                Config = XRSubsystemHelpers.GetConfiguration<AccessibilitySubsystemConfig, MRTKAccessibilitySubsystem>();
                invertTextColor = Config.InvertTextColor;
            }

            #region Describable object management

            private readonly Dictionary<ObjectClassification, List<GameObject>> describableObjects =
                new Dictionary<ObjectClassification, List<GameObject>>();

            /// <inheritdoc/>
            public override void GetDescribableObjects(
                ObjectClassification classifications,
                ReaderView readerView,
                float maxDistance,
                List<GameObject> objectList)
            {
                // todo: need to validate parameters, specifically a maxDistance of <= 0 is meaningless

                AssembleDescribableObjects(classifications, objectList);
                FilterDescribableObjects(readerView, maxDistance, objectList);
            }

            /// <inheritdoc/>
            public override bool TryRegisterDescribableObject(
                GameObject gameObj,
                ObjectClassification classification)
            {
                // todo: really, should we check _all_ of the classifications and enforce only _one_ registration?
                List<GameObject> objCollection = describableObjects[classification];
                if (objCollection.Contains(gameObj))
                {
                    Debug.LogError($"{gameObj.name} has already been registerd as a describable object");
                    return false;
                }

                objCollection.Add(gameObj);
                return true;
            }

            /// <inheritdoc/>
            public override bool TryUnregisterDescribableObject(
                GameObject gameObj,
                ObjectClassification classification)
            {
                List<GameObject> objCollection = describableObjects[classification];
                if (!objCollection.Contains(gameObj))
                {
                    Debug.LogError($"{gameObj.name} has not been registerd as a describable object");
                    return false;
                }

                if (!describableObjects[classification].Remove(gameObj))
                {
                    Debug.LogError($"Failed to unregister {gameObj.name} as a describable object");
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="classifications"></param>
            /// <param name="objectList"></param>
            private void AssembleDescribableObjects(
                ObjectClassification classifications,
                List<GameObject> objectList)
            {
                objectList.Clear();

                if ((int)(classifications & ObjectClassification.People) != 0)
                {
                    objectList.AddRange(describableObjects[ObjectClassification.People]);
                }
                else if ((int)(classifications & ObjectClassification.Places) != 0)
                {
                    objectList.AddRange(describableObjects[ObjectClassification.Places]);
                }
                else if ((int)(classifications & ObjectClassification.Things) != 0)
                {
                    objectList.AddRange(describableObjects[ObjectClassification.Things]);
                }
                else if ((int)(classifications & ObjectClassification.UserInterface) != 0)
                {
                    objectList.AddRange(describableObjects[ObjectClassification.UserInterface]);
                }
                else if ((int)(classifications & ObjectClassification.Background) != 0)
                {
                    objectList.AddRange(describableObjects[ObjectClassification.Background]);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="readerView"></param>
            /// <param name="maxDistance"></param>
            /// <param name="objectList"></param>
            private void FilterDescribableObjects(
                ReaderView readerView,
                float maxDistance,
                List<GameObject> objectList)
            {
                for (int i = (objectList.Count - 1); i != 0; i--)
                {
                    GameObject obj = objectList[i];

                    // Does the object have a collider?
                    Collider collider = obj.GetComponent<Collider>();
                    if (collider == null)
                    {
                        objectList.Remove(obj);
                        continue;
                    }

                    // If requested, is it within the main camera's field of view?
                    if ((readerView == ReaderView.FieldOfView) &&
                        !CameraFOVChecker.IsInFOVCached(Camera.main, collider))
                    {
                        objectList.Remove(obj);
                    }

                    // Is it closer than the maximum distance?
                    if (maxDistance < Mathf.Abs(Vector3.Distance(Camera.main.transform.position, obj.transform.position)))
                    {
                        objectList.Remove(obj);
                    }
                }
            }

            #endregion Describable object management

            #region Text color inversion

            private bool invertTextColor = false;

            /// <inheritdoc/>
            public override bool InvertTextColor
            {
                get => invertTextColor;
                set
                {
                    if (invertTextColor != value)
                    {
                        invertTextColor = value;
                        RaiseInvertTextColorChanged(invertTextColor);
                    }
                }
            }

            /// <inheritdoc/>
            public override event Action<bool> InvertTextColorChanged;

            /// <inheritdoc/>
            public override void ApplyTextColorInversion(
                Material material,
                bool enable)
            {
                AccessibilityUtilities.SetTextColorInversion(material, enable);
            }

            /// <summary>
            /// Sends a <see cref="InvertTextColorChanged"/> event to registered listeners.
            /// </summary>
            /// <param name="invert">
            /// True if text color inversion has been enabled, or false.
            /// </param>
            private void RaiseInvertTextColorChanged(bool invert)
            {
                InvertTextColorChanged?.Invoke(invert);
            }

            #endregion Text color inversion

            #endregion IAccessibilitySubsystem implementation
        }
    }
}

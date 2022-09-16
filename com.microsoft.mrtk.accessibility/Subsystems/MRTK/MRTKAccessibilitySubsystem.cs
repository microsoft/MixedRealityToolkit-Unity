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
            internal override bool TryGetDescribableObjects(ObjectClassification classifications, ReaderView readerView, float maxDistance, List<GameObject> objectList)
            {
                if (maxDistance <= 0)
                {
                    Debug.LogError("The distance from the camera to the objects cannot be less than or equal to zero.");
                    return false;
                }

                objectList.Clear();

                AssembleDescribableObjects(classifications, objectList);
                FilterDescribableObjects(readerView, maxDistance, objectList);

                return true;
            }

            /// <inheritdoc/>
            public override bool TryRegisterDescribableObject(GameObject gameObj, ObjectClassification classification)
            {
                // Make sure the specified game object has not been previously registered in any classification.
                foreach (IList<GameObject> list in describableObjects.Values)
                {
                    if (list.Contains(gameObj))
                    {
                        Debug.LogError($"{gameObj.name} has already been registerd as a describable object");
                        return false;
                    }
                }

                describableObjects[classification].Add(gameObj);
                return true;
            }

            /// <inheritdoc/>
            public override bool TryUnregisterDescribableObject(GameObject gameObj, ObjectClassification classification)
            {
                List<GameObject> objCollection = describableObjects[classification];
                if (!objCollection.Contains(gameObj))
                {
                    Debug.LogError($"{gameObj.name} has not been registerd as a describable object of classification {classification}");
                    return false;
                }

                if (!describableObjects[classification].Remove(gameObj))
                {
                    Debug.LogError($"Failed to unregister {gameObj.name} as a describable object of classification {classification}");
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Collects the registered <see cref="GameObject"/>s from the requested <see cref="ObjectClassification"/>s.
            /// </summary>
            /// <param name="classifications">The combined flags specifiying the desired classification(s) (people, places, things, etc.).</param>
            /// <param name="objectList">The collection which will receive the requested <see cref="GameObject"/>s.</param>
            /// <remarks>When this method is called, the objectList will be cleared prior to adding the requested <see cref="GameObject"/>s.</remarks>
            private void AssembleDescribableObjects(
                ObjectClassification classifications,
                List<GameObject> objectList)
            {
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
            /// Filters the provided collection of <see cref="GameObject"/>s based on the visibility to the main camera
            /// and the maximum allowed distance.
            /// </summary>
            /// <param name="readerView">The desired visibility (in field of view, full surround, etc.) of the <see cref="GameObject"/>s.</param>
            /// <param name="maxDistance">The distance, in meters, beyond which <see cref="GameObject"/>s will be filtered.</param>
            /// <param name="objectList">The collection of <see cref="GameObject"/>s that will be filtered based on visibility and distance.</param>
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

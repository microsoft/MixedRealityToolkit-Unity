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
                Config = XRSubsystemHelpers.GetConfiguration<AccessibilitySubsystemConfig, MRTKAccessibilitySubsystem>();
                invertTextColor = Config.InvertTextColor;
            }

            #region Describable object management

            private readonly Dictionary<DescribableObjectClassification, List<GameObject>> describableObjects =
                new Dictionary<DescribableObjectClassification, List<GameObject>>();

            /// <inheritdoc/>
            internal override bool TryGetDescribableObjects(List<DescribableObjectClassification> classifications, DescribableObjectVisibility visibility, float maxDistance, List<GameObject> describableObjectsList)
            {
                if (maxDistance <= 0)
                {
                    Debug.LogError("The distance from the camera to the objects cannot be less than or equal to zero.");
                    return false;
                }

                describableObjectsList.Clear();

                AssembleDescribableObjects(classifications, describableObjectsList);
                FilterDescribableObjects(visibility, maxDistance, describableObjectsList);

                return true;
            }

            /// <inheritdoc/>
            public override bool TryGetDescribableObjectClassifications(List<DescribableObjectClassification> classifications)
            {
                classifications.Clear();
                classifications.AddRange(describableObjects.Keys);
                return true;
            }

            /// <inheritdoc/>
            public override bool TryRegisterDescribableObject(GameObject describableObject, DescribableObjectClassification classification)
            {
                // Make sure the specified game object has not been previously registered in any classification.
                foreach (IList<GameObject> list in describableObjects.Values)
                {
                    if (list.Contains(describableObject))
                    {
                        Debug.LogError($"{describableObject.name} has already been registered as a describable object");
                        return false;
                    }
                }

                if (!describableObjects.ContainsKey(classification))
                {
                    describableObjects.Add(classification, new List<GameObject>());
                }
                describableObjects[classification].Add(describableObject);

                return true;
            }

            /// <inheritdoc/>
            public override bool TryUnregisterDescribableObject(GameObject describableObject, DescribableObjectClassification classification)
            {
                List<GameObject> objCollection = describableObjects[classification];
                if (!objCollection.Contains(describableObject))
                {
                    Debug.LogError($"{describableObject.name} has not been registered as a describable object of classification {classification}");
                    return false;
                }

                if (!objCollection.Remove(describableObject))
                {
                    Debug.LogError($"Failed to unregister {describableObject.name} as a describable object of classification {classification}");
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Collects the registered <see cref="GameObject"/>s from the requested <see cref="DescribableObjectClassification"/>s.
            /// </summary>
            /// <param name="classifications">The combined flags specifiying the desired classification(s) (people, places, things, etc.).</param>
            /// <param name="describableObjectsList">The collection which will receive the requested <see cref="GameObject"/>s.</param>
            /// <remarks>When this method is called, the objectList will be cleared prior to adding the requested <see cref="GameObject"/>s.</remarks>
            private void AssembleDescribableObjects(
                List<DescribableObjectClassification> classifications,
                List<GameObject> describableObjectsList)
            {
                foreach (DescribableObjectClassification classification in classifications)
                {
                    describableObjectsList.AddRange(describableObjects[classification]);
                }
            }

            /// <summary>
            /// Filters the provided collection of <see cref="GameObject"/>s based on the visibility to the main camera
            /// and the maximum allowed distance.
            /// </summary>
            /// <param name="visibility">The desired visibility (in field of view, full surround, etc.) of the <see cref="GameObject"/>s.</param>
            /// <param name="maxDistance">The distance, in meters, beyond which <see cref="GameObject"/>s will be filtered.</param>
            /// <param name="objectList">The collection of <see cref="GameObject"/>s that will be filtered based on visibility and distance.</param>
            private void FilterDescribableObjects(
                DescribableObjectVisibility visibility,
                float maxDistance,
                List<GameObject> objectList)
            {
                // Ensure there is a collection to filter.
                if (objectList.Count == 0) { return; }

                float maxDistanceSquared = maxDistance * maxDistance;

                // Walk the list backwards, so that it does not hit a collection changed exception.
                for (int i = (objectList.Count - 1); i != 0; i--)
                {
                    GameObject obj = objectList[i];

                    // Does the object have a collider?
                    if (!obj.TryGetComponent<Collider>(out Collider collider))
                    {
                        objectList.Remove(obj);
                        continue;
                    }

                    // If requested, is it within the main camera's field of view?
                    if ((visibility == DescribableObjectVisibility.FieldOfView) &&
                        !CameraFOVChecker.IsInFOVCached(Camera.main, collider))
                    {
                        objectList.Remove(obj);
                    }

                    // Is it closer than the maximum distance?
                    if (maxDistanceSquared < (Camera.main.transform.position - obj.transform.position).sqrMagnitude)
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

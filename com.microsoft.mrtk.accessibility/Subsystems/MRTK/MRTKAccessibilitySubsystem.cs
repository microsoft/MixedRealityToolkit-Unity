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

            #region Accessible object management

            private readonly Dictionary<AccessibleObjectClassification, List<GameObject>> accessibleObjects =
                new Dictionary<AccessibleObjectClassification, List<GameObject>>();

            /// <inheritdoc/>
            internal override bool TryGetAccessibleObjects(List<AccessibleObjectClassification> classifications, AccessibleObjectVisibility visibility, float maxDistance, List<GameObject> accessibleObjectsList)
            {
                if (maxDistance <= 0)
                {
                    Debug.LogError("The distance from the camera to the objects cannot be less than or equal to zero.");
                    return false;
                }

                accessibleObjectsList.Clear();

                AssembleAccessibleObjects(classifications, accessibleObjectsList);
                FilterAccessibleObjects(visibility, maxDistance, accessibleObjectsList);

                return true;
            }

            /// <inheritdoc/>
            public override bool TryGetAccessibleObjectClassifications(List<AccessibleObjectClassification> classifications)
            {
                classifications.Clear();
                classifications.AddRange(accessibleObjects.Keys);
                return true;
            }

            /// <inheritdoc/>
            public override bool TryRegisterAccessibleObject(GameObject accessibleObject, AccessibleObjectClassification classification)
            {
                // Make sure the specified game object has not been previously registered in any classification.
                foreach (IList<GameObject> list in accessibleObjects.Values)
                {
                    if (list.Contains(accessibleObject))
                    {
                        Debug.LogError($"{accessibleObject.name} has already been registered as a accessible object");
                        return false;
                    }
                }

                if (!accessibleObjects.ContainsKey(classification))
                {
                    accessibleObjects.Add(classification, new List<GameObject>());
                }
                accessibleObjects[classification].Add(accessibleObject);

                return true;
            }

            /// <inheritdoc/>
            public override bool TryUnregisterAccessibleObject(GameObject accessibleObject, AccessibleObjectClassification classification)
            {
                List<GameObject> objCollection = accessibleObjects[classification];
                if (!objCollection.Contains(accessibleObject))
                {
                    Debug.LogError($"{accessibleObject.name} has not been registered as a accessible object of classification {classification}");
                    return false;
                }

                if (!objCollection.Remove(accessibleObject))
                {
                    Debug.LogError($"Failed to unregister {accessibleObject.name} as a accessible object of classification {classification}");
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Collects the registered <see cref="GameObject"/>s from the requested <see cref="AccessibleObjectClassification"/>s.
            /// </summary>
            /// <param name="classifications">The combined flags specifiying the desired classification(s) (people, places, things, etc.).</param>
            /// <param name="accessibleObjectsList">The collection which will receive the requested <see cref="GameObject"/>s.</param>
            /// <remarks>When this method is called, the objectList will be cleared prior to adding the requested <see cref="GameObject"/>s.</remarks>
            private void AssembleAccessibleObjects(
                List<AccessibleObjectClassification> classifications,
                List<GameObject> accessibleObjectsList)
            {
                foreach (AccessibleObjectClassification classification in classifications)
                {
                    accessibleObjectsList.AddRange(accessibleObjects[classification]);
                }
            }

            /// <summary>
            /// Filters the provided collection of <see cref="GameObject"/>s based on the visibility to the main camera
            /// and the maximum allowed distance.
            /// </summary>
            /// <param name="visibility">The desired visibility (in field of view, full surround, etc.) of the <see cref="GameObject"/>s.</param>
            /// <param name="maxDistance">The distance, in meters, beyond which <see cref="GameObject"/>s will be filtered.</param>
            /// <param name="objectList">The collection of <see cref="GameObject"/>s that will be filtered based on visibility and distance.</param>
            private void FilterAccessibleObjects(
                AccessibleObjectVisibility visibility,
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
                    if ((visibility == AccessibleObjectVisibility.FieldOfView) &&
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

            #endregion Accessible object management

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

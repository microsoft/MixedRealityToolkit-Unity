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
            public override IReadOnlyList<GameObject> GetDescribableObjects(
                float maxDistance,
                ObjectClassification classification,
                ReaderView readerView)
            {
                // todo
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override bool RegisterDescribableObject(
                GameObject gameObj,
                ObjectClassification classification)
            {
                // todo
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override void UnregisterDescribableObject(GameObject gameObj)
            {
                // todo
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

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Devices.Hands;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.Services.InputSimulation
{
    public class InputTestRecordingBehaviour : PlayableBehaviour
    {
        public InputTestAsset InputTest = null;
        public GameObject TestedObject = null;

        private List<Component> recordedComponents;

        public override void OnGraphStart(Playable playable)
        {
            // Find all components in the scene that can be tested
            recordedComponents = new List<Component>();
            if (TestedObject)
            {
                Component[] components = TestedObject.GetComponentsInChildren<Component>(true);
                foreach (var comp in components)
                {
                    if (InteractionTester.TryGetTester(comp.GetType(), out var tester))
                    {
                        recordedComponents.Add(comp);
                    }
                }
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            recordedComponents = null;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // Only supported during play mode
            if (!Application.isPlaying)
            {
                return;
            }
            if (InputTest == null)
            {
                return;
            }

            double currentTime = playable.GetTime();
            InputTestAnimationUtils.ApplyInputTestAnimation(InputTest.InputAnimation, currentTime);
            InputTestAnimationUtils.RecordExpectedValues(InputTest.ExpectedValues, currentTime, recordedComponents);

            EditorUtility.SetDirty(InputTest);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (Application.isPlaying)
            {
                AssetDatabase.SaveAssets();

                var inputSimService = MixedRealityToolkit.Instance.GetService<InputSimulationService>();
                if (inputSimService != null)
                {
                    inputSimService.UserInputEnabled = true;
                }
            }
        }
    }
}
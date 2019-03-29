// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input
{
    // internal class InputTestAnimationRecorder : MonoBehaviour
    // {
    //     internal InputTestAnimation Animation = null;

    //     private int startFrame;
    //     private List<Component> recordedComponents;

    //     public void Start()
    //     {
    //         startFrame = Time.frameCount;

    //         // Find all components in the scene that can be tested
    //         recordedComponents = new List<Component>();
    //         GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
    //         foreach (var go in objects)
    //         {
    //             var components = new List<Component>();
    //             go.GetComponents<Component>(components);
    //             foreach (var comp in components)
    //             {
    //                 if (InteractionTester.TryGetTester(comp.GetType(), out var tester))
    //                 {
    //                     recordedComponents.Add(comp);
    //                 }
    //             }
    //         }
    //     }

    //     public void Update()
    //     {
    //         if (Animation == null)
    //         {
    //             return;
    //         }

    //         int currentFrame = Time.frameCount - startFrame;
    //         // TODO make these input sim settings or so
    //         float epsilonTime = 0.1f;
    //         float epsilonJointPositions = 0.01f;
    //         float epsilonCameraPosition = 0.05f;
    //         float epsilonCameraRotation = Mathf.Deg2Rad * 2.0f;
    //         InputTestAnimationUtils.RecordInputTestKeyframeFiltered(Animation, currentFrame, epsilonTime, epsilonJointPositions, epsilonCameraPosition, epsilonCameraRotation);
    //         InputTestAnimationUtils.RecordExpectedValues(Animation, currentFrame, recordedComponents);

    //         EditorUtility.SetDirty(Animation);
    //     }
    // }

    public static class InputTestAnimationEditorUtils
    {
    //     public static void AddSingleKeyframe(InputTestAnimation animation)
    //     {
    //         InputTestAnimationUtils.RecordInputTestKeyframe(animation, Time.frameCount);
    //         EditorUtility.SetDirty(animation);
    //     }

    //     public static void RemoveKeyframe(InputTestAnimation animation, InputTestData keyframe)
    //     {
    //         Undo.RecordObject(animation, "Removed keyframe");
    //         animation.InputCurve.RemoveKeyframe(keyframe);
    //         EditorUtility.SetDirty(animation);
    //     }

        // public static void ClearAllKeyframes(InputTestAsset inputTest)
        // {
        //     Undo.RecordObject(inputTest, "Cleared all keyframes");
        //     inputTest.InputAnimation.ClearKeyframes();
        //     inputTest.ExpectedValues.Clear();
        //     EditorUtility.SetDirty(inputTest);
        // }

    //     public static void SetKeyframeTime(InputTestAnimation animation, int oldFrame, int newFrame)
    //     {
    //         Undo.RecordObject(animation, "Changed keyframe time");
    //         animation.InputCurve.SetKeyframeTime(oldFrame, newFrame);
    //         EditorUtility.SetDirty(animation);
    //     }

    //     public static bool IsRecordingTestAnimation()
    //     {
    //         var mrtkObject = MixedRealityToolkit.Instance?.gameObject;
    //         if (!mrtkObject)
    //         {
    //             return false;
    //         }

    //         return mrtkObject.GetComponent<InputTestAnimationRecorder>() != null;
    //     }

    //     public static void StartRecordingTestAnimation(InputTestAnimation animation)
    //     {
    //         if (!Application.isPlaying)
    //         {
    //             return;
    //         }

    //         var mrtkObject = MixedRealityToolkit.Instance?.gameObject;
    //         if (!mrtkObject || mrtkObject.GetComponent<InputTestAnimationRecorder>() != null)
    //         {
    //             return;
    //         }

    //         Undo.RecordObject(animation, "Started recording test animation");

    //         var recorder = mrtkObject.AddComponent<InputTestAnimationRecorder>();
    //         recorder.Animation = animation;
    //     }

    //     public static void StopRecordingTestAnimation()
    //     {
    //         var mrtkObject = MixedRealityToolkit.Instance?.gameObject;
    //         var recorder = mrtkObject?.GetComponent<InputTestAnimationRecorder>();
    //         if (recorder)
    //         {
    //             GameObject.Destroy(recorder);
    //         }
    //     }
    }
}
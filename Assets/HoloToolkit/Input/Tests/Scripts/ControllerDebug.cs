// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using GLTF;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR && UNITY_WSA
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Input.Spatial;
#endif

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// This script can be used to test motion controller input.
    /// </summary>
    public class ControllerDebug : ControllerVisualizer
    {
        public bool LoadGLTFFile;
        public string GLTFName;
        public Text TextPanel;

#if !UNITY_EDITOR && UNITY_WSA
        private SpatialInteractionManager sim;
#endif

        private IEnumerator Start()
        {
            TextPanel.text += "Start\n";

            if (LoadGLTFFile)
            {
                if (File.Exists(Path.Combine(Application.streamingAssetsPath, GLTFName)))
                {
                    if (GLTFShader == null)
                    {
                        Debug.Log("If using glTF, please specify a shader on " + name + ".");
                        yield break;
                    }

                    byte[] gltfData = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, GLTFName));

                    GameObject controllerModelGO = new GameObject()
                    {
                        name = "Controller" + Guid.NewGuid().ToString()
                    };
                    controllerModelGO.transform.SetParent(transform);
                    GLTFComponent gltfScript = controllerModelGO.AddComponent<GLTFComponent>();
                    gltfScript.GLTFStandard = GLTFShader;
                    gltfScript.SetData(gltfData);
                    yield return gltfScript.LoadModel();

                    ControllerInfo newControllerInfo = controllerModelGO.AddComponent<ControllerInfo>();
                    newControllerInfo.LoadInfo(controllerModelGO.GetComponentsInChildren<Transform>(), this);
                }
                else
                {
                    TextPanel.text += "The GLTF file specified on " + name + " does not exist in the StreamingAssets folder.";
                }
            }

#if !UNITY_EDITOR && UNITY_WSA
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                sim = SpatialInteractionManager.GetForCurrentView();
                if (sim != null)
                {
                    sim.SourceDetected += Sim_SourceDetected;
                    sim.SourceLost += Sim_SourceLost;

                    sim.SourcePressed += Sim_SourcePressed;
                    sim.SourceReleased += Sim_SourceReleased;
                }
            }, true);
#endif
        }

#if !UNITY_EDITOR && UNITY_WSA
        private void Sim_SourceReleased(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            UnityEngine.WSA.Application.InvokeOnAppThread(() =>
            {
                TextPanel.text += "Release...\n";
            }, true);
        }

        private void Sim_SourcePressed(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            UnityEngine.WSA.Application.InvokeOnAppThread(() =>
            {
                TextPanel.text += "Press...\n";
            }, true);
        }

        private void Sim_SourceLost(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            SpatialInteractionSource source = args.State.Source;
            if (source.Kind == SpatialInteractionSourceKind.Controller)
            {
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    TextPanel.text += source.Handedness + " Controller Lost\n";
                }, true);
            }
        }

        private void Sim_SourceDetected(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            SpatialInteractionSource source = args.State.Source;
            if (source.Kind == SpatialInteractionSourceKind.Controller)
            {
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    TextPanel.text += source.Handedness + " Controller Detected\n";
                    StartCoroutine(CheckControllerModel(source.Controller));
                }, true);
            }
        }

        private IEnumerator CheckControllerModel(SpatialInteractionController controller)
        {
            IAsyncOperation<IRandomAccessStreamWithContentType> modelTask = controller.TryGetRenderableModelAsync();

            if (modelTask == null)
            {
                TextPanel.text += "Model task null\n";
                yield break;
            }

            while (modelTask.Status == AsyncStatus.Started)
            {
                yield return null;
            }

            IRandomAccessStreamWithContentType modelStream = modelTask.GetResults();

            if (modelStream == null)
            {
                TextPanel.text += "Model stream null\n";
                yield break;
            }
        }
#endif
    }
}
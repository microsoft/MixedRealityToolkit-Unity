// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoundingBoxTest : InputSystemGlobalListener, IMixedRealitySpeechHandler
{
    [SerializeField]
    private TextMesh statusText;

    private bool speechTriggeredFalg;
    private Vector3 cubePosition = new Vector3(0, 0, 2);

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(Sequence());
    }

    private void SetStatus(string status)
    {
        Debug.Assert(statusText != null, "statusText on BoundingBoxTest should not be null");
        statusText.text = $"Test {status}\nPress '1' or say 'select' to continue";
    }

    private IEnumerator Sequence()
    {
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = LoadAsset<Material>("MRTK_Standard_DarkGray");
            cube.transform.position = cubePosition;

            SetStatus("Instantiate BoundingBox");
            BoundingBox bbox = cube.AddComponent<BoundingBox>();
            bbox.HideElementsInInspector = false;
            bbox.BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateOnStart;
            var mh = cube.AddComponent<ManipulationHandler>();
            yield return WaitForSpeechCommand();

            SetStatus("FlattenX");
            bbox.FlattenAxis = BoundingBox.FlattenModeType.FlattenX;
            yield return WaitForSpeechCommand();

            SetStatus("FlattenY");
            bbox.FlattenAxis = BoundingBox.FlattenModeType.FlattenY;
            yield return WaitForSpeechCommand();

            SetStatus("FlattenNone");
            bbox.FlattenAxis = BoundingBox.FlattenModeType.DoNotFlatten;
            yield return WaitForSpeechCommand();

            SetStatus("BoxPadding 0.2f");
            bbox.BoxPadding = new Vector3(0.2f, 0.2f, 0.2f);
            yield return WaitForSpeechCommand();

            SetStatus("BoxPadding 0");
            bbox.BoxPadding = Vector3.zero;
            yield return WaitForSpeechCommand();

            SetStatus("Set scale handle widget prefab");
            bbox.ScaleHandleSize = 0.03f;
            bbox.ScaleHandlePrefab = LoadAsset<GameObject>("MRTK_BoundingBox_ScaleWidget");
            yield return WaitForSpeechCommand();

            SetStatus("Handles red");
            bbox.HandleMaterial = LoadAsset<Material>("MRTK_Standard_Red");
            yield return WaitForSpeechCommand();

            SetStatus("BBox material cyan");
            bbox.BoxMaterial = LoadAsset<Material>("MRTK_Standard_Cyan");
            yield return WaitForSpeechCommand();

            SetStatus("BBox material none");
            bbox.BoxMaterial = null;
            yield return WaitForSpeechCommand();

            SetStatus("BBox grabbed material green");
            bbox.BoxGrabbedMaterial = LoadAsset<Material>("MRTK_Standard_Emerald");
            yield return WaitForSpeechCommand();

            SetStatus("Wireframe radius 0.1");
            bbox.WireframeEdgeRadius = 0.1f;
            yield return WaitForSpeechCommand();

            SetStatus("Wireframe shape cylinder");
            bbox.WireframeShape = BoundingBox.WireframeType.Cylindrical;
            yield return WaitForSpeechCommand();

            GameObject.Destroy(cube);
        }

        {

            SetStatus("Many children");

            GameObject multiRoot = new GameObject();
            multiRoot.name = "multiRoot";
            multiRoot.transform.position = cubePosition;
            int numCubes = 10;
            for (int i = 0; i < numCubes; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = Vector3.one * 0.1f;
                cube.transform.parent = multiRoot.transform;
                cube.transform.localPosition = Random.insideUnitSphere;
                cube.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360f);
            }
            var bbox = multiRoot.AddComponent<BoundingBox>();
            bbox.BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateOnStart;
            bbox.HideElementsInInspector = false;
            multiRoot.AddComponent<ManipulationHandler>();
        }
    }

    private T LoadAsset<T>(string s) where T : UnityEngine.Object
    {
        string[] paths = AssetDatabase.FindAssets(s);
        var result = (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[0]), typeof(T));
        return result;
    }

    private IEnumerator WaitForSpeechCommand()
    {
        while (!speechTriggeredFalg)
        {
            yield return null;
        }
        speechTriggeredFalg = false;
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData.Command.Keyword.Equals("Select", System.StringComparison.CurrentCultureIgnoreCase))
        {
            speechTriggeredFalg = true;
        }
    }
}

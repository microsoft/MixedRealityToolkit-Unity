// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Text;
using UnityEngine;

public class BoundingBoxTest : InputSystemGlobalListener, IMixedRealitySpeechHandler
{

    public TextMesh statusText;

    public Material darkGrayMaterial;
    public Material redMaterial;
    public Material cyanMaterial;

    public GameObject scaleWidget;

    private bool speechTriggeredFalg;
    private Vector3 cubePosition = new Vector3(0, 0, 2);
    private BoundingBox bbox;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(Sequence());
    }

    private void SetStatus(string status)
    {
        Debug.Assert(statusText != null, "statusText on BoundingBoxTest should not be null");
        StringBuilder b = new StringBuilder();
        b.AppendLine($"{status}");
        b.AppendLine($"Press '1' or say 'select' to continue");
        statusText.text = b.ToString();
    }

    private IEnumerator Sequence()
    {

        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Debug.Assert(darkGrayMaterial != null);
            cube.GetComponent<MeshRenderer>().material = darkGrayMaterial;
            cube.transform.position = cubePosition;

            SetStatus("Instantiate BoundingBox");
            bbox = cube.AddComponent<BoundingBox>();
            bbox.HideElementsInInspector = false;
            bbox.BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateOnStart;
            var mh = cube.AddComponent<ManipulationHandler>();
            yield return WaitForSpeechCommand();

            SetStatus("HideElementsInInspector true");
            bbox.HideElementsInInspector = true;
            yield return WaitForSpeechCommand();

            SetStatus("HideElementsInInspector false");
            bbox.HideElementsInInspector = false;
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

            SetStatus("ShowWireframe false");
            bbox.ShowWireFrame = false;
            yield return WaitForSpeechCommand();

            SetStatus("ShowWireframe true");
            bbox.ShowWireFrame = true;
            yield return WaitForSpeechCommand();

            SetStatus("BoxPadding 0.2f");
            bbox.BoxPadding = new Vector3(0.2f, 0.2f, 0.2f);
            yield return WaitForSpeechCommand();

            SetStatus("BoxPadding 0");
            bbox.BoxPadding = Vector3.zero;
            yield return WaitForSpeechCommand();

            SetStatus("Set scale handle widget prefab");
            bbox.ScaleHandleSize = 0.3f;
            Debug.Assert(scaleWidget != null);
            bbox.ScaleHandlePrefab = scaleWidget;
            yield return WaitForSpeechCommand();

            SetStatus("Handles red");
            bbox.HandleMaterial = redMaterial;
            yield return WaitForSpeechCommand();

            SetStatus("BBox material cyan");
            Debug.Assert(cyanMaterial != null);
            bbox.BoxMaterial = cyanMaterial;
            yield return WaitForSpeechCommand();

            SetStatus("BBox grabbed material red");
            bbox.BoxGrabbedMaterial = redMaterial;
            mh.OnManipulationStarted.AddListener((med) => bbox.HighlightWires());
            mh.OnManipulationEnded.AddListener((med) => bbox.UnhighlightWires());
            yield return WaitForSpeechCommand();

            SetStatus("BBox material none");
            bbox.BoxMaterial = null;
            yield return WaitForSpeechCommand();

            SetStatus("Scale X and update rig");
            cube.transform.localScale = new Vector3(2, 1, 1);
            bbox.CreateRig();
            yield return WaitForSpeechCommand();

            SetStatus("Rotate 20 degrees and update rig");
            cube.transform.localRotation = Quaternion.Euler(0, 20, 0);
            bbox.ShowRotationHandleForY = true;
            bbox.CreateRig();
            yield return WaitForSpeechCommand();

            SetStatus("Wireframe radius 0.1");
            bbox.WireframeEdgeRadius = 0.1f;
            yield return WaitForSpeechCommand();

            SetStatus("Wireframe shape cylinder");
            bbox.WireframeShape = BoundingBox.WireframeType.Cylindrical;
            yield return WaitForSpeechCommand();

            Destroy(cube);
        }

        {
            SetStatus("Many children");

            GameObject multiRoot = new GameObject();
            multiRoot.name = "multiRoot";
            multiRoot.transform.position = cubePosition;
            int numCubes = 10;
            for (int i = 0; i < numCubes; i++)
            {
                var cubechild = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubechild.transform.localScale = Vector3.one * 0.1f;
                cubechild.transform.parent = multiRoot.transform;
                cubechild.transform.localPosition = Random.insideUnitSphere;
                cubechild.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360f);
            }
            bbox = multiRoot.AddComponent<BoundingBox>();
            bbox.BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateOnStart;
            bbox.HideElementsInInspector = false;
            multiRoot.AddComponent<ManipulationHandler>();
            yield return WaitForSpeechCommand();

            Destroy(multiRoot);
        }

        SetStatus("Done!");
    }
    private void DebugDrawObjectBounds(Bounds bounds)
    {
        DebugUtilities.DrawPoint(bounds.min, Color.magenta);
        DebugUtilities.DrawPoint(bounds.max, Color.yellow);
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

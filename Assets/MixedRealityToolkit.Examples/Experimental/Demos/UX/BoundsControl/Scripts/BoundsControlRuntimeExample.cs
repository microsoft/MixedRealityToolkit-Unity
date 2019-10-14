// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.Experimental;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental.Demos
{
    /// <summary>
    /// TODO: This demo probably needs to be adjusted
    /// Currently it's just a copy of BoundingBoxExampleTest
    /// </summary>
    public class BoundsControlRuntimeExample : MonoBehaviour, IMixedRealitySpeechHandler
    {
        public TextMeshPro statusText;

        public Material darkGrayMaterial;
        public Material redMaterial;
        public Material cyanMaterial;

        public GameObject scaleWidget;

        private bool speechTriggeredFlag;
        private Vector3 cubePosition = new Vector3(0, 0, 2);
        private BoundsControl bbox;

        protected virtual void OnEnable()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
        }

        protected virtual void OnDisable()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            StartCoroutine(Sequence());
        }

        private void SetStatus(string status)
        {
            Debug.Assert(statusText != null, "statusText on BoundsControlRuntimeExample should not be null");
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

                SetStatus("Instantiate BoundsControl");
                bbox = cube.AddComponent<BoundsControl>();
                bbox.HideElementsInInspector = false;
                bbox.BoundsControlActivation = UI.Experimental.BoundsControlTypes.BoundsControlActivationType.ActivateOnStart;
                var mh = cube.AddComponent<ManipulationHandler>();
                yield return WaitForSpeechCommand();

                SetStatus("Set Target bounds override");
                var newObject = new GameObject();
                var bc = newObject.AddComponent<BoxCollider>();
                bc.center = new Vector3(.25f, 0, 0);
                bc.size = new Vector3(0.162f, 0.1f, 1);
                bbox.BoundsOverride = bc;
                yield return WaitForSpeechCommand();

                SetStatus("Change target bounds override size");
                bc.size = new Vector3(0.5f, 0.1f, 1);
                yield return WaitForSpeechCommand();

                SetStatus("Remove target bounds override");
                bbox.BoundsOverride = null;
                Destroy(newObject);
                newObject = null;
                yield return WaitForSpeechCommand();

                SetStatus("HideElementsInInspector true");
                bbox.HideElementsInInspector = true;
                yield return WaitForSpeechCommand();

                SetStatus("HideElementsInInspector false");
                bbox.HideElementsInInspector = false;
                yield return WaitForSpeechCommand();

                SetStatus("FlattenX");
                bbox.FlattenAxis = UI.Experimental.BoundsControlTypes.FlattenModeType.FlattenX;
                yield return WaitForSpeechCommand();

                SetStatus("FlattenY");
                bbox.FlattenAxis = UI.Experimental.BoundsControlTypes.FlattenModeType.FlattenY;
                yield return WaitForSpeechCommand();

                SetStatus("FlattenNone");
                bbox.FlattenAxis = UI.Experimental.BoundsControlTypes.FlattenModeType.DoNotFlatten;
                yield return WaitForSpeechCommand();

                SetStatus("ShowWireframe false");
                bbox.Links.ShowWireFrame = false;
                yield return WaitForSpeechCommand();

                SetStatus("ShowWireframe true");
                bbox.Links.ShowWireFrame = true;
                yield return WaitForSpeechCommand();

                SetStatus("BoxPadding 0.2f");
                bbox.BoxPadding = new Vector3(0.2f, 0.2f, 0.2f);
                yield return WaitForSpeechCommand();

                SetStatus("BoxPadding 0");
                bbox.BoxPadding = Vector3.zero;
                yield return WaitForSpeechCommand();

                SetStatus("Set scale handle size 0.3");
                bbox.ScaleHandles.HandleSize = 0.3f;
                yield return WaitForSpeechCommand();

                SetStatus("Set scale handle widget prefab");
                Debug.Assert(scaleWidget != null);
                bbox.ScaleHandles.HandlePrefab = scaleWidget;
                yield return WaitForSpeechCommand();

                SetStatus("Handles red");
                bbox.ScaleHandles.HandleMaterial = redMaterial;
                bbox.RotationHandles.HandleMaterial = redMaterial;
                yield return WaitForSpeechCommand();

                SetStatus("BBox material cyan");
                Debug.Assert(cyanMaterial != null);
                bbox.BoxDisplay.BoxMaterial = cyanMaterial;
                yield return WaitForSpeechCommand();

                SetStatus("BBox grabbed material red");
                bbox.BoxDisplay.BoxGrabbedMaterial = redMaterial;
                mh.OnManipulationStarted.AddListener((med) => bbox.HighlightWires());
                mh.OnManipulationEnded.AddListener((med) => bbox.UnhighlightWires());
                yield return WaitForSpeechCommand();

                SetStatus("BBox material none");
                bbox.BoxDisplay.BoxMaterial = null;
                yield return WaitForSpeechCommand();

                SetStatus("Scale X and update rig");
                cube.transform.localScale = new Vector3(2, 1, 1);
                bbox.CreateRig();
                yield return WaitForSpeechCommand();

                SetStatus("Rotate 20 degrees and update rig");
                cube.transform.localRotation = Quaternion.Euler(0, 20, 0);
                bbox.RotationHandles.ShowRotationHandleForY = true;
                bbox.CreateRig();
                yield return WaitForSpeechCommand();

                SetStatus("Wireframe radius 0.1");
                bbox.Links.WireframeEdgeRadius = 0.1f;
                yield return WaitForSpeechCommand();

                SetStatus("Wireframe shape cylinder");
                bbox.Links.WireframeShape = UI.Experimental.BoundsControlTypes.WireframeType.Cylindrical;
                yield return WaitForSpeechCommand();

                Destroy(cube);
            }

            {
                SetStatus("Many children");

                GameObject multiRoot = new GameObject();
                multiRoot.name = "multiRoot";
                Vector3 forwardOffset = Vector3.forward * .5f;
                multiRoot.transform.position = cubePosition + forwardOffset;

                Transform lastParent = null;

                int numCubes = 10;
                for (int i = 0; i < numCubes; i++)
                {
                    var cubechild = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cubechild.transform.localPosition = Random.insideUnitSphere + cubePosition + forwardOffset;
                    cubechild.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360f);
                    cubechild.transform.parent = lastParent ?? multiRoot.transform;
                    float baseScale = lastParent == null ? 0.1f : 1f;
                    cubechild.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
                    lastParent = cubechild.transform;
                }

                bbox = multiRoot.AddComponent<BoundsControl>();
                bbox.BoundsControlActivation = UI.Experimental.BoundsControlTypes.BoundsControlActivationType.ActivateOnStart;
                bbox.HideElementsInInspector = false;
                bbox.Links.WireframeEdgeRadius = .05f;
                multiRoot.AddComponent<ManipulationHandler>();

                SetStatus("Randomize Child Scale for skewing");
                yield return WaitForSpeechCommand();

                multiRoot.transform.position += Vector3.forward * 200f;

                var childTransform = multiRoot.transform;
                while (childTransform.childCount > 0)
                {
                    childTransform = childTransform.GetChild(0);
                    float baseScale = lastParent == null ? 0.1f : 1f;
                    childTransform.transform.localScale = new Vector3(baseScale * Random.Range(.5f, 2f), baseScale * Random.Range(.5f, 2f), baseScale * Random.Range(.5f, 2f));
                }

                bbox.Links.WireframeEdgeRadius = 1f;
                bbox.CreateRig();
                SetStatus("Delete GameObject");
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
            while (!speechTriggeredFlag)
            {
                yield return null;
            }
            speechTriggeredFlag = false;
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (eventData.Command.Keyword.Equals("Select", System.StringComparison.CurrentCultureIgnoreCase))
            {
                speechTriggeredFlag = true;
            }
        }
    }
}
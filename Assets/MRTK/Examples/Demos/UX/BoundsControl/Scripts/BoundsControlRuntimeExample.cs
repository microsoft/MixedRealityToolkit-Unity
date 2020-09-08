// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This example script demonstrates various bounds control runtime configurations
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
        private BoundsControl boundsControl;

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
                boundsControl = cube.AddComponent<BoundsControl>();
                boundsControl.HideElementsInInspector = false;
                boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateOnStart;
                var om = cube.AddComponent<ObjectManipulator>();
                yield return WaitForSpeechCommand();

                SetStatus("Set Target bounds override");
                var newObject = new GameObject();
                var bc = newObject.AddComponent<BoxCollider>();
                bc.center = new Vector3(.25f, 0, 0);
                bc.size = new Vector3(0.162f, 0.1f, 1);
                boundsControl.BoundsOverride = bc;
                yield return WaitForSpeechCommand();

                SetStatus("Change target bounds override size");
                bc.size = new Vector3(0.5f, 0.1f, 1);
                yield return WaitForSpeechCommand();

                SetStatus("Remove target bounds override");
                boundsControl.BoundsOverride = null;
                Destroy(newObject);
                newObject = null;
                yield return WaitForSpeechCommand();

                SetStatus("HideElementsInInspector true");
                boundsControl.HideElementsInInspector = true;
                yield return WaitForSpeechCommand();

                SetStatus("HideElementsInInspector false");
                boundsControl.HideElementsInInspector = false;
                yield return WaitForSpeechCommand();

                SetStatus("FlattenX");
                boundsControl.FlattenAxis = FlattenModeType.FlattenX;
                yield return WaitForSpeechCommand();

                SetStatus("FlattenY");
                boundsControl.FlattenAxis = FlattenModeType.FlattenY;
                yield return WaitForSpeechCommand();

                SetStatus("FlattenNone");
                boundsControl.FlattenAxis = FlattenModeType.DoNotFlatten;
                yield return WaitForSpeechCommand();

                SetStatus("ShowWireframe false");
                boundsControl.LinksConfig.ShowWireFrame = false;
                yield return WaitForSpeechCommand();

                SetStatus("ShowWireframe true");
                boundsControl.LinksConfig.ShowWireFrame = true;
                yield return WaitForSpeechCommand();

                SetStatus("BoxPadding 0.2f");
                boundsControl.BoxPadding = new Vector3(0.2f, 0.2f, 0.2f);
                yield return WaitForSpeechCommand();

                SetStatus("BoxPadding 0");
                boundsControl.BoxPadding = Vector3.zero;
                yield return WaitForSpeechCommand();

                SetStatus("Set scale handle size 0.3");
                boundsControl.ScaleHandlesConfig.HandleSize = 0.3f;
                yield return WaitForSpeechCommand();

                SetStatus("Set scale handle widget prefab");
                Debug.Assert(scaleWidget != null);
                boundsControl.ScaleHandlesConfig.HandlePrefab = scaleWidget;
                yield return WaitForSpeechCommand();

                SetStatus("Handles red");
                boundsControl.ScaleHandlesConfig.HandleMaterial = redMaterial;
                boundsControl.RotationHandlesConfig.HandleMaterial = redMaterial;
                yield return WaitForSpeechCommand();

                SetStatus("BBox material cyan");
                Debug.Assert(cyanMaterial != null);
                boundsControl.BoxDisplayConfig.BoxMaterial = cyanMaterial;
                yield return WaitForSpeechCommand();

                SetStatus("BBox grabbed material red");
                boundsControl.BoxDisplayConfig.BoxGrabbedMaterial = redMaterial;
                om.OnManipulationStarted.AddListener((med) => boundsControl.HighlightWires());
                om.OnManipulationEnded.AddListener((med) => boundsControl.UnhighlightWires());
                yield return WaitForSpeechCommand();

                SetStatus("BBox material none");
                boundsControl.BoxDisplayConfig.BoxMaterial = null;
                yield return WaitForSpeechCommand();

                SetStatus("Scale X and update rig");
                cube.transform.localScale = new Vector3(2, 1, 1);
                yield return WaitForSpeechCommand();

                SetStatus("Rotate 20 degrees and update rig");
                cube.transform.localRotation = Quaternion.Euler(0, 20, 0);
                boundsControl.RotationHandlesConfig.ShowHandleForY = true;
                yield return WaitForSpeechCommand();

                SetStatus("Wireframe radius 0.1");
                boundsControl.LinksConfig.WireframeEdgeRadius = 0.1f;
                yield return WaitForSpeechCommand();

                SetStatus("Wireframe shape cylinder");
                boundsControl.LinksConfig.WireframeShape = WireframeType.Cylindrical;
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

                boundsControl = multiRoot.AddComponent<BoundsControl>();
                boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateOnStart;
                boundsControl.HideElementsInInspector = false;
                boundsControl.LinksConfig.WireframeEdgeRadius = .05f;
                multiRoot.AddComponent<ObjectManipulator>();

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

                boundsControl.LinksConfig.WireframeEdgeRadius = 1f;
                boundsControl.CreateRig();
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
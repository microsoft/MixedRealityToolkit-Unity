// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This example script demonstrates various bounds control runtime configurations
    /// </summary>
    public class BoundsControlRuntimeExample : MonoBehaviour//, IMixedRealitySpeechHandler
    {
        public TextMeshPro statusText;

        public Material darkGrayMaterial;
        public Material redMaterial;
        public Material cyanMaterial;

        public GameObject boundsVisualsPrefab;

        private bool speechTriggeredFlag;
        private Vector3 cubePosition = new Vector3(0, 1, 2);
        private BoundsControl boundsControl;

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
            b.AppendLine($"Press Next or say 'select' to continue");
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
                var cm = cube.AddComponent<ConstraintManager>();
                var om = cube.AddComponent<ObjectManipulator>();
                boundsControl = cube.AddComponent<BoundsControl>();
                boundsControl.BoundsVisualsPrefab = boundsVisualsPrefab;
                //boundsControl.HideElementsInInspector = false;
                //TODO: this looks like it would be controlled by stateful interactable or object manipulator?
                //boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateOnStart;
                yield return WaitForButtonPressOrCommand();

                SetStatus("Set Target bounds override");
                var newObject = new GameObject();
                var bc = newObject.AddComponent<BoxCollider>();
                bc.center = new Vector3(.25f, 0, 0);
                bc.size = new Vector3(0.162f, 0.1f, 1);
                boundsControl.OverrideBounds = true;
                boundsControl.BoundsOverride = bc.transform;
                yield return WaitForButtonPressOrCommand();

                SetStatus("Change target bounds override size");
                bc.size = new Vector3(0.5f, 0.1f, 1);
                yield return WaitForButtonPressOrCommand();

                SetStatus("Remove target bounds override");
                boundsControl.OverrideBounds = false;
                boundsControl.BoundsOverride = null;
                Destroy(newObject);
                yield return WaitForButtonPressOrCommand();

                SetStatus("FlattenAuto");
                boundsControl.FlattenMode = FlattenMode.Auto;
                yield return WaitForButtonPressOrCommand();

                SetStatus("FlattenAlways");
                boundsControl.FlattenMode = FlattenMode.Always;
                yield return WaitForButtonPressOrCommand();

                SetStatus("FlattenNever");
                boundsControl.FlattenMode = FlattenMode.Never;
                yield return WaitForButtonPressOrCommand();

                SetStatus("BoxPadding 0.2f");
                boundsControl.BoundsPadding = 0.2f;
                yield return WaitForButtonPressOrCommand();

                SetStatus("BoxPadding 0");
                boundsControl.BoundsPadding = 0.0f;
                yield return WaitForButtonPressOrCommand();

                SetStatus("Scale X and update rig");
                cube.transform.localScale = new Vector3(2, 1, 1);
                yield return WaitForButtonPressOrCommand();

                SetStatus("Rotate 20 degrees and update rig");
                cube.transform.localRotation = Quaternion.Euler(0, 20, 0);
                yield return WaitForButtonPressOrCommand();

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
                    cubechild.transform.parent = (lastParent != null) ? lastParent : multiRoot.transform;
                    float baseScale = lastParent == null ? 0.1f : 1f;
                    cubechild.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
                    lastParent = cubechild.transform;
                }

                multiRoot.AddComponent<ConstraintManager>();
                multiRoot.AddComponent<ObjectManipulator>();
                boundsControl = multiRoot.AddComponent<BoundsControl>();
                //boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateOnStart;
                //boundsControl.HideElementsInInspector = false;
                //boundsControl.LinksConfig.WireframeEdgeRadius = .05f;

                SetStatus("Randomize Child Scale for skewing");
                yield return WaitForButtonPressOrCommand();

                multiRoot.transform.position += Vector3.forward * 200f;

                var childTransform = multiRoot.transform;
                while (childTransform.childCount > 0)
                {
                    childTransform = childTransform.GetChild(0);
                    float baseScale = lastParent == null ? 0.1f : 1f;
                    childTransform.transform.localScale = new Vector3(baseScale * Random.Range(.5f, 2f), baseScale * Random.Range(.5f, 2f), baseScale * Random.Range(.5f, 2f));
                }

                //boundsControl.LinksConfig.WireframeEdgeRadius = 1f;
                boundsControl.RecomputeBounds(); //TODO: check that this does the same thing
                SetStatus("Delete GameObject");
                yield return WaitForButtonPressOrCommand();

                Destroy(multiRoot);
            }

            SetStatus("Done!");
        }

        private IEnumerator WaitForButtonPressOrCommand()
        {
            while (!speechTriggeredFlag)
            {
                yield return null;
            }
            speechTriggeredFlag = false;
        }

        public void OnShouldAdvanceSequence()
        {
            speechTriggeredFlag = true;
        }
    }
}

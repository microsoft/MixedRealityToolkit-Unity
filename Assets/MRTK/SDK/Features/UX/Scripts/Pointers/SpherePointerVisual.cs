// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [AddComponentMenu("Scripts/MRTK/SDK/SpherePointerVisual")]
    public class SpherePointerVisual : MonoBehaviour
    {
        public Transform TetherEndPoint => tetherEndPoint;

        public bool TetherVisualsEnabled { get; private set; }

        [Tooltip("The pointer these visuals decorate")]
        private SpherePointer pointer;

        [SerializeField]
        [Tooltip("Tether will not be shown unless it is at least this long")]
        private float minTetherLength = 0.03f;

        [SerializeField]
        private Transform visualsRoot = null;

        [SerializeField]
        private Transform tetherEndPoint = null;

        /// Assumption: Tether line is a child of the visuals!
        [SerializeField]
        private BaseMixedRealityLineDataProvider tetherLine = null;

        public void OnEnable()
        {
            CheckInitialization();
        }

        public void OnDestroy()
        {
            if (visualsRoot != null)
            {
                Destroy(visualsRoot.gameObject);
            }
        }

        public void Start()
        {
            // put it at root of scene
            MixedRealityPlayspace.AddChild(visualsRoot.transform);
            visualsRoot.gameObject.name = $"{gameObject.name}_NearTetherVisualsRoot";
        }

        private void CheckInitialization()
        {
            if (pointer == null)
            {
                pointer = GetComponent<SpherePointer>();
            }

            if (pointer == null)
            {
                Debug.LogError($"No SpherePointer found on {gameObject.name}.");
            }

            CheckAsset(visualsRoot, "Visuals Root");
            CheckAsset(tetherEndPoint, "Tether End Point");
            CheckAsset(tetherLine, "Tether Line");
        }

        private void CheckAsset(object asset, string fieldname)
        {
            if (asset == null)
            {
                Debug.LogError($"No {fieldname} specified on {gameObject.name}.SpherePointerVisual. Did you forget to set the {fieldname}?");
            }
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] SpherePointerVisual.Update");

        public void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                TetherVisualsEnabled = false;
                if (pointer.IsFocusLocked && pointer.IsTargetPositionLockedOnFocusLock && pointer.Result != null)
                {
                    NearInteractionGrabbable grabbedObject = GetGrabbedObject();
                    if (grabbedObject != null && grabbedObject.ShowTetherWhenManipulating)
                    {
                        Vector3 graspPosition;
                        pointer.TryGetNearGraspPoint(out graspPosition);
                        tetherLine.FirstPoint = graspPosition;
                        Vector3 endPoint = pointer.Result.Details.Object.transform.TransformPoint(pointer.Result.Details.PointLocalSpace);
                        tetherLine.LastPoint = endPoint;
                        TetherVisualsEnabled = Vector3.Distance(tetherLine.FirstPoint, tetherLine.LastPoint) > minTetherLength;
                        tetherLine.enabled = TetherVisualsEnabled;
                        tetherEndPoint.gameObject.SetActive(TetherVisualsEnabled);
                        tetherEndPoint.position = endPoint;
                    }
                }

                visualsRoot.gameObject.SetActive(TetherVisualsEnabled);
            }
        }

        private static readonly ProfilerMarker GetGrabbedObjectPerfMarker = new ProfilerMarker("[MRTK] SpherePointer.GetGrabbedObject");

        private NearInteractionGrabbable GetGrabbedObject()
        {
            using (GetGrabbedObjectPerfMarker.Auto())
            {
                if (pointer.Result?.Details.Object != null)
                {
                    return pointer.Result.Details.Object.GetComponent<NearInteractionGrabbable>();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
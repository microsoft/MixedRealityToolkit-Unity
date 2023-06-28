// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    using Input;
    using System;

    /// <summary>
    /// This visualizer can be used to represent pointer input data, e.g., from a handheld controller,
    /// from hand, head or eye tracking. In general, it assumes a pointing origin and direction,
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/InputPointerVisualizer")]
    public class InputPointerVisualizer : MonoBehaviour
    {
        public enum VisModes
        {
            ShowAll,
            ShowOnlyDestinations,
            ShowNone
        }

        [SerializeField]
        private bool useLiveInputStream = false;

        private bool showOrigins = false;
        private bool showDestinations = false;
        private bool showLinkD2D = false; // Destination to destination
        private bool showLinkO2D = false; // Origin to destination

        public VisModes ShowVisMode;
        private VisModes showVisMode; // Using a private showTrace to detect when the item is changed in the Editor to trigger an vis update

        [SerializeField]
        private bool onlyShowForHitTargets = false;

        [SerializeField]
        [Tooltip("Template for visualizing vector origin, e.g., a colored sphere.")]
        private ParticleHeatmap templateOrigins = null;

        [SerializeField]
        [Tooltip("Template for visualizing hit pos, e.g., a colored sphere.")]
        private ParticleHeatmap templateDestinations = null;

        [SerializeField]
        [Tooltip("Template for visualizing connecting lines between vector origins - Should be a line renderer.")]
        private GameObject templateLinkOrigToOrig = null;

        [SerializeField]
        [Tooltip("Template for visualizing connecting lines between vector destinations - Should be a line renderer.")]
        private GameObject templateLinkDestToDest = null;

        [SerializeField]
        [Tooltip("Template for visualizing the vector between vector origin and destination - Should be a line renderer.")]
        private GameObject templateLinkOrigToDest = null;

        [SerializeField]
        [Tooltip("Distance to default to in case of no hit target.")]
        private float cursorDist = 2f;

        [SerializeField]
        private FuzzyGazeInteractor gazeInteractor;

        public int numSamples = 20; // Sample-based. Better to make it time-based.
        public TextMesh textOutput;

        // Private variables
        private ParticleHeatmap samplesOrigins;
        private ParticleHeatmap samplesDestinations;

        private GameObject[] samplesLinkOrigToOrig;
        private GameObject[] samplesLinkDestToDest;
        private GameObject[] samplesLinkOrigToDest;

        private int currentItemIndex = 0;
        private VisModes rememberState = VisModes.ShowOnlyDestinations;
        private bool isPaused = false;
        private int numberOfTraceSamples;
        
        private void Start()
        {
            AmountOfSamples = numSamples;

            if (textOutput != null)
                textOutput.text = "";

            SetActive_DataVis(ShowVisMode);

            // Init visualizations
            ResetVisualizations();
        }

        public void ResetVisualizations()
        {
            ResetVis();
            Debug.Log(">>INIT VIS ARRAY " + numberOfTraceSamples);
            InitPointClouds(ref samplesOrigins, templateOrigins);
            InitPointClouds(ref samplesDestinations, templateDestinations);
            InitVisArrayObj(ref samplesLinkOrigToOrig, templateLinkOrigToOrig, numberOfTraceSamples);
            InitVisArrayObj(ref samplesLinkDestToDest, templateLinkDestToDest, numberOfTraceSamples);
            InitVisArrayObj(ref samplesLinkOrigToDest, templateLinkOrigToDest, numberOfTraceSamples);
        }

        private void InitPointClouds(ref ParticleHeatmap pointCloud, ParticleHeatmap templateParticleSystem)
        {
            if (templateParticleSystem != null)
            {
                // Initialize particle system to represent loaded point cloud data
                Debug.Log(">>InitPointClouds 02");
                pointCloud = templateParticleSystem;

            }
            else
                pointCloud = null;
        }

        private void InitVisArrayObj(ref GameObject[] array, GameObject template, int numOfSamples)
        {
            if (template != null)
            {
                // Initialize array of game objects to represent loaded data
                array = new GameObject[numOfSamples];

                // Instantiate copies of the provided template at (0,0,0) - later we simply change the position
                for (int i = 0; i < numberOfTraceSamples; i++)
                {
                    array[i] = Instantiate(template, Vector3.zero, Quaternion.identity) as GameObject;
                    array[i].transform.SetParent(transform, false);
                }
            }
            else
            {
                array = null;
            }
        }

        private void ResetVis()
        {
            ResetPointCloudVis(ref samplesOrigins);
            ResetPointCloudVis(ref samplesDestinations);

            ResetVis(ref samplesLinkOrigToOrig);
            ResetVis(ref samplesLinkDestToDest);
            ResetVis(ref samplesLinkOrigToDest);
        }

        private void ResetVis(ref GameObject[] array)
        {
            if (array != null)
            {
                for (int i = array.Length - 1; i >= 0; i--)
                {
                    Destroy(array[i]);
                }
            }
        }

        private void ResetPointCloudVis(ref ParticleHeatmap pointCloud)
        {
            if (pointCloud != null)
            {
                pointCloud.HideHeatmap();
            }
        }

        private void SetActive_DataVis(VisModes visMode)
        {
            Debug.Log("SetDataVis: " + visMode);

            showVisMode = visMode;
            switch (visMode)
            {
                case VisModes.ShowNone:
                    SetActive_DataVis(false, false, false, false, false);
                    break;
                case VisModes.ShowOnlyDestinations:
                    SetActive_DataVis(false, true, false, true, false);
                    break;
                case VisModes.ShowAll:
                    SetActive_DataVis(true, true, true, true, true);
                    break;
            }
        }

        private void SetActive_DataVis(bool showOrigins, bool showDest, bool showO2O, bool showD2D, bool showO2D)
        {
            this.showOrigins = showOrigins;
            showDestinations = showDest;
            showLinkO2D = showO2O;
            showLinkD2D = showD2D;
            showLinkO2D = showO2D;

            SetActive_PointCloudVis(ref samplesOrigins, showOrigins);
            SetActive_PointCloudVis(ref samplesDestinations, showDest);

            SetActive_DataVis(ref samplesLinkOrigToOrig, showO2O);

            Debug.Log("Set up D2D links: " + showD2D);
            SetActive_DataVis(ref samplesLinkDestToDest, showD2D);

            if (samplesLinkOrigToDest != null)
            {
                for (int i = 0; i < samplesLinkOrigToDest.Length; i++)
                {
                    samplesLinkOrigToDest[i].SetActive(true);
                }
            }
            SetActive_DataVis(ref samplesLinkOrigToDest, showO2D);
        }


        private void SetActive_DataVis(ref GameObject[] array, bool activate)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].SetActive(activate);
                }
            }
        }

        private void SetActive_PointCloudVis(ref ParticleHeatmap pointCloud, bool activate)
        {
            if (pointCloud != null)
            {
                pointCloud.enabled = true;
                pointCloud.ShowHeatmap();
            }
        }

        private void UpdateVis_PointCloud(ref ParticleHeatmap pointCloud, int index, Vector3 newPos, bool show)
        {
            if (pointCloud != null)
            {
                pointCloud.SetParticle(newPos);
                pointCloud.DisplayParticles();
            }
        }

        private void UpdateConnectorLines(ref GameObject[] array, int index, Vector3 fromPos, Vector3 toPos, bool show)
        {
            array[index].SetActive(show);
            LineRenderer line = array[index].GetComponent<LineRenderer>();

            line.SetPosition(0, fromPos);
            line.SetPosition(1, toPos);
        }

        Vector3? _lastDestination;
        public void UpdateDataVis(Ray cursorRay)
        {
            currentItemIndex++;
            if (currentItemIndex >= numberOfTraceSamples)
                currentItemIndex = 0;

            try
            {
                // Vector origin
                UpdateVis_PointCloud(ref samplesOrigins, currentItemIndex, cursorRay.origin, showOrigins);

                // Vector destination / hit pos
                Vector3? v = PerformHitTest(cursorRay);

                if (!v.HasValue && !onlyShowForHitTargets)
                {
                    v = cursorRay.origin + cursorRay.direction.normalized * cursorDist;
                }

                UpdateVis_PointCloud(ref samplesDestinations, currentItemIndex, v.Value, showDestinations);


                // ... Vector destinations 
                if (samplesDestinations != null && samplesLinkDestToDest != null)
                {
                    Vector3? pos1 = _lastDestination;
                    Vector3? pos2 = v.Value;

                    if ((pos1.HasValue) && (pos2.HasValue))
                    {
                        UpdateConnectorLines(ref samplesLinkDestToDest, currentItemIndex, pos1.Value, pos2.Value, showLinkD2D);
                    }
                }

                _lastDestination = v.Value;
                if (samplesDestinations != null && samplesLinkOrigToDest != null)
                {
                    Vector3? pos1 = cursorRay.origin;
                    Vector3? pos2 = v.Value;

                    if (pos1.HasValue && pos2.HasValue)
                    {
                        UpdateConnectorLines(ref samplesLinkOrigToDest, currentItemIndex, pos1.Value, pos2.Value, showLinkO2D);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Log("[CustomVisualizer] Exception: " + exc);
            }
        }

        private Vector3? PerformHitTest(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                return hitInfo.point;
            }

            return null;
        }

        private void Update()
        {
            if (ShowVisMode != showVisMode)
            {
                SetActive_DataVis(ShowVisMode);
            }

            if (!isPaused && useLiveInputStream)
            {
                UpdateDataVis(new Ray(gazeInteractor.rayOriginTransform.position, gazeInteractor.rayOriginTransform.forward));
            }
        }

        public int AmountOfSamples
        {
            get { return numberOfTraceSamples; }
            set
            {
                numberOfTraceSamples = value;
                ResetVisualizations();
            }
        }

        public void ToggleAppState()
        {
            SetAppState(!isPaused);
        }

        public void PauseApp()
        {
            SetAppState(true);
        }

        public void UnpauseApp()
        {
            SetAppState(false);
        }

        public void SetAppState(bool pauseIt)
        {
            isPaused = pauseIt;
            if (textOutput != null)
            {
                if (pauseIt)
                {
                    rememberState = showVisMode;
                    textOutput.text = "Paused...";
                    SetActive_DataVis(VisModes.ShowAll);
                }
                else
                {
                    textOutput.text = "";
                    SetActive_DataVis(rememberState);
                }
            }
        }
    }
}

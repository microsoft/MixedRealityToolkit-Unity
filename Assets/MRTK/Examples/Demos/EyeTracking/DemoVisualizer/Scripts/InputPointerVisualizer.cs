// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
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

        private bool show_Origins = false;
        private bool show_Destinations = false;
        private bool show_LinkD2D = false; // Destination to destination
        private bool show_LinkO2D = false; // Origin to destination

        public VisModes ShowVisMode;
        private VisModes _showVisMode; // Using a private showTrace to detect when the item is changed in the Editor to trigger an vis update

        [SerializeField]
        private bool onlyShowForHitTargets = false;

        [SerializeField]
        [Tooltip("Template for visualizing vector origin, e.g., a colored sphere.")]
        private ParticleHeatmap tmplt_Origins = null;

        [SerializeField]
        [Tooltip("Template for visualizing hit pos, e.g., a colored sphere.")]
        private ParticleHeatmap tmplt_Destinations = null;

        [SerializeField]
        [Tooltip("Template for visualizing connecting lines between vector origins - Should be a line renderer.")]
        private GameObject tmplt_LinkOrigToOrig = null;

        [SerializeField]
        [Tooltip("Template for visualizing connecting lines between vector destinations - Should be a line renderer.")]
        private GameObject tmplt_LinkDestToDest = null;

        [SerializeField]
        [Tooltip("Template for visualizing the vector between vector origin and destination - Should be a line renderer.")]
        private GameObject tmplt_LinkOrigToDest = null;

        [Tooltip("Distance to default to in case of no hit target.")]
        public float cursorDist = 2f;

        public float distThresh = 20.5f;
        public float nhist = 20; // Sample-based. Better to make it time-based.
        public float saccadeThresh = 20;
        public float minNrOfSamples = 4;
        public TextMesh textOutput;

        // Private variables
        private ParticleHeatmap samples_Origins;
        private ParticleHeatmap samples_Destinations;

        private GameObject[] samples_LinkOrigToOrig;
        private GameObject[] samples_LinkDestToDest;
        private GameObject[] samples_LinkOrigToDest;

        private int currentItemIndex = 0;
        private VisModes rememberState = VisModes.ShowOnlyDestinations;
        private bool isPaused = false;
        private int numberOfTraceSamples;

        private void Start()
        {
            AmountOfSamples = (int)nhist;

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
            InitPointClouds(ref samples_Origins, tmplt_Origins, numberOfTraceSamples);
            InitPointClouds(ref samples_Destinations, tmplt_Destinations, numberOfTraceSamples);
            InitVisArrayObj(ref samples_LinkOrigToOrig, tmplt_LinkOrigToOrig, numberOfTraceSamples);
            InitVisArrayObj(ref samples_LinkDestToDest, tmplt_LinkDestToDest, numberOfTraceSamples);
            InitVisArrayObj(ref samples_LinkOrigToDest, tmplt_LinkOrigToDest, numberOfTraceSamples);
        }

        private void InitPointClouds(ref ParticleHeatmap pointCloud, ParticleHeatmap templateParticleSystem, int nrOfSamples)
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

        private void InitVisArrayObj(ref GameObject[] array, GameObject template, int nrOfSamples)
        {
            if (template != null)
            {
                // Initialize array of game objects to represent loaded data
                array = new GameObject[nrOfSamples];

                // Instantiate copies of the provided template at (0,0,0) - later we simply change the position
                for (int i = 0; i < numberOfTraceSamples; i++)
                {
                    array[i] = Instantiate(template, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
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
            ResetPointCloudVis(ref samples_Origins);
            ResetPointCloudVis(ref samples_Destinations);

            ResetVis(ref samples_LinkOrigToOrig);
            ResetVis(ref samples_LinkDestToDest);
            ResetVis(ref samples_LinkOrigToDest);
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

        private void ResetPointCloudVis(ref ParticleHeatmap pntCloud)
        {
            if (pntCloud != null)
            {
                pntCloud.HideHeatmap();
            }
        }

        private void SetActive_DataVis(VisModes visMode)
        {
            Debug.Log("SetDataVis: " + visMode);

            _showVisMode = visMode;
            switch (visMode)
            {
                case VisModes.ShowNone:
                    SetActive_DataVis(false, false, false, false, false); break;
                case VisModes.ShowOnlyDestinations:
                    SetActive_DataVis(false, true, false, true, false); break;
                case VisModes.ShowAll:
                    SetActive_DataVis(true, true, true, true, true); break;
            }
        }

        private void SetActive_DataVis(bool showOrigins, bool showDest, bool showO2O, bool showD2D, bool showO2D)
        {
            show_Origins = showOrigins;
            show_Destinations = showDest;
            show_LinkO2D = showO2O;
            show_LinkD2D = showD2D;
            show_LinkO2D = showO2D;

            SetActive_PointCloudVis(ref samples_Origins, showOrigins);
            SetActive_PointCloudVis(ref samples_Destinations, showDest);

            SetActive_DataVis(ref samples_LinkOrigToOrig, showO2O);

            Debug.Log("Set up D2D links: " + showD2D);
            SetActive_DataVis(ref samples_LinkDestToDest, showD2D);

            if (samples_LinkOrigToDest != null)
            {
                for (int i = 0; i < samples_LinkOrigToDest.Length; i++)
                {
                    samples_LinkOrigToDest[i].SetActive(true);
                }
            }
            SetActive_DataVis(ref samples_LinkOrigToDest, showO2D);
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

        private void SetActive_PointCloudVis(ref ParticleHeatmap pntCloud, bool activate)
        {
            if (pntCloud != null)
            {
                pntCloud.enabled = true;
                pntCloud.ShowHeatmap();
            }
        }

        private void UpdateDataVisPos(ref GameObject[] array, int index, Vector3 newPos, bool show)
        {
            if (array != null)
            {
                array[index].SetActive(show);
                array[index].transform.position = newPos;
            }
        }

        private void UpdateVis_PointCloud(ref ParticleHeatmap pntCloud, int index, Vector3 newPos, bool show)
        {
            if (pntCloud != null)
            {
                pntCloud.SetParticle(newPos);
                pntCloud.DisplayParticles();
            }
        }

        private void UpdateConnectorLines(ref GameObject[] array, int index, Vector3 fromPos, Vector3 toPos, bool show)
        {
            array[index].SetActive(show);
            LineRenderer line = array[index].GetComponent<LineRenderer>();

            line.SetPosition(0, fromPos);
            line.SetPosition(1, toPos);
        }

        private int GetPrevLineIndex(int currentIndex, int arraySize)
        {
            if (currentIndex > 0)
                return (currentIndex - 1);
            else
                return (arraySize - 1);
        }

        Vector3? lastDestination = null;
        public void UpdateDataVis(Ray cursorRay)
        {
            currentItemIndex++;
            if (currentItemIndex >= numberOfTraceSamples)
                currentItemIndex = 0;

            try
            {
                // Vector origin
                UpdateVis_PointCloud(ref samples_Origins, currentItemIndex, cursorRay.origin, show_Origins);

                // Vector destination / hit pos
                Vector3? v = PerformHitTest(cursorRay);

                if ((!v.HasValue) && (!onlyShowForHitTargets))
                {
                    v = cursorRay.origin + cursorRay.direction.normalized * cursorDist;
                }

                UpdateVis_PointCloud(ref samples_Destinations, currentItemIndex, v.Value, show_Destinations);


                // ... Vector destinations 
                if ((samples_Destinations != null) && (samples_LinkDestToDest != null))
                {
                    Vector3? pos1 = lastDestination;
                    Vector3? pos2 = v.Value;

                    if ((pos1.HasValue) && (pos2.HasValue))
                    {
                        UpdateConnectorLines(ref samples_LinkDestToDest, currentItemIndex, pos1.Value, pos2.Value, show_LinkD2D);
                    }
                }

                lastDestination = v.Value;
                if ((samples_Destinations != null) && (samples_LinkOrigToDest != null))
                {
                    Vector3? pos1 = cursorRay.origin;
                    Vector3? pos2 = v.Value;

                    if ((pos1.HasValue) && (pos2.HasValue))
                    {
                        UpdateConnectorLines(ref samples_LinkOrigToDest, currentItemIndex, pos1.Value, pos2.Value, show_LinkO2D);
                    }
                }
            }
            catch (System.Exception exc)
            {
                Debug.Log("[CustomVisualizer] Exception: " + exc);
            }
        }

        private Vector3? PerformHitTest(Ray ray)
        {
            RaycastHit hitInfo = new RaycastHit();
            bool isHit = UnityEngine.Physics.Raycast(ray, out hitInfo);

            if (isHit)
            {
                return hitInfo.point;
            }
            else
                return null;
        }

        private void Update()
        {
            var eyeGazeProvider = CoreServices.InputSystem?.EyeGazeProvider;
            if (eyeGazeProvider == null)
            {
                return;
            }

            if (ShowVisMode != _showVisMode)
            {
                SetActive_DataVis(ShowVisMode);
            }

            if ((!isPaused) && (useLiveInputStream))
            {
                UpdateDataVis(new Ray(eyeGazeProvider.GazeOrigin, eyeGazeProvider.GazeDirection));
            }
        }

        public bool IsDwelling()
        {
            return false;
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
                    rememberState = _showVisMode;
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
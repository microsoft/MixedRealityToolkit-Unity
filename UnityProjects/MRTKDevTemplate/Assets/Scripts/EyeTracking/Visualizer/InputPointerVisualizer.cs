// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    using Input;
    using System;
    using System.Security.Cryptography;

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

        private bool _showOrigins = false;
        private bool _showDestinations = false;
        private bool _showLinkD2D = false; // Destination to destination
        private bool _showLinkO2D = false; // Origin to destination

        public VisModes ShowVisMode;
        private VisModes _showVisMode; // Using a private showTrace to detect when the item is changed in the Editor to trigger an vis update

        [SerializeField]
        private bool _onlyShowForHitTargets = false;

        [SerializeField]
        [Tooltip("Template for visualizing vector origin, e.g., a colored sphere.")]
        private ParticleHeatmap _tmpltOrigins = null;

        [SerializeField]
        [Tooltip("Template for visualizing hit pos, e.g., a colored sphere.")]
        private ParticleHeatmap _tmpltDestinations = null;

        [SerializeField]
        [Tooltip("Template for visualizing connecting lines between vector origins - Should be a line renderer.")]
        private GameObject _tmpltLinkOrigToOrig = null;

        [SerializeField]
        [Tooltip("Template for visualizing connecting lines between vector destinations - Should be a line renderer.")]
        private GameObject _tmpltLinkDestToDest = null;

        [SerializeField]
        [Tooltip("Template for visualizing the vector between vector origin and destination - Should be a line renderer.")]
        private GameObject _tmpltLinkOrigToDest = null;

        [SerializeField]
        [Tooltip("Distance to default to in case of no hit target.")]
        private float _cursorDist = 2f;

        public float nhist = 20; // Sample-based. Better to make it time-based.
        public TextMesh _textOutput;

        // Private variables
        private ParticleHeatmap _samplesOrigins;
        private ParticleHeatmap _samplesDestinations;

        private GameObject[] _samplesLinkOrigToOrig;
        private GameObject[] _samplesLinkDestToDest;
        private GameObject[] _samplesLinkOrigToDest;

        private int _currentItemIndex = 0;
        private VisModes _rememberState = VisModes.ShowOnlyDestinations;
        private bool _isPaused = false;
        private int _numberOfTraceSamples;

        [SerializeField]
        private FuzzyGazeInteractor _gazeInteractor;

        private void Start()
        {
            AmountOfSamples = (int)nhist;

            if (_textOutput != null)
                _textOutput.text = "";

            SetActive_DataVis(ShowVisMode);

            // Init visualizations
            ResetVisualizations();
        }

        public void ResetVisualizations()
        {
            ResetVis();
            Debug.Log(">>INIT VIS ARRAY " + _numberOfTraceSamples);
            InitPointClouds(ref _samplesOrigins, _tmpltOrigins);
            InitPointClouds(ref _samplesDestinations, _tmpltDestinations);
            InitVisArrayObj(ref _samplesLinkOrigToOrig, _tmpltLinkOrigToOrig, _numberOfTraceSamples);
            InitVisArrayObj(ref _samplesLinkDestToDest, _tmpltLinkDestToDest, _numberOfTraceSamples);
            InitVisArrayObj(ref _samplesLinkOrigToDest, _tmpltLinkOrigToDest, _numberOfTraceSamples);
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

        private void InitVisArrayObj(ref GameObject[] array, GameObject template, int nrOfSamples)
        {
            if (template != null)
            {
                // Initialize array of game objects to represent loaded data
                array = new GameObject[nrOfSamples];

                // Instantiate copies of the provided template at (0,0,0) - later we simply change the position
                for (int i = 0; i < _numberOfTraceSamples; i++)
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
            ResetPointCloudVis(ref _samplesOrigins);
            ResetPointCloudVis(ref _samplesDestinations);

            ResetVis(ref _samplesLinkOrigToOrig);
            ResetVis(ref _samplesLinkDestToDest);
            ResetVis(ref _samplesLinkOrigToDest);
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

            _showVisMode = visMode;
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
            _showOrigins = showOrigins;
            _showDestinations = showDest;
            _showLinkO2D = showO2O;
            _showLinkD2D = showD2D;
            _showLinkO2D = showO2D;

            SetActive_PointCloudVis(ref _samplesOrigins, showOrigins);
            SetActive_PointCloudVis(ref _samplesDestinations, showDest);

            SetActive_DataVis(ref _samplesLinkOrigToOrig, showO2O);

            Debug.Log("Set up D2D links: " + showD2D);
            SetActive_DataVis(ref _samplesLinkDestToDest, showD2D);

            if (_samplesLinkOrigToDest != null)
            {
                for (int i = 0; i < _samplesLinkOrigToDest.Length; i++)
                {
                    _samplesLinkOrigToDest[i].SetActive(true);
                }
            }
            SetActive_DataVis(ref _samplesLinkOrigToDest, showO2D);
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
            _currentItemIndex++;
            if (_currentItemIndex >= _numberOfTraceSamples)
                _currentItemIndex = 0;

            try
            {
                // Vector origin
                UpdateVis_PointCloud(ref _samplesOrigins, _currentItemIndex, cursorRay.origin, _showOrigins);

                // Vector destination / hit pos
                Vector3? v = PerformHitTest(cursorRay);

                if (!v.HasValue && !_onlyShowForHitTargets)
                {
                    v = cursorRay.origin + cursorRay.direction.normalized * _cursorDist;
                }

                UpdateVis_PointCloud(ref _samplesDestinations, _currentItemIndex, v.Value, _showDestinations);


                // ... Vector destinations 
                if (_samplesDestinations != null && _samplesLinkDestToDest != null)
                {
                    Vector3? pos1 = _lastDestination;
                    Vector3? pos2 = v.Value;

                    if ((pos1.HasValue) && (pos2.HasValue))
                    {
                        UpdateConnectorLines(ref _samplesLinkDestToDest, _currentItemIndex, pos1.Value, pos2.Value, _showLinkD2D);
                    }
                }

                _lastDestination = v.Value;
                if ((_samplesDestinations != null) && (_samplesLinkOrigToDest != null))
                {
                    Vector3? pos1 = cursorRay.origin;
                    Vector3? pos2 = v.Value;

                    if (pos1.HasValue && (pos2.HasValue))
                    {
                        UpdateConnectorLines(ref _samplesLinkOrigToDest, _currentItemIndex, pos1.Value, pos2.Value, _showLinkO2D);
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
            if (ShowVisMode != _showVisMode)
            {
                SetActive_DataVis(ShowVisMode);
            }

            if (!_isPaused && useLiveInputStream)
            {
                UpdateDataVis(new Ray(_gazeInteractor.rayOriginTransform.position, _gazeInteractor.rayOriginTransform.forward));
            }
        }

        public int AmountOfSamples
        {
            get { return _numberOfTraceSamples; }
            set
            {
                _numberOfTraceSamples = value;
                ResetVisualizations();
            }
        }

        public void ToggleAppState()
        {
            SetAppState(!_isPaused);
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
            _isPaused = pauseIt;
            if (_textOutput != null)
            {
                if (pauseIt)
                {
                    _rememberState = _showVisMode;
                    _textOutput.text = "Paused...";
                    SetActive_DataVis(VisModes.ShowAll);
                }
                else
                {
                    _textOutput.text = "";
                    SetActive_DataVis(_rememberState);
                }
            }
        }
    }
}

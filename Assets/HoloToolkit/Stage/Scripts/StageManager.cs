using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA;

namespace HoloToolkit.Unity.Stage
{
    /// <summary>
    /// Uses the StageRoot component to ensure we the coordinate system grounded at 0,0,0 for occluded devices.
    /// Places a floor quad as a child of the stage root at 0,0,0.
    /// Will also draw the bounds of your space if you set it during the Mixed Reality Portal first run experience.
    /// </summary>
    public class StageManager : SingleInstance<StageManager>
    {
        [Tooltip("Quad prefab to display as the floor.")]
        public GameObject FloorQuad;
        private GameObject floorQuadInstance;

        [Tooltip("Material used to draw bounds for the stage. Leave empty if you have not setup your space or don't want to render bounds.")]
        public Material StageBoundsMaterial;

        StageRoot stageRoot = null;
        bool updateStageBounds = true;

        public Vector3[] EditorLines;
        List<GameObject> boundingBoxLines = new List<GameObject>();

        private bool renderStage = true;
        public bool RenderStage
        {
            get
            {
                return renderStage;
            }
            set
            {
                if (renderStage != value)
                {
                    renderStage = value;
                    SetRendering();
                }
            }
        }

        private void SetRendering()
        {
            if (floorQuadInstance != null)
            {
                floorQuadInstance.SetActive(renderStage);
            }

            foreach (GameObject go in boundingBoxLines)
            {
                go.SetActive(renderStage);
            }
        }

        private void Start()
        {
            stageRoot = GetComponent<StageRoot>();
            if (stageRoot == null)
            {
                Debug.Log("Adding a StageRoot component to the game object.");
                stageRoot = gameObject.AddComponent<StageRoot>();
            }
            stageRoot.OnTrackingChanged += StageRoot_OnTrackingChanged;

            // Render the floor as a child of the StageRoot component.
            if (FloorQuad != null && stageRoot != null &&
                HolographicSettings.IsDisplayOpaque)
            {
                floorQuadInstance = Instantiate(FloorQuad);
                floorQuadInstance.SetActive(true);

                // Set the floor's parent to be the StageRoot's parent 
                floorQuadInstance.transform.SetParent(gameObject.transform.parent);

#if UNITY_EDITOR
                // So the floor quad does not occlude in editor testing, draw it lower.
                floorQuadInstance.transform.localPosition = new Vector3(0, -3, 0);
                updateStageBounds = true;

                UpdateStageBounds();
#else
                // Draw the floor at 0,0,0 under stage root.
                floorQuadInstance.transform.localPosition = Vector3.zero;
#endif
            }
        }

        private void StageRoot_OnTrackingChanged(StageRoot self, bool located)
        {
            Debug.Log("Stage root tracking changed " + located);
            // Hide the floor if tracking is lost or if StageRoot can't be located.
            if (floorQuadInstance != null &&
                HolographicSettings.IsDisplayOpaque)
            {
                floorQuadInstance.SetActive((located && renderStage));
                if (located)
                {
                    floorQuadInstance.transform.localPosition = new Vector3(0, Mathf.Min(-1.5f, transform.position.y), 0);
                }
                updateStageBounds = located;
            }
        }

        private void Update()
        {
            // This is simply showing how to draw the bounds.
            // Applications don't *need* to draw bounds. 
            // Bounds are more useful for placing objects.
#if !UNITY_EDITOR
            if (updateStageBounds && HolographicSettings.IsDisplayOpaque)
            {
                UpdateStageBounds();
            }
#endif
        }

        private void UpdateStageBounds()
        {
            RemoveBoundingBox();

#if UNITY_EDITOR
            Vector3[] bounds = EditorLines;
            bool tryGetBoundsSuccess = true;
#else
            Vector3[] bounds = null;
            bool tryGetBoundsSuccess = stageRoot.TryGetBounds(out bounds);
#endif
            if (tryGetBoundsSuccess && bounds != null && bounds.Length > 1)
            {
                if (StageBoundsMaterial != null)
                {
                    Vector3 start;
                    Vector3 end;
                    for (int i = 1; i < bounds.Length; i++)
                    {
                        start = bounds[i - 1];
                        end = bounds[i];
                        DrawLine(start, end);
                    }
                    DrawLine(bounds[0], bounds[bounds.Length - 1]);
                    updateStageBounds = false;
                }
            }
        }

        private void DrawLine(Vector3 start, Vector3 end)
        {
            GameObject boundingBox = new GameObject();
            boundingBoxLines.Add(boundingBox);
            boundingBox.transform.SetParent(this.transform.parent);

            LineRenderer lr = boundingBox.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.sharedMaterial = StageBoundsMaterial;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }

        private void RemoveBoundingBox()
        {
            if (boundingBoxLines != null)
            {
                foreach (GameObject boundingBoxLine in boundingBoxLines)
                {
                    DestroyImmediate(boundingBoxLine);
                }
            }
        }
    }
}
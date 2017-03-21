using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.Playspace
{
    /// <summary>
    /// Uses the StageRoot component to ensure we the coordinate system grounded at 0,0,0 for occluded devices.
    /// Places a floor quad as a child of the stage root at 0,0,0.
    /// Will also draw the bounds of your placespace if you set it during the Mixed Reality Portal first run experience.
    /// </summary>
    public class PlayspaceManager : Singleton<PlayspaceManager>
    {
        [Tooltip("Quad prefab to display as the floor.")]
        public GameObject FloorQuad;
        private GameObject floorQuadInstance;

        [Tooltip("Material used to draw bounds for play space. Leave empty if you have not setup your play space or don't want to render bounds.")]
        public Material PlayspaceBoundsMaterial;

        StageRoot stageRoot = null;
        bool updatePlayspaceBounds = true;

        List<GameObject> boundingBoxLines = new List<GameObject>();

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
                floorQuadInstance = GameObject.Instantiate(FloorQuad);
                floorQuadInstance.SetActive(true);
                
                // Parent this to the component that has the StageRoot attached.
                floorQuadInstance.transform.SetParent(this.gameObject.transform);

#if UNITY_EDITOR
                // So the floor quad does not occlude in editor testing, draw it lower.
                floorQuadInstance.transform.localPosition = new Vector3(0, -3, 0);
#else
                // Draw the floor at 0,0,0 under stage root.
                floorQuadInstance.transform.localPosition = Vector3.zero;
#endif
            }
        }

        private void StageRoot_OnTrackingChanged(StageRoot self, bool located)
        {
            // Hide the floor if tracking is lost or if StageRoot can't be located.
            if (floorQuadInstance != null &&
                HolographicSettings.IsDisplayOpaque)
            {
                floorQuadInstance.SetActive(located);
                updatePlayspaceBounds = located;
            }
        }

        private void Update()
        {
            // This is simply showing how to draw the bounds.
            // Applications don't *need* to draw bounds. 
            // Bounds are more useful for placing objects.
            if (updatePlayspaceBounds && HolographicSettings.IsDisplayOpaque)
            {
                UpdatePlayspaceBounds();
            }
        }

        private void UpdatePlayspaceBounds()
        {
            RemoveBoundingBox();

            Vector3[] bounds = null;
            bool tryGetBoundsSuccess = stageRoot.TryGetBounds(out bounds);

            if (tryGetBoundsSuccess && bounds != null)
            {
                if (PlayspaceBoundsMaterial != null)
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
                    updatePlayspaceBounds = false;
                }
            }
        }

        private void DrawLine(Vector3 start, Vector3 end)
        {
            GameObject boundingBox = new GameObject();
            boundingBoxLines.Add(boundingBox);

            boundingBox.transform.position = start;
                        
            LineRenderer lr = boundingBox.AddComponent<LineRenderer>();
            lr.sharedMaterial = PlayspaceBoundsMaterial;
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
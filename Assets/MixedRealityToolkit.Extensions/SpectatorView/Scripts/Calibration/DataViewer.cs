using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class DataViewer : MonoBehaviour
    {
        public GameObject imagePlane;
        public DebugVisualHelper markerVisualHelper;
        public Text text;
        private GameObject[] markerVisuals = new GameObject[18];
        private int currentIndex = 0;
        private int maxDataIndex = 0;

        private void Start()
        {
            CalibrationDataHelper.Initialize(out var nextChessboardImageId, out maxDataIndex);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
#if UNITY_EDITOR
                var texture = CalibrationDataHelper.LoadDSLRArUcoImage(currentIndex);
                var metadata = CalibrationDataHelper.LoadMetaInformation(currentIndex);

                if (texture == null ||
                    metadata == null)
                {
                    Debug.LogWarning($"Data doesn't exist for index: {currentIndex}");
                }
                else
                {
                    var material = imagePlane.GetComponent<Renderer>().material;
                    material.mainTexture = texture;
                    material.mainTextureScale = -1.0f * Vector2.one;
                    var markers = CalibrationAPI.CalcMarkerCornersRelativeToCamera(metadata);
                    for (int i = 0; i < markers.Count; i++)
                    {
                        GameObject temp = null;
                        temp = markerVisuals[i];
                        float dist = Vector3.Distance(markers[i].topLeft, markers[i].topRight);
                        markerVisualHelper.CreateOrUpdateVisual(ref temp, markers[i].topLeft, markers[i].orientation, dist * Vector3.one);
                        markerVisuals[i] = temp;
                    }

                    text.text = $"Dataset {currentIndex}";
                }

                currentIndex++;
                currentIndex %= maxDataIndex;
#endif
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Controls the generation of AR markers from a pool.
    /// </summary>
    public class SpectatorViewMarkerGenerator3D : MarkerGeneration3D
    {
        private void Start()
        {
            Generate();
        }

        /// <summary>
        /// Generates an AR marker and puts it in the scene
        /// The marker starts rotated in the scene, so it can't be read at this stage
        /// </summary>
        public override void Generate()
        {
            Texture2D marker = GetMarker();

            // Assume the marker is square
            int markerRes = marker.width;

            for(int x = 0; x<(MarkerResolutionInSquares + 2) * 2; x++)
            {
                for(int y = 0; y<(MarkerResolutionInSquares + 2) * 2; y++)
                {
                    int xCoord = ((x * (markerRes / ((MarkerResolutionInSquares + 2) * 2))) + (markerRes / ((MarkerResolutionInSquares + 2) * 4)));
                    int yCoord = ((y * (markerRes / ((MarkerResolutionInSquares + 2) * 2))) + (markerRes / ((MarkerResolutionInSquares + 2) * 4)));

                    float col = marker.GetPixel(xCoord, yCoord).r;
                    var res = 1f;

                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.layer = gameObject.layer;
                    var height = 0.0f;
                    cube.transform.parent = transform;
                    cube.transform.localPosition = new Vector3((float)xCoord / (float)markerRes - 0.5f, height, (float)yCoord / (float)markerRes - 0.5f);
                    var scale = 1.0f/((MarkerResolutionInSquares+2)*2) / res;
                    scale += 0.001f;
                    cube.transform.localScale = new Vector3(scale, scale, scale);

                    Cubes.Add(cube);
                    if (col > 0.1f)
                    {
                        cube.GetComponent<Renderer>().enabled = false;
                    }
                    else
                    {
                        cube.GetComponent<Renderer>().sharedMaterial = BlackMaterial;
                    }
                }
            }

            transform.localRotation = Quaternion.Euler(-40f, -65f, 55f);

            BlackMaterial.SetFloat("_TransitionCompletion", 0.0f);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                StartTransition();
            }
        }

        /// <summary>
        /// Starts the transition routine
        /// </summary>
        public void StartTransition()
        {
            StartCoroutine(Transition());
        }

        private void OnDestroy()
        {
            BlackMaterial.SetFloat("_TransitionCompletion", 0.0f);
        }

        /// <summary>
        /// Transitions from the rotated state to face the camera
        /// </summary>
        /// <returns></returns>
        private IEnumerator Transition()
        {
            var timer = 0f;
            const float transitionTime = 4.0f;

            while(timer < transitionTime)
            {
                timer += Time.deltaTime;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(360, 360, 360), timer/transitionTime);
                BlackMaterial.SetFloat("_TransitionCompletion", Mathf.Min(1.0f, timer));
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.ARCapture
{
	public class ARCAMarkerGenerator3D : MarkerGeneration3D
	{
		void Start()
		{
			Generate();
		}

		public override void Generate()
		{
			Texture2D marker = GetMarker();

			// Assume the marker is square
            int markerRes = marker.width;

			for(int x = 0; x<(markerResolutionInSquares + 2) * 2; x++)
            {
                for(int y = 0; y<(markerResolutionInSquares + 2) * 2; y++)
                {
                    int index = x * (markerResolutionInSquares + 2) * 4 + y;

                    int xCoord = ((x * (markerRes / ((markerResolutionInSquares + 2) * 2))) + (markerRes / ((markerResolutionInSquares + 2) * 4)));
                    int yCoord = ((y * (markerRes / ((markerResolutionInSquares + 2) * 2))) + (markerRes / ((markerResolutionInSquares + 2) * 4)));

                    float col = marker.GetPixel(xCoord, yCoord).r;
                    float res = 1;

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(cube.GetComponent<Collider>());
                    cube.layer = gameObject.layer;
                    float height = 0.0f;
                    cube.transform.parent = transform;
                    cube.transform.localPosition = new Vector3((float)xCoord / (float)markerRes - 0.5f, height, (float)yCoord / (float)markerRes - 0.5f);
                    float scale = 1.0f/((markerResolutionInSquares+2)*2) / res;
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

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                StartTransition();
            }
        }

        public void StartTransition()
        {
            StartCoroutine(Transition());
        }

        void OnDestroy()
        {
            BlackMaterial.SetFloat("_TransitionCompletion", 0.0f);
        }

        IEnumerator Transition()
        {
            float timer = 0;
            float transitionTime = 4.0f;

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

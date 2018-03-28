// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;

namespace HoloToolkit.ARCapture
{
	public class SimpleMarkerGeneration3D : MarkerGeneration3D
	{
		private void Start ()
		{
			Generate();
		}

		public override void Generate()
		{
			foreach(GameObject cube in Cubes)
			{
				DestroyImmediate(cube);
			}
			Cubes.Clear();

			Texture2D marker = GetMarker();

			// Assume the marker is square
			int markerRes = marker.width;

			for(int x = 0; x<(markerResolutionInSquares + 2); x++)
			{
				for(int y = 0; y<(markerResolutionInSquares + 2); y++)
				{
					int xCoord = ((x * (markerRes / ((markerResolutionInSquares + 2)))) + (markerRes / ((markerResolutionInSquares + 2) * 2)));
					int yCoord = ((y * (markerRes / ((markerResolutionInSquares + 2)))) + (markerRes / ((markerResolutionInSquares + 2)* 2)));
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                	Destroy(cube.GetComponent<Collider>());
					float col = marker.GetPixel(xCoord, yCoord).r;
					float res = 1;

					float scale = 1.0f/((markerResolutionInSquares+2)) / res;
					scale += 0.001f;
					cube.transform.parent = transform;
					cube.transform.localPosition = new Vector3((float)xCoord / (float)markerRes - 0.5f, 0.0f, (float)yCoord / (float)markerRes - 0.5f);
					cube.transform.localScale = new Vector3(scale, scale, scale);

					Cubes.Add(cube);
					if (col > 0.1f)
					{
						cube.GetComponent<Renderer>().sharedMaterial = WhiteMaterial;
					}
					else
					{
						cube.GetComponent<Renderer>().sharedMaterial = BlackMaterial;
					}
				}
			}
		}
	}
}

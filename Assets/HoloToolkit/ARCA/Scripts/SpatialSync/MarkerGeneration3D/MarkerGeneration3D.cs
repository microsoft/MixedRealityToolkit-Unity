// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARCA
{
	public class MarkerGeneration3D : MonoBehaviour
	{
	    public delegate void OnMarkerGeneratedEvent(int markerId);
		[Tooltip("An array of available pre generated markers")] 
		public Texture2D[] Markers;
		[Tooltip("Material applied to white sections of ARCA marker")]
		public Material WhiteMaterial;
		[Tooltip("Material applied to black sections of ARCA marker")]
		public Material BlackMaterial;
		// Execute once 3D marker has been generated
	    public OnMarkerGeneratedEvent OnMarkerGenerated;
		[HideInInspector]
		// The id of the marker generated
		public int MarkerId;
		
		protected List<GameObject> Cubes = new List<GameObject>();
		
		private Texture2D marker;
		protected int markerResolutionInSquares = 6;

		public virtual void Generate() { }

		protected Texture2D GetMarker()
		{
			if(!marker)
			{
				UnityEngine.Random.InitState(DateTime.Now.Millisecond);
			    MarkerId = UnityEngine.Random.Range(0, Markers.Length);
                marker = Markers[MarkerId];
			    if (OnMarkerGenerated != null)
				{
			        OnMarkerGenerated(MarkerId);
				}
			}

			return marker;
		}
	} 
}
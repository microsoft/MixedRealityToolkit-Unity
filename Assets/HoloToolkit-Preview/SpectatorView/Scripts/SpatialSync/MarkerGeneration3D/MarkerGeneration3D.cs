// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    public class MarkerGeneration3D : MonoBehaviour
    {
        public delegate void OnMarkerGeneratedEvent(int markerId);

        /// <summary>
        /// An array of available pre generated markers
        /// </summary>
        [Tooltip("An array of available pre generated markers")]
        [SerializeField]
        private Texture2D[] markers;

        /// <summary>
        /// Material applied to white sections of SpectatorView marker
        /// </summary>
        [Tooltip("Material applied to white sections of SpectatorView marker")]
        [SerializeField]
        protected Material WhiteMaterial;

        /// <summary>
        /// Material applied to black sections of SpectatorView marker
        /// </summary>
        [Tooltip("Material applied to black sections of SpectatorView marker")]
        [SerializeField]
        protected Material BlackMaterial;

        ///Execute once 3D marker has been generated
        public OnMarkerGeneratedEvent OnMarkerGenerated;

        /// <summary>
        /// // The id of the marker generated
        /// </summary>
        private int markerId;

        /// <summary>
        /// List of cubes that form the marker
        /// </summary>
        protected readonly List<GameObject> Cubes = new List<GameObject>();

        /// <summary>
        /// Texture from which the marker is generated
        /// </summary>
        private Texture2D marker;

        /// <summary>
        /// The resolution in squares for the marker.
        /// </summary>
        protected const int MarkerResolutionInSquares = 6;

        /// <summary>
        /// An array of available pre generated markers
        /// </summary>
        public Texture2D[] Markers
        {
            get { return markers; }
            set { markers = value; }
        }

        /// <summary>
        /// // The id of the marker generated
        /// </summary>
        public int MarkerId
        {
            get { return markerId; }
            set { markerId = value; }
        }

        /// <summary>
        /// Base function to generate a marker
        /// </summary>
        public virtual void Generate() { }

        /// <summary>
        /// Randomly gets a marker texture from the pool
        /// </summary>
        /// <returns></returns>
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

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Resets the gameobject's position to its starting position as soon as it's moving outside the provided bounds
    /// </summary>
    public class ReturnToBounds : MonoBehaviour
    {
        /// <summary>
        /// Limiting bounds for moving the gameobject in -Z direction
        /// </summary>
        [SerializeField]
        private Transform frontBounds = null;
    
        public Transform FrontBounds
        {
            get => frontBounds;
            set => frontBounds = value;
        }

        /// <summary>
        /// Limiting bounds for moving the gameobject in +Z direction
        /// </summary>
        [SerializeField]
        private Transform backBounds = null;
    
        public Transform BackBounds
        {
            get => backBounds;
            set => backBounds = value;
        }

        /// <summary>
        /// Limiting bounds for moving the gameobject in -X direction
        /// </summary>
        [SerializeField]
        private Transform leftBounds = null;
    
        public Transform LeftBounds
        {
            get => leftBounds;
            set => leftBounds = value;
        }

        /// <summary>
        /// Limiting bounds for moving the gameobject in +X direction
        /// </summary>
        [SerializeField]
        private Transform rightBounds = null;
    
        public Transform RightBounds
        {
            get => rightBounds;
            set => rightBounds = value;
        }

        /// <summary>
        /// Limiting bounds for moving the gameobject in -Y direction
        /// </summary>
        [SerializeField]
        private Transform bottomBounds = null;
    
        public Transform BottomBounds
        {
            get => bottomBounds;
            set => bottomBounds = value;
        }

        /// <summary>
        /// Limiting bounds for moving the gameobject in +Y direction
        /// </summary>
        [SerializeField]
        private Transform topBounds = null;
    
        public Transform TopBounds
        {
            get => topBounds;
            set => topBounds = value;
        }
    
        private Vector3 positionAtStart;
    

        /// <summary>
        /// Caches start position of gameobject this script is attached to
        /// </summary>
        void Start()
        {
            positionAtStart = transform.position;
        }

        /// <summary>
        /// Checks gameobjects position and resets to start position as soon as it's passing one of the bound limits.
        /// </summary>
        void Update()
        {
            if (transform.position.x > rightBounds.position.x || transform.position.x < leftBounds.position.x ||
                transform.position.y > topBounds.position.y || transform.position.y < bottomBounds.position.y ||
                transform.position.z > backBounds.position.z || transform.position.z < frontBounds.position.z)
            {
                transform.position = positionAtStart;
            }
        }
    }
}

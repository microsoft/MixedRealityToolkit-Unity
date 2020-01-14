// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental.Demos
{
    public class ReturnToBounds : MonoBehaviour
    {
        [SerializeField]
        private Transform frontBounds = null;
    
        public Transform FrontBounds
        {
            get => frontBounds;
            set => frontBounds = value;
        }
    
        [SerializeField]
        private Transform backBounds = null;
    
        public Transform BackBounds
        {
            get => backBounds;
            set => backBounds = value;
        }
    
        [SerializeField]
        private Transform leftBounds = null;
    
        public Transform LeftBounds
        {
            get => leftBounds;
            set => leftBounds = value;
        }
    
        [SerializeField]
        private Transform rightBounds = null;
    
        public Transform RightBounds
        {
            get => rightBounds;
            set => rightBounds = value;
        }
    
        [SerializeField]
        private Transform bottomBounds = null;
    
        public Transform BottomBounds
        {
            get => bottomBounds;
            set => bottomBounds = value;
        }
    
        [SerializeField]
        private Transform topBounds = null;
    
        public Transform TopBounds
        {
            get => topBounds;
            set => topBounds = value;
        }
    
        private Vector3 positionAtStart;
    
        // Start is called before the first frame update
        void Start()
        {
            positionAtStart = transform.position;
        }
    
        // Update is called once per frame
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

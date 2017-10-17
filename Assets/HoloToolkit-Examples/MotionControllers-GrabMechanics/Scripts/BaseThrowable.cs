// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Examples.Grabbables
{
    /// <summary>
    /// The abstract class that defines the minimum amount of content for any throwable object
    /// Variables declared at the bottom
    /// </summary>
    public abstract class BaseThrowable : MonoBehaviour
    {
        public float ThrowMultiplier { get { return throwMultiplier; } set { throwMultiplier = value; } }

        public bool ZeroGravityThrow { get { return zeroGravityThrow; } set { zeroGravityThrow = value; } }

        public bool Thrown { get { return thrown; } set { thrown = value; } }

        // To get velocity info straight from controller
        public Vector3 LatestControllerThrowVelocity { get; set; }
        public Vector3 LatestControllerThrowAngularVelocity { get; set; }

        // TODO: Not implemented yet. lower priority
        public AnimationCurve VelocityOverTime { get { return velocityOverTime; } set { velocityOverTime = value; } }

        public AnimationCurve UpDownCurveOverTime { get { return upDownCurveOverTime; } set { upDownCurveOverTime = value; } }

        public AnimationCurve LeftRightCurveOverTime { get { return leftRightCurveOverTime; } set { leftRightCurveOverTime = value; } }

        private BaseGrabbable grabbable;

        [SerializeField]
        private float throwMultiplier = 1.0f;

        [SerializeField]
        private bool zeroGravityThrow;

        [SerializeField]
        private AnimationCurve velocityOverTime;

        [SerializeField]
        private AnimationCurve upDownCurveOverTime;

        [SerializeField]
        private AnimationCurve leftRightCurveOverTime;

        private bool thrown;

        protected virtual void Awake()
        {
            grabbable = GetComponent<BaseGrabbable>();
        }

        /// <summary>
        /// throw needs to subscribe to grab events to know when to apply the appropriate force to an object
        /// </summary>
        protected virtual void OnEnable()
        {
            grabbable.OnReleased += Throw;
        }

        protected virtual void OnDisable()
        {
            grabbable.OnReleased -= Throw;
        }

        protected virtual void BeginThrow()
        {
            Debug.Log("Begin throw detected.");
        }

        protected virtual void MidThrow()
        {
            Debug.Log("mid throw...");
        }

        protected virtual void ReleaseThrow()
        {
            Debug.Log("Throw release...");
        }

        protected virtual void OnThrowCanceled()
        {
            Debug.Log("Throw canceled");
        }

        /// <summary>
        /// throw behaviour should be over ridden in a non-abstract class
        /// </summary>
        /// <param name="grabber"></param>
        public virtual void Throw(BaseGrabbable grabber)
        {
            Debug.Log("Throwing..");
            thrown = true;
        }
    }
}

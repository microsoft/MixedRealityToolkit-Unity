// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Blend.Automation
{
    /// <summary>
    /// Makes the assigned object follow and face another object.
    /// A potential use is moving a UI panel around, but is very flexible.
    /// 
    /// Add the reference object this object should follow and tell it to start running.
    /// To get this to follow the user around, add the camera as the reference object.
    /// Typical Use:
    /// Call Start Running();
    /// 
    /// Features:
    ///     - Independant adjustment speeds of position and rotation, feels really cool.
    ///     - Force the object to remain vertical or lock the x axis rotation.
    ///     - Force the object to face to reference object or maintain it's existing direction
    ///     - Force the object to be in front or at the reference object's transform.forward.
    ///     - Add magnitism to bring the object closer to the reference object
    /// </summary>
    public class MoveWithObject : MonoBehaviour
    {
        [Tooltip("The game object this object will follow : Main Camera by default")]
        public GameObject ReferenceObject;

        [HideInInspector]
        [Tooltip(" status")]
        public bool IsRunning = false;

        [Tooltip("should the component auto start?")]
        public bool AutoStart;

        [Tooltip("translation speed : higher is faster")]
        public float LerpPositionSpeed = 1f;

        [Tooltip("rotation speed : higher is faster")]
        public float LerpRotationSpeed = 0.5f;

        [Tooltip("Lock the x axis if the object is set to face the reference object")]
        public bool KeepUpRight = false;
        
        [Tooltip("Does not center the object to the reference object's transform.forward vector")]
        public bool KeepStartingOffset = true;

        [Tooltip("Force the object to always face the reference object")]
        public bool FaceObject = true;

        [Tooltip("Force the object to keep relative to the reference object's transform.forward")]
        public bool KeepInFront = true;

        [Tooltip("Magnitism speed to move closer to the reference object")]
        public float Magnetism = 0;

        [Tooltip("Minimum distance to stay away from the reference object if magnitism is used")]
        public float MagnetismPaddingDistance = 1f;
        
		[Tooltip("A distance the ref obeject moves before automatically starting movement")]
        public float AutoStartDistance = 0;

        [Tooltip("changes the way auto start distance is calculated, from raw distance to view angle")]
        public bool AutoStartByAngle;

        // the position different between the objects position and the reference object's transform.forward
        protected Vector3 mOffsetDirection;

        // this object's direction
        protected Vector3 mDirection;

        // the offset rotation at start
        protected Quaternion mOffsetRotation;

        // the offset distance at start
        protected float mOffsetDistance = 0;

        // the amount of magnitism to apply
        protected float mMagnetismPercentage = 0;

        protected Vector3 mNormalzedOffsetDirection;
		
		protected bool mAutoStarted;
		
        protected float FreeTimeRatio = 10;
        protected float FreeTimeRatioSeed = 0.5f;

        /// <summary>
        /// set the reference object if not set already
        /// </summary>
        protected virtual void Awake()
        {
            if (ReferenceObject == null)
            {
                ReferenceObject = Camera.main.gameObject;
            }
        }

        protected virtual void Start()
        {
            if (AutoStart)
            {
                StartRunning();
            }
        }

        // start the object following the reference object
        public virtual void StartRunning()
        {

            if (ReferenceObject == null)
                ReferenceObject = Camera.main.gameObject;

            mOffsetDirection = this.transform.position - ReferenceObject.transform.position;
            mOffsetDistance = mOffsetDirection.magnitude;
            mDirection = ReferenceObject.transform.forward.normalized;
            mNormalzedOffsetDirection = mOffsetDirection.normalized;
            mOffsetRotation = Quaternion.FromToRotation(mDirection, mNormalzedOffsetDirection);
            IsRunning = true;

            mMagnetismPercentage = 0;
        }

        /// <summary>
        /// stop the object from following
        /// </summary>
        public virtual void StopRunning()
        {
            IsRunning = false;
        }

        /// <summary>
        /// update the position of the object based on the reference object and configuration
        /// </summary>
        /// <param name="position"></param>
        /// <param name="time"></param>
        protected virtual void UpdatePosition(Vector3 position, float time)
        {
            float ratio = Mathf.Clamp01(LerpPositionSpeed * time);
            // update the position
            this.transform.position = Vector3.Lerp(this.transform.position, position, ratio);

            // rotate to face the reference object
            if (FaceObject)
            {
                Quaternion forwardRotation = Quaternion.LookRotation(this.transform.position - ReferenceObject.transform.position);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, forwardRotation, ratio);
            }

            // lock the x axis
            if (KeepUpRight)
            {
                Quaternion upRotation = Quaternion.FromToRotation(this.transform.up, Vector3.up);
                this.transform.rotation = upRotation * this.transform.rotation;
            }
        }

		protected virtual float ViewDistance()
        {
            Vector3 objectDirection = this.transform.position - ReferenceObject.transform.position;
            Vector3 viewDirection = ReferenceObject.transform.forward * objectDirection.magnitude;
            return Vector3.Distance(viewDirection, objectDirection);
        }
        /// <summary>
        /// Animate!
        /// </summary>
        protected virtual void Update()
        {
            if (IsRunning)
            {
                Vector3 newDirection = ReferenceObject.transform.forward;

                // move the object in front of the reference object
                if (KeepInFront)
                {
                    if (KeepStartingOffset)
                    {
                        newDirection = Vector3.Normalize(mOffsetRotation * ReferenceObject.transform.forward);
                    }
                }
                else
                {
                    newDirection = mNormalzedOffsetDirection;
                    // could we allow drifting?
                }

                // move toward the reference object
                float magnetismDelta = 0;
                if (Magnetism > 0)
                {
                    magnetismDelta = MagnetismPaddingDistance - mOffsetDistance;
                    mMagnetismPercentage = Mathf.Clamp01(mMagnetismPercentage + Time.deltaTime * Magnetism);
                }
                
                Vector3 lerpPosition = ReferenceObject.transform.position + newDirection * (mOffsetDistance + magnetismDelta * mMagnetismPercentage);

                UpdatePosition(lerpPosition, Time.deltaTime);
				if (mAutoStarted && ViewDistance() < AutoStartDistance)
                {
                    mAutoStarted = false;
                    StopRunning();
                }
            }
			else if (AutoStartDistance > 0)
            {
                Vector3 objectDirection = this.transform.position - ReferenceObject.transform.position;
                if (AutoStartByAngle)
                {
                    if (ViewDistance() > AutoStartDistance)
                    {
                        mAutoStarted = true;
                        StartRunning();
                    }
                }
                else if(objectDirection.magnitude > AutoStartDistance)
                {
                    mAutoStarted = true;
                    StartRunning();
                }
            }		
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// GestureInteractiveControl receives gesture updates from GestureInteractive.
    /// 
    /// We take raw gesture data and convert it into simple values that can be acted on, like
    /// percentage dragged from origin position, angle or vector from origin point, and distance from origin point.
    /// 
    /// These values can be flattened for 2D UI elements and rotated based on user orientation
    /// 
    /// GestureInteractiveData: Contains basic values about a gesture compared to a vector.
    /// Use the "GetGestureData" method to get a processed set of values based on the latest gesture information compared to a vector direction.
    /// 
    /// Use a gestureData object for different gesture directions, good for handling horizontal or vertical movements from one controller instead of the default raw or camera aligned data.
    ///
    /// Example : GestureInteractiveData vertData = new GetGestureData(new Vector(0,1,0), 0.1f, false);
    /// Result: a set a values (direction, percent and distance) comparing the current gesture along the y axis. If the movement is up,
    ///         the distance will be positive and the percent report if the movement is equal to the maxDistance in the aligned direction. 
    /// </summary>
    public struct GestureInteractiveData
    {
        public Vector3 AlignmentVector;
        public Vector3 Direction;
        public float Distance;
        public float Percentage;
        public float MaxDistance;
        public bool FlipDirecationOnCameraForward;

        public GestureInteractiveData(Vector3 alignmentVector, float maxDistance, bool flipDirectionOnCameraForward)
        {
            AlignmentVector = alignmentVector;
            MaxDistance = maxDistance;
            FlipDirecationOnCameraForward = flipDirectionOnCameraForward;
            Direction = new Vector3();
            Distance = 0;
            Percentage = 0;
        }
    }
    
    public class GestureInteractiveControl : MonoBehaviour
    {
        /// <summary>
        /// Dictates how the control processes data.
        /// Raw: no extra processing is done, just current gesture information compared to start gesture information.
        /// Camera: compares gestures to the facing camera direction, processes data based on camera's right vector, good for billboarded UI.
        /// Aligned: compares the gesture to the Alignment Vector, for instance (1, 0, 0) will compute distance and percentage of the gesture moving to the right along the x axis.
        /// </summary>
        public enum GestureDataType { Raw, Camera, Aligned }

        [Tooltip("What type of data processing should be used ? Raw, Camera aligned or vector aligned")]
        public GestureDataType GestureData;

        /// <summary>
        /// The amount we expect the user to drag from the origin point of the gesture.
        /// This value calculates the CurrentPercent, based on the gesture's CurrentDistance compared the MaxGestureDistance.
        /// </summary>
        [Tooltip("The distance in world space to compare the gesture's delta to")]
        public float MaxGestureDistance = 0.15f;
        
        /// <summary>
        /// The Vector to align the gesture to, uses the dot product of the gesture direction and this vector.
        /// Use this to restrict the basic values that are returned to a specific direction or range of directions.
        /// </summary>
        [Tooltip("The vector to align the gesture to, only used when the Aligned GestureDataType is selected.")]
        public Vector3 AlignmentVector = new Vector3(1,1,1);

        /// <summary>
        /// Flips the CurrentDirection based on the camera forward compared to Vector3.forward. For instance,
        /// a left direction is a positive movement when facing Vector3.forward, but facing Vector3.back a left direction is negative.
        /// Flips the CurrentDirection based on the camera forward compared to Vector3.forward.
        /// </summary>
        [Tooltip("Should we care if the Camera's forward is not Vector3.forward?")]
        public bool FlipDirectionOnCameraForward = false;
        
        /// <summary>
        /// Current gesture state
        /// </summary>
        protected GestureInteractive.GestureManipulationState GestureState;

        /// <summary>
        /// Camera reference
        /// </summary>
        protected Camera MainCamera { get { return CameraCache.Main; } }

        /// <summary>
        /// Orientation based on the user facing direction.
        /// Can be used when translating a gesture to an object based on the camera direction and the objects direction.
        /// </summary>
        protected Matrix4x4 CameraMatrix;

        /// <summary>
        /// The origin of the gesture (cached)
        /// </summary>
        protected Vector3 StartGesturePosition;

        /// <summary>
        /// Current gesture position (cached)
        /// </summary>
        protected Vector3 CurrentGesturePosition;

        /// <summary>
        /// The vector from the origin position to the current gesture position
        /// </summary>
        protected Vector3 DirectionVector;

        /// <summary>
        /// The head position when the gesture started (cached)
        /// </summary>
        protected Vector3 StartHeadPosition;

        /// <summary>
        /// the camera forward when the gesture started (cached)
        /// </summary>
        protected Vector3 StartHeadRay;

        /// <summary>
        /// Is a gesture in progress?
        /// </summary>
        protected bool GestureStarted = false;

        /// <summary>
        /// The current gesture position from the gesture origin (delta)
        /// Some of the settings above help to determine if this value is positive or negative
        /// so we can easily know which direction the gesture is moving (left vs. right, up vs. down, forward vs. backward).
        /// </summary>
        protected float CurrentDistance;

        /// <summary>
        /// The percentage of gesture distance compared to the MaxGestureDistance
        /// </summary>
        protected float CurrentPercentage;

        /// <summary>
        /// animation time for a keyword triggered gesture
        /// </summary>
        protected float KeywordGestureTime = 0.5f;

        /// <summary>
        /// animation counter for a keyword triggered gesture
        /// </summary>
        protected float KeywordGestureTimeCounter = 0.5f;

        /// <summary>
        /// animation direction for a keyword triggered gesture
        /// </summary>
        protected Vector3 KeywordGestureVector;

        // set the default gesture state
        virtual protected void Awake()
        {
            GestureState = GestureInteractive.GestureManipulationState.None;
           
        }

        /// <summary>
        /// Gesture updates called by GestureInteractive
        /// </summary>
        /// <param name="startGesturePosition">The gesture origin position</param>
        /// <param name="currentGesturePosition">the current gesture position</param>
        /// <param name="startHeadOrigin">the origin of the camera when the gesture started</param>
        /// <param name="startHeadRay">the camera forward when the gesture started</param>
        /// <param name="gestureState">current gesture state</param>
        public virtual void ManipulationUpdate(Vector3 startGesturePosition, Vector3 currentGesturePosition, Vector3 startHeadOrigin, Vector3 startHeadRay, GestureInteractive.GestureManipulationState gestureState)
        {
            if (gestureState == GestureInteractive.GestureManipulationState.Start || (!GestureStarted && gestureState != GestureInteractive.GestureManipulationState.Start))
            {
                CameraMatrix = GetCameraMatrix();
                StartGesturePosition = startGesturePosition;
                CurrentGesturePosition = startGesturePosition;
                StartHeadPosition = startHeadOrigin;
                StartHeadRay = startHeadRay;
                GestureStarted = true;
            }
            else
            {
                CurrentGesturePosition = currentGesturePosition;
            }

            UpdateGesture();

            if (gestureState == GestureInteractive.GestureManipulationState.None)
            {
                GestureStarted = false;
            }
        }

        /// <summary>
        /// Returns a data set about the current gesture information compared to a specific vector.
        /// For instance, to compare if the gesture is moving vertically or horizontally,
        /// create two instances of this data set and compare the distance for each.
        /// If the vertical percentage is greater than the horizontal percentage then the gesture is moving vertically.
        /// </summary>
        /// <param name="alignmentVector"></param>
        /// <param name="maxDistance"></param>
        /// <param name="flipDirecationOnCameraForward"></param>
        /// <returns></returns>
        public GestureInteractiveData GetGestureData(Vector3 alignmentVector, float maxDistance, bool flipDirecationOnCameraForward)
        {
            GestureInteractiveData data = new GestureInteractiveData(alignmentVector, maxDistance, flipDirecationOnCameraForward);

            data.Direction = DirectionVector;
            bool flipDirection = Vector3.Dot(Vector3.forward, StartHeadRay) < 0 && flipDirecationOnCameraForward;

            if (flipDirection)
            {
                data.Direction = -data.Direction;
            }
            data.Distance = Vector3.Dot(data.Direction, alignmentVector);

            data.Percentage = Mathf.Min(Mathf.Abs(data.Distance) / maxDistance, 1);

            return data;
        }
        
        /// <summary>
        /// Get the current camera's world matrix in world space
        /// </summary>
        /// <returns>Matix4x4</returns>
        public Matrix4x4 GetCameraMatrix()
        {
            // get the preferred body direction
            Vector3 up = Vector3.up;
            Vector3 forward = MainCamera.transform.forward;
            // protecting from a weird cross value
            if (Vector3.Angle(up, forward) < 10)
            {
                up = MainCamera.transform.up;
            }
            Vector3 right = Vector3.Cross(up, forward);
            right.Normalize();

            // build a matrix based on body/camera direction
            Matrix4x4 cameraWorld = new Matrix4x4();
            cameraWorld.SetColumn(0, right);
            cameraWorld.SetColumn(1, up);
            cameraWorld.SetColumn(2, forward);
            cameraWorld.SetColumn(3, new Vector4(0, 0, 0, 1));

            return cameraWorld;
        }

        /// <summary>
        /// Rotates the gesture vector around a pivot point based on the camera matrix
        /// </summary>
        /// <param name="direction">Current gesture position</param>
        /// <param name="orientation">Gesture origin position</param>
        /// <returns></returns>
        public Vector3 WorldForwardVector(Vector3 direction, Vector3 orientation, bool flipY = false, bool flipZ = false)
        {
            Matrix4x4 cameraWorld = GetCameraMatrix(); //CameraMatrix

            // create a new vector from the raw gesture data
            Vector3 rawVector = direction - orientation;

            // flip z index?
            if (flipZ)
            {
                rawVector.z = -rawVector.z;
            }

            if (flipY)
            {
                rawVector.y = -rawVector.y;
            }
            
            Vector3 newDirection = cameraWorld.MultiplyVector(rawVector);

            // replace the y
            newDirection.y = rawVector.y;

            return newDirection;
        }

        /// <summary>
        /// A way to programmatically override a gesture, used for keyword gestures.
        /// </summary>
        /// <param name="gestureVector"></param>
        public void SetGestureVector(Vector3 gestureVector)
        {
            if (GestureStarted)
            {
                return;
            }

            KeywordGestureVector = gestureVector;
            KeywordGestureTimeCounter = 0;
            CameraMatrix = GetCameraMatrix();
            StartGesturePosition = gestureVector * MaxGestureDistance * (KeywordGestureTimeCounter / KeywordGestureTime);
            StartHeadPosition = new Vector3();
            StartHeadRay = Vector3.forward;

            CurrentGesturePosition = gestureVector * MaxGestureDistance * (KeywordGestureTimeCounter/KeywordGestureTime);

            ManipulationUpdate(StartGesturePosition, Vector3.up, StartHeadPosition, StartHeadRay, GestureInteractive.GestureManipulationState.Start);
        }

        /// <summary>
        /// a place holder function for taking value and settings a gesture direction.
        /// Used by the keyword gesture system so that we can have multiple keywords for a single control.
        /// For instance: forward/backward or Min/Center/Max
        /// </summary>
        /// <param name="gestureValue"></param>
        public virtual void setGestureValue(int gestureValue)
        {
            // override to convert keyword index to vectors.
            switch (gestureValue)
            {
                case 0:
                    SetGestureVector(Vector3.left);
                    break;
                case 1:
                    SetGestureVector(Vector3.right);
                    break;
                case 2:
                    SetGestureVector(Vector3.up);
                    break;
                case 3:
                    SetGestureVector(Vector3.down);
                    break;
            }
        }

        /// <summary>
        /// Aligns a gesture to the camera direction
        /// </summary>
        /// <param name="directionVector"></param>
        /// <param name="flipX"></param>
        /// <param name="flipY"></param>
        /// <returns></returns>
        protected Vector3 DirectionVectorToZPlane(Vector3 directionVector, bool flipX, bool flipY)
        {
            // set the direction based on camera and gesture updates
            float cameraDirectionX = flipX ? -directionVector.x : directionVector.x;
            float cameraDirectionY = flipY ? -directionVector.y : directionVector.y;
            return MainCamera.transform.forward * cameraDirectionY + MainCamera.transform.right * cameraDirectionX;
        }

        /// <summary>
        /// Rotates a gesture based on camera forward.
        /// Helps to take a vector in world space and rotate it to Vector3.forward
        /// </summary>
        /// <param name="gesturePosition"></param>
        /// <returns></returns>
        public Vector3 GesturePosition(Vector3 gesturePosition)
        {
            // rotate the screen space mouse position to world space, based on the camera direction and compress pixels to world 
            // get current angle from forward - returns an absolute value of 0 - 180
            float angleDiff = Vector3.Angle(Vector3.forward, MainCamera.transform.forward);
            // make sure angle works 360 degrees, find the left or right side
            float dot = Vector3.Dot(Vector3.right, MainCamera.transform.forward);
            // if dot is positive, camera is pointing right of Vector3.forward or rotating clockwise
            if (dot < 0)
                angleDiff = -angleDiff;
            // rotate the world space converted mouse vector on the Y axis
            return RotateVectorOnY(gesturePosition, MainCamera.transform.position, angleDiff);
        }

        /// <summary>
        /// Rotates a vector on the y axis
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="pivot"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        private Vector3 RotateVectorOnY(Vector3 vector, Vector3 pivot, float angle)
        {
            return Quaternion.Euler(0, angle, 0) * (vector - pivot) + pivot;
        }

        /// <summary>
        /// Flips the direction based on the direction of the camera and the direction of the control.
        /// Used when FlipDirecationOnCameraForward is true to correct the calculations based on if the user
        /// is facing the control or not, compared to if the user if facing forward or not.
        /// </summary>
        /// <param name="toFlip"></param>
        /// <param name="controlPosition"></param>
        /// <param name="controlForward"></param>
        /// <returns></returns>
        protected float FlipDistanceOnFacingControl(float toFlip, Vector3 controlPosition, Vector3 controlForward)
        {
            Vector3 cameraRay = StartHeadPosition - controlPosition;
            bool facingForward = Vector3.Dot(MainCamera.transform.forward, StartHeadRay) >= 0;
            bool facingControl = Vector3.Dot(cameraRay, controlForward) >= 0;

            if (!facingForward)
            {
                // then the direction was flipped
                // facing back
                if (facingControl)
                {
                    toFlip = -toFlip;
                }
            }
            else
            {
                if (!facingControl)
                {
                    toFlip = -toFlip;
                }
            }

            return toFlip;
        }
        
        /// <summary>
        /// Update all the simplified gesture values because an update has occurred
        /// incorporates the settings, such as data type, alignment vectors and max gesture distance.
        /// </summary>
        protected void UpdateGesture()
        {
            DirectionVector = CurrentGesturePosition - StartGesturePosition;
            CurrentDistance = DirectionVector.magnitude;
            bool flipDirection = Vector3.Dot(Vector3.forward, StartHeadRay) < 0 && FlipDirectionOnCameraForward;
            switch (GestureData)
            {
                case GestureDataType.Raw:
                    CurrentDistance = DirectionVector.magnitude;
                    break;
                case GestureDataType.Camera:
                    if (flipDirection)
                    {
                        DirectionVector = -DirectionVector;
                    }
                    CurrentDistance = Vector3.Dot(DirectionVector, MainCamera.transform.right);
                    break;
                case GestureDataType.Aligned:
                    CurrentDistance = Vector3.Dot(DirectionVector, AlignmentVector);
                    break;
                default:
                    break;
            }

            CurrentPercentage = Mathf.Min(Mathf.Abs(CurrentDistance) / MaxGestureDistance, 1);
        }

        protected virtual void Update()
        {
            if (KeywordGestureTimeCounter < 0)
            {
                if (GestureStarted)
                {
                    KeywordGestureTimeCounter = KeywordGestureTime;
                    return;
                }

                GestureInteractive.GestureManipulationState state = GestureInteractive.GestureManipulationState.Update;
                KeywordGestureTimeCounter += Time.deltaTime;
                if (KeywordGestureTimeCounter > KeywordGestureTime)
                {
                    KeywordGestureTimeCounter = KeywordGestureTime;
                    state = GestureInteractive.GestureManipulationState.None;
                }

                CurrentGesturePosition = KeywordGestureVector * MaxGestureDistance * (KeywordGestureTimeCounter / KeywordGestureTime);
                
                ManipulationUpdate(StartGesturePosition, Vector3.up, StartHeadPosition, StartHeadRay, state);
            }
        }
    }
}

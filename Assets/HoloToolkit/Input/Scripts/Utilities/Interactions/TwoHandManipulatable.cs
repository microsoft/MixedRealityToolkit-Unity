using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// TO DO
    /// </summary>
    public class TwoHandManipulatable : MonoBehaviour, IInputHandler, ISourceStateHandler
    {
        public event Action StartedManipulating;
        public event Action StoppedManipulating;

        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        public Transform HostTransform;

        public enum TwoHandedManipulation
        {
            Scale,
            Rotate,
            MoveScale,
            RotateScale,
            MoveRotateScale
        };

        [Tooltip("What manipulation will two hands perform?")]
        public TwoHandedManipulation TwoHandManipulationMode;

        [Tooltip("Constrain rotation along an axis")]
        public HandlebarRotateLogic.RotationConstraint ConstraintOnRotation = HandlebarRotateLogic.RotationConstraint.None;
        [Tooltip("If true, grabbing the object with one hand will initiate movement.")]
        public bool OneHandMovement = true;


        public void OnInputDown(InputEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnInputUp(InputEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// RotateLogic implements common logic for rotating holograms using a handlebar metaphor. 
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    public class HandlebarRotateLogic
    {
        private const float MinHandDistanceForPitchM = 0.1f;
        private const float RotationMultiplier = 2f;
        public enum RotationConstraint
        {
            None,
            XAxisOnly,
            YAxisOnly,
            ZAxisOnly,
            // Make the object rotate about X if the hands are vertical initially
            XOrYBasedOnInitialHandPosition,
            PointToAxis
        };

        private readonly RotationConstraint m_rotationConstraint;
        /// <summary>
        /// The current rotation constraint might be modified based on disambiguation logic, for example
        /// XOrYBasedOnInitialHandPosition might change the current rotation constraint based on the 
        /// initial hand positions at the start
        /// </summary>
        private RotationConstraint m_currentRotationConstraint;
        public RotationConstraint GetCurrentRotationConstraint()
        {
            return m_currentRotationConstraint;
        }
        private Vector3 m_previousHandlebarRotation;

        public HandlebarRotateLogic(RotationConstraint rotationConstraint)
        {
            m_rotationConstraint = rotationConstraint;
        }

        public void Setup(Dictionary<uint, Vector3> handsPressedMap, GameObject manipulationRoot)
        {

            if (m_rotationConstraint == RotationConstraint.XOrYBasedOnInitialHandPosition)
            {
                throw new NotImplementedException();
                //var hand0 = HandManager.Instance.GetHandState(0);
                //var hand1 = HandManager.Instance.GetHandState(1);
                //var handDistance = Vector3.Distance(hand0.location, hand1.location);
                //var hand0ToHand1 = Camera.main.WorldToScreenPoint(hand1.location) - Camera.main.WorldToScreenPoint(hand0.location);
                //hand0ToHand1.Normalize();

                //// Since the vector is normalized, the X component of hand1ToHand2 is just cos(theta)
                //if (Math.Abs(hand0ToHand1.x) >= Mathf.Cos(Mathf.PI / 4) || handDistance < MinHandDistanceForPitchM)
                //{
                //    m_currentRotationConstraint = RotationConstraint.YAxisOnly;
                //}
                //else
                //{
                //    m_currentRotationConstraint = RotationConstraint.XAxisOnly;
                //}
            }
            else
            {
                m_currentRotationConstraint = m_rotationConstraint;
            }
            m_previousHandlebarRotation = GetHandlebarDirection(handsPressedMap, manipulationRoot);
        }

        private Vector3 ProjectHandlebarGivenConstraint(RotationConstraint constraint, Vector3 handlebarRotation, GameObject manipulationRoot)
        {
            Vector3 result = handlebarRotation;
            switch (constraint)
            {
                case RotationConstraint.XAxisOnly:
                    result.x = 0;
                    break;
                case RotationConstraint.YAxisOnly:
                    result.y = 0;
                    break;
                case RotationConstraint.ZAxisOnly:
                    result.z = 0;
                    break;
            }
            return Camera.main.transform.TransformDirection(result);
        }

        private Vector3 GetHandlebarDirection(Dictionary<uint, Vector3> handsPressedMap, GameObject manipulationRoot)
        {
            Assert.IsTrue(handsPressedMap.Count > 1);
            var handsEnumerator = handsPressedMap.Values.GetEnumerator();
            handsEnumerator.MoveNext();
            var hand1 = handsEnumerator.Current;
            handsEnumerator.MoveNext();
            var hand2 = handsEnumerator.Current;

            // We project the handlebar direction into camera space because otherwise when we move our body the handlebard will move even 
            // though, relative to our heads, the handlebar is not moving.
            hand1 = Camera.main.transform.InverseTransformPoint(hand1);
            hand2 = Camera.main.transform.InverseTransformPoint(hand2);

            return hand2 - hand1;
        }

        public Quaternion Update(Dictionary<uint, Vector3> handsPressedMap, GameObject manipulationRoot, Quaternion currentRotation)
        {
            var handlebarDirection = GetHandlebarDirection(handsPressedMap, manipulationRoot);
            var handlebarDirectionProjected = ProjectHandlebarGivenConstraint(m_currentRotationConstraint, handlebarDirection,
                manipulationRoot);
            var prevHandlebarDirectionProjected = ProjectHandlebarGivenConstraint(m_currentRotationConstraint,
                m_previousHandlebarRotation, manipulationRoot);
            m_previousHandlebarRotation = handlebarDirection;

            var rotationDelta = Quaternion.FromToRotation(prevHandlebarDirectionProjected, handlebarDirectionProjected);

            var angle = 0f;
            var axis = Vector3.zero;
            rotationDelta.ToAngleAxis(out angle, out axis);
            angle *= RotationMultiplier;

            if (m_currentRotationConstraint == RotationConstraint.YAxisOnly)
            {
                // If we are rotating about Y axis, then make sure we rotate about global Y axis.
                // Since the angle is obtained from a quaternion, we need to properly orient it (up or down) based
                // on the original axis-angle representation. 
                axis = Vector3.up * Vector3.Dot(axis, Vector3.up);
            }
            return Quaternion.AngleAxis(angle, axis) * currentRotation;
        }
    }
}
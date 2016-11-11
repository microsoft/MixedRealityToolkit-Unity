using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Allows the user to place and rotate GameObjects using a game controller.
    /// TODO This should be converted to an input source.
    /// </summary>
    /// <remarks>Make sure to enable the HumanInterfaceDevice capability before using.</remarks>
    public class GameControllerManipulator : MonoBehaviour
    {
        [Tooltip("Name of the joystick axis that moves the object along X as set in InputManager")]
        public string MoveXAxisName = "ControllerLeftStickX";
        [Tooltip("Speed of movement along X")]
        public float MoveXAxisSpeed = 0.2f;

        [Tooltip("Name of the joystick axis that moves the object along Y as set in InputManager")]
        public string MoveYAxisName = "ControllerLeftStickY";
        [Tooltip("Speed of movement along Y")]
        public float MoveYAxisSpeed = 0.2f;

        [Tooltip("Name of the joystick axis that moves the object along Z as set in InputManager")]
        public string MoveZAxisName = "ControllerTriggerAxis";
        [Tooltip("Speed of movement along Z")]
        public float MoveZAxisSpeed = 0.2f;

        [Tooltip("Name of the button the user presses to move the object with gaze")]
        public string MoveWithGazeButtonName = "Fire1";

        [Tooltip("Name of the joystick axis that rotates around X as set in InputManager")]
        public string RotateAroundXAxisName = "ControllerLeftStickY";
        [Tooltip("Speed of rotation around X")]
        public float RotateAroundXAxisSpeed = -1f;

        [Tooltip("Name of the joystick axis that rotates around Y as set in InputManager")]
        public string RotateAroundYAxisName = "ControllerLeftStickX";
        [Tooltip("Speed of rotation around Y")]
        public float RotateAroundYAxisSpeed = -1f;

        [Tooltip("Name of the joystick axis that rotates around Z as set in InputManager")]
        public string RotateAroundZAxisName = "";
        [Tooltip("Speed of rotation around Z")]
        public float RotateAroundZAxisSpeed = 1f;

        [Tooltip("Name of the button used to enable rotation. Useful for single stick controllers")]
        public string RotateModifierButtonName = "Fire2";

        [Tooltip("Move the gaze target instead of the GameObject this script is attached to")]
        public bool MoveGazeTarget = false;

        private GameObject lastAffectedObject;
        private float stickDistance = 0;

        // Update is called once per frame
        private void Update()
        {
            GameObject objectToManipulate;

            if (!MoveGazeTarget)
            {
                objectToManipulate = gameObject;
            }
            else
            {
                objectToManipulate = lastAffectedObject ?? GazeManager.Instance.HitObject;
            }

            if (objectToManipulate == null)
            {
                return;
            }

            var cameraTransform = Camera.main.transform;

            //Rotate
            var noRotateModifier = string.IsNullOrEmpty(RotateModifierButtonName);
            var enableRotate = noRotateModifier || Input.GetButton(RotateModifierButtonName);

            float rz = 0;
            float ry = 0;
            float rx = 0;

            if (enableRotate)
            {
                rz = RotateAroundAxis(objectToManipulate, RotateAroundZAxisName, cameraTransform.forward, RotateAroundZAxisSpeed);
                ry = RotateAroundAxis(objectToManipulate, RotateAroundYAxisName, cameraTransform.up, RotateAroundYAxisSpeed);
                rx = RotateAroundAxis(objectToManipulate, RotateAroundXAxisName, cameraTransform.right, RotateAroundXAxisSpeed);
            }

            //Move
            bool usingSameAxisForRotateAndMove = RotateAroundYAxisName == MoveXAxisName ||
                                                 RotateAroundYAxisName == MoveZAxisName || 
                                                 RotateAroundZAxisName == MoveXAxisName ||
                                                 RotateAroundZAxisName == MoveYAxisName;

            float x = 0;
            float y = 0;

            //only move if the movement axis has not been used for rotation
            if (!(enableRotate && usingSameAxisForRotateAndMove))
            {
                x = Input.GetAxis(MoveXAxisName) * MoveXAxisSpeed * 60 * Time.deltaTime;
                objectToManipulate.transform.RotateAround(cameraTransform.position, cameraTransform.up, x);

                y = Input.GetAxis(MoveYAxisName) * MoveYAxisSpeed * 60 * Time.deltaTime;
                objectToManipulate.transform.RotateAround(cameraTransform.position, cameraTransform.right, y);
            }

            var z = Input.GetAxis(MoveZAxisName) * MoveZAxisSpeed * 60 * Time.deltaTime;
            objectToManipulate.transform.position += cameraTransform.forward*z*-0.03f;

            var stickInFrontOfMe = Input.GetButton(MoveWithGazeButtonName);

            if (stickInFrontOfMe)
            {
                if (stickDistance == 0)
                {
                    stickDistance = (objectToManipulate.transform.position - cameraTransform.position).magnitude;
                }

                objectToManipulate.transform.position = cameraTransform.position + cameraTransform.forward * stickDistance;
            }
            else
            {
                stickDistance = 0;
            }


            if (stickInFrontOfMe || x != 0 || y != 0 || z != 0 || rz != 0 || ry != 0 || rx != 0)
            {
                lastAffectedObject = objectToManipulate;
            }
#if !UNITY_EDITOR
            else
            {
                lastAffectedObject = null; //easier to test in Unity this way
            }
#endif
        }

        /// <summary>
        /// Rotates the specified GameObject as dictated by joystick values and multipliers
        /// </summary>
        /// <param name="objectToRotate">the gameObject to rotate</param>
        /// <param name="joyAxisName">the name of the joystick axis</param>
        /// <param name="vectorToRotateAround">vector to rotate around</param>
        /// <param name="speed">rotation speed</param>
        /// <returns>the amount of rotation applied</returns>
        private float RotateAroundAxis(GameObject objectToRotate, string joyAxisName, Vector3 vectorToRotateAround, float speed)
        {
            if (string.IsNullOrEmpty(joyAxisName))
            {
                return 0;
            }

            var result = Input.GetAxis(joyAxisName) * speed * 60 * Time.deltaTime;
            objectToRotate.transform.rotation = Quaternion.Euler(vectorToRotateAround * result) * objectToRotate.transform.rotation;
            return result;
        }
    }
}

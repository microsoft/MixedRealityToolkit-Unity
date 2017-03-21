using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Script teleports the user to the location being gazed at when Y was pressed on a Gamepad.
    /// </summary>
    public class MixedRealityTeleport : Singleton<MixedRealityTeleport>
    {
        [Tooltip("Game pad button to press for teleporting or jump.")]
        public string TeleportButtonName = "Jump";

        [Tooltip("Game pad button to press for going back to a state.")]
        public string GoBackButtonName = "Fire2";

        [Tooltip("Name of the joystick axis to move along X.")]
        public string LeftJoystickX = "ControllerLeftStickX";

        [Tooltip("Name of the joystick axis to move along Y.")]
        public string LeftJoystickY = "ControllerLeftStickY";

        [Tooltip("Name of the joystick axis to look left and right along Y.")]
        public string RightJoystickX = "ControllerRightStickX";

        [Tooltip("Name of the joystick axis to look up and down along X.")]
        public string RightJoystickY = "ControllerRightStickY";

        public bool EnableLookUpDown = false;

        public bool EnableLookLeftRight = true;

        public float SpeedScale { get; set; }

        GazeManager gazeManager;
        Vector3 positionBeforeJump = Vector3.zero;

        float xrot = 0;
        float yrot = 0;

        private void Start()
        {
            gazeManager = GazeManager.Instance;
            SpeedScale = 0.6f;
        }

        void Update()
        {
            HandleGamepadYPressed();
            HandleGamepadBPressed();
            HandleJoystickLookAround();
        }

        private void HandleGamepadYPressed()
        {
            if (Input.GetButtonUp(TeleportButtonName))
            {
                Debug.Log("Gamepad: Y pressed");

                positionBeforeJump = transform.position;
                Debug.Log("Position before Jump" + positionBeforeJump);

                Vector3 hitPos = gazeManager.HitObject == null ?
                    (Camera.main.transform.position + Camera.main.transform.forward * 2.0f) :
                    gazeManager.HitPosition;

                transform.position += hitPos - Camera.main.transform.position;
            }
        }

        private void HandleGamepadBPressed()
        {
            if (Input.GetButtonDown(GoBackButtonName))
            {
                Debug.Log("Gamepad: B pressed");

                transform.position = positionBeforeJump;
            }
        }

        private void HandleJoystickLookAround()
        {
            float forwardAmount = Input.GetAxis(LeftJoystickY) * -1;
            float strafeAmount = Input.GetAxis(LeftJoystickX);

            float XRotAmount = Input.GetAxis(RightJoystickY);
            float YRotAmount = Input.GetAxis(RightJoystickX);

            Vector3 forwardDirection = Camera.main.transform.forward;
            Vector3 rightDirection = Camera.main.transform.right;

            Vector3 startPos = transform.position;
            transform.position += forwardDirection * (forwardAmount * SpeedScale * Time.deltaTime);
            transform.position += rightDirection * (strafeAmount * SpeedScale * Time.deltaTime);

            if (Physics.BoxCast(Camera.main.transform.position, Vector3.one * 0.2f, transform.position - startPos, Quaternion.identity, 0.2f))
            {
                transform.position = startPos;
            }

            if (EnableLookUpDown)
            {
                xrot += XRotAmount * Time.deltaTime * 30;
            }

            if (EnableLookLeftRight)
            {
                yrot += YRotAmount * Time.deltaTime * 30;
            }

            transform.localRotation = Quaternion.Euler(xrot, yrot, 0);
        }
    }
}
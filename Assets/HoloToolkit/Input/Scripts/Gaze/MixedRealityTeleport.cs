using UnityEngine;
using UnityEngine.VR.WSA.Input;

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

        public bool EnableTeleport = true;

        public bool EnableJoystickMovement = false;

        public float SpeedScale { get; set; }

        public float BumperRotationSize = 30.0f;

        public GameObject TeleportMarker;
        private Animator animationController;

        /// <summary>
        /// The fade control allows us to fade out and fade in the scene.
        /// This is done to improve comfort when using an immersive display.
        /// </summary>
        private FadeScript fadeControl;

        private GazeManager gazeManager;
        private Vector3 positionBeforeJump = Vector3.zero;
        private GameObject teleportMarker;
        private bool teleportValid;
        private bool teleporting;

        private void Start()
        {
            gazeManager = GazeManager.Instance;
            fadeControl = FadeScript.Instance;
            SpeedScale = 0.6f;

            TeleportMarker.SetActive(false);
            teleportMarker = Instantiate(TeleportMarker);

            animationController = teleportMarker.GetComponentInChildren<Animator>();
            if (animationController != null)
            {
                animationController.StopPlayback();
            }
        }

        void Update()
        {
            HandleTeleport();
            HandleGoBackPressed();
            HandleJoystickMovement();
            if (InteractionManager.numSourceStates == 0)
            {
                HandleBumperRotation();
            }
        }

        private void HandleTeleport()
        {
            if (EnableTeleport)
            {
                if (teleporting)
                {
                    if (Input.GetButtonUp(TeleportButtonName))
                    {
                        teleporting = false;
                        if (teleportValid)
                        {
                            positionBeforeJump = transform.position;
                            float verticalOffset;
                            RaycastHit hitInfo;
                            if (Physics.Raycast(Camera.main.transform.position, Vector3.down, out hitInfo, 5.0f))
                            {
                                verticalOffset = hitInfo.distance;
                            }
                            else
                            {
                                verticalOffset = 2.6f;
                            }

                            Vector3 hitPos = teleportMarker.transform.position + Vector3.up * verticalOffset;

                            fadeControl.DoFade(0.25f, 0.5f, () =>
                            {
                                SetWorldPosition(hitPos);
                            }, null);
                        }

                        DisableMarker();
                    }
                    else
                    {
                        PositionMarker();
                    }
                }
                else
                {
                    if (fadeControl.Busy == false && Input.GetButtonDown(TeleportButtonName))
                    {
                        teleporting = true;
                        EnableMarker();
                        PositionMarker();
                    }
                }
            }
        }

        private void HandleGoBackPressed()
        {
            if (EnableTeleport && Input.GetButtonDown(GoBackButtonName))
            {
                Vector3 oldPositionBeforeJump = positionBeforeJump;
                positionBeforeJump = transform.position;

                fadeControl.DoFade(0.25f, 0.5f, () =>
                {
                    SetWorldPosition(oldPositionBeforeJump);
                }, null);
            }
        }

        private void HandleJoystickMovement()
        {
            if (EnableJoystickMovement)
            {
                float forwardAmount = Input.GetAxis(LeftJoystickY) * -1;
                float strafeAmount = Input.GetAxis(LeftJoystickX);

                Vector3 forwardDirection = Camera.main.transform.forward;
                Vector3 rightDirection = Camera.main.transform.right;

                Vector3 startPos = transform.position;
                transform.position += forwardDirection * (forwardAmount * SpeedScale * Time.deltaTime);
                transform.position += rightDirection * (strafeAmount * SpeedScale * Time.deltaTime);

                if (Physics.BoxCast(Camera.main.transform.position, Vector3.one * 0.2f, transform.position - startPos, Quaternion.identity, 0.2f))
                {
                    transform.position = startPos;
                }
            }
        }

        private void HandleBumperRotation()
        {
            // Check bumpers for coarse rotation
            float bumperRot = 0;

            if (Input.GetButtonUp("LeftBumper"))
            {
                bumperRot = -BumperRotationSize;
            }

            if (Input.GetButtonUp("RightBumper"))
            {
                bumperRot = BumperRotationSize;
            }

            if (bumperRot != 0)
            {
                fadeControl.DoFade(
                    0.25f, // Fade out time
                    0.25f, // Fade in time
                    () => // Action after fade out
                    {
                        transform.RotateAround(Camera.main.transform.position, Vector3.up, bumperRot);
                    },
                    null); // Action after fade in
            }
        }

        /// <summary>
        /// Places the player in the specified position of the world
        /// </summary>
        /// <param name="worldPosition"></param>
        public void SetWorldPosition(Vector3 worldPosition)
        {
            // There are two things moving the camera: the camera parent (that this script is attached to)
            // and the user's head (which the MR device is attached to. :)). When setting the world position,
            // we need to set it relative to the user's head in the scene so they are looking/standing where 
            // we expect.
            transform.position = worldPosition - Camera.main.transform.localPosition;
        }

        private void EnableMarker()
        {
            teleportMarker.SetActive(true);
            if (animationController != null)
            {
                animationController.StartPlayback();
            }
        }

        private void DisableMarker()
        {
            if (animationController != null)
            {
                animationController.StopPlayback();
            }
            teleportMarker.SetActive(false);
        }

        private void PositionMarker()
        {
            Vector3 hitNormal = HitNormal();
            print(hitNormal);
            if (Vector3.Dot(hitNormal, Vector3.up) > 0.90f)
            {
                teleportValid = true;
                teleportMarker.transform.position = gazeManager.HitPosition;
            }
            else
            {
                teleportValid = false;
            }

            animationController.speed = teleportValid ? 1 : 0;
        }

        private Vector3 HitNormal()
        {
            Vector3 retval = Vector3.zero;
            if (gazeManager.HitObject != null)
            {
                retval = gazeManager.HitNormal;
            }
            return retval;
        }
    }
}
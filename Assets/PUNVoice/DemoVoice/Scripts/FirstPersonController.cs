// ----------------------------------------------------------------------------
// <copyright file="FirstPersonController.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// Custom fist person character controller.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace ExitGames.Demos.DemoPunVoice {

    using UnityEngine;

    public class FirstPersonController : BaseController {

        [SerializeField]
        private MouseLookHelper mouseLook = new MouseLookHelper();

        private float oldYRotation;
        private Quaternion velRotation;

        public Vector3 Velocity {
            get { return rigidBody.velocity; }
        }

        protected override void SetCamera() {
            base.SetCamera();
            mouseLook.Init(transform, camTrans);
        }

        protected override void Move(float h, float v) {
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = camTrans.forward * v + camTrans.right * h;
            desiredMove.x = desiredMove.x * speed;
            desiredMove.z = desiredMove.z * speed;
            desiredMove.y = 0;
            rigidBody.velocity = desiredMove;
        }

        private void Update() {
            RotateView();
        }

        private void RotateView() {
            // get the rotation before it's changed
            oldYRotation = transform.eulerAngles.y;
            mouseLook.LookRotation(transform, camTrans);
            // Rotate the rigidbody velocity to match the new direction that the character is looking
            velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            rigidBody.velocity = velRotation * rigidBody.velocity;
        }
    }

}
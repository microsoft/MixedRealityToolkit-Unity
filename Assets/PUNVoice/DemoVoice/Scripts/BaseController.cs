// ----------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// Base class of character controllers.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace ExitGames.Demos.DemoPunVoice {

    using UnityEngine;
    using UnityStandardAssets.CrossPlatformInput;

    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public abstract class BaseController : MonoBehaviour {
        public Camera ControllerCamera;

        protected Rigidbody rigidBody;
        protected Animator animator;
        protected Transform camTrans;             // A reference to transform of the third person camera

        private float h, v;

        [SerializeField]
        protected float speed = 5f;

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE)
    private Touch myTouch;
    private float x, y;
    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.
#endif

        [SerializeField]
        private float cameraDistance = 0f;

        protected virtual void OnEnable() {
            ChangePOV.CameraChanged += ChangePOV_CameraChanged;
        }

        protected virtual void OnDisable() {
            ChangePOV.CameraChanged -= ChangePOV_CameraChanged;
        }

        protected virtual void ChangePOV_CameraChanged(Camera camera) {
            if (camera != ControllerCamera) {
                enabled = false;
                HideCamera(ControllerCamera);
            }
            else {
                ShowCamera(ControllerCamera);
            }
        }

        protected virtual void Start() {
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView.isMine) {
                Init();
                SetCamera();
            }
            else {
                enabled = false;
            }

        }

        protected virtual void Init() {
            rigidBody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        protected virtual void SetCamera() {
            camTrans = ControllerCamera.transform;
            camTrans.position += cameraDistance * transform.forward;
        }

        protected virtual void UpdateAnimator(float h, float v) {
            // Create a boolean that is true if either of the input axes is non-zero.
            bool walking = h != 0 || v != 0;
            // Tell the animator whether or not the player is walking.
            animator.SetBool("IsWalking", walking);
        }

        protected virtual void FixedUpdate() {
            // Store the input axes.
            h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            v = CrossPlatformInputManager.GetAxisRaw("Vertical");
#if MOBILE_INPUT
            if (Mathf.Abs(h) < 0.5f) { h = 0f; }
            else { h = Mathf.Sign(h); }
            if (Mathf.Abs(v) < 0.5f) { v = 0f; }
            else { v = Mathf.Sign(v); }
#endif  
            // send input to the animator
            UpdateAnimator(h, v);
            // Move the player around the scene.
            Move(h, v);
        }

        protected virtual void ShowCamera(Camera camera) {
            if (camera != null) { camera.gameObject.SetActive(true); }
        }

        protected virtual void HideCamera(Camera camera) {
            if (camera != null) { camera.gameObject.SetActive(false); }
        }

        protected abstract void Move(float h, float v);
    }
}

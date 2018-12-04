using Pixie.Core;
using Pixie.DeviceControl;
using Pixie.DeviceControl.Users;
using UnityEngine;

namespace Pixie.Demos
{
    public class UserTransformsDemo : MonoBehaviour
    {
        [SerializeField]
        private GameObject avatar;
        // Our dummy input for user transform positions
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Transform animHeadTransform;
        [SerializeField]
        private Transform animRHandTransform;
        [SerializeField]
        private Transform animLHandTransform;
        // Display object for our user transforms
        [SerializeField]
        private GameObject displayHeadObject;
        [SerializeField]
        private GameObject displayLHandObject;
        [SerializeField]
        private GameObject displayRHandObject;

        private IUserObject userObject;
        private IUserTransforms userTransforms;
        private bool waveHi;
        private float moveSpeed = 0f;
        private float moveDir = 0f;

        private void OnEnable()
        {
            userObject = (IUserObject)gameObject.GetComponent(typeof(IUserObject));
            userTransforms = (IUserTransforms)gameObject.GetComponent(typeof(IUserTransforms));
        }

        private void Update()
        {
            if (!userObject.IsAssigned)
                return;

            Transform userTransform;

            if (userObject.IsLocalUser)
            {
                // We're a local user driving the positions of these transforms
                // Assign their positions / rotations based on our animated dummy's positions
                if (userTransforms.GetTransform(TransformTypeEnum.Head, out userTransform))
                {
                    userTransform.position = animHeadTransform.position;
                    userTransform.rotation = animHeadTransform.rotation;
                }

                if (userTransforms.GetTransform(TransformTypeEnum.RightHand, out userTransform))
                {
                    userTransform.position = animRHandTransform.position;
                    userTransform.rotation = animRHandTransform.rotation;
                }

                if (userTransforms.GetTransform(TransformTypeEnum.LeftHand, out userTransform))
                {
                    userTransform.position = animLHandTransform.position;
                    userTransform.rotation = animLHandTransform.rotation;
                }

                // Generate random motion for the animator
                moveSpeed = Mathf.Abs(Mathf.Sin(Time.time) * 0.25f);
                moveDir = Mathf.Cos(Time.time) * 2;

                animator.SetFloat("Speed", moveSpeed);
                animator.SetFloat("Direction", moveDir);

            }

            // In all cases, get our user transforms and parent our display objects under them
            // so we can see where they are
            if (userTransforms.GetTransform(TransformTypeEnum.Head, out userTransform))
            {
                displayHeadObject.SetActive(true);
                displayHeadObject.transform.parent = userTransform;
                displayHeadObject.transform.localPosition = Vector3.zero;
                displayHeadObject.transform.localRotation = Quaternion.identity;
            }
            else
            {
                displayHeadObject.SetActive(false);
            }

            if (userTransforms.GetTransform(TransformTypeEnum.RightHand, out userTransform))
            {
                displayLHandObject.SetActive(true);
                displayRHandObject.transform.parent = userTransform;
                displayRHandObject.transform.localPosition = Vector3.zero;
                displayRHandObject.transform.localRotation = Quaternion.identity;
            }
            else
            {
                displayLHandObject.SetActive(false);
            }

            if (userTransforms.GetTransform(TransformTypeEnum.LeftHand, out userTransform))
            {
                displayLHandObject.SetActive(true);
                displayLHandObject.transform.parent = userTransform;
                displayLHandObject.transform.localPosition = Vector3.zero;
                displayLHandObject.transform.localRotation = Quaternion.identity;
            }
            else
            {
                displayLHandObject.SetActive(false);
            }
        }
    }
}
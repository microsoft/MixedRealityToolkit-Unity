using UnityEngine;

namespace MRTK.Grabbables
{
    /// <summary>
    /// Extends its behaviour from BaseThrowable. This is a non-abstract script that's actually attached to throwable object
    /// This script will not work without a grab script attached to the same gameObject
    /// </summary>


    public class ThrowableObject : BaseThrowable
    {
        public GameObject ThrowObject;
        private Vector3 startPos;
        private Color startColor;

        private void Start()
        {
            startPos = transform.position;
            startColor = GetComponent<Renderer>().material.color;
        }

        public override void Throw(BaseGrabbable grabbable)
        {
            base.Throw(grabbable);
            GetComponent<Rigidbody>().velocity = grabbable.GetAverageVelocity() * ThrowMultiplier;
            if (ZeroGravityThrow)
            {
                grabbable.GetComponent<Rigidbody>().useGravity = false;
            }

            SpawnDuplicate();
            
        }

        void SpawnDuplicate()
        {

            GameObject thrw = Instantiate(ThrowObject, startPos, Quaternion.identity) as GameObject;

            thrw.GetComponent<ThrowableObject>().ZeroGravityThrow = ZeroGravityThrow;
            thrw.GetComponent<ThrowableObject>().ThrowMultiplier = ThrowMultiplier;
            thrw.GetComponent<Renderer>().material.color = Color.gray;


        }
    }
}
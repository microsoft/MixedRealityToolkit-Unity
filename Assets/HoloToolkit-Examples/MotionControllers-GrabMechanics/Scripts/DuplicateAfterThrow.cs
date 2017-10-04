using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.Grabbables
{

    public class DuplicateAfterThrow : MonoBehaviour
    {

        public GameObject ThrowObject;
        private Vector3 startPos;
        private Color startColor;

        protected virtual void OnEnable()
        {
            grabbable.OnReleased += SpawnDuplicate;
        }

        protected virtual void OnDisable()
        {
            grabbable.OnReleased -= SpawnDuplicate;
        }
        protected virtual void Awake()
        {
            grabbable = GetComponent<BaseGrabbable>();
        }

        private void Start()
        {
            startPos = transform.position;
            startColor = GetComponent<Renderer>().material.color;
            ThrowObject = gameObject;
        }

        /// <summary>
        /// For the demo only - if we throw an object, we respawn it at its initial location with the same throw properties as the previous one.
        /// This way a user can try out throw a few times
        /// </summary>
        /// <param name="grabbable"></param>
        void SpawnDuplicate(BaseGrabbable grabbable)
        {
            GameObject thrwn = Instantiate(ThrowObject, startPos, Quaternion.identity) as GameObject;
            thrwn.GetComponent<ThrowableObject>().ZeroGravityThrow = GetComponent<BaseThrowable>().ZeroGravityThrow;
            thrwn.GetComponent<ThrowableObject>().ThrowMultiplier = GetComponent<BaseThrowable>().ThrowMultiplier;
            thrwn.GetComponent<Renderer>().material.color = startColor;
        }

        private BaseGrabbable grabbable;
    }


}

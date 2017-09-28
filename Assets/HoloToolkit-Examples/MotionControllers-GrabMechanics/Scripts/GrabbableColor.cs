using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.Grabbables
{
    /// <summary>
    /// Simple class to change the color of grabbable objects based on state
    /// </summary>
    public class GrabbableColor : MonoBehaviour
    {
        private void Awake()
        {
            if (grabbable == null)
                grabbable = GetComponent<BaseGrabbable>();
            if (targetRenderer == null)
                targetRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
            
            grabbable.OnContactStateChange += OnContactStateChange;
            grabbable.OnGrabStateChange += OnGrabStateChange;
        }

        private void OnContactStateChange (BaseGrabbable g)
        {
            switch (g.ContactState)
            {
                case GrabStateEnum.Inactive:
                    targetRenderer.material.color = originalColor;
                    break;

                case GrabStateEnum.Multi:
                    targetRenderer.material.color = colorOnContactMulti;
                    break;

                case GrabStateEnum.Single:
                    targetRenderer.material.color = colorOnContactSingle;
                    break;
            }
        }

        private void OnGrabStateChange(BaseGrabbable g)
        {
            switch (g.GrabState)
            {
                case GrabStateEnum.Inactive:
                    targetRenderer.material.color = originalColor;
                    break;

                case GrabStateEnum.Multi:
                    targetRenderer.material.color = colorOnGrabMulti;
                    break;

                case GrabStateEnum.Single:
                    targetRenderer.material.color = colorOnGrabSingle;
                    break;
            }
        }

        [Header("Colors")]
        [SerializeField]
        private Color colorOnContactSingle = Color.blue;
        [SerializeField]
        private Color colorOnContactMulti = Color.cyan;
        [SerializeField]
        private Color colorOnGrabSingle = Color.yellow;
        [SerializeField]
        private Color colorOnGrabMulti = Color.red;
        [SerializeField]
        private Color colorOnUse = Color.magenta;

        [Header("Objects")]
        [SerializeField]
        private Renderer targetRenderer;
        [SerializeField]
        private BaseGrabbable grabbable;

        private Color originalColor;
    }
}
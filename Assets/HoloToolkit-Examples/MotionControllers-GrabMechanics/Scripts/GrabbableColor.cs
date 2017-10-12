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

            originalColor = targetRenderer.material.color;
            grabbable.OnContactStateChange += RefreshColor;
            grabbable.OnGrabStateChange += RefreshColor;
        }

        private void RefreshColor(BaseGrabbable g)
        {
            Color finalColor = originalColor;

            switch (g.ContactState)
            {
                case GrabStateEnum.Inactive:
                default:
                    break;

                case GrabStateEnum.Multi:
                    finalColor = colorOnContactMulti;
                    break;

                case GrabStateEnum.Single:
                    finalColor = colorOnContactSingle;
                    break;
            }

            switch (g.GrabState)
            {
                case GrabStateEnum.Inactive:
                default:
                    break;

                case GrabStateEnum.Multi:
                    finalColor = colorOnGrabMulti;
                    break;

                case GrabStateEnum.Single:
                    finalColor = colorOnGrabSingle;
                    break;
            }

            targetRenderer.material.color = finalColor;
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

        [Header("Objects")]
        [SerializeField]
        private Renderer targetRenderer;
        [SerializeField]
        private BaseGrabbable grabbable;

        private Color originalColor;
    }
}
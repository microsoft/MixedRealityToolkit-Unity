// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnClickInstantiate.cs" company="Exit Games GmbH">
// Part of: Photon Unity Utilities
// </copyright>
// <summary>A compact script for prototyping.</summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections;


namespace Photon.Pun.UtilityScripts
{
    using UnityEngine;
    using UnityEngine.EventSystems;


    /// <summary>
    /// This component will instantiate a network GameObject when in a room and the user click on that component's GameObject.
    /// Uses PhysicsRaycaster for positioning.
    /// </summary>
    public class OnClickRpc : MonoBehaviourPun, IPointerClickHandler
    {
        public PointerEventData.InputButton Button;
        public KeyCode ModifierKey;

        public RpcTarget Target;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!PhotonNetwork.InRoom || (this.ModifierKey != KeyCode.None && !Input.GetKey(this.ModifierKey)) || eventData.button != this.Button)
            {
                return;
            }
            
            this.photonView.RPC("ClickRpc", this.Target);
        }


        #region RPC Implementation

        private Material originalMaterial;
        private Color originalColor;
        private bool isFlashing;

        [PunRPC]
        public void ClickRpc()
        {
            //Debug.Log("ClickRpc Called");
            this.StartCoroutine(this.ClickFlash());
        }
        
        public IEnumerator ClickFlash()
        {
            if (isFlashing)
            {
                yield break;
            }
            isFlashing = true;

            this.originalMaterial = GetComponent<Renderer>().material;
            if (!this.originalMaterial.HasProperty("_EmissionColor"))
            {
                Debug.LogWarning("Doesn't have emission, can't flash " + gameObject);
                yield break;
            }

            bool wasEmissive = this.originalMaterial.IsKeywordEnabled("_EMISSION");
            this.originalMaterial.EnableKeyword("_EMISSION");

            this.originalColor = this.originalMaterial.GetColor("_EmissionColor");
            this.originalMaterial.SetColor("_EmissionColor", Color.white);

            for (float f = 0.0f; f <= 1.0f; f += 0.08f)
            {
                Color lerped = Color.Lerp(Color.white, this.originalColor, f);
                this.originalMaterial.SetColor("_EmissionColor", lerped);
                yield return null;
            }

            this.originalMaterial.SetColor("_EmissionColor", this.originalColor);
            if (!wasEmissive) this.originalMaterial.DisableKeyword("_EMISSION");
            isFlashing = false;
        }

        #endregion
    }
}
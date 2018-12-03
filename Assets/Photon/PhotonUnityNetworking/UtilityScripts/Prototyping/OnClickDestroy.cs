// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnClickDestroy.cs" company="Exit Games GmbH">
// Part of: Photon Unity Utilities
// </copyright>
// <summary>A compact script for prototyping.</summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


namespace Photon.Pun.UtilityScripts
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Destroys the networked GameObject either by PhotonNetwork.Destroy or by sending an RPC which calls Object.Destroy().
    /// </summary>
    /// <remarks>
    /// Using an RPC to Destroy a GameObject is typically a bad idea.
    /// It allows any player to Destroy a GameObject and may cause errors.
    ///
    /// A client has to clean up the server's event-cache, which contains events for Instantiate and
    /// buffered RPCs related to the GO.
    /// 
    /// A buffered RPC gets cleaned up when the sending player leaves the room, so players joining later
    /// won't get those buffered RPCs. This in turn, may mean they don't destroy the GO due to coming later.
    ///
    /// Vice versa, a GameObject Instantiate might get cleaned up when the creating player leaves a room.
    /// This way, the GameObject that a RPC targets might become lost.
    ///
    /// It makes sense to test those cases. Many are not breaking errors and you just have to be aware of them.
    ///
    /// 
    /// Gets OnClick() calls by Unity's IPointerClickHandler. Needs a PhysicsRaycaster on the camera.
    /// See: https://docs.unity3d.com/ScriptReference/EventSystems.IPointerClickHandler.html
    /// </remarks>
    public class OnClickDestroy : MonoBehaviourPun, IPointerClickHandler
    {
        public PointerEventData.InputButton Button;
        public KeyCode ModifierKey;

        public bool DestroyByRpc;
        

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!PhotonNetwork.InRoom || (this.ModifierKey != KeyCode.None && !Input.GetKey(this.ModifierKey)) || eventData.button != this.Button )
            {
                return;
            }


            if (this.DestroyByRpc)
            {
                this.photonView.RPC("DestroyRpc", RpcTarget.AllBuffered);
            }
            else
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }


        [PunRPC]
        public IEnumerator DestroyRpc()
        {
            Destroy(this.gameObject);
            yield return 0; // if you allow 1 frame to pass, the object's OnDestroy() method gets called and cleans up references.
        }
    }
}
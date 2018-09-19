using UnityEngine;
using System.Collections;

/// <summary>
/// Implements OnClick to destroy the GameObject it's attached to. Optionally a RPC is sent to do this.
/// </summary>
/// <remarks>
/// Using an RPC to Destroy a GameObject allows any player to Destroy a GameObject. But it might cause errors.
/// RPC and the Instantiated GameObject are not fully linked on the server. One might stick in the server witout
/// the other.
///
/// A buffered RPC gets cleaned up when the sending player leaves the room. This means, the RPC gets lost.
///
/// Vice versus, a GameObject Instantiate might get cleaned up when the creating player leaves a room.
/// This way, the GameObject that a RPC targets might become lost.
///
/// It makes sense to test those cases. Many are not breaking errors and you just have to be aware of them.
///
/// Gets OnClick() calls by InputToEvent class attached to a camera.
/// </remarks>
[RequireComponent(typeof(PhotonView))]
public class OnClickDestroy : Photon.MonoBehaviour
{
    public bool DestroyByRpc;

    public void OnClick()
    {
        if (!DestroyByRpc)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
        else
        {
            this.photonView.RPC("DestroyRpc", PhotonTargets.AllBuffered);
        }
    }

    [PunRPC]
    public IEnumerator DestroyRpc()
    {
        GameObject.Destroy(this.gameObject);
        yield return 0; // if you allow 1 frame to pass, the object's OnDestroy() method gets called and cleans up references.
        PhotonNetwork.UnAllocateViewID(this.photonView.viewID);
    }
}

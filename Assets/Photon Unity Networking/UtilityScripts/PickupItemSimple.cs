using UnityEngine;
using System.Collections;

/// <summary>
/// Makes a scene object pickup-able. Needs a PhotonView which belongs to the scene.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class PickupItemSimple : Photon.MonoBehaviour
{
    public float SecondsBeforeRespawn = 2;
    public bool PickupOnCollide;
    public bool SentPickup;

    public void OnTriggerEnter(Collider other)
    {
        // we only call Pickup() if "our" character collides with this PickupItem.
        // note: if you "position" remote characters by setting their translation, triggers won't be hit.

        PhotonView otherpv = other.GetComponent<PhotonView>();
        if (this.PickupOnCollide && otherpv != null && otherpv.isMine)
        {
            //Debug.Log("OnTriggerEnter() calls Pickup().");
            this.Pickup();
        }
    }

    public void Pickup()
    {
        if (this.SentPickup)
        {
            // skip sending more pickups until the original pickup-RPC got back to this client
            return;
        }
        
        this.SentPickup = true;
        this.photonView.RPC("PunPickupSimple", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    public void PunPickupSimple(PhotonMessageInfo msgInfo)
    {
        // one of the messages might be ours
        // note: you could check "active" first, if you're not interested in your own, failed pickup-attempts.
        if (this.SentPickup && msgInfo.sender.isLocal)
        {
            if (this.gameObject.GetActive())
            {
                // picked up! yay.
            }
            else
            {
                // pickup failed. too late (compared to others)
            }
        }

        this.SentPickup = false;

        if (!this.gameObject.GetActive())
        {
            Debug.Log("Ignored PU RPC, cause item is inactive. " + this.gameObject);
            return;
        }
        

        // how long it is until this item respanws, depends on the pickup time and the respawn time
        double timeSinceRpcCall = (PhotonNetwork.time - msgInfo.timestamp);
        float timeUntilRespawn = SecondsBeforeRespawn - (float)timeSinceRpcCall;
        //Debug.Log("msg timestamp: " + msgInfo.timestamp + " time until respawn: " + timeUntilRespawn);

        if (timeUntilRespawn > 0)
        {
            // this script simply disables the GO for a while until it respawns. 
            this.gameObject.SetActive(false);
            Invoke("RespawnAfter", timeUntilRespawn);
        }
    }

    public void RespawnAfter()
    {
        if (this.gameObject != null)
        {
            this.gameObject.SetActive(true);
        }
    }
}

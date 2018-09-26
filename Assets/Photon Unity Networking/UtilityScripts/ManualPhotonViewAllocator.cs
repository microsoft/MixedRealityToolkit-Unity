using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class ManualPhotonViewAllocator : MonoBehaviour
{
    public GameObject Prefab;

    public void AllocateManualPhotonView()
    {
        PhotonView pv = this.gameObject.GetPhotonView();
        if (pv == null)
        {
            Debug.LogError("Can't do manual instantiation without PhotonView component.");
            return;
        }

        int viewID = PhotonNetwork.AllocateViewID();
        pv.RPC("InstantiateRpc", PhotonTargets.AllBuffered, viewID);
    }

    [PunRPC]
    public void InstantiateRpc(int viewID)
    {
        GameObject go = GameObject.Instantiate(Prefab, InputToEvent.inputHitPos + new Vector3(0, 5f, 0), Quaternion.identity) as GameObject;
        go.GetPhotonView().viewID = viewID;

        OnClickDestroy ocd = go.GetComponent<OnClickDestroy>();
        ocd.DestroyByRpc = true;
    }
}

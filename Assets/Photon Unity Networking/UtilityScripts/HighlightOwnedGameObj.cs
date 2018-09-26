using UnityEngine;

[RequireComponent(typeof (PhotonView))]
public class HighlightOwnedGameObj : Photon.MonoBehaviour
{
    public GameObject PointerPrefab;
    public float Offset = 0.5f;
    private Transform markerTransform;


    // Update is called once per frame
    private void Update()
    {
        if (photonView.isMine)
        {
            if (this.markerTransform == null)
            {
                GameObject markerObject = (GameObject) GameObject.Instantiate(this.PointerPrefab);
                markerObject.transform.parent = gameObject.transform;
                this.markerTransform = markerObject.transform;
            }

            Vector3 parentPos = gameObject.transform.position;
            this.markerTransform.position = new Vector3(parentPos.x, parentPos.y + this.Offset, parentPos.z);
            this.markerTransform.rotation = Quaternion.identity;
        }
        else if (this.markerTransform != null)
        {
            Destroy(this.markerTransform.gameObject);
            this.markerTransform = null;
        }
    }
}
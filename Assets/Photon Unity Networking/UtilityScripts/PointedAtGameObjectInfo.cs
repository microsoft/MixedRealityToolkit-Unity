using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputToEvent))]
public class PointedAtGameObjectInfo : MonoBehaviour 
{
    void OnGUI()
    {
        if (InputToEvent.goPointedAt != null)
        {
            PhotonView pv = InputToEvent.goPointedAt.GetPhotonView();
            if (pv != null)
            {
                GUI.Label(new Rect(Input.mousePosition.x + 5, Screen.height - Input.mousePosition.y - 15, 300, 30), string.Format("ViewID {0} {1}{2}", pv.viewID, (pv.isSceneView) ? "scene " : "", (pv.isMine) ? "mine" : "owner: " + pv.ownerId));
            }
        }
    }

}

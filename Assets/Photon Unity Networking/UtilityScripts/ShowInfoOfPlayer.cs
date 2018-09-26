using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
/// <summary>
/// Can be attached to a GameObject to show info about the owner of the PhotonView.
/// </summary>
/// <remarks>
/// This is a Photon.Monobehaviour, which adds the property photonView (that's all).
/// </remarks>
[RequireComponent(typeof(PhotonView))]
public class ShowInfoOfPlayer : Photon.MonoBehaviour
{
    private GameObject textGo;
    private TextMesh tm;
    public float CharacterSize = 0;

    public Font font;
    public bool DisableOnOwnObjects;

    void Start()
    {
        if (font == null)
        {
            #if UNITY_3_5
            font = (Font)FindObjectsOfTypeIncludingAssets(typeof(Font))[0];
            #else
            font = (Font)Resources.FindObjectsOfTypeAll(typeof(Font))[0];
            #endif
            Debug.LogWarning("No font defined. Found font: " + font);
        }

        if (tm == null)
        {
            textGo = new GameObject("3d text");
            //textGo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            textGo.transform.parent = this.gameObject.transform;
            textGo.transform.localPosition = Vector3.zero;

            MeshRenderer mr = textGo.AddComponent<MeshRenderer>();
            mr.material = font.material;
            tm = textGo.AddComponent<TextMesh>();
            tm.font = font;
            tm.anchor = TextAnchor.MiddleCenter;
            if (this.CharacterSize > 0)
            {
                tm.characterSize = this.CharacterSize;
            }
        }
    }

    void Update()
    {
        bool showInfo = !this.DisableOnOwnObjects || this.photonView.isMine;
        if (textGo != null)
        {
            textGo.SetActive(showInfo);
        }
        if (!showInfo)
        {
            return;
        }

        
        PhotonPlayer owner = this.photonView.owner;
        if (owner != null)
        {
            tm.text = (string.IsNullOrEmpty(owner.name)) ? "player"+owner.ID : owner.name;
        }
        else if (this.photonView.isSceneView)
        {
            tm.text = "scn";
        }
        else
        {
            tm.text = "n/a";
        }
    }
}

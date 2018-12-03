// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatAppIdCheckerUI.cs" company="Exit Games GmbH">
//   Part of: PhotonChat demo, 
// </copyright>                                                                                             
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

#if PHOTON_UNITY_NETWORKING
using UnityEngine.UI;
using Photon.Pun;


/// <summary>
/// This is used in the Editor Splash to properly inform the developer about the chat AppId requirement.
/// </summary>
[ExecuteInEditMode]
public class ChatAppIdCheckerUI : MonoBehaviour
{
    public Text Description;

    public void Update()
    {
		if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
        {
            Description.text = "<Color=Red>WARNING:</Color>\nPlease setup a Chat AppId in the PhotonServerSettings file.";
        }
        else
        {
            Description.text = string.Empty;
        }
    }
}
#else

public class ChatAppIdCheckerUI : MonoBehaviour
{
    // empty class. if PUN is not present, we currently don't check Chat-AppId "presence".
}

#endif
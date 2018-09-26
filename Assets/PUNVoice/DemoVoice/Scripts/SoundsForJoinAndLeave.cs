using Photon;
using UnityEngine;

public class SoundsForJoinAndLeave : PunBehaviour
{
    public AudioClip JoinClip;
    public AudioClip LeaveClip;
    private AudioSource source;

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (this.JoinClip != null)
        {
            if (this.source == null) this.source = FindObjectOfType<AudioSource>();
            this.source.PlayOneShot(this.JoinClip);
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (this.LeaveClip != null)
        {
            if (this.source == null) this.source = FindObjectOfType<AudioSource>();
            this.source.PlayOneShot(this.LeaveClip);
        }
    }
}
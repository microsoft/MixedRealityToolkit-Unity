using UnityEngine;
using System.Collections;

public class HighlightSpeaker : MonoBehaviour
{
    // used in recorder and speaker visualisation
    private PhotonVoiceSpeaker speaker;
    private Renderer rendererComp;

    // Use this for initialization
    void Start()
    {
        speaker = this.transform.parent.GetComponent<PhotonVoiceSpeaker>();
        if (speaker == null)
        {
            this.enabled = false;
            return;
        }

        rendererComp = this.GetComponent<Renderer>();

        if (rendererComp == null)
        {
            this.enabled = false;
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (this.speaker != null)
        {
            this.rendererComp.enabled = this.speaker.IsPlaying;
        }

    }
}
 
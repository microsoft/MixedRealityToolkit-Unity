using UnityEngine;
using System.Collections;

public class HighlightRecorder: MonoBehaviour
{
    private PhotonVoiceRecorder recorder;
    private Renderer rendererComp;

    // Use this for initialization
    void Start()
    {

        recorder = this.transform.parent.GetComponent<PhotonVoiceRecorder>();
        if (recorder == null)
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
        if (this.recorder != null)
        {
            this.rendererComp.enabled = recorder.IsTransmitting;
        }
    }
}
 
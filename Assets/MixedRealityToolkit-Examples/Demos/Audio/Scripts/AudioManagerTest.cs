using UnityEngine;
using Microsoft.MixedReality.Toolkit.Audio;

public class AudioManagerTest : MonoBehaviour
{
    [SerializeField]
    private AudioEvent testEvent;

    private void Start()
    {
        AudioManager.PlayEvent(this.testEvent, this.gameObject);
    }
}
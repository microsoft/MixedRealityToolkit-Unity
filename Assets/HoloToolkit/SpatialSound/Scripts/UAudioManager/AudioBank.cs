using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class AudioBank<T> : ScriptableObject
    {
        public T[] Events;
    }

    [CreateAssetMenu(fileName = "AudioEventBank")]
    public class AudioEventBank : AudioBank<AudioEvent>
    {
    }
}


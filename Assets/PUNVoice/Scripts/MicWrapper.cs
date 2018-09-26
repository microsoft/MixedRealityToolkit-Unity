using UnityEngine;
using System.Collections;
using System;

// Wraps UnityEngine.Microphone with Voice.IAudioStream interface.
internal class MicWrapper : ExitGames.Client.Photon.Voice.IAudioStreamFloat
{
    private AudioClip mic;
    private string device;

    public MicWrapper(string device, int suggestedFrequency)
    {
        if (Microphone.devices.Length < 1)
        {
            return;
        }
        this.device = device;
        int minFreq;
        int maxFreq;
        Microphone.GetDeviceCaps(device, out minFreq, out maxFreq);
        var frequency = suggestedFrequency;
//        minFreq = maxFreq = 44100; // test like android client
        if (suggestedFrequency < minFreq || maxFreq != 0 && suggestedFrequency > maxFreq)
        {
            Debug.LogWarningFormat("PUNVoice: MicWrapper does not support suggested frequency {0} (min: {1}, max: {2}). Setting to {2}",
                suggestedFrequency, minFreq, maxFreq);
            frequency = maxFreq;
        }
        this.mic = Microphone.Start(device, true, 1, frequency);
    }

    public void Dispose()
    {
		Microphone.End(this.device);
    }

    public int SamplingRate { get { return this.mic.frequency; } }
    public int Channels { get { return this.mic.channels; } }

    private int micPrevPos;
    private int micLoopCnt;
    private int readAbsPos;

    public bool GetData(float[] buffer)
    {
        int micPos = Microphone.GetPosition(this.device);
        // loop detection
        if (micPos < micPrevPos)
        {
            micLoopCnt++;            
        }
        micPrevPos = micPos;

        var micAbsPos = micLoopCnt * this.mic.samples + micPos;

        var bufferSamplesCount = buffer.Length / mic.channels;

        var nextReadPos = this.readAbsPos + bufferSamplesCount;
        if (nextReadPos < micAbsPos)
        {
            this.mic.GetData(buffer, this.readAbsPos % this.mic.samples);
            this.readAbsPos = nextReadPos;
            return true;
        }
        else
        {
            return false;
        }        
    }
}

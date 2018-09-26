using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The PhotonStreamQueue helps you poll object states at higher frequencies then what
/// PhotonNetwork.sendRate dictates and then sends all those states at once when
/// Serialize() is called.
/// On the receiving end you can call Deserialize() and then the stream will roll out
/// the received object states in the same order and timeStep they were recorded in.
/// </summary>
public class PhotonStreamQueue
{
    #region Members

    private int m_SampleRate;
    private int m_SampleCount;
    private int m_ObjectsPerSample = -1;

    private float m_LastSampleTime = -Mathf.Infinity;
    private int m_LastFrameCount = -1;
    private int m_NextObjectIndex = -1;

    private List<object> m_Objects = new List<object>();

    private bool m_IsWriting;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PhotonStreamQueue"/> class.
    /// </summary>
    /// <param name="sampleRate">How many times per second should the object states be sampled</param>
    public PhotonStreamQueue(int sampleRate)
    {
        this.m_SampleRate = sampleRate;
    }

    private void BeginWritePackage()
    {
        //If not enough time has passed since the last sample, we don't want to write anything
        if (Time.realtimeSinceStartup < this.m_LastSampleTime + 1f/this.m_SampleRate)
        {
            this.m_IsWriting = false;
            return;
        }

        if (this.m_SampleCount == 1)
        {
            this.m_ObjectsPerSample = this.m_Objects.Count;
            //Debug.Log( "Setting m_ObjectsPerSample to " + m_ObjectsPerSample );
        }
        else if (this.m_SampleCount > 1)
        {
            if (this.m_Objects.Count/this.m_SampleCount != this.m_ObjectsPerSample)
            {
                Debug.LogWarning("The number of objects sent via a PhotonStreamQueue has to be the same each frame");
                Debug.LogWarning("Objects in List: " + this.m_Objects.Count + " / Sample Count: " + this.m_SampleCount + " = " + (this.m_Objects.Count/this.m_SampleCount) + " != " + this.m_ObjectsPerSample);
            }
        }

        this.m_IsWriting = true;
        this.m_SampleCount++;
        this.m_LastSampleTime = Time.realtimeSinceStartup;

        /*if( m_SampleCount  > 1 )
        {
            Debug.Log( "Check: " + m_Objects.Count + " / " + m_SampleCount + " = " + ( m_Objects.Count / m_SampleCount ) + " = " + m_ObjectsPerSample );
        }*/
    }

    /// <summary>
    /// Resets the PhotonStreamQueue. You need to do this whenever the amount of objects you are observing changes
    /// </summary>
    public void Reset()
    {
        this.m_SampleCount = 0;
        this.m_ObjectsPerSample = -1;

        this.m_LastSampleTime = -Mathf.Infinity;
        this.m_LastFrameCount = -1;

        this.m_Objects.Clear();
    }

    /// <summary>
    /// Adds the next object to the queue. This works just like PhotonStream.SendNext
    /// </summary>
    /// <param name="obj">The object you want to add to the queue</param>
    public void SendNext(object obj)
    {
        if (Time.frameCount != this.m_LastFrameCount)
        {
            BeginWritePackage();
        }

        this.m_LastFrameCount = Time.frameCount;

        if (this.m_IsWriting == false)
        {
            return;
        }

        this.m_Objects.Add(obj);
    }

    /// <summary>
    /// Determines whether the queue has stored any objects
    /// </summary>
    public bool HasQueuedObjects()
    {
        return this.m_NextObjectIndex != -1;
    }

    /// <summary>
    /// Receives the next object from the queue. This works just like PhotonStream.ReceiveNext
    /// </summary>
    /// <returns></returns>
    public object ReceiveNext()
    {
        if (this.m_NextObjectIndex == -1)
        {
            return null;
        }

        if (this.m_NextObjectIndex >= this.m_Objects.Count)
        {
            this.m_NextObjectIndex -= this.m_ObjectsPerSample;
        }

        return this.m_Objects[this.m_NextObjectIndex++];
    }

    /// <summary>
    /// Serializes the specified stream. Call this in your OnPhotonSerializeView method to send the whole recorded stream.
    /// </summary>
    /// <param name="stream">The PhotonStream you receive as a parameter in OnPhotonSerializeView</param>
    public void Serialize(PhotonStream stream)
    {
        // TODO: find a better solution for this:
        // the "if" is a workaround for packages which have only 1 sample/frame. in that case, SendNext didn't set the obj per sample.
        if (m_Objects.Count > 0 && this.m_ObjectsPerSample < 0)
        {
            this.m_ObjectsPerSample = m_Objects.Count;
        }

        stream.SendNext(this.m_SampleCount);
        stream.SendNext(this.m_ObjectsPerSample);

        for (int i = 0; i < this.m_Objects.Count; ++i)
        {
            stream.SendNext(this.m_Objects[i]);
        }

        //Debug.Log( "Serialize " + m_SampleCount + " samples with " + m_ObjectsPerSample + " objects per sample. object count: " + m_Objects.Count + " / " + ( m_SampleCount * m_ObjectsPerSample ) );

        this.m_Objects.Clear();
        this.m_SampleCount = 0;
    }

    /// <summary>
    /// Deserializes the specified stream. Call this in your OnPhotonSerializeView method to receive the whole recorded stream.
    /// </summary>
    /// <param name="stream">The PhotonStream you receive as a parameter in OnPhotonSerializeView</param>
    public void Deserialize(PhotonStream stream)
    {
        this.m_Objects.Clear();

        this.m_SampleCount = (int)stream.ReceiveNext();
        this.m_ObjectsPerSample = (int)stream.ReceiveNext();

        for (int i = 0; i < this.m_SampleCount*this.m_ObjectsPerSample; ++i)
        {
            this.m_Objects.Add(stream.ReceiveNext());
        }

        if (this.m_Objects.Count > 0)
        {
            this.m_NextObjectIndex = 0;
        }
        else
        {
            this.m_NextObjectIndex = -1;
        }

        //Debug.Log( "Deserialized " + m_SampleCount + " samples with " + m_ObjectsPerSample + " objects per sample. object count: " + m_Objects.Count + " / " + ( m_SampleCount * m_ObjectsPerSample ) );
    }
}
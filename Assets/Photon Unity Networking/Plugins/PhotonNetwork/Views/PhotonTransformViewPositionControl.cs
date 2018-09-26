// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformViewPositionControl.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize position via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhotonTransformViewPositionControl 
{
    PhotonTransformViewPositionModel m_Model;
    float m_CurrentSpeed;
    double m_LastSerializeTime;
    Vector3 m_SynchronizedSpeed = Vector3.zero;
    float m_SynchronizedTurnSpeed = 0;

    Vector3 m_NetworkPosition;
    Queue<Vector3> m_OldNetworkPositions = new Queue<Vector3>();

    bool m_UpdatedPositionAfterOnSerialize = true;

    public PhotonTransformViewPositionControl( PhotonTransformViewPositionModel model )
    {
        m_Model = model;
    }

    Vector3 GetOldestStoredNetworkPosition()
    {
        Vector3 oldPosition = m_NetworkPosition;

        if( m_OldNetworkPositions.Count > 0 )
        {
            oldPosition = m_OldNetworkPositions.Peek();
        }

        return oldPosition;
    }

    /// <summary>
    /// These values are synchronized to the remote objects if the interpolation mode
    /// or the extrapolation mode SynchronizeValues is used. Your movement script should pass on
    /// the current speed (in units/second) and turning speed (in angles/second) so the remote
    /// object can use them to predict the objects movement.
    /// </summary>
    /// <param name="speed">The current movement vector of the object in units/second.</param>
    /// <param name="turnSpeed">The current turn speed of the object in angles/second.</param>
    public void SetSynchronizedValues( Vector3 speed, float turnSpeed )
    {
        m_SynchronizedSpeed = speed;
        m_SynchronizedTurnSpeed = turnSpeed;
    }

    /// <summary>
    /// Calculates the new position based on the values setup in the inspector
    /// </summary>
    /// <param name="currentPosition">The current position.</param>
    /// <returns>The new position.</returns>
    public Vector3 UpdatePosition( Vector3 currentPosition )
    {
        Vector3 targetPosition = GetNetworkPosition() + GetExtrapolatedPositionOffset();

        switch( m_Model.InterpolateOption )
        {
            case PhotonTransformViewPositionModel.InterpolateOptions.Disabled:
                if( m_UpdatedPositionAfterOnSerialize == false )
                {
                    currentPosition = targetPosition;
                    m_UpdatedPositionAfterOnSerialize = true;
                }
                break;

            case PhotonTransformViewPositionModel.InterpolateOptions.FixedSpeed:
                currentPosition = Vector3.MoveTowards( currentPosition, targetPosition, Time.deltaTime * m_Model.InterpolateMoveTowardsSpeed );
                break;

            case PhotonTransformViewPositionModel.InterpolateOptions.EstimatedSpeed:
                if (m_OldNetworkPositions.Count == 0)
                {
                    // special case: we have no previous updates in memory, so we can't guess a speed!
                    break;
                }

                // knowing the last (incoming) position and the one before, we can guess a speed.
                // note that the speed is times sendRateOnSerialize! we send X updates/sec, so our estimate has to factor that in.
                float estimatedSpeed = (Vector3.Distance(m_NetworkPosition, GetOldestStoredNetworkPosition()) / m_OldNetworkPositions.Count) * PhotonNetwork.sendRateOnSerialize;
            
                // move towards the targetPosition (including estimates, if that's active) with the speed calculated from the last updates.
                currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, Time.deltaTime * estimatedSpeed );
                break;

            case PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues:
                if( m_SynchronizedSpeed.magnitude == 0 )
                {
                    currentPosition = targetPosition;
                }
                else
                {
                    currentPosition = Vector3.MoveTowards( currentPosition, targetPosition, Time.deltaTime * m_SynchronizedSpeed.magnitude );
                }
                break;

            case PhotonTransformViewPositionModel.InterpolateOptions.Lerp:
                currentPosition = Vector3.Lerp( currentPosition, targetPosition, Time.deltaTime * m_Model.InterpolateLerpSpeed );
                break;

            /*case PhotonTransformViewPositionModel.InterpolateOptions.MoveTowardsComplex:
                float distanceToTarget = Vector3.Distance( currentPosition, targetPosition );
                float targetSpeed = m_Model.InterpolateSpeedCurve.Evaluate( distanceToTarget ) * m_Model.InterpolateMoveTowardsSpeed;

                if( targetSpeed > m_CurrentSpeed )
                {
                    m_CurrentSpeed = Mathf.MoveTowards( m_CurrentSpeed, targetSpeed, Time.deltaTime * m_Model.InterpolateMoveTowardsAcceleration );
                }
                else
                {
                    m_CurrentSpeed = Mathf.MoveTowards( m_CurrentSpeed, targetSpeed, Time.deltaTime * m_Model.InterpolateMoveTowardsDeceleration );
                }

                //Debug.Log( m_CurrentSpeed + " - " + targetSpeed + " - " + transform.localPosition + " - " + targetPosition );

                currentPosition = Vector3.MoveTowards( currentPosition, targetPosition, Time.deltaTime * m_CurrentSpeed );
                break;*/
        }

        if( m_Model.TeleportEnabled == true )
        {
            if( Vector3.Distance( currentPosition, GetNetworkPosition() ) > m_Model.TeleportIfDistanceGreaterThan )
            {
                currentPosition = GetNetworkPosition();
            }
        }

        return currentPosition;
    }

    /// <summary>
    /// Gets the last position that was received through the network
    /// </summary>
    /// <returns></returns>
    public Vector3 GetNetworkPosition()
    {
        return m_NetworkPosition;
    }

    /// <summary>
    /// Calculates an estimated position based on the last synchronized position,
    /// the time when the last position was received and the movement speed of the object
    /// </summary>
    /// <returns>Estimated position of the remote object</returns>
    public Vector3 GetExtrapolatedPositionOffset()
    {
        float timePassed = (float)( PhotonNetwork.time - m_LastSerializeTime );

        if( m_Model.ExtrapolateIncludingRoundTripTime == true )
        {
            timePassed += (float)PhotonNetwork.GetPing() / 1000f;
        }

        Vector3 extrapolatePosition = Vector3.zero;

        switch( m_Model.ExtrapolateOption )
        {
        case PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues:
            Quaternion turnRotation = Quaternion.Euler( 0, m_SynchronizedTurnSpeed * timePassed, 0 );
            extrapolatePosition = turnRotation * ( m_SynchronizedSpeed * timePassed );
            break;
        case PhotonTransformViewPositionModel.ExtrapolateOptions.FixedSpeed:
            Vector3 moveDirection = ( m_NetworkPosition - GetOldestStoredNetworkPosition() ).normalized;

            extrapolatePosition = moveDirection * m_Model.ExtrapolateSpeed * timePassed;
            break;
        case PhotonTransformViewPositionModel.ExtrapolateOptions.EstimateSpeedAndTurn:
            Vector3 moveDelta = ( m_NetworkPosition - GetOldestStoredNetworkPosition() ) * PhotonNetwork.sendRateOnSerialize;
            extrapolatePosition = moveDelta * timePassed;
            break;
        }

        return extrapolatePosition;
    }

    public void OnPhotonSerializeView( Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info )
    {
        if( m_Model.SynchronizeEnabled == false )
        {
            return;
        }

        if( stream.isWriting == true )
        {
            SerializeData( currentPosition, stream, info );
        }
        else
        {
            DeserializeData( stream, info );
        }

        m_LastSerializeTime = PhotonNetwork.time;
        m_UpdatedPositionAfterOnSerialize = false;
    }

    void SerializeData( Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info )
    {
        stream.SendNext( currentPosition );
        m_NetworkPosition = currentPosition;

        if( m_Model.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues ||
            m_Model.InterpolateOption == PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues )
        {
            stream.SendNext( m_SynchronizedSpeed );
            stream.SendNext( m_SynchronizedTurnSpeed );
        }
    }

    void DeserializeData( PhotonStream stream, PhotonMessageInfo info )
    {
        Vector3 readPosition = (Vector3)stream.ReceiveNext();
        if( m_Model.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues ||
            m_Model.InterpolateOption == PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues )
        {
            m_SynchronizedSpeed = (Vector3)stream.ReceiveNext();
            m_SynchronizedTurnSpeed = (float)stream.ReceiveNext();
        }

        if (m_OldNetworkPositions.Count == 0)
        {
            // if we don't have old positions yet, this is the very first update this client reads. let's use this as current AND old position.
            m_NetworkPosition = readPosition;
        }

        // the previously received position becomes the old(er) one and queued. the new one is the m_NetworkPosition
        m_OldNetworkPositions.Enqueue( m_NetworkPosition );
        m_NetworkPosition = readPosition;

        // reduce items in queue to defined number of stored positions.
        while( m_OldNetworkPositions.Count > m_Model.ExtrapolateNumberOfStoredPositions )
        {
            m_OldNetworkPositions.Dequeue();
        }
    }
}

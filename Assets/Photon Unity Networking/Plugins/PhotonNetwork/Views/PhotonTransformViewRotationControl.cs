// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformViewRotationControl.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize rotations via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class PhotonTransformViewRotationControl 
{
    PhotonTransformViewRotationModel m_Model;
    Quaternion m_NetworkRotation;

    public PhotonTransformViewRotationControl( PhotonTransformViewRotationModel model )
    {
        m_Model = model;
    }

	/// <summary>
	/// Gets the last rotation that was received through the network
	/// </summary>
	/// <returns></returns>
	public Quaternion GetNetworkRotation()
	{
		return m_NetworkRotation;
	}

    public Quaternion GetRotation( Quaternion currentRotation )
    {
        switch( m_Model.InterpolateOption )
        {
        default:
        case PhotonTransformViewRotationModel.InterpolateOptions.Disabled:
            return m_NetworkRotation;
        case PhotonTransformViewRotationModel.InterpolateOptions.RotateTowards:
            return Quaternion.RotateTowards( currentRotation, m_NetworkRotation, m_Model.InterpolateRotateTowardsSpeed * Time.deltaTime );
        case PhotonTransformViewRotationModel.InterpolateOptions.Lerp:
            return Quaternion.Lerp( currentRotation, m_NetworkRotation, m_Model.InterpolateLerpSpeed * Time.deltaTime );
        }
    }

    public void OnPhotonSerializeView( Quaternion currentRotation, PhotonStream stream, PhotonMessageInfo info )
    {
        if( m_Model.SynchronizeEnabled == false )
        {
            return;
        }

        if( stream.isWriting == true )
        {
            stream.SendNext( currentRotation );
            m_NetworkRotation = currentRotation;
        }
        else
        {
            m_NetworkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
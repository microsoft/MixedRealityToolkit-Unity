// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformViewScaleControl.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize scale via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class PhotonTransformViewScaleControl 
{
    PhotonTransformViewScaleModel m_Model;
    Vector3 m_NetworkScale = Vector3.one;

    public PhotonTransformViewScaleControl( PhotonTransformViewScaleModel model )
    {
        m_Model = model;
    }

	/// <summary>
	/// Gets the last scale that was received through the network
	/// </summary>
	/// <returns></returns>
	public Vector3 GetNetworkScale()
	{
		return m_NetworkScale;
	}

    public Vector3 GetScale( Vector3 currentScale )
    {
        switch( m_Model.InterpolateOption )
        {
        default:
        case PhotonTransformViewScaleModel.InterpolateOptions.Disabled:
            return m_NetworkScale;
        case PhotonTransformViewScaleModel.InterpolateOptions.MoveTowards:
            return Vector3.MoveTowards( currentScale, m_NetworkScale, m_Model.InterpolateMoveTowardsSpeed * Time.deltaTime );
        case PhotonTransformViewScaleModel.InterpolateOptions.Lerp:
            return Vector3.Lerp( currentScale, m_NetworkScale, m_Model.InterpolateLerpSpeed * Time.deltaTime );
        }
    }

    public void OnPhotonSerializeView( Vector3 currentScale, PhotonStream stream, PhotonMessageInfo info )
    {
        if( m_Model.SynchronizeEnabled == false )
        {
            return;
        }

        if( stream.isWriting == true )
        {
            stream.SendNext( currentScale );
            m_NetworkScale = currentScale;
        }
        else
        {
            m_NetworkScale = (Vector3)stream.ReceiveNext();
        }
    }
}

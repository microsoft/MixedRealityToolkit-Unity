// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformViewScaleModel.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   Model to synchronize scale via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[System.Serializable]
public class PhotonTransformViewScaleModel 
{
    public enum InterpolateOptions
    {
        Disabled,
        MoveTowards,
        Lerp,
    }

    public bool SynchronizeEnabled;

    public InterpolateOptions InterpolateOption = InterpolateOptions.Disabled;
    public float InterpolateMoveTowardsSpeed = 1f;
    public float InterpolateLerpSpeed;
}

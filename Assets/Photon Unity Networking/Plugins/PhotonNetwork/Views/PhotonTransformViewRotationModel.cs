// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformViewRotationModel.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   Model class to synchronize rotations via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[System.Serializable]
public class PhotonTransformViewRotationModel 
{
    public enum InterpolateOptions
    {
        Disabled,
        RotateTowards,
        Lerp,
    }

    public bool SynchronizeEnabled;

    public InterpolateOptions InterpolateOption = InterpolateOptions.RotateTowards;
    public float InterpolateRotateTowardsSpeed = 180;
    public float InterpolateLerpSpeed = 5;
}

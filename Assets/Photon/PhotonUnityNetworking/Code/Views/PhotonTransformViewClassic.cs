// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformViewClassic.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using UnityEngine;
    using System.Collections.Generic;


    /// <summary>
    /// This class helps you to synchronize position, rotation and scale
    /// of a GameObject. It also gives you many different options to make
    /// the synchronized values appear smooth, even when the data is only
    /// send a couple of times per second.
    /// Simply add the component to your GameObject and make sure that
    /// the PhotonTransformViewClassic is added to the list of observed components
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    [AddComponentMenu("Photon Networking/Photon Transform View Classic")]
    public class PhotonTransformViewClassic : MonoBehaviour, IPunObservable
    {
        //As this component is very complex, we separated it into multiple classes.
        //The PositionModel, RotationModel and ScaleMode store the data you are able to
        //configure in the inspector while the "control" objects below are actually moving
        //the object and calculating all the inter- and extrapolation

        [SerializeField]
        public PhotonTransformViewPositionModel m_PositionModel = new PhotonTransformViewPositionModel();

        [SerializeField]
        public PhotonTransformViewRotationModel m_RotationModel = new PhotonTransformViewRotationModel();

        [SerializeField]
        public PhotonTransformViewScaleModel m_ScaleModel = new PhotonTransformViewScaleModel();

        PhotonTransformViewPositionControl m_PositionControl;
        PhotonTransformViewRotationControl m_RotationControl;
        PhotonTransformViewScaleControl m_ScaleControl;

        PhotonView m_PhotonView;

        bool m_ReceivedNetworkUpdate = false;

        /// <summary>
        /// Flag to skip initial data when Object is instantiated and rely on the first deserialized data instead.
        /// </summary>
        bool m_firstTake = false;

        void Awake()
        {
            this.m_PhotonView = GetComponent<PhotonView>();

            this.m_PositionControl = new PhotonTransformViewPositionControl(this.m_PositionModel);
            this.m_RotationControl = new PhotonTransformViewRotationControl(this.m_RotationModel);
            this.m_ScaleControl = new PhotonTransformViewScaleControl(this.m_ScaleModel);
        }

        void OnEnable()
        {
            m_firstTake = true;
        }

        void Update()
        {
            if (this.m_PhotonView == null || this.m_PhotonView.IsMine == true || PhotonNetwork.IsConnectedAndReady == false)
            {
                return;
            }

            this.UpdatePosition();
            this.UpdateRotation();
            this.UpdateScale();
        }

        void UpdatePosition()
        {
            if (this.m_PositionModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false)
            {
                return;
            }

            transform.localPosition = this.m_PositionControl.UpdatePosition(transform.localPosition);
        }

        void UpdateRotation()
        {
            if (this.m_RotationModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false)
            {
                return;
            }

            transform.localRotation = this.m_RotationControl.GetRotation(transform.localRotation);
        }

        void UpdateScale()
        {
            if (this.m_ScaleModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false)
            {
                return;
            }

            transform.localScale = this.m_ScaleControl.GetScale(transform.localScale);
        }

        /// <summary>
        /// These values are synchronized to the remote objects if the interpolation mode
        /// or the extrapolation mode SynchronizeValues is used. Your movement script should pass on
        /// the current speed (in units/second) and turning speed (in angles/second) so the remote
        /// object can use them to predict the objects movement.
        /// </summary>
        /// <param name="speed">The current movement vector of the object in units/second.</param>
        /// <param name="turnSpeed">The current turn speed of the object in angles/second.</param>
        public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
        {
            this.m_PositionControl.SetSynchronizedValues(speed, turnSpeed);
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            this.m_PositionControl.OnPhotonSerializeView(transform.localPosition, stream, info);
            this.m_RotationControl.OnPhotonSerializeView(transform.localRotation, stream, info);
            this.m_ScaleControl.OnPhotonSerializeView(transform.localScale, stream, info);

            if (stream.IsReading == true)
            {
                this.m_ReceivedNetworkUpdate = true;

                // force latest data to avoid initial drifts when player is instantiated.
                if (m_firstTake)
                {
                    m_firstTake = false;

                    if (this.m_PositionModel.SynchronizeEnabled)
                    {
                        this.transform.localPosition = this.m_PositionControl.GetNetworkPosition();
                    }

                    if (this.m_RotationModel.SynchronizeEnabled)
                    {
                        this.transform.localRotation = this.m_RotationControl.GetNetworkRotation();
                    }

                    if (this.m_ScaleModel.SynchronizeEnabled)
                    {
                        this.transform.localScale = this.m_ScaleControl.GetNetworkScale();
                    }
                }
            }
        }
    }


    [System.Serializable]
    public class PhotonTransformViewPositionModel
    {
        public enum InterpolateOptions
        {
            Disabled,
            FixedSpeed,
            EstimatedSpeed,
            SynchronizeValues,
            Lerp
        }


        public enum ExtrapolateOptions
        {
            Disabled,
            SynchronizeValues,
            EstimateSpeedAndTurn,
            FixedSpeed,
        }


        public bool SynchronizeEnabled;

        public bool TeleportEnabled = true;
        public float TeleportIfDistanceGreaterThan = 3f;

        public InterpolateOptions InterpolateOption = InterpolateOptions.EstimatedSpeed;
        public float InterpolateMoveTowardsSpeed = 1f;

        public float InterpolateLerpSpeed = 1f;

        public ExtrapolateOptions ExtrapolateOption = ExtrapolateOptions.Disabled;
        public float ExtrapolateSpeed = 1f;
        public bool ExtrapolateIncludingRoundTripTime = true;
        public int ExtrapolateNumberOfStoredPositions = 1;
    }

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

        public PhotonTransformViewPositionControl(PhotonTransformViewPositionModel model)
        {
            m_Model = model;
        }

        Vector3 GetOldestStoredNetworkPosition()
        {
            Vector3 oldPosition = m_NetworkPosition;

            if (m_OldNetworkPositions.Count > 0)
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
        public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
        {
            m_SynchronizedSpeed = speed;
            m_SynchronizedTurnSpeed = turnSpeed;
        }

        /// <summary>
        /// Calculates the new position based on the values setup in the inspector
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        /// <returns>The new position.</returns>
        public Vector3 UpdatePosition(Vector3 currentPosition)
        {
            Vector3 targetPosition = GetNetworkPosition() + GetExtrapolatedPositionOffset();

            switch (m_Model.InterpolateOption)
            {
                case PhotonTransformViewPositionModel.InterpolateOptions.Disabled:
                    if (m_UpdatedPositionAfterOnSerialize == false)
                    {
                        currentPosition = targetPosition;
                        m_UpdatedPositionAfterOnSerialize = true;
                    }

                    break;

                case PhotonTransformViewPositionModel.InterpolateOptions.FixedSpeed:
                    currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, Time.deltaTime * m_Model.InterpolateMoveTowardsSpeed);
                    break;

                case PhotonTransformViewPositionModel.InterpolateOptions.EstimatedSpeed:
                    if (m_OldNetworkPositions.Count == 0)
                    {
                        // special case: we have no previous updates in memory, so we can't guess a speed!
                        break;
                    }

                    // knowing the last (incoming) position and the one before, we can guess a speed.
                    // note that the speed is times sendRateOnSerialize! we send X updates/sec, so our estimate has to factor that in.
                    float estimatedSpeed = (Vector3.Distance(m_NetworkPosition, GetOldestStoredNetworkPosition()) / m_OldNetworkPositions.Count) * PhotonNetwork.SerializationRate;

                    // move towards the targetPosition (including estimates, if that's active) with the speed calculated from the last updates.
                    currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, Time.deltaTime * estimatedSpeed);
                    break;

                case PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues:
                    if (m_SynchronizedSpeed.magnitude == 0)
                    {
                        currentPosition = targetPosition;
                    }
                    else
                    {
                        currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, Time.deltaTime * m_SynchronizedSpeed.magnitude);
                    }

                    break;

                case PhotonTransformViewPositionModel.InterpolateOptions.Lerp:
                    currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * m_Model.InterpolateLerpSpeed);
                    break;
            }

            if (m_Model.TeleportEnabled == true)
            {
                if (Vector3.Distance(currentPosition, GetNetworkPosition()) > m_Model.TeleportIfDistanceGreaterThan)
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
            float timePassed = (float)(PhotonNetwork.Time - m_LastSerializeTime);

            if (m_Model.ExtrapolateIncludingRoundTripTime == true)
            {
                timePassed += (float)PhotonNetwork.GetPing() / 1000f;
            }

            Vector3 extrapolatePosition = Vector3.zero;

            switch (m_Model.ExtrapolateOption)
            {
                case PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues:
                    Quaternion turnRotation = Quaternion.Euler(0, m_SynchronizedTurnSpeed * timePassed, 0);
                    extrapolatePosition = turnRotation * (m_SynchronizedSpeed * timePassed);
                    break;
                case PhotonTransformViewPositionModel.ExtrapolateOptions.FixedSpeed:
                    Vector3 moveDirection = (m_NetworkPosition - GetOldestStoredNetworkPosition()).normalized;

                    extrapolatePosition = moveDirection * m_Model.ExtrapolateSpeed * timePassed;
                    break;
                case PhotonTransformViewPositionModel.ExtrapolateOptions.EstimateSpeedAndTurn:
                    Vector3 moveDelta = (m_NetworkPosition - GetOldestStoredNetworkPosition()) * PhotonNetwork.SerializationRate;
                    extrapolatePosition = moveDelta * timePassed;
                    break;
            }

            return extrapolatePosition;
        }

        public void OnPhotonSerializeView(Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info)
        {
            if (m_Model.SynchronizeEnabled == false)
            {
                return;
            }

            if (stream.IsWriting == true)
            {
                SerializeData(currentPosition, stream, info);
            }
            else
            {
                DeserializeData(stream, info);
            }

            m_LastSerializeTime = PhotonNetwork.Time;
            m_UpdatedPositionAfterOnSerialize = false;
        }

        void SerializeData(Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info)
        {
            stream.SendNext(currentPosition);
            m_NetworkPosition = currentPosition;

            if (m_Model.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues ||
                m_Model.InterpolateOption == PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues)
            {
                stream.SendNext(m_SynchronizedSpeed);
                stream.SendNext(m_SynchronizedTurnSpeed);
            }
        }

        void DeserializeData(PhotonStream stream, PhotonMessageInfo info)
        {
            Vector3 readPosition = (Vector3)stream.ReceiveNext();
            if (m_Model.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues ||
                m_Model.InterpolateOption == PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues)
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
            m_OldNetworkPositions.Enqueue(m_NetworkPosition);
            m_NetworkPosition = readPosition;

            // reduce items in queue to defined number of stored positions.
            while (m_OldNetworkPositions.Count > m_Model.ExtrapolateNumberOfStoredPositions)
            {
                m_OldNetworkPositions.Dequeue();
            }
        }
    }


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

    public class PhotonTransformViewRotationControl
    {
        PhotonTransformViewRotationModel m_Model;
        Quaternion m_NetworkRotation;

        public PhotonTransformViewRotationControl(PhotonTransformViewRotationModel model)
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

        public Quaternion GetRotation(Quaternion currentRotation)
        {
            switch (m_Model.InterpolateOption)
            {
                default:
                case PhotonTransformViewRotationModel.InterpolateOptions.Disabled:
                    return m_NetworkRotation;
                case PhotonTransformViewRotationModel.InterpolateOptions.RotateTowards:
                    return Quaternion.RotateTowards(currentRotation, m_NetworkRotation, m_Model.InterpolateRotateTowardsSpeed * Time.deltaTime);
                case PhotonTransformViewRotationModel.InterpolateOptions.Lerp:
                    return Quaternion.Lerp(currentRotation, m_NetworkRotation, m_Model.InterpolateLerpSpeed * Time.deltaTime);
            }
        }

        public void OnPhotonSerializeView(Quaternion currentRotation, PhotonStream stream, PhotonMessageInfo info)
        {
            if (m_Model.SynchronizeEnabled == false)
            {
                return;
            }

            if (stream.IsWriting == true)
            {
                stream.SendNext(currentRotation);
                m_NetworkRotation = currentRotation;
            }
            else
            {
                m_NetworkRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }


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

    public class PhotonTransformViewScaleControl
    {
        PhotonTransformViewScaleModel m_Model;
        Vector3 m_NetworkScale = Vector3.one;

        public PhotonTransformViewScaleControl(PhotonTransformViewScaleModel model)
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

        public Vector3 GetScale(Vector3 currentScale)
        {
            switch (m_Model.InterpolateOption)
            {
                default:
                case PhotonTransformViewScaleModel.InterpolateOptions.Disabled:
                    return m_NetworkScale;
                case PhotonTransformViewScaleModel.InterpolateOptions.MoveTowards:
                    return Vector3.MoveTowards(currentScale, m_NetworkScale, m_Model.InterpolateMoveTowardsSpeed * Time.deltaTime);
                case PhotonTransformViewScaleModel.InterpolateOptions.Lerp:
                    return Vector3.Lerp(currentScale, m_NetworkScale, m_Model.InterpolateLerpSpeed * Time.deltaTime);
            }
        }

        public void OnPhotonSerializeView(Vector3 currentScale, PhotonStream stream, PhotonMessageInfo info)
        {
            if (m_Model.SynchronizeEnabled == false)
            {
                return;
            }

            if (stream.IsWriting == true)
            {
                stream.SendNext(currentScale);
                m_NetworkScale = currentScale;
            }
            else
            {
                m_NetworkScale = (Vector3)stream.ReceiveNext();
            }
        }
    }
}
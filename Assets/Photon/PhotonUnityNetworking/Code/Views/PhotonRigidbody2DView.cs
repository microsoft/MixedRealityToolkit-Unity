// ----------------------------------------------------------------------------
// <copyright file="PhotonRigidbody2DView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize 2d rigidbodies via PUN.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using UnityEngine;


    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Rigidbody2D))]
    [AddComponentMenu("Photon Networking/Photon Rigidbody 2D View")]
    public class PhotonRigidbody2DView : MonoBehaviour, IPunObservable
    {
        private float m_Distance;
        private float m_Angle;

        private Rigidbody2D m_Body;

        private PhotonView m_PhotonView;

        private Vector2 m_NetworkPosition;

        private float m_NetworkRotation;

        public bool m_SynchronizeVelocity = true;
        public bool m_SynchronizeAngularVelocity = false;

        public bool m_TeleportEnabled = false;
        public float m_TeleportIfDistanceGreaterThan = 3.0f;

        public void Awake()
        {
            this.m_Body = GetComponent<Rigidbody2D>();
            this.m_PhotonView = GetComponent<PhotonView>();

            this.m_NetworkPosition = new Vector2();
        }

        public void FixedUpdate()
        {
            if (!this.m_PhotonView.IsMine)
            {
                this.m_Body.position = Vector2.MoveTowards(this.m_Body.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                this.m_Body.rotation = Mathf.MoveTowards(this.m_Body.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(this.m_Body.position);
                stream.SendNext(this.m_Body.rotation);

                if (this.m_SynchronizeVelocity)
                {
                    stream.SendNext(this.m_Body.velocity);
                }

                if (this.m_SynchronizeAngularVelocity)
                {
                    stream.SendNext(this.m_Body.angularVelocity);
                }
            }
            else
            {
                this.m_NetworkPosition = (Vector2)stream.ReceiveNext();
                this.m_NetworkRotation = (float)stream.ReceiveNext();

                if (this.m_TeleportEnabled)
                {
                    if (Vector3.Distance(this.m_Body.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
                    {
                        this.m_Body.position = this.m_NetworkPosition;
                    }
                }

                if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));

                    if (m_SynchronizeVelocity)
                    {
                        this.m_Body.velocity = (Vector2)stream.ReceiveNext();

                        this.m_NetworkPosition += this.m_Body.velocity * lag;

                        this.m_Distance = Vector2.Distance(this.m_Body.position, this.m_NetworkPosition);
                    }

                    if (this.m_SynchronizeAngularVelocity)
                    {
                        this.m_Body.angularVelocity = (float)stream.ReceiveNext();

                        this.m_NetworkRotation += this.m_Body.angularVelocity * lag;

                        this.m_Angle = Mathf.Abs(this.m_Body.rotation - this.m_NetworkRotation);
                    }
                }
            }
        }
    }
}
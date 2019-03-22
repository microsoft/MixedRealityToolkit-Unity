// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Asteroid.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo
// </copyright>
// <summary>
//  Asteroid Component
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

using Random = UnityEngine.Random;
using Photon.Pun.UtilityScripts;

namespace Photon.Pun.Demo.Asteroids
{
    public class Asteroid : MonoBehaviour
    {
        public bool isLargeAsteroid;

        private bool isDestroyed;

        private PhotonView photonView;

        private new Rigidbody rigidbody;

        #region UNITY

        public void Awake()
        {
            photonView = GetComponent<PhotonView>();

            rigidbody = GetComponent<Rigidbody>();

            if (photonView.InstantiationData != null)
            {
                rigidbody.AddForce((Vector3) photonView.InstantiationData[0]);
                rigidbody.AddTorque((Vector3) photonView.InstantiationData[1]);

                isLargeAsteroid = (bool) photonView.InstantiationData[2];
            }
        }

        public void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (Mathf.Abs(transform.position.x) > Camera.main.orthographicSize * Camera.main.aspect || Mathf.Abs(transform.position.z) > Camera.main.orthographicSize)
            {
                // Out of the screen
                PhotonNetwork.Destroy(gameObject);
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (isDestroyed)
            {
                return;
            }

            if (collision.gameObject.CompareTag("Bullet"))
            {
                if (photonView.IsMine)
                {
                    Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                    bullet.Owner.AddScore(isLargeAsteroid ? 2 : 1);

                    DestroyAsteroidGlobally();
                }
                else
                {
                    DestroyAsteroidLocally();
                }
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                if (photonView.IsMine)
                {
                    collision.gameObject.GetComponent<PhotonView>().RPC("DestroySpaceship", RpcTarget.All);

                    DestroyAsteroidGlobally();
                }
            }
        }

        #endregion

        private void DestroyAsteroidGlobally()
        {
            isDestroyed = true;

            if (isLargeAsteroid)
            {
                int numberToSpawn = Random.Range(3, 6);

                for (int counter = 0; counter < numberToSpawn; ++counter)
                {
                    Vector3 force = Quaternion.Euler(0, counter * 360.0f / numberToSpawn, 0) * Vector3.forward * Random.Range(0.5f, 1.5f) * 300.0f;
                    Vector3 torque = Random.insideUnitSphere * Random.Range(500.0f, 1500.0f);
                    object[] instantiationData = {force, torque, false, PhotonNetwork.Time};

                    PhotonNetwork.InstantiateSceneObject("SmallAsteroid", transform.position + force.normalized * 10.0f, Quaternion.Euler(0, Random.value * 180.0f, 0), 0, instantiationData);
                }
            }

            PhotonNetwork.Destroy(gameObject);
        }

        private void DestroyAsteroidLocally()
        {
            isDestroyed = true;

            GetComponent<Renderer>().enabled = false;
        }
    }
}
using UnityEngine;
using UnityEngine.XR.iOS;

namespace ARKit.Examples
{
    public class PointCloudParticleExample : MonoBehaviour
    {
        public ParticleSystem pointCloudParticlePrefab;
        public int maxPointsToShow;
        public float particleSize = 1.0f;
        private Vector3[] m_PointCloudData;
        private bool frameUpdated = false;
        private ParticleSystem currentPS;
        private ParticleSystem.Particle[] particles;

        private void Start()
        {
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
            currentPS = Instantiate(pointCloudParticlePrefab);
            frameUpdated = false;
        }

        public void ARFrameUpdated(UnityARCamera camera)
        {
            m_PointCloudData = camera.pointCloudData;
            frameUpdated = true;
        }

        private void Update()
        {
            if (frameUpdated)
            {
                if (m_PointCloudData != null && m_PointCloudData.Length > 0)
                {
                    int numParticles = Mathf.Min(m_PointCloudData.Length, maxPointsToShow);
                    var newParticles = new ParticleSystem.Particle[numParticles];
                    for (var i = 0; i < m_PointCloudData.Length; i++)
                    {
                        Vector3 currentPoint = m_PointCloudData[i];
                        newParticles[i].position = currentPoint;
                        newParticles[i].startColor = new Color(1.0f, 1.0f, 1.0f);
                        newParticles[i].startSize = particleSize;
                    }

                    currentPS.SetParticles(newParticles, numParticles);
                }
                else
                {
                    var newParticles = new ParticleSystem.Particle[1];
                    newParticles[0].startSize = 0.0f;
                    currentPS.SetParticles(newParticles, 1);
                }

                frameUpdated = false;
            }
        }
    }
}

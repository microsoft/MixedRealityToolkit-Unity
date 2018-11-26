using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class PointCloudParticleExample : MonoBehaviour 
{
    public ParticleSystem pointCloudParticlePrefab;
    public int maxPointsToShow;
    public float particleSize = 1.0f;
    private Vector3[] m_PointCloudData;
#if UNITY_IOS || UNITY_EDITOR
    private bool frameUpdated = false;
#endif
    private ParticleSystem currentPS;
    private ParticleSystem.Particle [] particles;

    // Use this for initialization
    void Start () 
    {
#if UNITY_IOS || UNITY_EDITOR
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
        currentPS = Instantiate (pointCloudParticlePrefab);
        frameUpdated = false;
#endif
    }
    
#if UNITY_IOS || UNITY_EDITOR
    public void ARFrameUpdated(UnityARCamera camera)
    {
        m_PointCloudData = camera.pointCloudData;
        frameUpdated = true;
    }
#endif

    // Update is called once per frame
    void Update () 
    {
#if UNITY_IOS || UNITY_EDITOR
        if (frameUpdated) 
        {
            if (m_PointCloudData != null && m_PointCloudData.Length > 0) 
            {
                int numParticles = Mathf.Min (m_PointCloudData.Length, maxPointsToShow);
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
                int index = 0;
                foreach (Vector3 currentPoint in m_PointCloudData) 
                {     
                    particles [index].position = currentPoint;
                    particles [index].startColor = new Color (1.0f, 1.0f, 1.0f);
                    particles [index].startSize = particleSize;
                    index++;
                }
                currentPS.SetParticles (particles, numParticles);
            } 
            else
            {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];
                particles [0].startSize = 0.0f;
                currentPS.SetParticles (particles, 1);
            }
            frameUpdated = false;
        }
#endif
    }
}

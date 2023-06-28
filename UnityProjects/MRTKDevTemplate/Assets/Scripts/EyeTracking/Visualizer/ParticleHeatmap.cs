// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu("Scripts/MRTK/Examples/ParticleHeatmap")]
    public class ParticleHeatmap : MonoBehaviour
    {
        [SerializeField]
        private int maxNumberOfParticles = 100;

        [SerializeField]
        private float minParticleSize = 0.01f;

        [SerializeField]
        private float maxParticleSize = 0.5f;

        private int particleDecalDataIndex;
        private ParticleSystem particleSys;
        private ParticleSystem.EmissionModule emissionModule;
        private List<ParticleHeatmapParticleData> particleData;

        private void Start()
        {
            // Initialize particle data handlers
            particleSys = GetComponent<ParticleSystem>();
            emissionModule = particleSys.emission;
            particleData = new List<ParticleHeatmapParticleData>();
        }

        public void SetParticle(Vector3 pos)
        {
            if (particleDecalDataIndex >= maxNumberOfParticles)
            {
                particleDecalDataIndex = 0;
            }

            ParticleHeatmapParticleData newParticle = new ParticleHeatmapParticleData();
            newParticle.position = pos;
            newParticle.radiusInMeter = Random.Range(minParticleSize, maxParticleSize);

            if (particleDecalDataIndex >= particleData.Count)
            {
                particleData.Add(newParticle);
            }
            else
            {
                particleData[particleDecalDataIndex] = newParticle;
            }

            particleDecalDataIndex++;
        }

        public void DisplayParticles()
        {
            ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[particleData.Count];
            for (int i = 0; i < particleData.Count; i++)
            {
                particleArray[i].position = particleData[i].position;
                particleArray[i].startColor = particleData[i].color;
                particleArray[i].startSize = particleData[i].radiusInMeter;
            }

            particleSys.SetParticles(particleArray, particleArray.Length);
        }

        public void ShowHeatmap()
        {
            emissionModule.enabled = true;
        }

        public void HideHeatmap()
        {
            emissionModule.enabled = false;
        }
    }
}

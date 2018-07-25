// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    public class LineParticles : LineRendererBase
    {
        private const int GlobalMaxParticles = 2048;
        private const float GlobalParticleStartLifetime = 0.5f;

        [Header("Particle Settings")]
        [SerializeField]
        private Material lineMaterial;

        [Range(128, GlobalMaxParticles)]
        [SerializeField]
        private int maxParticles = GlobalMaxParticles;
        
        [Header("Noise settings")]
        [SerializeField]
        private bool particleNoiseOnDisabled = true;
        [SerializeField]
        private Vector3 noiseStrength = Vector3.one;
        [SerializeField]
        private float noiseFrequency = 1.2f;
        [Range(1, 10)]
        [SerializeField]
        private int noiseOcatives = 3;
        [Range(-10f, 10f)]
        [SerializeField]
        private float noiseSpeed = 1f;
        [Range(0.01f, 0.5f)]
        [SerializeField]
        private float lifetimeAfterDisabled = 0.25f;
        [SerializeField]
        private Gradient decayGradient = new Gradient();

        [SerializeField]
        private ParticleSystem particles;
        private ParticleSystem.Particle[] mainParticleArray = new ParticleSystem.Particle[GlobalMaxParticles];
        private ParticleSystemRenderer mainParticleRenderer;
        private ParticleSystem.NoiseModule mainNoise;
        private float decayStartTime = 0f;

        protected void OnEnable()
        {
            particles = gameObject.EnsureComponent<ParticleSystem>();

            mainNoise = particles.noise;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
            emission.rateOverDistance = new ParticleSystem.MinMaxCurve(0);
            emission.enabled = true;

            ParticleSystem.MainModule main = particles.main;
            main.loop = false;
            main.playOnAwake = false;
            main.maxParticles = Mathf.Min(maxParticles, GlobalMaxParticles);
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.enabled = false;

            mainParticleRenderer = particles.GetComponent<ParticleSystemRenderer>();
            mainParticleRenderer.sharedMaterial = lineMaterial;

            // Initialize our particles
            for (int i = 0; i < mainParticleArray.Length; i++)
            {
                ParticleSystem.Particle particle = mainParticleArray[i];
                particle.startColor = Color.white;
                particle.startSize = 1f;
                particle.startLifetime = float.MaxValue;
                particle.remainingLifetime = float.MaxValue;
                particle.velocity = Vector3.zero;
                particle.angularVelocity = 0;
                mainParticleArray[i] = particle;
            }
        }

        private void OnDisable()
        {
            if (mainParticleRenderer != null)
            {
                mainParticleRenderer.enabled = false;
            }
        }

        private void Update()
        {
            if (!Source.enabled)
            {
                mainNoise.enabled = particleNoiseOnDisabled;
                mainNoise.strengthX = noiseStrength.x;
                mainNoise.strengthY = noiseStrength.y;
                mainNoise.strengthZ = noiseStrength.z;
                mainNoise.octaveCount = noiseOcatives;
                mainNoise.scrollSpeed = noiseSpeed;
                mainNoise.frequency = noiseFrequency;

                if (decayStartTime < 0)
                {
                    decayStartTime = Time.unscaledTime;
                }
            }
            else
            {
                mainNoise.enabled = false;
                decayStartTime = -1;
            }

            if (Source.enabled)
            {
                for (int i = 0; i < LineStepCount; i++)
                {
                    float normalizedDistance = (1f / (LineStepCount - 1)) * i;
                    ParticleSystem.Particle particle = mainParticleArray[i];
                    particle.position = Source.GetPoint(normalizedDistance);
                    particle.startColor = GetColor(normalizedDistance);
                    particle.startSize = GetWidth(normalizedDistance);
                    mainParticleArray[i] = particle;
                }
            }
            else
            {
                int numDecayingParticles = particles.GetParticles(mainParticleArray);

                for (int i = 0; i < numDecayingParticles; i++)
                {
                    float normalizedDistance = (1f / (LineStepCount - 1)) * i;
                    mainParticleArray[i].startColor = decayGradient.Evaluate((Time.unscaledTime - decayStartTime) / lifetimeAfterDisabled) * GetColor(normalizedDistance);
                }
            }

            particles.SetParticles(mainParticleArray, LineStepCount);
        }
    }
}
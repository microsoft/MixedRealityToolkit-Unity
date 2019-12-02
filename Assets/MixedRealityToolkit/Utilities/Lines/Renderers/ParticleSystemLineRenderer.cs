// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// attaches a set of particles to the line
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu("Scripts/MRTK/Core/ParticleSystemLineRenderer")]
    public class ParticleSystemLineRenderer : BaseMixedRealityLineRenderer
    {
        private const int GlobalMaxParticles = 2048;

        [Header("Particle Settings")]

        [SerializeField]
        private Material lineMaterial;

        public Material LineMaterial
        {
            get { return lineMaterial; }
            set { lineMaterial = value; }
        }

        [SerializeField]
        [Range(128, GlobalMaxParticles)]
        private int maxParticles = GlobalMaxParticles;

        public int MaxParticles
        {
            get { return maxParticles; }
            set { maxParticles = Mathf.Clamp(value, 128, GlobalMaxParticles); }
        }

        [Header("Noise settings")]

        [SerializeField]
        private bool particleNoiseOnDisabled = true;

        public bool ParticleNoiseOnDisabled
        {
            get { return particleNoiseOnDisabled; }
            set { particleNoiseOnDisabled = value; }
        }

        [SerializeField]
        private Vector3 noiseStrength = Vector3.one;

        public Vector3 NoiseStrength
        {
            get { return noiseStrength; }
            set { noiseStrength = value; }
        }

        [SerializeField]
        private float noiseFrequency = 1.2f;

        public float NoiseFrequency
        {
            get { return noiseFrequency; }
            set { noiseFrequency = value; }
        }

        [Range(1, 10)]
        [SerializeField]
        private int noiseOcatives = 3;

        public int NoiseOcatives
        {
            get { return noiseOcatives; }
            set { noiseOcatives = Mathf.Clamp(value, 1, 10); }
        }

        [SerializeField]
        [Range(-10f, 10f)]
        private float noiseSpeed = 1f;

        public float NoiseSpeed
        {
            get { return noiseSpeed; }
            set { noiseSpeed = Mathf.Clamp(value, -10f, 10f); }
        }

        [SerializeField]
        [Range(0.01f, 0.5f)]
        private float lifetimeAfterDisabled = 0.25f;

        public float LifetimeAfterDisabled
        {
            get { return lifetimeAfterDisabled; }
            set { lifetimeAfterDisabled = Mathf.Clamp(value, 0.01f, 0.5f); }
        }

        [SerializeField]
        private Gradient decayGradient = new Gradient();

        public Gradient DecayGradient
        {
            get { return decayGradient; }
            set { decayGradient = value; }
        }

        [SerializeField]
        [HideInInspector]
        private ParticleSystem particles;

        [SerializeField]
        [HideInInspector]
        private ParticleSystemRenderer mainParticleRenderer;

        public ParticleSystemRenderer MainParticleRenderer
        {
            get
            {
                if (particles == null)
                {
                    particles = gameObject.EnsureComponent<ParticleSystem>();
                }

                if (mainParticleRenderer == null)
                {
                    mainParticleRenderer = particles.EnsureComponent<ParticleSystemRenderer>();
                }

                return mainParticleRenderer;
            }
            set { mainParticleRenderer = value; }
        }

        private readonly ParticleSystem.Particle[] mainParticleArray = new ParticleSystem.Particle[GlobalMaxParticles];

        private ParticleSystem.NoiseModule mainNoiseModule;

        private float decayStartTime = 0f;

        private void OnEnable()
        {
            if (particles == null)
            {
                particles = gameObject.EnsureComponent<ParticleSystem>();
            }

            mainNoiseModule = particles.noise;

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

            MainParticleRenderer.sharedMaterial = lineMaterial;
            MainParticleRenderer.enabled = true;

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

        /// <inheritdoc />
        protected override void UpdateLine()
        {
            if (!LineDataSource.enabled)
            {
                mainNoiseModule.enabled = particleNoiseOnDisabled;
                mainNoiseModule.strengthX = noiseStrength.x;
                mainNoiseModule.strengthY = noiseStrength.y;
                mainNoiseModule.strengthZ = noiseStrength.z;
                mainNoiseModule.octaveCount = noiseOcatives;
                mainNoiseModule.scrollSpeed = noiseSpeed;
                mainNoiseModule.frequency = noiseFrequency;

                if (decayStartTime < 0)
                {
                    decayStartTime = Time.unscaledTime;
                }
            }
            else
            {
                mainNoiseModule.enabled = false;
                decayStartTime = -1;
            }

            if (LineDataSource.enabled)
            {
                for (int i = 0; i < LineStepCount; i++)
                {
                    float normalizedDistance = GetNormalizedPointAlongLine(i);
                    ParticleSystem.Particle particle = mainParticleArray[i];
                    particle.position = LineDataSource.GetPoint(normalizedDistance);
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

        private void OnDisable()
        {
            MainParticleRenderer.enabled = false;
        }
    }
}
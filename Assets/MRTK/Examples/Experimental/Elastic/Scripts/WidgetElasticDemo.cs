using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental.Demos
{
    /// <summary>
    /// Demonstration script to show how <see cref="ElasticSystem{T}"/> can be used to drive
    /// a satisfying UI inflation/deflation effect of an example UI widget.
    /// </summary>
    public class WidgetElasticDemo : MonoBehaviour
    {
        // The backplate, which will scale horizontally.
        public Transform backplateTransform;

        // A list of the panels that will flip up. These will be
        // inflated in the order that they're listed.
        public List<Transform> flipPanels = new List<Transform>();

        // Internal list of of the elastic systems. Each ElasticSystem holds its
        // own state. We use a LinearElasticSystem because we are only controlling
        // a single value for each element that will be elastic-ified.
        private List<LinearElasticSystem> flipElastics = new List<LinearElasticSystem>();

        // The corresponding list of "goal values" for each of the flip panels' elastic systems.
        private List<float> flipGoals = new List<float>();

        // The elastic system for the backplate (treated separately)
        private LinearElasticSystem backplateElastic;

        // The goal value for the backplate's horizontal scale.
        private float backplateGoal;

        // The elastic extent for our flippy panels.
        private static LinearElasticExtent flipExtent = new LinearElasticExtent
        {
            MinStretch = -180.0f,
            MaxStretch = 0.0f,
            SnapPoints = new float[] { },
            SnapToEnds = false
        };

        // The elastic extent for our backplate scaling.
        private static LinearElasticExtent scaleExtent = new LinearElasticExtent
        {
            MinStretch = 0.0f,
            MaxStretch = 1.0f,
            SnapPoints = new float[] { },
            SnapToEnds = false
        };

        // The elastic properties of our springs.
        private static ElasticProperties elasticProperties = new ElasticProperties
        {
            Mass = 0.03f,
            HandK = 4.0f,
            EndK = 3.0f,
            SnapK = 1.0f,
            Drag = 0.2f
        };

        // Allow the user to configure the enabled/disabled states of the
        // elements controlled by our elastic systems.
        public float inflateAngle = -170.0f;
        public float deflateAngle = -20.0f;
        public float inflateScale = 1.0f;
        public float deflateScale = 0.1f;

        // Is the doodad inflated or not?
        private bool isInflated = false;

        // Start is called before the first frame update
        void Start()
        {
            for (var i = 0; i < flipPanels.Count; i++)
            {
                flipElastics.Add(new LinearElasticSystem(deflateAngle, 0.0f, flipExtent, elasticProperties));
                flipGoals.Add(deflateAngle);
            }

            backplateElastic = new LinearElasticSystem(deflateScale, 0.0f, scaleExtent, elasticProperties);
            backplateGoal = deflateScale;
        }

        // Update is called once per frame
        void Update()
        {
            backplateTransform.localScale = new Vector3(Mathf.Clamp01(backplateElastic.ComputeIteration(backplateGoal, Time.deltaTime)), backplateTransform.localScale.y, backplateTransform.localScale.z);

            for (var i = 0; i < flipElastics.Count; i++)
            {
                flipPanels[i].localEulerAngles = new Vector3(Mathf.Clamp(flipElastics[i].ComputeIteration(flipGoals[i], Time.deltaTime), -360, 0), flipPanels[i].localRotation.y, flipPanels[i].localRotation.z);
            }
        }

        public void Inflate()
        {
            if (!isInflated)
            {
                isInflated = true;
                StartCoroutine(InflateCoroutine());
            }
            else
            {
                isInflated = false;
                StartCoroutine(DeflateCoroutine());
            }
        }

        public IEnumerator DeflateCoroutine()
        {
            for (var i = 0; i < flipElastics.Count; i++)
            {
                flipGoals[i] = deflateAngle;
                yield return new WaitForSeconds(0.1f);
            }
            backplateGoal = deflateScale;
        }

        public IEnumerator InflateCoroutine()
        {
            backplateGoal = inflateScale;
            yield return new WaitForSeconds(0.1f);
            for (var i = 0; i < flipElastics.Count; i++)
            {
                flipGoals[i] = inflateAngle;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}

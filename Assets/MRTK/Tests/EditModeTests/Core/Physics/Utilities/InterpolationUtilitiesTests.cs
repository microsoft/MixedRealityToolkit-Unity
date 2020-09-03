// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using NUnit.Framework;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Physics
{
    class InterpolationUtilitiesTests
    {
        [SetUp]
        public void SetUp()
        {
            // The rest of the tests do floating point comparison, so equality comparison must have some
            // fudge factor.
            GlobalSettings.DefaultFloatingPointTolerance = .001;
        }

        [Test]
        public void TestExpCoefficient()
        {
            // Passing a half life value of 0 always returns a value of 1 (this is an 'invalid' calling convention).
            Assert.That(InterpolationUtilities.ExpCoefficient(0.0f /*hLife*/, 10.0f /*dTime*/), Is.EqualTo(1.0f));
            Assert.That(InterpolationUtilities.ExpCoefficient(0.0f /*hLife*/, 0.0f /*dTime*/), Is.EqualTo(1.0f));

            // Passing a valid half life and delta time should result in increasingly higher numbers according to
            // the formula (1-.5^(half life / delta time))
            Assert.That(InterpolationUtilities.ExpCoefficient(2.0f /*hLife*/, 2.0f /*dTime*/), Is.EqualTo(0.5f));
            Assert.That(InterpolationUtilities.ExpCoefficient(2.0f /*hLife*/, 4.0f /*dTime*/), Is.EqualTo(0.75f));
            Assert.That(InterpolationUtilities.ExpCoefficient(2.0f /*hLife*/, 6.0f /*dTime*/), Is.EqualTo(0.875f));
        }

        [Test]
        public void TestFloatExpDecay()
        {
            // Validates the float overload of InterpolationUtilities::ExpDecay, which lerps between the from value
            // toward the to value in an exponential fashion.
            Assert.That(InterpolationUtilities.ExpDecay(50.0f /*from*/, 150.0f /*to*/, 0.0f /*hLife*/, 5.0f /*dTime*/), Is.EqualTo(150f));
            Assert.That(InterpolationUtilities.ExpDecay(50.0f /*from*/, 150.0f /*to*/, 1.0f /*hLife*/, 1.0f /*dTime*/), Is.EqualTo(100.0f));
            Assert.That(InterpolationUtilities.ExpDecay(50.0f /*from*/, 150.0f /*to*/, 2.0f /*hLife*/, 2.0f /*dTime*/), Is.EqualTo(100.0f));
            Assert.That(InterpolationUtilities.ExpDecay(50.0f /*from*/, 150.0f /*to*/, 2.0f /*hLife*/, 4.0f /*dTime*/), Is.EqualTo(125.0f));
            Assert.That(InterpolationUtilities.ExpDecay(50.0f /*from*/, 150.0f /*to*/, 2.0f /*hLife*/, 6.0f /*dTime*/), Is.EqualTo(137.5f));
            Assert.That(InterpolationUtilities.ExpDecay(50.0f /*from*/, 150.0f /*to*/, 2.0f /*hLife*/, 20.0f /*dTime*/), Is.EqualTo(149.90234f));
        }

        [Test]
        public void TestVector2ExpDecay()
        {
            Vector2 from = new Vector2(50.0f, 150.0f);
            Vector2 to = new Vector2(150.0f, 250.0f);

            // Validates the Vector2 overload of InterpolationUtilities::ExpDecay, which lerps between the from value
            // toward the to value in an exponential fashion.
            Assert.That(InterpolationUtilities.ExpDecay(from, to, 0.0f /*hLife*/, 5.0f /*dTime*/), Is.EqualTo(new Vector2(150.0f, 250.0f)));
            Assert.That(InterpolationUtilities.ExpDecay(from, to, 1.0f /*hLife*/, 1.0f /*dTime*/), Is.EqualTo(new Vector2(100.0f, 200.0f)));
            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 2.0f /*dTime*/), Is.EqualTo(new Vector2(100.0f, 200.0f)));
            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 4.0f /*dTime*/), Is.EqualTo(new Vector2(125.0f, 225.0f)));
            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 6.0f /*dTime*/), Is.EqualTo(new Vector2(137.5f, 237.5f)));
            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 20.0f /*dTime*/), Is.EqualTo(new Vector2(149.90234f, 249.90234f)));
        }

        [Test]
        public void TestVector3ExpDecay()
        {
            Vector3 from = new Vector3(50.0f, 150.0f, 250.0f);
            Vector3 to = new Vector3(150.0f, 250.0f, 350.0f);

            // Validates the Vector3 overload of InterpolationUtilities::ExpDecay, which lerps between the from value
            // toward the to value in an exponential fashion.
            Assert.That(InterpolationUtilities.ExpDecay(from, to, 0.0f /*hLife*/, 5.0f /*dTime*/),
                Is.EqualTo(new Vector3(150.0f, 250.0f, 350.0f)));

            Assert.That(InterpolationUtilities.ExpDecay(from, to, 1.0f /*hLife*/, 1.0f /*dTime*/),
                Is.EqualTo(new Vector3(100.0f, 200.0f, 300.0f)));

            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 2.0f /*dTime*/),
                Is.EqualTo(new Vector3(100.0f, 200.0f, 300.0f)));

            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 4.0f /*dTime*/),
                Is.EqualTo(new Vector3(125.0f, 225.0f, 325.0f)));

            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 6.0f /*dTime*/),
                Is.EqualTo(new Vector3(137.5f, 237.5f, 337.5f)));

            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 20.0f /*dTime*/),
                Is.EqualTo(new Vector3(149.90234f, 249.90234f, 349.90234f)));
        }

        [Test]
        public void TestColorExpDecay()
        {
            Color from = new Color(0.0f, 0.0f, 1.0f);
            Color to = new Color(1.0f, 0.0f, 0.0f);

            // Validates the Color overload of InterpolationUtilities::ExpDecay, which lerps between the from value
            // toward the to value in an exponential fashion.
            Assert.That(InterpolationUtilities.ExpDecay(from, to, 0.0f /*hLife*/, 5.0f /*dTime*/),
                Is.EqualTo(new Color(1.0f, 0.0f, 0.0f)));

            Assert.That(InterpolationUtilities.ExpDecay(from, to, 1.0f /*hLife*/, 1.0f /*dTime*/),
                Is.EqualTo(new Color(0.5f, 0.0f, 0.5f)));

            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 2.0f /*dTime*/),
                Is.EqualTo(new Color(0.5f, 0.0f, 0.5f)));

            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 4.0f /*dTime*/),
                Is.EqualTo(new Color(0.75f, 0.0f, 0.25f)));

            Assert.That(InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 6.0f /*dTime*/),
                Is.EqualTo(new Color(0.875f, 0.0f, 0.125f)));
        }

        [Test]
        public void TestQuaternionExpDecay()
        {
            // Validates the Quaternion overload of InterpolationUtilities::ExpDecay, which lerps between the from value
            // toward the to value in an exponential fashion.
            // Note that Quaternions are a bit funky in that they are using spherical lerps, which means the lerp occurs
            // on the surface of a sphere (instead of a linear lerp between points)
            Quaternion from = Quaternion.Euler(new Vector3(0, 0, 0));
            Quaternion to = Quaternion.Euler(new Vector3(0, 45, 0));

            Assert.IsTrue(InterpolationUtilities.ExpDecay(from, to, 0.0f /*hLife*/, 5.0f /*dTime*/) == to);

            // This test ultimately converts the lerped values into rotation effects on the forward vector, to show
            // that this is properly lerping in a more human-readable fashion (i.e. reading unit vector directions, instead
            // of quaternions)
            Vector3 result = InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 2.0f /*dTime*/) * Vector3.forward;
            Assert.IsTrue(
                AreEqual(
                    InterpolationUtilities.ExpDecay(from, to, 1.0f /*hLife*/, 1.0f /*dTime*/) * Vector3.forward,
                    new Vector3(0.3826835f, 0.0f, 0.9238795f)));

            Assert.IsTrue(
                AreEqual(
                    InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 2.0f /*dTime*/) * Vector3.forward,
                    new Vector3(0.3826835f, 0.0f, 0.9238795f)));

            Assert.IsTrue(
                AreEqual(
                    InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 4.0f /*dTime*/) * Vector3.forward,
                    new Vector3(0.5555702f, 0.0f, 0.8314696f)));

            Assert.IsTrue(
                AreEqual(
                    InterpolationUtilities.ExpDecay(from, to, 2.0f /*hLife*/, 6.0f /*dTime*/) * Vector3.forward,
                    new Vector3(0.6343933f, 0.0f, 0.7730104f)));
        }

        /// <summary>
        /// A helper to compares two vectors with slightly looser tolerance than the default Vector3 comparison.
        /// </summary>
        /// <remarks>
        /// Primarily useful when looking the results of quaternion-effected rotations, which tends to lead
        /// to a lot of decimal place digits.
        /// </remarks>
        private static bool AreEqual(Vector3 left, Vector3 right)
        {
            return Vector3.Distance(left, right) <= GlobalSettings.DefaultFloatingPointTolerance;
        }
    }
}

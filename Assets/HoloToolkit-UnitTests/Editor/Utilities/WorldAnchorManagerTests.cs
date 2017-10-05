using UnityEngine;
using NUnit.Framework;
using HoloToolkit.Unity.Tests.Extensions;

namespace HoloToolkit.Unity.Tests
{
    public class WorldAnchorManagerTests
    {
        [Test]
        public void TestGenerateAnchorNameFromGameObject()
        {
            const string expected = "AnchorName";
            var gameObject = new GameObject(expected);
            var result = WorldAnchorManager.Instance.GenerateAnchorName(gameObject);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TestGenerateAnchorNameFromParameter()
        {
            const string expected = "AnchorName";
            var gameObject = new GameObject();
            var result = WorldAnchorManager.Instance.GenerateAnchorName(gameObject, expected);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
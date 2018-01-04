using MixedRealityToolkit.Common;
using NUnit.Framework;
using System;

namespace MixedRealityToolkit.Tests
{
    public class EnumerableExtensionsTests
    {
        [Test]
        public void TestMaxOrDefault()
        {
            Assert.Throws<ArgumentNullException>(() => ((int[])null).MaxOrDefault());
        }

        [Test]
        public void TestMaxOrDefaultEmpty()
        {
            var items = new int[0];

            Assert.That(items.MaxOrDefault(), Is.Zero);
        }

        [Test]
        public void TestMaxOrDefaultUnordered()
        {
            var items = new[]
            {
                -5, -20, 100, 5
            };

            Assert.That(items.MaxOrDefault(), NUnit.Framework.Is.EqualTo(100));
        }

        [Test]
        public void TestMaxOrDefaultOrdered()
        {
            var items = new[]
            {
                -20, -5, 5, 100
            };

            Assert.That(items.MaxOrDefault(), NUnit.Framework.Is.EqualTo(100));
        }
    }
}

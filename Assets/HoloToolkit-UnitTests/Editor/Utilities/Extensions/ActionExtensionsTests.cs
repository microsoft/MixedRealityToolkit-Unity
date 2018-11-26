// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using NUnit.Framework;

namespace HoloToolkit.Unity.Tests
{
    public class ActionExtensionsTests
    {
        private bool hasBeenRaised;
        private object arg1;
        private object arg2;
        private object arg3;
        private object arg4;

        [SetUp]
        public void SetUpTests()
        {
            hasBeenRaised = false;
            arg1 = arg2 = arg3 = arg4 = null;
        }

        private void Raiseable()
        {
            hasBeenRaised = true;
        }

        private void Raiseable<T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            Raiseable();
            arg1 = param1;
            arg2 = param2;
            arg3 = param3;
            arg4 = param4;
        }

        private void Raiseable<T1, T2, T3>(T1 param1, T2 param2, T3 param3)
        {
            Raiseable(param1, param2, param3, "");
        }

        private void Raiseable<T1, T2>(T1 param1, T2 param2)
        {
            Raiseable(param1, param2, "");
        }

        private void Raiseable<T>(T param1)
        {
            Raiseable(param1, "");
        }


        [Test]
        public void TestHasBeenRaisedNoArgs()
        {
            Action action = Raiseable;
            action.RaiseEvent();
            Assert.That(hasBeenRaised, Is.True);
        }

        [Test]
        public void TestHasBeenRaisedOneArg()
        {
            Action<object> action = Raiseable;
            action.RaiseEvent(null);
            Assert.That(hasBeenRaised, Is.True);
        }

        [Test]
        public void TestHasBeenRaisedTwoArgs()
        {
            Action<object, object> action = Raiseable;
            action.RaiseEvent(null, null);
            Assert.That(hasBeenRaised, Is.True);
        }

        [Test]
        public void TestHasBeenRaisedThreeArgs()
        {
            Action<object, object, object> action = Raiseable;
            action.RaiseEvent(null, null, null);
            Assert.That(hasBeenRaised, Is.True);
        }

        [Test]
        public void TestHasBeenRaisedFourArgs()
        {
            Action<object, object, object, object> action = Raiseable;
            action.RaiseEvent(null, null, null, null);
            Assert.That(hasBeenRaised, Is.True);
        }

        [TestCase(1)]
        [TestCase(null)]
        [TestCase("Foo")]
        [TestCase(30f)]
        [TestCase(false)]

        public void RaiseDataOneArgs(object param1)
        {
            Action<object> action = Raiseable;
            action.RaiseEvent(param1);
            Assert.That(arg1, Is.EqualTo(param1));
        }

        [TestCase(1, 2)]
        [TestCase(null, "Foo")]
        [TestCase(30f, false)]

        public void RaiseDataTwoArgs(object param1, object param2)
        {
            Action<object, object> action = Raiseable;
            action.RaiseEvent(param1, param2);
            Assert.That(arg1, Is.EqualTo(param1));
            Assert.That(arg2, Is.EqualTo(param2));
        }

        [TestCase(1, 2, 3)]
        [TestCase(null, "Foo", 30f)]
        [TestCase(false, "Foo", 30f)]

        public void RaiseDataThreeArgs(object param1, object param2, object param3)
        {
            Action<object, object, object> action = Raiseable;
            action.RaiseEvent(param1, param2, param3);
            Assert.That(arg1, Is.EqualTo(param1));
            Assert.That(arg2, Is.EqualTo(param2));
            Assert.That(arg3, Is.EqualTo(param3));
        }

        [TestCase(1, 2, 3, 4)]
        [TestCase(null, "Foo", 30f, false)]

        public void RaiseDataFourArgs(object param1, object param2, object param3, object param4)
        {
            Action<object, object, object, object> action = Raiseable;
            action.RaiseEvent(param1, param2, param3, param4);
            Assert.That(arg1, Is.EqualTo(param1));
            Assert.That(arg2, Is.EqualTo(param2));
            Assert.That(arg3, Is.EqualTo(param3));
            Assert.That(arg4, Is.EqualTo(param4));
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework.Constraints;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    /// <summary>
    /// Extend Is to add UnityNull as static property
    /// </summary>
    public class Is : NUnit.Framework.Is
    {
        /// <summary>
        /// Returns a constraint that checks unity objects for null
        /// </summary>
        public static UnityNullConstraint UnityNull
        {
            get
            {
                return new UnityNullConstraint();
            }
        }
    }

    /// <summary>
    /// Unity classes behave differently when compared to null so the normal Is.Null does not work
    /// </summary>
    public sealed class UnityNullConstraint : Constraint
    {
        public UnityNullConstraint()
        {
            Description = "<null>";
        }

        public override ConstraintResult ApplyTo(object actual)
        {
            var result = false;
            if (actual == null)
            {
                result = true;
            }
            else if (actual is Object && actual as Object == null)
            {
                //Visual studio is wrong about this part not being reachable, but it will never be true while debugging
                result = true;
            }
            return new ConstraintResult(this, actual, result);
        }
    }

    /// <summary>
    /// Adds UnityNull to ConstraintExpressions like Is.Not.UnityNull()
    /// </summary>
    public static class CustomConstraintExtensions
    {
        public static UnityNullConstraint UnityNull(this ConstraintExpression expression)
        {
            var constraint = new UnityNullConstraint();
            expression.Append(constraint);
            return constraint;
        }
    }
}

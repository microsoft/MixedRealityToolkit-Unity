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
            var unityObject = actual as Object;
            var result = unityObject == null;
            return new ConstraintResult(this, unityObject, result);
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

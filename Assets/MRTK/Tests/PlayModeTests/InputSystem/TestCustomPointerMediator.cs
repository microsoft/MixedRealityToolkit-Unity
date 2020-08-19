using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    /// <summary>
    /// A custom pointer mediator used to validate that the common behaviors of DefaultPointerMediator
    /// carry over even as a subclass.
    /// </summary>
    public class TestPointerMediator : DefaultPointerMediator
    {
        public override void UpdatePointers()
        {
            base.UpdatePointers();
        }
    }
}
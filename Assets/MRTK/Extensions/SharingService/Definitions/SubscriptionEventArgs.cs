using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public struct SubscriptionEventArgs
    {
        public SubscriptionMode Mode;
        public IEnumerable<short> Types;
    }
}
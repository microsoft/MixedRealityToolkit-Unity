using System;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public struct ResponseUtils
    {
        /// <summary>
        /// Static Func for create convert Task
        /// </summary>
        public static Func<byte[], Task<string>> BytesToString = async (byteArray) => await Task.Run(() =>
             byteArray != null ? System.Text.Encoding.Default.GetString(byteArray) : string.Empty).ConfigureAwait(false);
    }
}

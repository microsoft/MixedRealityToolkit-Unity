using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.Controllers;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers
{
    /// <summary>
    /// Base speech data provider to inherit from when implementing <see cref="IMixedRealitySpeechDataProvider"/>s
    /// </summary>
    public abstract class BaseSpeechDataProvider : BaseControllerDataProvider, IMixedRealitySpeechDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        protected BaseSpeechDataProvider(string name, uint priority) : base(name, priority) { }

        /// <inheritdoc />
        public virtual bool IsRecognitionActive { get; protected set; } = false;

        /// <inheritdoc />
        public virtual void StartRecognition() { }

        /// <inheritdoc />
        public virtual void StopRecognition() { }
    }
}
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Marshalling
{
    /// <summary>
    /// A statistics wrapper that can be used to track frame times
    /// </summary>
    public class FrameQueueStat : IReadonlyFrameQueueStat
    {
        /// <summary>
        /// The average value calculated for the stat
        /// </summary>
        public float Value
        {
            get
            {
                lock (times)
                {
                    int count = times.Count;
                    float firstTime = 0;
                    float lastTime = 0;

                    if (count >= 2)
                    {
                        firstTime = times.Get(0);
                        lastTime = times.Get(count - 1);
                        return (count - 1) / (firstTime - lastTime);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// Internal storage of timing data
        /// </summary>
        private Deque<float> times;

        /// <summary>
        /// Internal timing watch
        /// </summary>
        private Stopwatch watch;

        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="samples">the max number of possible timing samples</param>
        public FrameQueueStat(int samples = 100)
        {
            this.times = new Deque<float>(samples);
            this.watch = new Stopwatch();

            this.watch.Start();
        }

        /// <summary>
        /// Track that the stat has just occured
        /// </summary>
        /// <remarks>
        /// This should be called each time the event you wish to monitor occurs
        /// </remarks>
        public void Track()
        {
            times.AddFront(watch.ElapsedMilliseconds * 0.001f);
            if (times.Count >= times.Capacity)
            {
                times.RemoveBack();
            }
        }
    }
}

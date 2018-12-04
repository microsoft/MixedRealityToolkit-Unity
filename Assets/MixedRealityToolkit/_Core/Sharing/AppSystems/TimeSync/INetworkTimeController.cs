using System.Collections.Generic;

namespace Pixie.AppSystems.TimeSync
{
    public interface INetworkTimeController
    {
        /// <summary>
        /// Whether synchronized time has begin.
        /// </summary>
        bool Started { get; }

        /// <summary>
        /// The time our local network time object should seek to match.
        /// </summary>
        float TargetTime { get; }

        /// <summary>
        /// The device time components that have been detected
        /// </summary>
        IEnumerable<IDeviceTime> DeviceTimeSources { get; }
    }
}
using System.Collections.Generic;

namespace Pixie.DeviceControl.Users
{
    public interface IAppStateUsers
    {
        /// <summary>
        /// Generates player states for all sessions.
        /// This function will generate all 'coupled' states
        /// </summary>
        void GenerateUserStates(IEnumerable<UserSlot> userSlots);

        /// <summary>
        /// Generates target states for all players for all sessions.
        /// </summary>
        void GenerateTargetStates(IEnumerable<UserSlot> userSlots);
    }
}

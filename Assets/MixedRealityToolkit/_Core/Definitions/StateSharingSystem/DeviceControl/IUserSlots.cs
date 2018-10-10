namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl
{
    public interface IUserSlots
    {
        bool TryAssignDeviceToSlot(IUserDevice device, sbyte slotNum);
        void RevokeAssignment(IUserDevice device, sbyte slotNum);
    }
}
using Pixie.Core;

namespace Pixie.DeviceControl
{
    public interface ILocalDevicePrefs : ISharingAppObject
    {
        void ClearPrefs();
        void SavePrefs();
        bool GetSavedPrefs(out LocalDevicePrefsState prefs);
    }
}
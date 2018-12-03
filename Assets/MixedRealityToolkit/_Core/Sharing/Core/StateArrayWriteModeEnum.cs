namespace Pixie.Core
{
    public enum StateArrayWriteModeEnum
    {
        Write,          // Anyone can read and write
        Playback,       // Anyone can read, state pipe can write
        Locked,         // Read-only
    }
}
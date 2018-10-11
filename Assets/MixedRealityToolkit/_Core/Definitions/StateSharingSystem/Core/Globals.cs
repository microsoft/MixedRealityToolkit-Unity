namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core
{
    public static class Globals
    {
        public static class UNet
        {
            public const int ChannelReliable = 0;
            public const int ChannelUnreliableFragmented = 1;
            public const int ChannelAllCosts = 2;
            public const int ChannelReliableSequenced = 3;
            public const int ChannelUnreliable = 4;
            public const int ChannelReliableFragmented = 5;

            public const float SendIntervalTimeSync = 3f;
            public const float SendIntervalLatencyCheck = 0.5f;
            public const float SendIntervalSlow = 0.33f;
            public const float SendIntervalModerate = 0.2f;
            public const float SendIntervalFast = 0.15f;
            public const float SendIntervalAllCosts = 0.05f;

            public const float MaxPositionRange = 50f;
            public const float MaxTargetLengthRange = 10f;
            public const float MaxLaunchVelocityRange = 16f;
            public const float MaxObjectVelocity = 16f;

            public const int ServerTargetFrameRate = 60;

            public const float PushIntervalSceneAlign = SendIntervalFast;
            public const float PushIntervalPlayer = SendIntervalModerate;
            public const float PlayerActionReceivedTimeout = 1.5f;

            public const int MaxConnectedDevices = 10;

            public const int MaxAverageLatencyValues = 10;
            public const int MinLatencyChecks = 3;
        }
    }
}
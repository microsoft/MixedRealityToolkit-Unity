namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Since the InteractionSourceState is internal to UnityEngine.VR.WSA.Input,
    /// this is a fake SourceState structure to keep the test code consistent.
    /// </summary>
    public struct DebugInteractionSourceState
    {
        public bool Pressed;
        public bool Grasped;
        public bool MenuPressed;
        public bool SelectPressed;
        public DebugInteractionSourcePose SourcePose;
    }
}
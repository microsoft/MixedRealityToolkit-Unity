public static class INetworkingBase<T> {

    public enum NetworkingType
    {
        Photon, 
        WebRTC
    }

    public static NetworkingType NetworkingMode = NetworkingType.Photon;

    public static void Send(T data)
    {
        Send(0, data, 1);
    }

    public static void Send(int ID, T data)
    {
        Send(ID, data, 1);
    }

    public static void Send(int ID, T data, float reliability)
    {
        switch (NetworkingMode)
        {
            case NetworkingType.Photon:
                IPhotonNetworking<T>.Instance.Send(ID, data, reliability);
                break;
            case NetworkingType.WebRTC:
                //TODO
                break;
        }
                
    }
}

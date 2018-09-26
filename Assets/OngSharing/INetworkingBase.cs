public static class INetworkingBase<T> {    

    public static NetworkingType NetworkingMode = NetworkingType.Photon;

    public static int defualtID = 0;

    public static int defaultReliability = 1;

    public static void Send(T data)
    {
        Send(defualtID, data, defaultReliability);
    }

    public static void Send(int ID, T data)
    {
        Send(ID, data, defaultReliability);
    }

    public static void Send(T data, float reliability)
    {
        Send(defualtID, data, reliability);
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

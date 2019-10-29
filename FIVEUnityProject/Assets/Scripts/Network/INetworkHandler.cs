namespace FIVE.Network
{
    internal interface INetworkHandler
    {
        int UpdateRate { get; set; }
        bool IsRunning { get; }
        void Start();
        void Stop();
    }
}
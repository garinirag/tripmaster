namespace TripMaster.Services
{
    public interface IBluetoothCommandService
    {
        event Action<RemoteCommand>? OnCommandReceived;
        event Action? OnConnected;
        event Action? OnDisconnected;

        Task StartScanAndConnectAsync();
        Task DisconnectAsync();
    }
}
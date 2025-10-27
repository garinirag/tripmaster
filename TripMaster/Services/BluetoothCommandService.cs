using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System.Text;

namespace TripMaster.Services;

public enum RemoteCommand
{
    None,
    Start,
    Stop,
    Reset,
    Next
}

public class BluetoothCommandService : IBluetoothCommandService
{
    private const string TargetDeviceName = "MyCustomRemote";
    private const string ServiceUuid = "0000ffe0-0000-1000-8000-00805f9b34fb";
    private const string CharacteristicUuid = "0000ffe1-0000-1000-8000-00805f9b34fb";

    public event Action<RemoteCommand>? OnCommandReceived;
    public event Action? OnConnected;
    public event Action? OnDisconnected;

    private readonly IBluetoothLE _ble;
    private readonly IAdapter _adapter;
    private IDevice? _device;
    private ICharacteristic? _commandCharacteristic;
    private bool _reconnecting = false;

    public BluetoothCommandService()
    {
        //_ble = CrossBluetoothLE.Current;
        //_adapter = CrossBluetoothLE.Current.Adapter;
        //_adapter.DeviceDisconnected += async (s, d) =>
        //{
        //    if (d.Id == _device?.Id)
        //    {
        //        OnDisconnected?.Invoke();
        //        await TryReconnectAsync();
        //    }
        //};
    }

    public async Task StartScanAndConnectAsync()
    {
        try
        {
            //if (_device != null && _device.State == DeviceState.Connected)
            //    return;

            Console.WriteLine("[BLE] Scanning for devices...");
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            _adapter.ScanTimeout = 10000;
            _adapter.ScanMode = ScanMode.Balanced;

            IDevice? foundDevice = null;
            _adapter.DeviceDiscovered += (s, a) =>
            {
                if (a.Device.Name == TargetDeviceName)
                    foundDevice = a.Device;
            };

            //await _adapter.StartScanningForDevicesAsync(cts.Token);
            await _adapter.StopScanningForDevicesAsync();

            if (foundDevice == null)
            {
                Console.WriteLine("[BLE] Device not found.");
                return;
            }

            _device = foundDevice;
            Console.WriteLine($"[BLE] Connecting to {_device.Name}...");
            await _adapter.ConnectToDeviceAsync(_device);

            OnConnected?.Invoke();

            var service = await _device.GetServiceAsync(Guid.Parse(ServiceUuid));
            if (service == null)
            {
                Console.WriteLine("[BLE] Service not found.");
                return;
            }

            _commandCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicUuid));
            if (_commandCharacteristic == null)
            {
                Console.WriteLine("[BLE] Characteristic not found.");
                return;
            }

            _commandCharacteristic.ValueUpdated += (s, e) =>
            {
                var data = e.Characteristic.Value;
                if (data != null && data.Length > 0)
                {
                    var cmd = DecodeCommand(data[0]);
                    if (cmd != RemoteCommand.None)
                        OnCommandReceived?.Invoke(cmd);
                }
            };

            await _commandCharacteristic.StartUpdatesAsync();
            Console.WriteLine("[BLE] Listening for commands...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BLE] Error: {ex.Message}");
        }
    }

    private async Task TryReconnectAsync()
    {
        if (_reconnecting || _device == null)
            return;

        _reconnecting = true;
        Console.WriteLine("[BLE] Attempting to reconnect...");
        for (int i = 0; i < 5; i++)
        {
            try
            {
                await _adapter.ConnectToDeviceAsync(_device);
                _reconnecting = false;
                OnConnected?.Invoke();
                return;
            }
            catch
            {
                await Task.Delay(2000);
            }
        }

        Console.WriteLine("[BLE] Reconnection failed.");
        _reconnecting = false;
    }

    private RemoteCommand DecodeCommand(byte value)
    {
        return value switch
        {
            0x01 => RemoteCommand.Start,
            0x02 => RemoteCommand.Stop,
            0x03 => RemoteCommand.Reset,
            0x04 => RemoteCommand.Next,
            _ => RemoteCommand.None
        };
    }

    public async Task DisconnectAsync()
    {
        if (_device != null)
        {
            try
            {
                await _adapter.DisconnectDeviceAsync(_device);
                OnDisconnected?.Invoke();
            }
            catch (DeviceConnectionException ex)
            {
                Console.WriteLine($"[BLE] Disconnect failed: {ex.Message}");
            }
        }
    }
}

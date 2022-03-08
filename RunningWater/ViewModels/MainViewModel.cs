using System;
using System.Text;
using System.Text.Json;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RunningWater.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        [Reactive]
        public bool IsConnected { get; private set; }

        public MainViewModel()
        {
            var serviceId = Guid.Parse("12345678-1234-5678-1234-56789abcdef0");
            var characteristicId = Guid.Parse("12345678-1234-5678-1234-56789abcdef1");
            var deviceName = "Raspberry BLE";

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                var ble = CrossBluetoothLE.Current;
                var adapter = CrossBluetoothLE.Current.Adapter;

                adapter.DeviceDiscovered += (s, e) =>
                {
                    Console.WriteLine($"Device -> {e.Device.Name} {e.Device.NativeDevice}");

                    if (e.Device.Name == deviceName)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await adapter.StopScanningForDevicesAsync();
                            await adapter.ConnectToDeviceAsync(e.Device, connectParameters: new ConnectParameters(autoConnect: true, forceBleTransport: true));

                            var service = await e.Device.GetServiceAsync(serviceId);
                            var characteristic = await service.GetCharacteristicAsync(characteristicId);

                            var value = await characteristic.ReadAsync();
                            Console.WriteLine(Encoding.UTF8.GetString(value));

                            var json = JsonSerializer.Serialize(
                                new
                                {
                                    hello = "Lorem Ipsum - это текст-рыба, часто используемый в печати и вэб-дизайне. Lorem Ipsum является стандартной рыбой для текстов на латинице с начала XVI века. В то время некий безымянный печатник создал большую коллекцию размеров и форм шрифтов, используя Lorem Ipsum для распечатки образцов. Lorem Ipsum не только успешно пережил без заметных изменений пять веков, но и перешагнул в электронный дизайн. Его популяризации в новое время послужили публикация листов Letraset с образцами Lorem Ipsum в 60-х годах и, в более недавнее время, программы электронной вёрстки типа Aldus PageMaker, в шаблонах которых используется Lorem Ipsum.",
                                    test = 123,
                                },
                                new JsonSerializerOptions
                                {
                                    WriteIndented = false,
                                });

                            var b = Encoding.UTF8.GetBytes(json);

                            await characteristic.WriteAsync(b);
                        });
                    }
                };

                await adapter.StartScanningForDevicesAsync();
            });
        }
    }

    public class BaseRequest
    {

    }

    public class BaseResponse
    {
        
    }
}

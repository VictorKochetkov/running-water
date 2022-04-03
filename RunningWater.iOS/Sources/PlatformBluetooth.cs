using System;
using System.Threading.Tasks;
using CoreBluetooth;
using CoreFoundation;
using RunningWater.Sources;
using Xamarin.Essentials;

namespace RunningWater.iOS.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class PlatformBluetooth : Bluetooth
    {
        /// <summary>
        /// 
        /// </summary>
        private class DefaultDelegate : CBCentralManagerDelegate
        {
            private Action<bool> action;

            public DefaultDelegate(Action<bool> action)
                => this.action = action;

            public override void UpdatedState(CBCentralManager central)
                => action(central.State == CBCentralManagerState.PoweredOn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adapter"></param>
        public PlatformBluetooth()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task EnableBluetoothAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            MainThread.BeginInvokeOnMainThread(() => new CBCentralManager(
                new DefaultDelegate(success => tcs.SetResult(success)),
                DispatchQueue.MainQueue,
                new CBCentralInitOptions { ShowPowerAlert = true }));

            if (!await tcs.Task)
                throw new Exception();
        }
    }
}

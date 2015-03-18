using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace CySmart.DataModel
{

    public class MyBluetoothLEDevice
    {
        public BluetoothLEDevice BluetoothLEDevice { get; private set; }

        public MyBluetoothLEDevice(BluetoothLEDevice gattDeviceService)
        {
            this.BluetoothLEDevice = gattDeviceService;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.BluetoothLEDevice.Name, this.BluetoothLEDevice.ConnectionStatus.ToString());
        }
    }
}

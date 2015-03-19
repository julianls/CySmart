using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace CySmart.DataModel
{
    /// <summary>
    /// Class to hold BluetoothLEDevice data
    /// </summary>
    public class MyBluetoothLEDevice
    {
        /// <summary>
        /// BLE Device
        /// </summary>
        public BluetoothLEDevice BluetoothLEDevice { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gattDeviceService"></param>
        public MyBluetoothLEDevice(BluetoothLEDevice gattDeviceService)
        {
            this.BluetoothLEDevice = gattDeviceService;
        }

        /// <summary>
        /// Object to string for list item presentation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.BluetoothLEDevice.Name, this.BluetoothLEDevice.ConnectionStatus.ToString());
        }
    }
}

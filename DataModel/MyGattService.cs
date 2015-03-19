using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace CySmart.DataModel
{
    /// <summary>
    /// Class to hold GATT service data
    /// </summary>
    public class MyGattService
    {
        /// <summary>
        /// GATT device service
        /// </summary>
        public GattDeviceService GattDeviceService { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description"></param>
        /// <param name="gattDeviceService"></param>
        public MyGattService(string description, GattDeviceService gattDeviceService)
        {
            this.Description = description;
            this.GattDeviceService = gattDeviceService;
        }

        /// <summary>
        /// Object to string for UI presentation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace CySmart.DataModel
{
    public class MyGattService
    {
        public GattDeviceService GattDeviceService { get; private set; }

        public string Description { get; private set; }

        public MyGattService(string description, GattDeviceService gattDeviceService)
        {
            this.Description = description;
            this.GattDeviceService = gattDeviceService;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}

using CySmart.Common;
using CySmart.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Pivot Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace CySmart
{
    public sealed partial class PivotPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        /// <summary>
        /// Currently selected device
        /// </summary>
        private BluetoothLEDevice currentDevice;

        /// <summary>
        /// Currently selected CapSense Characteristic
        /// </summary>
        GattCharacteristic currentCapSenseCharacteristic;

        /// <summary>
        /// Currently selected RGB Led Characteristic
        /// </summary>
        GattCharacteristic currentRGBLedCharacteristic;

        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            await ScanDevices();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache. Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async Task ScanDevices()
        {
            lstDevices.Items.Clear();
            string genericUUID = GattDeviceService.GetDeviceSelectorFromUuid(GattServiceUuids.GenericAccess);
            try
            {
                foreach (DeviceInformation di in await DeviceInformation.FindAllAsync(genericUUID))
                {
                    BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromIdAsync(di.Id);
                    lstDevices.Items.Add(new MyBluetoothLEDevice(bleDevice));
                }
            }
            catch 
            { 
            }
        }

        private void lstDevices_Tapped(object sender, TappedRoutedEventArgs e)
        {
            currentDevice = (lstDevices.SelectedItem as MyBluetoothLEDevice).BluetoothLEDevice;
            LoadGattServices();
        }

        private void LoadGattServices()
        {
            if (currentCapSenseCharacteristic != null)
                currentCapSenseCharacteristic.ValueChanged -= currentCapSenseCharacteristic_ValueChanged;
            currentCapSenseCharacteristic = null;
            currentRGBLedCharacteristic = null;
            lstGattServices.Items.Clear();
            lstCharacteristics.Items.Clear();

            string description = string.Empty;
            foreach (var service in currentDevice.GattServices)
            {
                description = UUIDHelper.GetUUIDDescription(service.Uuid.ToString());
                lstGattServices.Items.Add(new MyGattService(description, service));

                foreach (var characteristic in service.GetAllCharacteristics())
                {
                    if (characteristic.Uuid == Guid.Parse("0000caa2-0000-1000-8000-00805f9b34fb"))
                        AttachToCapSnese(characteristic);
                    else if (characteristic.Uuid == Guid.Parse("0000cbb1-0000-1000-8000-00805f9b34fb"))
                        AttachToRGBLed(characteristic);
                }
            }
        }

        private async void lstGattServices_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Task.Delay(1);

            lstCharacteristics.Items.Clear();
            GattDeviceService selectedService = (lstGattServices.SelectedItem as MyGattService).GattDeviceService;

            foreach (var characteristic in selectedService.GetAllCharacteristics())
            {
                lstCharacteristics.Items.Add(characteristic.UserDescription + " " + characteristic.Uuid);
            }
        }

        private async void lstCharacteristics_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Task.Delay(1);
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.currentDevice = null;
            await ScanDevices();
        }

        private async void AttachToCapSnese(GattCharacteristic characteristic)
        {
            currentCapSenseCharacteristic = characteristic;
            currentCapSenseCharacteristic.ValueChanged += currentCapSenseCharacteristic_ValueChanged;
            GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            if (status == GattCommunicationStatus.Unreachable)
            {
                MessageDialog dialog = new MessageDialog("Sensor not connected or out of range");
                await dialog.ShowAsync();
            }
        }

        private async void currentCapSenseCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var data = new byte[args.CharacteristicValue.Length];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);
            int value = Convert.ToInt32(data[0]);
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                double height = (CapSneseOuterGrid.ActualHeight / 100) * value;
                CapSneseSliderGrid.Height = height;
            });
        }

        private void AttachToRGBLed(GattCharacteristic characteristic)
        {
            currentRGBLedCharacteristic = characteristic;
        }

        private async void SliderBrightness_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (ColorPicker != null)
                await SendRGBLedData(e.NewValue, ColorPicker.RedValue, ColorPicker.GreenValue, ColorPicker.BlueValue);
        }

        private async void ColorPicker_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await SendRGBLedData(BrightnessSlider.Value, ColorPicker.RedValue, ColorPicker.GreenValue, ColorPicker.BlueValue);
        }

        private async Task SendRGBLedData(double brightness, int red, int green, int blue)
        {
            if (currentRGBLedCharacteristic != null)
            {
                byte[] sensorData = new byte[4] { Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue), Convert.ToByte((int)brightness) };
                GattCommunicationStatus status = await currentRGBLedCharacteristic.WriteValueAsync(sensorData.AsBuffer());
                if (status == GattCommunicationStatus.Unreachable)
                {
                    MessageDialog dialog = new MessageDialog("Sensor not connected or out of range");
                    await dialog.ShowAsync();
                }
            }
        }
    }
}

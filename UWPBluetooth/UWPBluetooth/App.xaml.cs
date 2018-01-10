using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth.Advertisement;

namespace UWPBluetooth
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }

            RunBluetooth();
            
        }

        public const string MainPath = @"C:\Bluetooth\";
        public const string logPath = MainPath + "log.txt";
        public List<BluetoothLEAdvertisementReceivedEventArgs> advs = new List<BluetoothLEAdvertisementReceivedEventArgs>();
        public List<DeviceInformation> devs = new List<DeviceInformation>();


        async void RunBluetooth()
        {
            //FileStream console = new FileStream(logPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            //StreamWriter csl = new StreamWriter(console);
            var t = Task.Run(() => GetBand());
            t.Wait();

            await Task.Delay(16000);

            List<BluetoothLEDevice> conDevs = new List<BluetoothLEDevice>();
            foreach(var dev in devs)
            {
                conDevs.Add(await BluetoothLEDevice.FromIdAsync(dev.Id));
            }
            List<BluetoothLEDevice> conAdvs = new List<BluetoothLEDevice>();
            foreach(var adv in advs)
            {
                conAdvs.Add(await BluetoothLEDevice.FromBluetoothAddressAsync(adv.BluetoothAddress));
            }

            List<GattDeviceService> services = new List<GattDeviceService>();
            List<GattCharacteristic> characteristics = new List<GattCharacteristic>();
            List<GattDescriptor> descriptors = new List<GattDescriptor>();
            foreach(var dev in conDevs)
            {
                Debug.WriteLine(dev.DeviceInformation.Name + " - " + dev.DeviceInformation.Kind.ToString());
                var result = await dev.GetGattServicesAsync();
                if (result.Status == GattCommunicationStatus.Success)
                {
                    Debug.WriteLine("Services:");
                    services.AddRange(result.Services);
                    foreach(var service in result.Services)
                    {
                        Debug.WriteLine("    " + service.Uuid);
                        var chrs = await service.GetCharacteristicsAsync();
                        if (chrs.Status == GattCommunicationStatus.Success)
                        {
                            characteristics.AddRange(chrs.Characteristics);
                            foreach(var chr in chrs.Characteristics)
                            {
                                string temp;
                                try
                                {
                                    var res = (await chr.ReadValueAsync(BluetoothCacheMode.Cached)).Value;
                                    byte[] bytes = new byte[res.Capacity];
                                    res.CopyTo(bytes);
                                    temp = System.Text.Encoding.ASCII.GetString(bytes);
                                }
                                catch { temp = "READ_ERROR"; }

                                Debug.WriteLine("        " + chr.Uuid.ToString() + " :=: " + temp);
                                Debug.WriteLine("        Properties: " + chr.CharacteristicProperties.ToString());

                                var descs = await chr.GetDescriptorsAsync();
                                if (descs.Status == GattCommunicationStatus.Success)
                                {
                                    descriptors.AddRange(descs.Descriptors);
                                    foreach (var desc in descs.Descriptors)
                                    {
                                        temp = null;
                                        try
                                        {
                                            var res = (await desc.ReadValueAsync(BluetoothCacheMode.Cached)).Value;
                                            byte[] bytes = new byte[res.Capacity];
                                            res.CopyTo(bytes);
                                            temp = System.Text.Encoding.ASCII.GetString(bytes);
                                        }
                                        catch { temp = "READ_ERROR"; }

                                        Debug.WriteLine("            " + desc.Uuid.ToString() + " :=: " + temp);
                                    }
                                }
                                else
                                    Debug.WriteLine("        STATUS - " + descs.Status);
                                Debug.WriteLine("\r\n");

                            }
                        }
                        else
                            Debug.WriteLine("    STATUS - " + chrs.Status);
                    }
                }
                else
                    Debug.WriteLine("STATUS - " + result.Status.ToString());

                Debug.WriteLine("\r\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\r\n");
            }
        }

        public async void GetBand()
        {
            /*
            var selector = BluetoothDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync();
            foreach (var device in devices)
            {
                if (device.Name.Contains("Band"))
                {
                    foreach (var prop in device.GetType().GetProperties())
                        lstConsole.Items.Add(prop.Name + " - " + prop.GetValue(device, null));

                    DevicePairingResult dpr = await device.Pairing.PairAsync();
                    lstConsole.Items.Add("Pairing Status - " + dpr.Status.ToString());
                    lstConsole.Items.Add("");
                }
            }
            */
            
            BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();
            watcher.ScanningMode = BluetoothLEScanningMode.Active;
            
            watcher.Received += new TypedEventHandler<BluetoothLEAdvertisementWatcher, BluetoothLEAdvertisementReceivedEventArgs>((w, e) => {
                if (!advs.Any(a => a.BluetoothAddress == e.BluetoothAddress))
                {
                    advs.Add(e);

                    Debug.WriteLine(e.BluetoothAddress + " - " + e.Advertisement.ManufacturerData);
                    Debug.WriteLine("    Signal Strength: " + e.RawSignalStrengthInDBm.ToString());
                    if (e.Advertisement.Flags != null)
                        Debug.WriteLine("    Flags: " + e.Advertisement.Flags.Value);
                    Debug.WriteLine("    Service IDs:");
                    //FileStream fs = new FileStream(MainPath + e.Advertisement.LocalName + ".txt", FileMode.Append, FileAccess.Write);
                    //StreamWriter sw = new StreamWriter(fs);
                    foreach (var id in e.Advertisement.ServiceUuids)
                    {
                        Debug.WriteLine("       " + id.ToString());
                    }
                    foreach (var data in e.Advertisement.DataSections)
                    {
                        Debug.Write("Data: " + System.Text.Encoding.ASCII.GetString(data.Data.ToArray()));
                    }
                    //sw.Dispose();
                    //fs.Dispose();
                    Debug.WriteLine("\r\n");

                    
                }
            });

            DeviceWatcher dWatch = DeviceInformation.CreateWatcher();
            dWatch.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>((w, e) => {
                if (!devs.Any(d => d.Id == e.Id) && e.Name.Contains("Band"))
                {
                    devs.Add(e);
                    Debug.WriteLine(e.Name + " - " + e.Kind);

                    Debug.WriteLine("    ID: " + e.Id);

                    Debug.WriteLine("    Properties:");
                    foreach (var prop in e.Properties)
                    {
                        Debug.WriteLine("        " + prop.Key + " :=: " + prop.Value);
                    }
                    Debug.WriteLine("\r\n");
                }
            });

            dWatch.Updated += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>((w, e) =>
            {
                foreach(var dev in devs)
                {
                    if (dev.Id == e.Id)
                        dev.Update(e);
                }
            });

            dWatch.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, object>((w, o) =>
            {
                dWatch.Stop();
                watcher.Start();
            });

            dWatch.Start();

            
            
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}

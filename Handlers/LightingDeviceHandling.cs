﻿using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Devices.Lights;

namespace Dynamic_Lighting_Key_Indicator
{
    public sealed partial class MainWindow : Window
    {
        private async Task<LampArrayInfo?> AttachToDevice_Async(DeviceInformation device)
        {
            var lampArray = await LampArray.FromIdAsync(device.Id); // This actually takes control of the device
            var info = new LampArrayInfo(device.Id, device.Name, lampArray);

            if (info.lampArray == null)
            {
                // Update on UI thread
                DispatcherQueue.TryEnqueue(() =>
                {
                    ViewModel.DeviceStatusMessage = new DeviceStatusInfo(DeviceStatusInfo.Msg.ErrorInitializing, suffix: info.displayName);
                });
                return null;
            }

            // Set up the AvailabilityChanged event callback
            info.lampArray.AvailabilityChanged += LampArray_AvailabilityChanged;

            // Add to the list (thread-safe)
            AttachedDevice = info;

            // Update UI on the UI thread
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdateAttachedLampArrayDisplayList();
            });

            // Set user config device ID
            currentConfig.DeviceId = device.Id;

            // Initialize the keyboard hook and callback to monitor key states
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            KeyStatesHandler.InitializeRawInput(hwnd);


            if (info != null)
                ColorSetter.BuildMonitoredKeyIndicesDict(info.lampArray);

            return info;
        }


        private static async Task<BindingList<DeviceInformation>> FindKeyboardLampArrayDevices()
        {
            string keyboardSelector = HidDevice.GetDeviceSelector((ushort)WinEnums.HIDUsagePage.HID_USAGE_PAGE_GENERIC, (ushort)WinEnums.HIDGenericDesktopUsage.HID_USAGE_GENERIC_KEYBOARD);
            string lampArraySelector = LampArray.GetDeviceSelector();

            // Get both sets of devices
            var keyboardDevices = await DeviceInformation.FindAllAsync(keyboardSelector);
            var lampArrayDevices = await DeviceInformation.FindAllAsync(lampArraySelector);

            var keyboardDict = new Dictionary<string, DeviceInformation>();
            foreach (var keyboardDevice in keyboardDevices)
            {
                var containerId = keyboardDevice.Properties["System.Devices.ContainerId"].ToString();
                if (containerId != null)
                {
                    keyboardDict.Add(containerId, keyboardDevice);
                }
            }

            var lampArrayDevicesDict = new Dictionary<string, DeviceInformation>();
            foreach (var lampArrayDevice in lampArrayDevices)
            {
                var containerId = lampArrayDevice.Properties["System.Devices.ContainerId"].ToString();
                if (containerId != null && !lampArrayDevicesDict.ContainsKey(containerId)) // Check if it's not null and not already in the dictionary
                {
                    lampArrayDevicesDict.Add(containerId, lampArrayDevice);
                }
            }

            // Find devices that have both interfaces by comparing their container IDs
            BindingList<DeviceInformation> matchingDevices = [];
            foreach (var containerId in keyboardDict.Keys.Intersect(lampArrayDevicesDict.Keys))
            {
                matchingDevices.Add(lampArrayDevicesDict[containerId]);
            }

            return matchingDevices;
        }

        private static async Task<string> GetKeyboardLampArrayDeviceSelectorAsync()
        {
            BindingList<DeviceInformation> matchingDevices = await FindKeyboardLampArrayDevices();

            if (matchingDevices.Count == 0)
            {
                return ""; // No matching devices found
            }

            string lampArraySelector = LampArray.GetDeviceSelector();

            // Construct combination of lamparrayselector and container id
            int deviceIndex = 0;

            string newSelector = lampArraySelector + " AND System.Devices.ContainerId:(";
            foreach (var device in matchingDevices)
            {
                if (deviceIndex != 0)
                {
                    newSelector += " OR ";
                }
                newSelector += "={" + device.Properties["System.Devices.ContainerId"].ToString() + "}";
                deviceIndex++;
            }
            newSelector += ")";

            return newSelector;
        }

        // -------------------------------------- CUSTOM EVENT HANDLERS --------------------------------------

        // The AvailabilityChanged event will fire when this calling process gains or loses control of RGB lighting
        // for the specified LampArray.
        private void LampArray_AvailabilityChanged(LampArray sender, object args)
        {
            // Update UI on the UI thread
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdateAttachedLampArrayDisplayList();
            });

            UpdateStatusMessage();
        }

        private void Watcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (AttachedDevice == null)
                return;

            lock (AttachedDevice)
            {
                AttachedDevice = null;
            }

            // Update UI on the UI thread
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdateAttachedLampArrayDisplayList();
                UpdatAvailableLampArrayDisplayList();
            });
        }

        private void Watcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            string addedArrayID = args.Id;
            string addedArrayName = args.Name;

            if (addedArrayID == null)
            {
                return;
            }

            availableDevices.Add(args);

            // Don't update the UI until enumeration is done to avoid interupting the creation of availableDevices
            if (m_deviceWatcher == null || m_deviceWatcher.Status != DeviceWatcherStatus.EnumerationCompleted)
            {
                return;
            }

            // Update UI on the UI thread. Only update the available devices since we might not attach to it.
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdatAvailableLampArrayDisplayList();
            });
        }

        private void OnEnumerationCompleted(DeviceWatcher sender, object args)
        {
            DispatcherQueue.TryEnqueue(() => AttachToSavedDevice());
        }

        private void OnDeviceWatcherStopped(DeviceWatcher sender, object args)
        {
            Console.WriteLine("DeviceWatcher stopped.");
            ViewModel.DeviceWatcherStatusMessage = "DeviceWatcher Status: Stopped.";

            if (KeyStatesHandler.rawInputWatcherActive == true)
            {
                KeyStatesHandler.CleanupInputWatcher(); // Stop the keyboard hook 
            }
        }
    }
}

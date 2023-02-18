using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

namespace WitSensor
{
    public static class WitSensorNativeInterface
    {
        public static Dictionary<int, string> BrowsedDeviceDict => s_browsedDeviceDict;

        public static Dictionary<int, string> ConnectedDeviceDict => s_connectedDeviceDict;

        /// <summary>
		/// The dict stores the nearby bluetooth devices found in the current scanning.
		/// </summary>
        private static readonly Dictionary<int, string> s_browsedDeviceDict = new();

        /// <summary>
		/// The dict stores the currently connected bluetooth devices.
		/// </summary>
        private static readonly Dictionary<int, string> s_connectedDeviceDict = new();

        [DllImport("__Internal")]
        private static extern void WitSensor_InitCallbacks(Action<int, string> onFoundBluetoothDevice,
                                                           Action<int, string> onBluetoothDeviceConnected,
                                                           Action<int, string> onBluetoothDeviceDisconnected,
                                                           Action<int, IntPtr> onReceivedBluetoothDeviceData);

        /// <summary>
		/// Start scanning for nearby bluetooth devices.
		/// </summary>
        [DllImport("__Internal")]
        private static extern void WitSensor_StartScanning();

        /// <summary>
		/// Stop scanning for nearby bluetooth devices.
		/// </summary>
        [DllImport("__Internal")]
        private static extern void WitSensor_StopScanning();

        /// <summary>
		/// Returns whether the phone is currently scanning for bluetooth devices.
		/// </summary>
		/// <returns>Is the phone scanning</returns>
        [DllImport("__Internal")]
        private static extern bool WitSensor_IsScanning();

        /// <summary>
		/// Try to connect to a scanned bluetooth device.
		/// </summary>
		/// <param name="deviceKey">The key of the scanned device in the browsed dict</param>
        [DllImport("__Internal")]
        private static extern void WitSensor_ConnectDevice(int deviceKey);

        /// <summary>
		/// Links to a native callback which is invoked when the phone finds a bluetooth device.
		/// </summary>
		/// <param name="deviceKey">Device key in the dict</param>
		/// <param name="deviceName">Device name</param>
        [AOT.MonoPInvokeCallback(typeof(Action<int, string>))]
        private static void OnFoundBluetoothDeviceCallback(int deviceKey, string deviceName)
        {
            s_browsedDeviceDict.Add(deviceKey, deviceName);
            OnFoundBluetoothDevice?.Invoke(deviceKey, deviceName);
        }

        /// <summary>
		/// Links to a native callback which is invoked when a bluetooth device has connected.
		/// </summary>
		/// <param name="deviceKey">Device key in the connected device dict</param>
		/// <param name="deviceName">The name of the device</param>
        [AOT.MonoPInvokeCallback(typeof(Action<int, string>))]
        private static void OnBluetoothDeviceConnectedCallback(int deviceKey, string deviceName)
        {
            if (s_browsedDeviceDict.ContainsKey(deviceKey))
            {
                s_browsedDeviceDict.Remove(deviceKey);
            }

            s_connectedDeviceDict.Add(deviceKey, deviceName);
            OnBluetoothDeviceConnected?.Invoke(deviceKey, deviceName);
        }

        /// <summary>
		/// Links to a native callback which is invoked when a bluetooth device has disconnected.
		/// </summary>
		/// <param name="deviceKey">Device key in the connected device dict</param>
		/// <param name="deviceName">The name of the device</param>
        [AOT.MonoPInvokeCallback(typeof(Action<int, string>))]
        private static void OnBluetoothDeviceDisconnectedCallback(int deviceKey, string deviceName)
        {
            
        }

        /// <summary>
		/// Links to a native callback which is invoked when the phone receives data from a connected bluetooth device.
		/// </summary>
		/// <param name="deviceKey">Device key in the connected device dict</param>
		/// <param name="dataPtr">The raw data pointer</param>
        [AOT.MonoPInvokeCallback(typeof(Action<int, IntPtr>))]
        private static void OnReceivedBluetoothDeviceDataCallback(int deviceKey, IntPtr dataPtr)
        {
            float[] data = new float[8];
            Marshal.Copy(dataPtr, data, 0, 8);
            Vector3 acceleration = new(data[0], data[1], data[2]);
            Vector3 angle = new(data[3], data[4], data[5]);
            float electricity = data[6];
            float temperature = data[7];
            OnReceivedBluetoothDeviceData?.Invoke(deviceKey, acceleration, angle, electricity, temperature);
        }

        /// <summary>
		/// Invoked when the phone finds a nearby bluetooth device.
		/// The first parameter is the device key in the browsed device dict.
		/// The second parameter is the name of the device.
		/// </summary>
        public static event Action<int, string> OnFoundBluetoothDevice;

        /// <summary>
		/// Invoked when a bluetooth device has connected.
		/// The first parameter is the device key in the connected device list.
		/// The second paramter is the name of the device.
		/// </summary>
        public static event Action<int, string> OnBluetoothDeviceConnected;

        /// <summary>
		/// Invoked when a bluetooth device has disconnected.
		/// The first parameter is the device key in the connected device list.
		/// The second paramter is the name of the device.
		/// </summary>
        public static event Action<int, string> OnBluetoothDeviceDisconnected;

        /// <summary>
        /// Invoked when receives a bluetooth device data message,.
        /// The first parameter is the device key in the connected device list.
        /// The second parameter is the accleration vector.
        /// The third parameter is the angle vector.
        /// The fourth parameter is the device electricity percentage.
        /// The fifth parameter is the device temperature.
        /// </summary>
        public static event Action<int, Vector3, Vector3, float, float> OnReceivedBluetoothDeviceData;

        public static void InitCallbacks()
        {
            WitSensor_InitCallbacks(OnFoundBluetoothDeviceCallback,
                                    OnBluetoothDeviceConnectedCallback,
                                    OnBluetoothDeviceDisconnectedCallback,
                                    OnReceivedBluetoothDeviceDataCallback);
        }

        public static void StartScanning()
        {
            WitSensor_StartScanning();
        }

        public static void StopScanning()
        {
            WitSensor_StopScanning();
        }

        public static bool IsScanning()
        {
            return WitSensor_IsScanning();
        }

        public static void ConnectDevice(int deviceKey)
        {
            WitSensor_ConnectDevice(deviceKey);
        }
    }
}

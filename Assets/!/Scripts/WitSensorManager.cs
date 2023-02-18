using UnityEngine;
using WitSensor;

public class WitSensorManager : MonoBehaviour
{
    private void Start()
    {
        WitSensorNativeInterface.InitCallbacks();
        WitSensorNativeInterface.OnFoundBluetoothDevice += OnFoundBluetoothDevice;
    }

    private void OnDestroy()
    {
        WitSensorNativeInterface.OnFoundBluetoothDevice -= OnFoundBluetoothDevice;
    }

    private void OnFoundBluetoothDevice(int deviceKey, string deviceName)
    {
        Debug.Log($"[WitSensorManager] OnFoundBluetoothDevice deviceKey: {deviceKey} deviceName: {deviceName}");
        WitSensorNativeInterface.ConnectDevice(deviceKey);
    }

    public void StartScanning()
    {
        WitSensorNativeInterface.StartScanning();
    }

    public void StopScanning()
    {
        WitSensorNativeInterface.StopScanning();
    }
}

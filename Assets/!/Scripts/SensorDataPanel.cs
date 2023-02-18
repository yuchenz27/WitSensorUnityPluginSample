using UnityEngine;
using TMPro;
using WitSensor;

public class SensorDataPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _deviceNameText;

    [SerializeField] private TMP_Text _deviceKeyText;

    [SerializeField] private TMP_Text _elecText;

    [SerializeField] private TMP_Text _tempText;

    [SerializeField] private TMP_Text _accText;

    [SerializeField] private TMP_Text _angText;

    private void Start()
    {
        WitSensorNativeInterface.OnReceivedBluetoothDeviceData += OnReceivedBluetoothDeviceData;
    }

    private void OnDestroy()
    {
        WitSensorNativeInterface.OnReceivedBluetoothDeviceData -= OnReceivedBluetoothDeviceData;
    }

    private void OnReceivedBluetoothDeviceData(int deviceKey, Vector3 acceleration, Vector3 angle, float electricity, float temperature)
    {
        if (WitSensorNativeInterface.ConnectedDeviceDict.ContainsKey(deviceKey))
        {
            _deviceNameText.text = "Device Name: " + WitSensorNativeInterface.ConnectedDeviceDict[deviceKey];
        }
        _deviceKeyText.text = "Device Key: " + deviceKey.ToString();
        _elecText.text = "Elec: " + electricity.ToString("F2");
        _tempText.text = "Temp: " + temperature.ToString("F2");
        _accText.text = "Acc: " + acceleration.ToString("F2");
        _angText.text = "Ang: " + angle.ToString("F2");
    }
}

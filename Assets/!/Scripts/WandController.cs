using UnityEngine;
using WitSensor;

public class WandController : MonoBehaviour
{
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
        transform.rotation = Quaternion.Euler(-angle.x, -angle.z, -angle.y);

        Debug.Log($"angle.magnitude: {acceleration.magnitude}");
    }
}

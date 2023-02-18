using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WitSensor;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Button _startScanningButton;

    [SerializeField] private Button _stopScanningButton;

    private void Update()
    {
        if (WitSensorNativeInterface.IsScanning())
        {
            _startScanningButton.interactable = false;
            _stopScanningButton.interactable = true;
        }
        else
        {
            _startScanningButton.interactable = true;
            _stopScanningButton.interactable = false;
        }
    }
}

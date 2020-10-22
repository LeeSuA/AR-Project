using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QRCodeReaderDemo : MonoBehaviour {

    private IReader QRReader;
    public TMP_Text resultText;

    // Use this for initialization
    private void Start()
    {
        QRReader = new QRCodeReader();
        QRReader.Camera.Play();
        QRReader.StatusChanged += QRReader_StatusChanged;
    }

    private void QRReader_StatusChanged(object sender, System.EventArgs e)
    {
        resultText.text = "Status: " + QRReader.Status;
    }
    
    // Update is called once per frame
    void Update () {

        if (QRReader == null)
        {
            return;
        }

        QRReader.Update();
	}

    public void StartScanning()
    {
        if (QRReader == null)
        {
            Debug.LogWarning("No valid camera - Click Start");
            return;
        }

        // Start Scanning
        QRReader.Scan((barCodeType, barCodeValue) => {
            QRReader.Stop();
            SingletonManager.drillCode = barCodeValue;
            resultText.text = "Found: [" + barCodeType + "] " + "<b>" + SingletonManager.drillCode +"</b>";

#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        });
    }
}

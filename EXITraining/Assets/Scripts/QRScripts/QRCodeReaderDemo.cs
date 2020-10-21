using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QRCodeReaderDemo : MonoBehaviour {

    private IReader QRReader;
    public TMP_Text resultText;


	void Awake () {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
	}

    // Use this for initialization
    private void Start()
    {
        QRReader = new QRCodeReader();
        QRReader.Camera.Play();
        QRReader.StatusChanged += QRReader_StatusChanged;

        StartScanning();
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
            resultText.text = "Found: [" + barCodeType + "] " + "<b>" + barCodeValue +"</b>";

#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        });
    }
}

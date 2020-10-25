using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using BarcodeScanner;
using BarcodeScanner.Scanner;
using UnityEngine.UI;
using TMPro;

public class MakeDrillManager : MonoBehaviour
{

    public GameObject arSession;
    public GameObject arSession_Origin;
    public GameObject arCam;

    public GameObject markerPrefab_start;

    public GameObject cntText;
    public GameObject buttons;

    public GameObject initialGuide; // 훈련생성시작안내창
    public GameObject finishAlert; // 훈련생성완료안내창

    // for QR
    private IScanner codeScanner;
    public RawImage image;

    [SerializeField]
    bool qrWasRead = false;

    private List<GameObject> markerObjects = new List<GameObject>();

    private List<Vector4> markersPosition = SingletonManager.markersPosition;
 
    private void Start()
    {
        // for QR Read
        arSession_Origin.SetActive(false);
        arSession.SetActive(false);

        codeScanner = new Scanner();
        codeScanner.Camera.Play();
        codeScanner.OnReady += StartReadingQR;
        //

        markersPosition.RemoveRange(0, markersPosition.Count);
    }
    private void ScannerUpdate()
    {
        if (!qrWasRead) codeScanner.Update();
        else return;
    }

    private void Update()
    {
        ScannerUpdate();
    }

    private void StartReadingQR(object sender, System.EventArgs e)
    {
        // Set Orientation & Texture
        image.texture = codeScanner.Camera.Texture;

        RectTransform rect = image.GetComponent<RectTransform>();
        float width = image.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        float height = image.transform.parent.GetComponent<RectTransform>().sizeDelta.y;
        float screenRatio = height / width;
        float cameraRatio = (float)codeScanner.Camera.Width / (float)codeScanner.Camera.Height;
        float dif_ratio = screenRatio - cameraRatio;

        rect.sizeDelta = new Vector2(height, height / cameraRatio);

        Debug.Log(rect.localScale);
        image.transform.localEulerAngles = new Vector3(0,0,-90);

    }

    public void ScanCode()
    {
        codeScanner.Scan((barCodeType, barCodeValue) => {

            codeScanner.Stop();
            /*
            if ("코드" == barCodeValue)
            {
                qrText.text = barCodeValue;
                QRisCorrect();
            }
            else
            {
                qrText.text = "_" + barCodeValue + "_ 훈련 코드가 올바르지 않습니다. 훈련 코드 _" + SingletonManager.drillCode + "_";
                StartCoroutine(wait());
            }
            */
            QRisCorrect();
        });
    }
    IEnumerator wait()
    {
        for(float i=0; i<10; i+= Time.deltaTime*2)
        {
            yield return null;
        }
        ScanCode();
    }

    public void QRisCorrect()
    {
        codeScanner.Stop();
        arSession_Origin.SetActive(true);
        arSession.SetActive(true);
        initialGuide.SetActive(false);
        InitializePosition();
        image.gameObject.SetActive(false);
        buttons.SetActive(true);
        MarkerAdd(markerPrefab_start);
        qrWasRead = true;
    }

    public void InitializePosition()
    {
        arSession_Origin.transform.Rotate(0, -arCam.transform.rotation.eulerAngles.y, 0);
        arSession_Origin.transform.position -= arCam.transform.position;
    }
    
    public void MarkerAdd(GameObject mp)
    {
        Vector3 addingPos = arCam.transform.position + arCam.transform.forward * 0.4f - Vector3.up * 0.2f;
        markersPosition.Add(addingPos);
        markerObjects.Add( Instantiate(mp, addingPos, Quaternion.Euler(90, 0, 0) ) );

        cntText.GetComponent<TMP_Text>().text = markerObjects.Count.ToString();
    }

    public void FireAdd(GameObject mp)
    {
        Vector3 addingPos = arCam.transform.position + arCam.transform.forward * 0.4f - Vector3.up * 1.4f;
        Vector4 firePosition = new Vector4(addingPos.x, addingPos.y, addingPos.z, 1);
        markersPosition.Add(firePosition);
        markerObjects.Add(Instantiate(mp, addingPos, Quaternion.Euler(0, 0, 0)));

#if ANDROID
        Handheld.Vibrate();
#endif

        cntText.GetComponent<TMP_Text>().text = markerObjects.Count.ToString();
    }

    public void MarkerAdd_EndDrill(GameObject mp)
    {   
        MarkerAdd(mp);
        FinishMakingDrill(markersPosition.Count);
    }

    public void MarkerDelete()
    {
        if (markersPosition.Count > 1) // 첫째 마커는 못지워!
        {
            markersPosition.RemoveAt(markersPosition.Count - 1);
            Destroy(markerObjects[markerObjects.Count - 1]);
            markerObjects.RemoveAt(markerObjects.Count - 1);
            cntText.GetComponent<TMP_Text>().text = markerObjects.Count.ToString();
#if ANDROID
            Handheld.Vibrate();
#endif
        }
    }
    
    public void FinishMakingDrill(int cnt)
    {
        finishAlert.SetActive(true);
    }
    

}

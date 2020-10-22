using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using BarcodeScanner;
using BarcodeScanner.Scanner;
using UnityEngine.UI;

public class MakeDrillManager : MonoBehaviour
{

    public ARTrackedImageManager trackedImageManager;
    public GameObject arSession;
    public GameObject arSession_Origin;
    public GameObject arCam;
    public GameObject markerPrefab_start;
    public GameObject cntText;
    public GameObject buttons;

    public GameObject initialGuide; // 훈련생성시작안내창
    public GameObject finishAlert; // 훈련생성완료안내창

    private IScanner codeScanner;
    public TMP_Text qrText = null;
    public RawImage image;

    private List<GameObject> markerObjects = new List<GameObject>();
    private List<GameObject> fireObjects = new List<GameObject>();

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
    
    private void Update()
    {
        // QR Reader의 Update
        if (codeScanner != null)
        {
            codeScanner.Update();
        }
        else
        {
            qrText.text = "null";
        }
    }

    public void StartReadingQR(object sender, System.EventArgs e)
    {
        // Set Orientation & Texture
        image.transform.localEulerAngles = codeScanner.Camera.GetEulerAngles();
        image.transform.localScale = codeScanner.Camera.GetScale();
        image.texture = codeScanner.Camera.Texture;
        image.transform.localScale = new Vector3((float)Screen.height / (float)Screen.width, (float)Screen.width / (float)Screen.height, 1);
        image.transform.localEulerAngles = new Vector3(0,0,-90);

        scanCode();

        //Keep Image Aspect Ratio
        var rect = image.GetComponent<RectTransform>();
        var newHeight = rect.sizeDelta.x * codeScanner.Camera.Height / codeScanner.Camera.Width;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

    }

    private void scanCode()
    {
        codeScanner.Scan((barCodeType, barCodeValue) => {

            codeScanner.Stop();
            if (barCodeValue == SingletonManager.drillCode)
            {
                qrText.text = barCodeValue;
                QRisCorrect();
            }
        });
    }

    private void QRisCorrect()
    {
        initialGuide.SetActive(false);
        arSession_Origin.SetActive(true);
        arSession.SetActive(true);
        InitializePosition();
        buttons.SetActive(true);
        MarkerAdd(markerPrefab_start);
        Destroy(image);
    }

    public void InitializePosition()
    {
        arSession_Origin.transform.Rotate(0, -arCam.transform.rotation.eulerAngles.y, 0);
        arSession_Origin.transform.position -= arCam.transform.position;
    }
    
    public void MarkerAdd(GameObject mp)
    {
        markersPosition.Add(arCam.transform.position);
        markerObjects.Add( Instantiate(mp, arCam.transform.position, Quaternion.Euler(90, 0, 0) ) );

#if ANDROID
        Handheld.Vibrate();
#endif

        cntText.GetComponent<TMP_Text>().text = markerObjects.Count.ToString();
    }

    public void FireAdd(GameObject mp)
    {
        Vector3 addingPos = arCam.transform.position;
        Vector4 firePosition = new Vector4(addingPos.x, addingPos.y, addingPos.z, 1);
        markersPosition.Add(firePosition);
        fireObjects.Add(Instantiate(mp, arCam.transform.position - Vector3.up * 1.3f, Quaternion.Euler(0, 0, 0)));

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

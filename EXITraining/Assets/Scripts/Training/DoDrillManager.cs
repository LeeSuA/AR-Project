using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BarcodeScanner;
using BarcodeScanner.Scanner;

public class DoDrillManager : MonoBehaviour
{
    public GameObject arSession;
    public GameObject arSession_Origin;
    public GameObject arCam;
    public GameObject arrow; //안내할 화살표
    // 생성 프리팹
    public GameObject points; // 저장포인트 시각화할 프리팹
    public GameObject fires; // 불 시각화할 프리팹

    public GameObject fireExtinguisher;
    public GameObject FEControllPad;
    private FE_Setting extinguisherSetting;

    public GameObject initialGuide; // 훈련시작안내창
    public GameObject completed; // 훈련종료안내창
    
    // for QR
    private IScanner codeScanner;
    public TMP_Text qrText;
    public RawImage image;

    private List<Vector4> markersPosition = SingletonManager.markersPosition; //마커 위치정보 리스트
    private List<GameObject> markerObjects = new List<GameObject>();

    // for Navigation
    public Vector3 nextPosition; //다음 추적할 포인트의 위치값
    public int markerCount = 0; //markersPosition의 인덱스


    [SerializeField]
    bool qrWasRead = false;
    [SerializeField]
    bool nextisFire = false;
    [SerializeField]
    float fireLife = 5f;
    float fireMaxLife = 5f;

    public void QuickCheckPointAdd()
    {
        markersPosition.RemoveRange(0, markersPosition.Count - 1);
        markersPosition.Add(new Vector4(1, 0, 0, 0));
        markersPosition.Add(new Vector4(2, 0, 0, 0));
        markersPosition.Add(new Vector4(3, 0, 1, 0));
        markersPosition.Add(new Vector4(4, 0, 2, 1));
        markersPosition.Add(new Vector4(5, 0, 3, 0));
        markersPosition.Add(new Vector4(5, 0, 4, 0));
    }

    private void Start()
    {
        // for QR Read
        arSession_Origin.SetActive(false);
        arSession.SetActive(false);
        codeScanner = new Scanner();
        codeScanner.Camera.Play();
        codeScanner.OnReady += StartReadingQR;
        extinguisherSetting = fireExtinguisher.GetComponent<FE_Setting>();
    }
    
    private void ScannerUpdate()
    {
        if (codeScanner != null) codeScanner.Update();
    }

    private void Set_Arrow_FE_Positions()
    {
        fireExtinguisher.transform.parent.position = Vector3.Lerp(fireExtinguisher.transform.parent.position, arCam.transform.position + arCam.transform.forward * 0.4f - arCam.transform.up * 0.2f, 0.4f);
        arrow.transform.position = Vector3.Lerp(arrow.transform.position, arCam.transform.position + arCam.transform.forward * 0.4f - arCam.transform.up * 0.2f, 0.4f);
        arrow.transform.LookAt(nextPosition);
    }


    private void FireExtinguishEvent()
    {
        if (Vector3.Distance(arCam.transform.position, nextPosition) <= 3.2f) // 불에 가까이 갔을 때.
        {
            arrow.SetActive(false);
            if (!fireExtinguisher.activeSelf) extinguisherSetting.Set();
            if (Extiguish())
            {
                UpdateNextPoint();
                if (fireExtinguisher.activeSelf) extinguisherSetting.Eliminate();
            }
        }
        else
        {
            if (fireExtinguisher.activeSelf) extinguisherSetting.Eliminate();
            arrow.SetActive(true);
        }
    }

    private void NextIsCheckPoint()
    {
        if (fireExtinguisher.activeSelf) extinguisherSetting.Eliminate();
        arrow.SetActive(true);
        if (Vector3.Distance(arCam.transform.position, nextPosition) <= 0.7f)
        {
            UpdateNextPoint();
        }
    }

    void Update()
    {
        ScannerUpdate();

        // 마커 비어있거나 QR이 아직 안읽힌 경우
        if (markersPosition == null || !qrWasRead)
        {
            return;
        }

        Set_Arrow_FE_Positions();

        if (nextisFire) // 불 이벤트
        {
            FireExtinguishEvent();
        }
        else // 포인트
        {
            NextIsCheckPoint();
        }


        // 마지막
        if (markerCount == markersPosition.Count)
        {
            completed.SetActive(true);
        }

    }

    private void StartReadingQR(object sender, System.EventArgs e)
    {
        // Set Orientation & Texture
        image.texture = codeScanner.Camera.Texture;

        RectTransform rect = image.GetComponent<RectTransform>();
        float screenRatio = (float)Screen.height / (float)Screen.width;
        float cameraRatio = (float)codeScanner.Camera.Width / (float)codeScanner.Camera.Height;
        float dif_ratio = screenRatio - cameraRatio;

        rect.sizeDelta = new Vector2(Screen.height, Screen.height / cameraRatio);
        Debug.Log(rect.localScale);
        image.transform.localEulerAngles = new Vector3(0, 0, -90);

        qrText.text = "스크린 : " + Screen.width + " x " + Screen.height + "\n";
        qrText.text += "카메라 : " + codeScanner.Camera.Width + " x " + codeScanner.Camera.Height + "\n";
        qrText.text += "패널 : " + rect.sizeDelta.y + " x " + rect.sizeDelta.x + "\n";

        ScanCode();

    }

    private void ScanCode()
    {
        codeScanner.Scan((barCodeType, barCodeValue) => {

            codeScanner.Stop();
            qrText.text = barCodeValue;
            QRisRead();

        });
    }

    public void QRisRead()
    {
        codeScanner.Stop();
        arSession_Origin.SetActive(true);
        arSession.SetActive(true);
        initialGuide.SetActive(false);
        //InitializePosition();
        Destroy(image);
        qrWasRead = true;

        MarkersAdd();
        if (markersPosition.Count != 0)
        {
            nextPosition = markersPosition[0];
            markerObjects[0].SetActive(true);
        }
        arrow.SetActive(true);
    }
    


    public void InitializePosition()
    {
        arSession_Origin.transform.Rotate(0, -arCam.transform.rotation.eulerAngles.y, 0);
        arSession_Origin.transform.position -= arCam.transform.position;
    }

    public void MarkersAdd()
    {
        for (int i = 0; i < markersPosition.Count; ++i)
        {
            if (markersPosition[i].w == 0)
            {
                Debug.Log(markersPosition);
                markerObjects.Add(Instantiate(points, markersPosition[i], Quaternion.Euler(90, 0, 0)));
            }
            else
            {
                Debug.Log(markersPosition);
                markerObjects.Add(Instantiate(fires, markersPosition[i], Quaternion.Euler(0, 0, 0)));
            }
        }
    }

    void UpdateNextPoint() //nextPoint를 다음 포인트로 업데이트
    {
        fireLife = fireMaxLife;
        if (markerCount+1 < markersPosition.Count)
        {
            markerObjects[markerCount].SetActive(true);
            nextPosition = markersPosition[markerCount + 1];
            if (markersPosition[markerCount + 1].w == 1) nextisFire = true;
            else nextisFire = false;
        }
        else
        {
            nextPosition = new Vector3(0, 0, 0);
            arrow.SetActive(false);
        }

        if (markerObjects.Count > 1 && markerCount >= 0 && markerCount < markersPosition.Count)
        {
            markerObjects[markerCount].SetActive(false);
        }

        markerCount++;

    }

    bool Extiguish()
    {
        Transform CO2Pivot = fireExtinguisher.transform.GetChild(0).transform;
        GameObject fireParticle = markerObjects[markerCount].transform.GetChild(0).gameObject;
        Debug.Log(fireParticle);
        // 소화기 핸들 누른 상태에서 이산화탄소 피벗이 불에 닿아있을 때.
        if ( Vector3.Distance(CO2Pivot.position, nextPosition) <= 1.5f && FEControllPad.GetComponent<FE_Control>().handle_is_touched)
        {
            Debug.Log("소화중");
            fireLife -= Time.deltaTime * 3f;
            Material mat = fireParticle.GetComponent<ParticleSystemRenderer>().material;
            Debug.Log(mat.GetColor("_TintColor"));
            mat.SetColor("_TintColor", new Color(1,1,1, fireLife/fireMaxLife));
        }
        if (fireLife <= 0f) return true;
        else return false;
    }

}

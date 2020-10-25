using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        if (!qrWasRead) codeScanner.Update();
        else return;
    }


    void Update()
    {

        ScannerUpdate();

        // 마커 비어있거나 QR이 아직 안읽힌 경우
        if (markerObjects == null || !qrWasRead)
        {
            return;
        }

        if (markerCount < markerObjects.Count)
        {
            if (!markerObjects[markerCount].activeSelf)
            {
                markerObjects[markerCount].SetActive(true);
            }
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
            arrow.SetActive(false);
        }
        // AR 트래킹 튀는현상 완화
        TrackingAccuration(0.6f);

    }
    // for tracking accuracy
    Vector3 pos_last = Vector3.zero;
    Vector3 pos_now = Vector3.zero;
    float rot_last = 0;
    float rot_now = 0;

    private void TrackingAccuration(float accuracy)
    {
        // rotation - y축만 고려
        rot_now = arCam.transform.rotation.eulerAngles.y;
        float rot_dif = rot_now - rot_last;
        if (Mathf.Abs(rot_dif) > 90 && Mathf.Abs(rot_dif) < 270)
        {
            arSession_Origin.transform.Rotate(0, -rot_dif, 0);
        }
        //
        
        // position
        pos_now = arCam.transform.position;
        Vector3 pos_dif = pos_now - pos_last;
        if (pos_dif.magnitude > accuracy)
        {
            Debug.Log(pos_dif.magnitude);
            arSession_Origin.transform.position -= pos_dif;
        }

        pos_last = arCam.transform.position;
        rot_last = arCam.transform.rotation.eulerAngles.y;
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
        image.transform.localEulerAngles = new Vector3(0, 0, -90);
        
        ScanCode();

    }

    private void ScanCode()
    {
        codeScanner.Scan((barCodeType, barCodeValue) => {

            codeScanner.Stop();
            QRisRead();

        });
    }

    public void QRisRead()
    {
        codeScanner.Stop();
        arSession_Origin.SetActive(true);
        arSession.SetActive(true);
        initialGuide.SetActive(false);
        InitializePosition();
        image.gameObject.SetActive(false);
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
                markerObjects.Add(Instantiate(points, markersPosition[i], Quaternion.Euler(90, 0, 0)));
            }
            else
            {
                markerObjects.Add(Instantiate(fires, markersPosition[i], Quaternion.Euler(0, 0, 0)));
            }
            markerObjects[i].SetActive(false);
        }
    }

    private void Set_Arrow_FE_Positions()
    {
        fireExtinguisher.transform.parent.position = arCam.transform.position + arCam.transform.forward * 0.4f - arCam.transform.up * 0.3f;
        fireExtinguisher.transform.parent.rotation = arCam.transform.rotation;
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
        if (Vector3.Distance(arCam.transform.position, nextPosition) <= 0.7f)
        {
            UpdateNextPoint();
        }
    }

    void UpdateNextPoint() //nextPoint를 다음 포인트로 업데이트
    {
        fireLife = fireMaxLife;
        if (markerCount+1 < markersPosition.Count)
        {
            arrow.SetActive(true);
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
            mat.SetColor("_TintColor", new Color(1,1,1, fireLife/fireMaxLife));
        }
        if (fireLife <= 0f) return true;
        else return false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Firebase.Auth;

public class StartNavigate : MonoBehaviour
{
    FirebaseAuth auth;
    public ARTrackedImageManager trackedImageManager;
    public GameObject arSession_Origin;
    public GameObject arCam;
    public GameObject arrow; //안내할 화살표
    public GameObject points; // 저장포인트 시각화할 프리팹

    public GameObject initialGuide; // 훈련시작안내창
    public GameObject completed; // 훈련종료안내창


    private List<Vector3> markersPosition = SingletonManager.markersPosition; //마커 위치정보 리스트
    private List<GameObject> markerObjects = new List<GameObject>();
    public Vector3 nextPosition; //다음 추적할 포인트의 위치값
    public int markerCount = 0; //markersPosition의 인덱스


    bool qrRead = false;

    private void Start()
    {
        auth =FirebaseAuth.DefaultInstance;
        trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }

    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        List<ARTrackedImage> addedImages = args.added;
        List<ARTrackedImage> removedImages = args.removed;

        foreach (ARTrackedImage image in addedImages)
        {
            if (image.referenceImage.name == "QR")
            {
                if (markersPosition.Count == 0)
                {
                    OnQRwasRead();
                }
            }
        }

    }

    public void OnQRwasRead()
    {
        qrRead = true;
        MarkersAdd();
        arrow.SetActive(true);
        initialGuide.SetActive(false);
        InitializePosition();
        nextPosition = markersPosition[0];
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
            markerObjects.Add(Instantiate(points, markersPosition[i], Quaternion.Euler(90, 0, 0)));
        }
    }

    void Update()
    {
        // 마커 비어있는 경우 예외
        if (markersPosition == null || !qrRead)
        {
            return;
        }

        // 화살표 위치 카메라에 고정
        arrow.transform.position = arCam.transform.position + arCam.transform.forward * 0.35f - arCam.transform.up * 0.15f;
        arrow.transform.LookAt(nextPosition);

        // 접촉 연산
        if (Vector3.Distance(arCam.transform.position, nextPosition) <= 0.3f)
        {
            UpdateNextPoint();
        }
        
        // 마지막
        if (markerCount == markersPosition.Count)
        {
            completed.SetActive(true);
            bool check = false;
            FirebaseGoogleAuth SetScore = new FirebaseGoogleAuth();
            SetScore.checkDrillCode();
        }

        Navigate(nextPosition);

    }


    void UpdateNextPoint() //nextPoint를 다음 포인트로 업데이트
    {
        if (markerCount+1 < markersPosition.Count)
        {
            nextPosition = markersPosition[markerCount+1];
        }
        else
        {
            nextPosition = new Vector3(9999, 9999, 9999);
            arrow.SetActive(false);
        }

        if (markerObjects.Count > 1 && markerCount >= 0 && markerCount < markersPosition.Count)
        {
            Destroy(markerObjects[markerCount]);
        }

        markerCount++;

#if ANDROID
        Handheld.Vibrate();
#endif
    }


    void Navigate(Vector3 nextPosition)//화살표가 nextPosition을 가리키게 함
    {
        arrow.transform.LookAt(nextPosition);
    }
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class StartNavigate : MonoBehaviour
{ 
    public GameObject arSession_Origin;
    public GameObject arSession; 
    public GameObject arCam;
    public GameObject arrow; //안내할 화살표
    public Vector3 nextPosition; //다음 추적할 포인트의 위치값
    public int markerCount = 0;//markersPosition의 인덱스
    public GameObject a;
    public GameObject completed; //훈련종료

    private List<Vector3> markersPosition = SingletonManager.markersPosition;//마커 위치정보 리스트


    

    public void InitializePosition()
    {
        arSession_Origin.transform.Rotate(0, -arCam.transform.rotation.eulerAngles.y, 0);
        arSession_Origin.transform.position -= arCam.transform.position;
    }

    void Update()
    {
        if (markersPosition.Count > 0)
        {
            if (Vector3.Distance(arCam.transform.position, nextPosition) <= 0.2f)//next포인트와의 거리가 0.2m 이하면 updateNextPoint()
            {
                markerCount++;
                UpdateNextPoint();
            }
        }
        else
        {
            Debug.Log("마커가 없어!");
        }
        

        arrow.transform.position = arCam.transform.position + arCam.transform.forward * 0.25f - arCam.transform.up * 0.3f;
        arrow.transform.LookAt(nextPosition);

        if (markerCount == (markersPosition.Count - 1))
        {
            //훈련 끝
            completed.SetActive(true);
        }

        //훈련종료 창이 떴을때
        if(completed.activeSelf == true)
        {
            //화면 더블 클릭시
            Touch touch = Input.GetTouch(0);

            //앱 종료
            if (touch.phase == TouchPhase.Began)
            {
                SingletonManager.Quit();
            }
        }
        
        

       Navigate(nextPosition);

    }

    public void MarkersAdd()
    {
        for(int i=0; i<markersPosition.Count; ++i)
        {
            Instantiate(a, markersPosition[i], new Quaternion(0,0,0,0));
        }
    }

    void UpdateNextPoint()//nextPoint를 다음 포인트로 업데이트
    {
        nextPosition = markersPosition[markerCount];
    }

    void Navigate(Vector3 nextPosition)//화살표가 nextPosition을 가리키게 함
    {
        arrow.transform.LookAt(nextPosition);
    }
}

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
    public GameObject arrow;//안내할 화살표
    public Vector3 nextPosition;//다음 추적할 포인트의 위치값
    public int markerCount = 0;//markersPosition의 인덱스
    public GameObject a;
    public GameObject complete;
    public GameObject completed; //훈련종료
    public GameObject certify; //체크포인트 도달시 확인

    private List<Vector3> markersPosition = new List<Vector3>();//마커 위치정보 리스트


    // Start is called before the first frame update
    void Start()
    {
       
        arCam.transform.position = arSession_Origin.transform.position;
        arCam.transform.rotation = arSession_Origin.transform.rotation;



        certify.SetActive(false);
        
        arrow.SetActive(true);
        complete.SetActive(false);
        completed.SetActive(false);

        markersPosition.Add(new Vector3(0, 0, 0));
        markersPosition.Add(new Vector3(0.5f, 0.5f, 0));
        markersPosition.Add(new Vector3(1f, -0.5f, 0));
        markersPosition.Add(new Vector3(1f, -2f, -2f));
        markersPosition.Add(new Vector3(2f, -2f, -2f));
        markersPosition.Add(new Vector3(2f, -0.5f, -3f));
        markersPosition.Add(new Vector3(22f, 0.5f, -3f));
        markersPosition.Add(new Vector3(22.5f, -0.5f, -3f));
        markersPosition.Add(new Vector3(22.5f, -2f, -4f));
        markersPosition.Add(new Vector3(23.5f, -2f, -4f));
        markersPosition.Add(new Vector3(23.5f, 0.5f, -5f));
        markersPosition.Add(new Vector3(12f, 0.5f, -5f));
        /*markersPosition.Add(new Vector3(0.8f, -0.1f, 0.3f));
        markersPosition.Add(new Vector3(-1.1f, -0.1f, 1.6f));
        markersPosition.Add(new Vector3(-0.1f, -0.1f, 3.2f));
        markersPosition.Add(new Vector3(4.1f, -0.1f, 0.4f));
        markersPosition.Add(new Vector3(8.1f, -2f, -2f));
        markersPosition.Add(new Vector3(9.1f, -2f, -0.7f));
        markersPosition.Add(new Vector3(4.9f, -3.8f, 2.1f));
        markersPosition.Add(new Vector3(0.9f, -3.7f, 3.9f));
        markersPosition.Add(new Vector3(11.1f, -3.7f, 20.5f));
        markersPosition.Add(new Vector3(23.8f, -3.7f, 40.7f));
        markersPosition.Add(new Vector3(28.1f, -3.6f, 38.1f));
        markersPosition.Add(new Vector3(31.7f, -5.4f, 35.5f));
        markersPosition.Add(new Vector3(30.9f, -5.4f, 34.2f));
        markersPosition.Add(new Vector3(27.1f, -7.3f, 36.4f));
        markersPosition.Add(new Vector3(22.5f, -7.4f, 39.4f));
        markersPosition.Add(new Vector3(10.5f, -7.7f, 20.3f));
        markersPosition.Add(new Vector3(17.5f, -7.3f, 15.6f));*/


        MarkersAdd();     
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(arCam.transform.position, nextPosition) <= 0.2f)//next포인트와의 거리가 0.2m 이하면 updateNextPoint()
        {
            markerCount++;
            updateNextPoint();
            certify.SetActive(true);
        }

        if (certify.activeSelf == true)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                certify.SetActive(false);
            }
        }

        arrow.transform.position = arCam.transform.position + arCam.transform.forward * 0.2f - Vector3.up * 0.4f;
        arrow.transform.LookAt(nextPosition);

        if (markerCount == (markersPosition.Count - 1))
        {
            complete.SetActive(true);
            //훈련 끝
            completed.SetActive(true);
        }

        //훈련종료 창이 떴을때
        if(completed.activeSelf == true)
        {
            //화면 더블 클릭시
            Touch touch = Input.GetTouch(1);

            //앱 종료
            if (touch.phase == TouchPhase.Began)
            {
                Application.Quit();
            }
        }
        
        

       navigate(nextPosition);

    }

    public void MarkersAdd()
    {
        for(int i=0; i<markersPosition.Count; ++i)
        {
            Instantiate(a, markersPosition[i], new Quaternion(0,0,0,0));
        }
    }

    void updateNextPoint()//nextPoint를 다음 포인트로 업데이트
    {
        nextPosition = markersPosition[markerCount];
    }
    void navigate(Vector3 nextPosition)//화살표가 nextPosition을 가리키게 함
    {
      arrow.transform.LookAt(nextPosition);
    }
}

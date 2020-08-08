using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class StartNavigate : MonoBehaviour
{
    public GameObject arSession_Origin;
    public GameObject arSession; 
    public GameObject arCam;
    public GameObject arrow;//안내할 화살표
    public Vector3 nextPosition;//다음 추적할 포인트의 위치값
    public GameObject complete;//훈련이 완료됐을 때 띄울 오브젝트
    public int markerCount = 0;//markersPosition의 인덱스
    public GameObject a;

    private List<Vector3> markersPosition = new List<Vector3>();//마커 위치정보 리스트


    // Start is called before the first frame update
    void Start()
    {
        arCam.transform.position = arSession_Origin.transform.position;
        arCam.transform.rotation = arSession_Origin.transform.rotation;


        arrow.SetActive(true);
        complete.SetActive(false);

        markersPosition.Add(new Vector3(0, 0, 0));
        markersPosition.Add(new Vector3(0, 0, 0.5f));
        markersPosition.Add(new Vector3(0, 0, 1.2f));
        markersPosition.Add(new Vector3(0, 0, 1.8f));
        markersPosition.Add(new Vector3(0, 0, 2.3f));

        MarkersAdd();     
    }

    // Update is called once per frame
    void Update()
    {

        arrow.transform.position = arCam.transform.position + arCam.transform.forward * 0.3f - Vector3.up * 0.2f;

        navigate(nextPosition);

        if(markerCount == (markersPosition.Count - 1))
        {
            //훈련 끝
            complete.SetActive(true);
        }
        
        if (Vector3.Distance(arCam.transform.position, nextPosition) <= 0.2f)//next포인트와의 거리가 0.2m 이하면 updateNextPoint()
        {
            markerCount++;
            updateNextPoint();
        }
        
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class MakeDrillManager : MonoBehaviour
{

    public ARTrackedImageManager trackedImageManager;
    public GameObject arSession_Origin;
    public GameObject arSession;
    public GameObject arCam;
    public GameObject markerPrefab_start;
    public GameObject cntText;
    public GameObject buttons;
    public GameObject initialGuide;

    public GameObject finishAlert;

    private List<GameObject> markerObjects = new List<GameObject>();
    private List<Vector3> markersPosition = SingletonManager.markersPosition;

    private void Start()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }
    
    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        List<ARTrackedImage> addedImages = args.added;
        List<ARTrackedImage> removedImages = args.removed;

        foreach(ARTrackedImage image in addedImages)
        {
            if (image.referenceImage.name == "QR")
            {
                if (markersPosition.Count == 0)
                {
                    InitializePosition();
                    buttons.SetActive(true);
                    MarkerAdd(markerPrefab_start);
                }
            }
        }

    }
    

    public void InitializePosition()
    {
        arSession_Origin.transform.Rotate(0, -arCam.transform.rotation.eulerAngles.y, 0);
        arSession_Origin.transform.position -= arCam.transform.position;
    }
    
    public void MarkerAdd(GameObject mp)
    {
        
        markersPosition.Add(arCam.transform.position);
        markerObjects.Add( Instantiate(mp, (arCam.transform.position + arCam.transform.forward * 0.2f - Vector3.up * 0.35f), Quaternion.Euler(90, 0, 0) ) );

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
        }
    }


    public void FinishMakingDrill(int cnt)
    {
        finishAlert.SetActive(true);
    }
    
}

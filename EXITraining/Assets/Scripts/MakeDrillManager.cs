using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using System;

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
    private List<Vector3> markersPosition = new List<Vector3>();
    public String location;

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
                }
            }
        }

    }

    public void TrackButtonEvent()
    {
        InitializePosition();
    }

    private void InitializePosition()
    {
        arSession_Origin.transform.position = arCam.transform.position;
        arSession_Origin.transform.rotation = arCam.transform.rotation;

        buttons.SetActive(true);
        initialGuide.SetActive(false);

        MarkerAdd(markerPrefab_start);
    }
    
    public void MarkerAdd(GameObject mp)
    {
        markersPosition.Add(arCam.transform.position);
        location = arCam.transform.position.ToString();
        CreateLocation();
        markerObjects.Add( Instantiate(mp, (arCam.transform.position + arCam.transform.forward * 0.3f - Vector3.up * 0.2f), Quaternion.Euler(90, 0, 0) ) );

        cntText.GetComponent<TMP_Text>().text = markerObjects.Count.ToString();       
    }

    public void MarkerAdd_EndDrill(GameObject mp)
    {
        markersPosition.Add(arCam.transform.position);
        location = arCam.transform.position.ToString();
        CreateLocation();
        markerObjects.Add(Instantiate(mp, (arCam.transform.position + arCam.transform.forward * 0.3f - Vector3.up*0.2f), Quaternion.Euler(90, 0, 0)));

        cntText.GetComponent<TMP_Text>().text = markerObjects.Count.ToString();

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

    public void Scenemove()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void CreateLocation()
    {
        Debug.Log(TempDrill.code);
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://exitraining-3962c.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Location").Child(TempDrill.code).Child((markersPosition.Count-1).ToString()).SetValueAsync(location);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class MakeDrillManager : MonoBehaviour
{

    public ARTrackedImageManager trackedImageManager;
    public GameObject arSession_Origin;
    public GameObject arSession;
    public GameObject arCam;
    public GameObject markerPrefab;
    public GameObject cntText;

    public GameObject makeButton, removeButton;

    private List<GameObject> markerObjects = new List<GameObject>();
    private List<Vector3> markersPosition = new List<Vector3>();

    private void Start()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }

    private void Update()
    {
        if(markersPosition.Count <= 1)
        {
            removeButton.SetActive(false);
        }
        else
        {
            removeButton.SetActive(true);
        }


        if(markerObjects.Count > 0)
        {
            cntText.SetActive(true);
            cntText.GetComponent<TMP_Text>().text = "Save Points : " + markerObjects.Count;
        }


    }
    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        List<ARTrackedImage> addedImages = args.added;
        List<ARTrackedImage> removedImages = args.removed;

        foreach(ARTrackedImage image in addedImages)
        {
            if(image.referenceImage.name == "QR")
            {
                InitializePosition();
            }
        }

        cntText.SetActive(true);
    }

    private void InitializePosition()
    {
        if (markersPosition.Count == 0)
        {
            arCam.transform.position = arSession_Origin.transform.position;
            arCam.transform.rotation = arSession_Origin.transform.rotation;
            makeButton.SetActive(true);
            removeButton.SetActive(true);
            MarkerAdd();
        }
    }

    string debstr = null;

    public void MarkerAdd()
    {
        markersPosition.Add(arCam.transform.position);
        markerObjects.Add( Instantiate(markerPrefab, arCam.transform.position, Quaternion.Euler(90, 0, 0) ) );
        debstr += "(" + markersPosition.Count + ") " + markerObjects[markersPosition.Count-1].transform.position.ToString() + "\n";
        Debug.Log(debstr);
    }

    public void MarkerAdd_EndDrill()
    {
        markersPosition.Add(arCam.transform.position);
        markerObjects.Add(Instantiate(markerPrefab, arCam.transform.position, Quaternion.Euler(90, 0, 0)));
        debstr += "(" + markersPosition.Count + ") " + markerObjects[markersPosition.Count - 1].transform.position.ToString() + "\n";
        Debug.Log(debstr);
    }

    public void MarkerDelete()
    {
        if (markersPosition.Count > 1) // 첫째 마커는 못지워!
        {
            markersPosition.RemoveAt(markersPosition.Count - 1);
            Destroy(markerObjects[markerObjects.Count - 1]);
            markerObjects.RemoveAt(markerObjects.Count - 1);
            debstr.Remove(debstr.LastIndexOf("("));
            Debug.Log(debstr);
        }
    }
}

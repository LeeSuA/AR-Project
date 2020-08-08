using UnityEngine;
using System.Collections;

public class PlayerLookAt : MonoBehaviour {


    public GameObject arCam;

	void Update () {
		transform.LookAt(arCam.transform.position);
        Debug.Log(arCam.transform.name);
	}
}

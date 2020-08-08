using UnityEngine;
using System.Collections;

public class PlayerLookAt : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
 		//카메라 쳐다오라우~
		transform.LookAt(Camera.main.transform.position);
	}
}

using System;
using System.Collections;
using UnityEngine;

public class Booting : MonoBehaviour
{
    IEnumerator Start()
    {
        // When the app start, ask for the authorization to use the webcam
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            throw new Exception("This Webcam library can't work without the webcam authorization");
        }
    }
}

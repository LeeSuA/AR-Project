using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class SingletonManager : MonoBehaviour
{
    private FirebaseAuth auth;
    static SingletonManager current = null;
    static GameObject container = null;

    public static SingletonManager Instance
    {
        get
        {
            if(current == null)
            {
                container = new GameObject();
                container.name = "Singleton";
                current = container.AddComponent(typeof(SingletonManager)) as SingletonManager;
                DontDestroyOnLoad(current);
            }
            return current;
        }
    }
    

    public static List<Vector3> markersPosition = new List<Vector3>();
    public static string drillCode = null;
    public static string uID = null;
    
}

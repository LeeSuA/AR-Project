using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonManager : MonoBehaviour
{
    /// -------------------------------

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
    
    /// ----------------------------
    
        // 여기부터 공용 변수 선언

    public static List<Vector4> markersPosition = new List<Vector4>();
    public static string drillCode = null;
    
}

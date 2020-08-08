using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonManager : MonoBehaviour
{
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

    public static void Quit()
    {
        Application.Quit();
    }

    public static void SceneSwitch(string s)
    {
        SceneManager.LoadScene(s);
    }

    public static List<Vector3> markersPosition = null;
    public static string drillCode = null;
    
}

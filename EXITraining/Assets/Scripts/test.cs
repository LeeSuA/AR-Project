using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Unity.Editor;
using Firebase;
using Firebase.Database;
using TMPro;
public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   


    public void CreateDB()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://exitraining-3962c.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("User").SetValueAsync("null");

    }
}

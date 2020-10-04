using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Unity.Editor;
using Firebase;
using Firebase.Database;
using TMPro;

public class CreateCode : MonoBehaviour
{
    public GameObject input;
    private string code;

    public void CreateDB()
    {
        code = input.GetComponent<TMP_Text>().text;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://exitraining-3962c.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("DrillCode").Child(code).SetValueAsync("null");

    }
}

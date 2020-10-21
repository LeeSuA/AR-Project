using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase.Unity.Editor;
using Firebase;
using Firebase.Database;
using TMPro;
public class SignUp : MonoBehaviour
{
    private FirebaseAuth auth;
    public GameObject emailInput;
    public GameObject passInput;
 
    // Start is called before the first frame update
   private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Join()

    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://exitraining-3962c.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
   
        string email = emailInput.gameObject.GetComponent<TMP_Text>().text;
        string pass = passInput.gameObject.GetComponent<TMP_Text>().text;
 
        if (email.Length != 0 && pass.Length != 0)
        {
            // 이메일과 비밀번호로 가입하는 함수
            auth.CreateUserWithEmailAndPasswordAsync(email, pass).ContinueWith(
             task =>
             {
                 if (!task.IsCanceled && !task.IsFaulted)
                 {
                     Debug.Log(email + " 로 회원가입 하셨습니다.");
                     reference.Child("User").Child(auth.CurrentUser.UserId).Child("count").SetValueAsync(0);
                     reference.Child("User").Child(auth.CurrentUser.UserId).Child("completeDrills").SetValueAsync("null");
                     SceneManager.LoadScene(0);
                 }
                 else
                 {
                     Debug.Log("회원가입에 실패하셨습니다.");
                 }
             }
         );
        }
    }
}

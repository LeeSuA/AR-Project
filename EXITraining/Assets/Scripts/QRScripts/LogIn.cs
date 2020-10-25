using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using TMPro;
public class LogIn : MonoBehaviour
{
    private FirebaseAuth auth;
    public GameObject emailInput;
    public GameObject passInput;
    private string email;
    private string pass;
    // Start is called before the first frame update
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
     

    }

    // Update is called once per frame
    void Update()
    {

    }

   public void SignIn()
    {
        string email = emailInput.gameObject.GetComponent<TMP_Text>().text;
        string pass = passInput.gameObject.GetComponent<TMP_Text>().text;
    

        auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWith(
                task =>
                {
                    if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                    {
                        Debug.Log(email + " 로 로그인 하셨습니다.");
                        SingletonManager.uID=auth.CurrentUser.UserId;
                        SceneManager.LoadScene("InitialScene");

                    }
                    else
                    {
                        Debug.Log("로그인에 실패하셨습니다.");
                    }
                }
            );
    }
    public void activeObjects()
    {

    }
}


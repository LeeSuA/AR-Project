using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Auth;

public class FirebaseGoogleAuth : MonoBehaviour
{
    private FirebaseAuth auth;
    public GameObject googleText;
    public GameObject ranking;
    public GameObject LogOut;
    bool bWait = false;

        void Start()
    {
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder()
          .RequestIdToken()
          .RequestEmail()
          .Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        // 구글 플레이 게임 활성화

        auth = FirebaseAuth.DefaultInstance; // Firebase 액세스
    }

  public void TryGoogleLogin()
    {
        if (!Social.localUser.authenticated) // 로그인 되어 있지 않다면
        {
            Social.localUser.Authenticate(success => // 로그인 시도
            {
                if (success) // 성공하면
                {
                    Debug.Log("Success");
                    googleText.SetActive(false);
                    ranking.SetActive(true);
                    LogOut.SetActive(true);
                    StartCoroutine(TryFirebaseLogin()); // Firebase Login 시도
                }
                else // 실패하면
                {
                    Debug.Log("Fail");
                }
            });
        }
    }

   public void TryGoogleLogout()
    {
        if (Social.localUser.authenticated) // 로그인 되어 있다면
        {
            PlayGamesPlatform.Instance.SignOut(); // Google 로그아웃
            auth.SignOut(); // Firebase 로그아웃
            googleText.SetActive(true);
            ranking.SetActive(false);
            LogOut.SetActive(false);
        }
    }
    IEnumerator TryFirebaseLogin()
    {
        while (string.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
            yield return null;
        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();


        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Success!");
        });
    }
    // 리더보드에 점수등록 후 보기
    public void ReportLeaderBoard()
    {
        

        // 1000점을 등록
        Social.ReportScore(1000, "CgkIjezStN4VEAIQBA", (bool bSuccess) =>
        {
            if (bSuccess)
            {
                Debug.Log("ReportLeaderBoard Success");
               
            }
            else
            {
                Debug.Log("ReportLeaderBoard Fall");
         
            }
        }
        );
        doLeaderboardShow();


    }
 public void doLeaderboardShow()
    {
       
        Social.ShowLeaderboardUI();
    }

  
}
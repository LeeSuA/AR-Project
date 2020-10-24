using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using Firebase.Unity.Editor;
using System.Threading.Tasks;
using Google;
using TMPro;
public class FirebaseGoogleAuth : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseUser user;//사용자 계정
    private DataSnapshot dataSnapshot=null;
    public GameObject googleText;
    public GameObject ranking;
    public GameObject LogOut;
    public TMP_Text debText;
    int score;
    string code;
    string uid;

    private void Awake()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }
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
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
           bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out" + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed In" + user.UserId);
            }
        }
    }
    private void init()
    { 
         FirebaseDatabase.DefaultInstance.GetReference("User").ValueChanged += HandleValueChanged;
    }
    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if(args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
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
                    StartCoroutine(TryFirebaseLogin()); // Firebase Login 시도
                    googleText.SetActive(false);
                    ranking.SetActive(true);
                    LogOut.SetActive(true);

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
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://exitraining-3962c.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

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
            auth = FirebaseAuth.DefaultInstance;
            user = auth.CurrentUser;
            SingletonManager.uID = user.UserId;
            reference.Child("User").Child(SingletonManager.uID).Child("score").SetValueAsync(0);

        });
    }
    // 리더보드에 점수등록
    public void ReportLeaderBoard(int score)
    {
       
        // 1000점을 등록
        Social.ReportScore(score, "CgkIjezStN4VEAIQBA", (bool bSuccess) =>
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
        

    }
    public void checkDrillCode()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://exitraining-3962c.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        DataSnapshot compareCode = null;
        if (dataSnapshot != null)
        {
            compareCode = dataSnapshot.Child("User").Child(SingletonManager.uID);
            if (!compareCode.Exists)
            {
                reference.Child("User").Child(SingletonManager.uID).Child("score").SetValueAsync(10);
                ReportLeaderBoard(10);
            }
            else
            {
                compareCode = dataSnapshot.Child("User").Child(SingletonManager.uID).Child("completeDrills").Child(SingletonManager.drillCode);
                if (!compareCode.Exists)
                {
                    score = (int)dataSnapshot.Child("User").Child(SingletonManager.uID).Child("score").Value;
                    score += 10;
                    reference.Child("User").Child(SingletonManager.uID).Child("score").SetValueAsync(score);
                    reference.Child("User").Child(SingletonManager.uID).Child("completeDrills").Child(SingletonManager.drillCode);
                    ReportLeaderBoard(score);
                }

            }
        }
    }
    public void doLeaderboardShow()
    {   
        Social.ShowLeaderboardUI();
    }
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

}
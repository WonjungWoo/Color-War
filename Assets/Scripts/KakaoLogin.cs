using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class KakaoLogin : MonoBehaviour
{
    private AndroidJavaObject ajo;
    [SerializeField] private TMP_InputField NicknameInput;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject LoginPage;
    [SerializeField] private GameObject MakeAccountPage;
    [SerializeField] private GameObject MainPage;
    [SerializeField] private Button NicknameButton;

    // Flask 서버의 IP 주소와 포트를 변경
    private const string serverUrl = "http://172.10.7.41:80/sign_up";

    
    void Start()
    {   
        if (PlayerPrefs.GetString("PlayerNickname", "") != "") {
            LoginPage.SetActive(false);
            MainPage.SetActive(true);
        }
        else {
            ajo = new AndroidJavaObject( "com.Sailors.ColorWar.UKakao" );
        }
    }

    public void login()
    {
        Debug.Log("Clicked");
        ajo.Call( "KakaoLogin" );
    }

    public void OnResult(string result)
    {
        // If the result is not null, save it as the player's nickname.
        Debug.Log("Result received: " + result);
        PlayerPrefs.SetString("PlayerkakaoID", result);
        PlayerPrefs.Save();
        EnterNickname();
    }

    public void EnterNickname() 
    {
        MakeAccountPage.SetActive(true);
        NicknameInput.onValueChanged.AddListener(delegate { ValidateNicknameLength(); });
    }

     private void ValidateNicknameLength()
    {
        NicknameButton.interactable = NicknameInput.text.Length >= 1;
    }

    public void OnLogin() 
    {   
        string nickname = NicknameInput.text;
        string userid = PlayerPrefs.GetString("PlayerkakaoID", "");

        Debug.Log(userid);
        Debug.Log(nickname);

        if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(nickname))
        {
            Debug.Log("Wrong input!");
        }
        else{
             // 데이터를 JSON 형식으로 생성
            string jsonRequestBody = $"{{\"userid\":\"{userid}\", \"nickname\":\"{nickname}\"}}";
            Debug.Log("start checklogin");
            StartCoroutine(CheckLogin(jsonRequestBody));
        }
    }

    private IEnumerator CheckLogin(string jsonRequestBody)
    {
        // UnityWebRequest 생성 시 URL에 "https://"를 사용
        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            
            // encoding 후 서버로 데이터 전송하기
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
            UploadHandlerRaw uH = new UploadHandlerRaw(bodyRaw);
            DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
            www.uploadHandler = uH;
            www.downloadHandler = dH;
            www.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("Requesting..");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Request succeeded!");

                if (www.responseCode == 200)
                {
                    byte[] resultData = www.downloadHandler.data;
                    if (resultData == null) {
                        resultText.SetText("Duplicated Nickname!");
                    }
                    else {
                        string result = System.Text.Encoding.UTF8.GetString(resultData);

                        Debug.Log("Received result: " + result);

                        if (resultText != null)
                        {
                            PlayerPrefs.SetString("PlayerNickname", result);
                            PlayerPrefs.Save();
                            LoginPage.SetActive(false);
                            MakeAccountPage.SetActive(false);
                            MainPage.SetActive(true);
                        }
                        else
                        {
                            Debug.LogError("Error: resultText is null");
                        }
                    }
                }
                else
                {
                    Debug.LogError("Error: Unexpected response code - " + www.responseCode);
                }
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }
    

}

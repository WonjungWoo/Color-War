using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class NewRoom : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private TMP_InputField CreateGameInput;
    [SerializeField] private TMP_InputField PasswordInput;

    // Flask 서버의 IP 주소와 포트
    private const string serverUrl = "http://172.10.7.41:80/create_room";

    private void Awake()
    {
        animator = GetComponent<Animator>();;
    }

    public void Close()
    {
        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        animator.SetTrigger("close");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        animator.ResetTrigger("close");
    }

    public void CreateGame()
    {
        Debug.Log("creating game..");
        Debug.Log(CreateGameInput.text);
        // 방 이름 중복되는지 확인하고 중복되지 않았을 경우에만 아래 코드 실행
        string roomname = CreateGameInput.text;
        string roompw = PasswordInput.text;

        if (string.IsNullOrEmpty(roomname))
        {
            Debug.Log("Wrong input!");
        }
        else {
            string jsonRequestBody = $"{{\"roomname\":\"{roomname}\", \"roompw\":\"{roompw}\"}}";

            StartCoroutine(CheckDuplicateRoomname(jsonRequestBody));
        }

        
    }

    private IEnumerator CheckDuplicateRoomname(string jsonRequestBody)
    {
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

            // 서버로부터 응답을 수신했을 경우
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Request succeeded!");

                if (www.responseCode == 200)
                {
                    byte[] resultData = www.downloadHandler.data;
                    if (resultData != null) {
                        // JSON 형식의 응답을 문자열로 변환
                        string resultJson = System.Text.Encoding.UTF8.GetString(resultData);

                        // 응답을 담을 객체 생성
                        ResultResponse resultResponse = JsonUtility.FromJson<ResultResponse>(resultJson);

                        // 결과에 따라 처리
                        if (resultResponse != null)
                        {
                            if (resultResponse.status)
                            {
                                // 방 생성에 성공했을 경우 처리. 추가로 구현할 필요 있음.
                                Debug.Log("true");
                                PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions() { MaxPlayers = 2}, null);
                            }
                            else
                            {
                                // 방 생성에 실패했을 경우 처리. 추가로 구현할 필요 있음.
                                Debug.Log("false");
                            }
                        }
                    }
                }

                
            }
        }
    }

    [System.Serializable]
    private class ResultResponse
    {
        public bool status;
        public string message;
    }
}

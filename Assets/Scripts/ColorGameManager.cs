using System.Collections;
using System.Collections.Generic;
using Photon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class ColorGameManager : PunBehaviour
{
    // Flask 서버의 IP 주소와 포트
    private const string serverUrl = "http://172.10.7.41:80/delete_room";
    public static ColorGameManager Instance;
    public GameObject PlayerPrefab;
    public GameObject GameCanvas;
    public GameObject SceneCamera;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>(); 
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;
    [SerializeField] private Button startButton;

    private void Awake()
    {
        Instance = this;
        GameCanvas.SetActive(true);
        UpdatePlayerList();
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

        UpdatePlayerList();
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            playerProperties["isReady"] = false;
            PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player);

            if (player == PhotonNetwork.player) 
            {
                newPlayerItem.ApplyLocalChanges();
            }
            playerItemsList.Add(newPlayerItem);
        }
    }


    public void SpawnPlayer()
    {
        Debug.Log("spawning");

        float randomValuex = Random.Range(-17242f, 17242f);
        float randomValuey = Random.Range(-14395f, 14395f);

        Debug.Log(PlayerPrefab.name);
            GameObject newPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(randomValuex, randomValuey, 0), Quaternion.identity, 0);
            StartCoroutine(wait());
            Debug.Log(newPlayer == null);
            Color playerColor;
            if (TryGetPlayerColor(player, out playerColor))
            {

                // 여기서 playerObject에 색상을 적용합니다.
                newPlayer.GetComponentInChildren<SpriteRenderer>().color = playerColor;
            }

            // 플레이어의 카메라를 활성화합니다.
            newPlayer.GetComponent<Player>().EnablePlayerCamera();

        GameCanvas.SetActive(false);
        SceneCamera.SetActive(false);
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(1.0f);
    }

    private bool TryGetPlayerColor(PhotonPlayer player, out Color color)
    {
        object colorValue;
        if (player.CustomProperties.TryGetValue("color", out colorValue))
        {
            Vector3 colorVector = (Vector3)colorValue;
            color = new Color(colorVector.x, colorVector.y, colorVector.z);
            return true;
        }

        color = Color.white; // 기본 색상
        return false;
    }

    
    public void LeaveRoom()
    {   
        if (PhotonNetwork.room.PlayerCount == 1) {
            //request the server to remove the room from the list
            string jsonRequestBody = $"{{\"roomname\":\"{PhotonNetwork.room.Name}\"}}";
            StartCoroutine(DeleteRoomFromServer(jsonRequestBody));
        }
        PhotonNetwork.LeaveRoom();
        Debug.Log("room left");
        PhotonNetwork.LoadLevel("Main Menu");
    }

    // 서버에 방 삭제를 요청
    private IEnumerator DeleteRoomFromServer(string jsonRequestBody)
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

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                if (www.responseCode == 200)
                {
                    byte[] resultData = www.downloadHandler.data;

                    if (resultData != null) {
                        Debug.Log("ch1");
                        // JSON 형식의 응답을 문자열로 변환
                        string resultJson = System.Text.Encoding.UTF8.GetString(resultData);

                        // 응답을 담을 객체 생성
                        ResultResponse resultResponse = JsonUtility.FromJson<ResultResponse>(resultJson);

                        // 결과에 따라 처리
                        if (resultResponse != null)
                        {
                            Debug.Log("zzz");
                            // 삭제 여부에 따른 메시지 출력
                            Debug.Log(resultResponse.message);
                        }
                    }
                    else {
                        Debug.LogError("Response failed");
                    }
                }
            }
            // 서버로부터 응답을 수신하는 데 실패한 경우
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    [System.Serializable]
    private class ResultResponse
    {
        public bool status;
        public string message;
    }

    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        CheckPlayersReady();
    }

    void CheckPlayersReady()
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (!(bool)player.CustomProperties["isReady"])
            {
                startButton.gameObject.SetActive(false);
                return;
            }
        }
        // All players are ready
        startButton.gameObject.SetActive(true);
    }
    public void SelectColor(Color color, Button colorButton)
    {
        if (PhotonNetwork.playerList.Length == 2)
        {
            PlayerItem currentPlayerItem = FindPlayerItem(PhotonNetwork.player);

            if (currentPlayerItem != null)
            {
                currentPlayerItem.SetPlayercolor(color);
                if (currentPlayerItem.GetOldButton() != null)
                {
                    currentPlayerItem.GetOldButton().interactable = true;
                }
                currentPlayerItem.SetOldButton(colorButton);
                colorButton.interactable = false;
            }
        }
        else 
        {
            PlayerItem currentPlayerItem = FindPlayerItem(PhotonNetwork.player);

            if (currentPlayerItem != null)
            {
                currentPlayerItem.SetPlayercolor(color);
            }
        }

    }

    // PhotonPlayer 객체를 기반으로 PlayerItem 인스턴스를 찾는 메소드
    private PlayerItem FindPlayerItem(PhotonPlayer player)
    {
        foreach (PlayerItem item in playerItemsList)
        {
            if (item.findPlayerInfo() == player)
            {
                return item;
            }
        }
        return null; // 해당하는 PlayerItem이 없으면 null 반환
    }
}

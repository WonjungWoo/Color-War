using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Unity.VisualScripting;
using Newtonsoft.Json;

public class LobbyController : MonoBehaviour {
    [SerializeField] private string VersioName = "0.1";
    [SerializeField] private GameObject ConnectPannel;

    [SerializeField] private GameObject NewRoomButton;
    [SerializeField] private RoomBox roomEntryPrefab;
    [SerializeField] private Transform contentPanel;

    private List<RoomBox> roomItemsList = new List<RoomBox>();

    // Flask 서버의 IP 주소와 포트
    private const string serverUrl = "http://172.10.7.41:80/search_room_list";

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersioName);

        //set User Name
        PhotonNetwork.playerName = PlayerPrefs.GetString("PlayerNickname", "");

        //PopulateRooms(RequestRoomsList());
        RequestRoomsList();
    }

    private void RequestRoomsList() {
        List<(string, string)> roomsList = new List<(string, string)>();

        StartCoroutine(GetRoomsListFromServer());
    }

    // 서버에 현재 생성되어 있는 방 목록 요청
    private IEnumerator GetRoomsListFromServer()
    {
        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            string jsonRequestBody = $"{{}}";
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
                    string jsonResponse = www.downloadHandler.text;
                    List<(string, string)> rooms = ParseRoomsJson(jsonResponse);
                    PopulateRooms(rooms);
                }
                else
                {
                    Debug.LogError("Error: Unexpected response code - " + www.responseCode);
                }
            }
            // 서버로부터 응답을 수신하는 데 실패한 경우
            else
            {
                Debug.LogError("Error: " + www.error);
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                List<(string, string)> rooms = ParseRoomsJson(jsonResponse);
            }
        }
        
    }

    // 요청받은 방 목록 데이터를 parsing
    private List<(string, string)> ParseRoomsJson(string json)
    {
        List<(string, string)> rooms = new List<(string, string)>();

        try
        {
            RoomListResponse response = JsonConvert.DeserializeObject<RoomListResponse>(json);

            foreach (RoomInfo roomInfo in response.result)
            {
                string roomName = roomInfo.roomname;
                string roomPassword = roomInfo.roompw;

                rooms.Add((roomName, roomPassword));
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error parsing JSON: " + e.Message);
        }

        return rooms;
    }

    [Serializable]
    public class RoomListResponse
    {
        public List<RoomInfo> result;
    }

    [Serializable]
    public class RoomInfo
    {
        public string roomname;
        public string roompw;
    }

    [SerializeField] private SettingsUI settingsUI;
    public void PopulateRooms(List<(String, String)> rooms)
    {
        foreach (RoomBox item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();
        
        foreach ((string roomName, string password) in rooms)
        {
            Debug.Log("roomName: " + roomName + ", password: " + password);
            RoomBox newEntry = Instantiate(roomEntryPrefab, contentPanel);
            newEntry.SetRoomName(roomName);
            if (password == "") {
                Debug.Log("password set to null");
                newEntry.NoPassword();
            }
            newEntry.SetListener().onClick.AddListener(() => settingsUI.JoinGame(roomName, password));;
            roomItemsList.Add(newEntry);
        }
    }


    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RequestRoomsList();
    }


    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Main Game");
    }

}
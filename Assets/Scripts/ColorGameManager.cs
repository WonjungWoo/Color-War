using System.Collections;
using System.Collections.Generic;
using Photon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorGameManager : PunBehaviour
{
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
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            // 각 플레이어의 위치는 랜덤으로 결정합니다.
            float randomValuex = Random.Range(-17242f, 17242f);
            float randomValuey = Random.Range(-14395f, 14395f);

            GameObject newPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector2(randomValuex, randomValuey), Quaternion.identity, 0);
            Color playerColor;
            if (TryGetPlayerColor(player, out playerColor))
            {
                // 여기서 playerObject에 색상을 적용합니다.
                newPlayer.GetComponent<SpriteRenderer>().color = playerColor;
            }
        }

        GameCanvas.SetActive(false);
        SceneCamera.SetActive(false);
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
            //PhotonNetwork.room.Name이 현재 방의 이름
        }
        PhotonNetwork.LeaveRoom();
        Debug.Log("room left");
        PhotonNetwork.LoadLevel("Main Menu");
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

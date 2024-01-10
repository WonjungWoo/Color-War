using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
using Photon;
using Unity.VisualScripting;


public class PlayerItem : PunBehaviour
{
    public TMP_Text playerName;
    public Image playerImage;
    public ReadyButtonSettings readyButton;
    public Button ReadyButton;
    public PhotonPlayer player;

    private Button oldButton;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    public void OnReadyButtonClicked()
    {
        if (GetOldButton() != null) 
        {
            playerProperties["isReady"] = true;
            PhotonNetwork.SetPlayerCustomProperties(playerProperties);
            readyButton.EnableButton(ReadyButton);
        }
    }

    public PhotonPlayer findPlayerInfo()
    {
        return player;
    }
    
    public void SetPlayerInfo(PhotonPlayer _player)
    {
        playerName.text = _player.NickName;
        player = _player;
    }

    public Button GetOldButton()
    {
        return oldButton;
    }

    public void SetOldButton(Button newButton)
    {
        oldButton = newButton;
    }

    public void ApplyLocalChanges()
    {
        ReadyButton.interactable = true;
    }


    public void SetPlayercolor(Color colorname)
    {
        playerImage.color = colorname;
        playerProperties["color"] = new Vector3(colorname.r, colorname.g, colorname.b);
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public Color GetPlayercolor()
    {
        return playerImage.color;
    }

    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        PhotonPlayer changedPlayer = playerAndUpdatedProps[0] as PhotonPlayer;
        ExitGames.Client.Photon.Hashtable changedProps = playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable;

        // 변경된 플레이어가 이 PlayerItem이 나타내는 플레이어인지 확인
        if (changedPlayer != null && changedPlayer == player)
        {
            // "color" 속성이 변경되었는지 확인
            if (changedProps.ContainsKey("color"))
            {
                // 변경된 색상으로 UI 업데이트
                Vector3 newColor = (Vector3)changedProps["color"];
                playerImage.color = new Color(newColor.x, newColor.y, newColor.z);

                if (PhotonNetwork.playerList.Length == 2)
                {
                    oldButton.interactable = false;
                }
            }
        }
    }

}

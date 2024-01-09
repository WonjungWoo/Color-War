using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButtonSettings : Photon.MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private PhotonView photonView;

    public void EnableButton(Button button)
    {
        // 로컬 플레이어의 버튼을 숨깁니다.
        button.gameObject.SetActive(false);

        // RPC를 사용하여 다른 플레이어에게도 로컬 플레이어의 버튼 숨김을 알립니다.
        photonView.RPC("DisableButton", PhotonTargets.OthersBuffered, button.gameObject.name);
    }

    [PunRPC]
    private void DisableButton(Button button)
    {
        button.gameObject.SetActive(false);
    }
}

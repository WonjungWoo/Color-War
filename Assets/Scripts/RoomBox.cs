using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomBox : MonoBehaviour
{
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private Image lockImage;
    [SerializeField] private Button joinButton;
    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
        Debug.Log("text set");
    }

    public void NoPassword()
    {
        lockImage.enabled = false;
    }

    public Button SetListener()
    {
        return joinButton;
    }
}

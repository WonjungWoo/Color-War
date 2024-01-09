using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsUI : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private TMP_InputField RoomNameInput;
    [SerializeField] private GameObject PasswordUI;
    [SerializeField] private TMP_InputField PasswordInput;
    [SerializeField] private Button PasswordEnterButton;
    [SerializeField] private TMP_Text IncorrectPassword;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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

    public void JoinGame(string gamename, string password)
    {
        Debug.Log(gamename +',' + password);
        if (password == "") {
            PhotonNetwork.JoinRoom(gamename);
        }
        else {
            PasswordUI.SetActive(true);
            PasswordEnterButton.onClick.RemoveAllListeners();
            PasswordEnterButton.onClick.AddListener(() => EnterPassword(gamename, password));
        }
    }

    public void EnterPassword(String gamename, String password)
    {
        string EnteredPassword = PasswordInput.text;
        if (EnteredPassword == password) {
            PasswordUI.SetActive(false);
            PhotonNetwork.JoinRoom(gamename);
        }
        else {
            IncorrectPassword.enabled = true;
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : Photon.MonoBehaviour
{
    public PhotonView photonView;
    public Animator anim;
    public GameObject PlayerCamera;
    public SpriteRenderer sr;

    public Rigidbody2D rg;

    public float MoveSpeed;
    public VirtualJoystick joystick; 

    GameObject closestNPC = null;
    Color playercolor;

    bool isClosestPlayer = false;

    // Start is called before the first frame update
    private void Start()
    {
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
        }
    }


    private void Update()
    {
        if (photonView.isMine)
        {
            CheckInput();
            ClampPosition();

            // 상대방의 위치를 얻기 위한 함수 호출
            Vector3 opponentPosition = GetOpponentLoc();
            Debug.Log("Opponent Pos: " + opponentPosition);
        }
    }

    private void CheckInput()
    {
        Vector2 inputDirection = joystick.GetInputDirection();
        var move = new Vector3(inputDirection.x, inputDirection.y, 0);
        transform.position += move * MoveSpeed * Time.deltaTime;
        Debug.Log(inputDirection.x + "," + inputDirection.y);

        if (inputDirection.x > 0)
        {
            photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
        }
        else
        {
            photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
        }

        if (move != new Vector3(0, 0, 0))
        {
            anim.SetBool("isMove", true);
        }
        else 
        {
            anim.SetBool("isMove", false);
        }
    }

    private void ClampPosition()
    {
        // 가정: leftBound와 rightBound는 배경의 왼쪽과 오른쪽 경계를 나타냄
        float leftBound = -17242.0f;  // 배경의 왼쪽 경계
        float rightBound = 17242.0f; // 배경의 오른쪽 경계
        float TopBound = 14395.0f;
        float BottomBound = -14395.0f;

        // 플레이어의 위치를 경계 내로 제한
        float clampedX = Mathf.Clamp(transform.position.x, leftBound, rightBound);
        float clampedY = Mathf.Clamp(transform.position.y, BottomBound, TopBound);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
    public float CheckDistanceWithClosestNPC()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        float closestDistance = float.MaxValue;

        foreach (GameObject npc in npcs)
        {
            float distance = Vector2.Distance(rg.position, npc.GetComponentInChildren<Rigidbody2D>().transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNPC = npc;
            }
        }
        // 상대 플레이어와의 거리를 계산
        float distancep = Vector2.Distance(rg.position, GetOpponentLoc());
        // 가장 가까운 캐릭터가 상대 플레이어일 경우
        if (distancep < closestDistance)
        {
            closestDistance = distancep;
            isClosestPlayer = true;
        }

        if (closestNPC != null && closestDistance <= 4000f)
        {
            Debug.Log($"Closest Distance: {closestDistance}");
        }

        return closestDistance;
    }

    // 상대방의 위치를 얻기 위한 함수
    private Vector3 GetOpponentLoc()
    {
        PhotonPlayer[] otherPlayers = PhotonNetwork.playerList;

        if (otherPlayers != null && otherPlayers.Length > 0)
        {
            // 여러 플레이어 중 하나를 선택하여 위치 정보 가져오기
            PhotonPlayer opponentPlayer = otherPlayers[0];
            GameObject opponentObject = GameObject.Find(opponentPlayer.NickName); // 상대방의 닉네임을 통해 GameObject를 찾음

            if (opponentObject != null)
            {
                return opponentObject.GetComponentInChildren<Rigidbody2D>().transform.position;
            }
        }

        return Vector3.zero; // 상대방이 없거나 위치를 찾을 수 없을 경우 (원하는 처리에 따라 변경 가능)
    }

    public bool GetIsClosestPlayer() {
        return isClosestPlayer;
    }

    public GameObject getClosestNPC()
    {
        return closestNPC;
    }

    public void ChangeSpeed(int speed)
    {
        MoveSpeed = speed;
    }

    [PunRPC]
    private void FlipTrue()
    {
        sr.flipX = true;
    }

    [PunRPC]
    private void FlipFalse()
    {
        sr.flipX = false;
    }

}

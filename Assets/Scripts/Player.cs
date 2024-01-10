using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    private void Start()
    {
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
            
        }
        StartCoroutine(CheckDistanceWithClosestNPCCoroutine());
    }

    public void EnablePlayerCamera()
    {
        PlayerCamera.SetActive(true);
    }


    private void Update()
    {
        if (photonView.isMine)
        {
            CheckInput();
            ClampPosition();
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

    private IEnumerator CheckDistanceWithClosestNPCCoroutine()
    {
        while (true) {
            {
                yield return new WaitForSeconds(0.5f);

                CheckDistanceWithClosestNPC();
            }
        }
    }

    private void CheckDistanceWithClosestNPC()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        float closestDistance = float.MaxValue;
        GameObject closestNPC = null;

        foreach (GameObject npc in npcs)
        {
            float distance = Vector2.Distance(transform.position, npc.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNPC = npc;
            }
        }

        if (closestNPC != null && closestDistance <= 4000f)
        {
            Debug.Log($"Closest NPC: {closestNPC.name}, Distance: {closestDistance}");
        }
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

using System.Collections;
using UnityEngine;

public class Npc : Photon.MonoBehaviour
{
    public PhotonView photonView;
    public Animator anim;
    public SpriteRenderer sr;

    public Rigidbody2D rg;

    public float walkSpeed = 1000.0f; // 걷기 속도

    private bool isWalking = false;
    private Vector2 moveDirection; // 움직일 방향을 저장할 변수
    public int uniqueID;

    void Start()
    {
        StartCoroutine(WalkRandomly());
    }

    void Update()
    {
        if (isWalking)
        {
            // NPC가 걷고 있으면 전진
            transform.Translate(moveDirection * walkSpeed * Time.deltaTime);

            // 맵 경계 체크 후 보정
            ClampPosition();
        }
    }

    IEnumerator WalkRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5.0f, 10.0f)); // 5~10초 동안 대기

            anim.SetBool("isMove", true);
            // 무작위 방향 설정
            moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            if (moveDirection.x > 0)
            {
                sr.flipX = false;
                photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
            }
            else
            {
                sr.flipX = true;
                photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
            }

            isWalking = true;
            yield return new WaitForSeconds(Random.Range(5.0f, 10.0f)); // 5~10초 동안 걷기

            anim.SetBool("isMove", false);
            isWalking = false;
        }
    }

    void ClampPosition()
    {
        // 맵 경계 설정
        float minX = -17242.0f;
        float maxX = 17242.0f;
        float minY = -14395.0f;
        float maxY = 14395.0f;

        // 현재 NPC 위치
        Vector3 currentPosition = transform.position;

        // 맵 경계 안으로 보정
        float clampedX = Mathf.Clamp(currentPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(currentPosition.y, minY, maxY);

        // 보정된 위치로 이동
        transform.position = new Vector3(clampedX, clampedY, currentPosition.z);
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

using System.Collections;
using UnityEngine;

public class Npc : Photon.MonoBehaviour
{
    public PhotonView photonView;
    public Animator anim;
    public SpriteRenderer sr;

    public Rigidbody2D rg;

    public float walkSpeed = 5.0f; // 걷기 속도
    public float rotationSpeed = 100.0f; // 회전 속도

    private bool isWalking = false;

    void Start()
    {
        StartCoroutine(WalkRandomly());
    }

    void Update()
    {
        if (isWalking)
        {
            // NPC가 걷고 있으면 전진
            transform.Translate(Vector2.up * walkSpeed * Time.deltaTime);

            // 맵 경계 체크 후 보정
            ClampPosition();
        }
    }

    IEnumerator WalkRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5.0f, 10.0f)); // 5~10초 동안 대기

            isWalking = true;
            anim.SetBool("isMove", true);
            yield return new WaitForSeconds(Random.Range(1.0f, 5.0f)); // 1~5초 동안 걷기

            isWalking = false;
            anim.SetBool("isMove", false);

            // 무작위로 회전
            transform.Rotate(Vector3.forward, Random.Range(0.0f, 360.0f));

            // 현재 회전 각도 가져오기
            float currentRotation = transform.rotation.eulerAngles.z;

            // 회전 각도가 0에서 180도 사이에 있는 경우 (오른쪽 방향)
            if (currentRotation >= 0.0f && currentRotation <= 180.0f)
            {
                // NPC의 이동 방향이 x값이 양수입니다.
                photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
            }
            else
            {
                // NPC의 이동 방향이 x값이 음수입니다.
                photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
            }
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

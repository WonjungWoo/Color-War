using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour
{
    [SerializeField] private GameObject joystickBackground;
    [SerializeField] private GameObject joystickHandle;
    [SerializeField] private Camera playerCamera;
    public Vector3 joystickVec; // Vector2로 변경
    private Vector3 joystickOriginalWorldPos;
    private Vector3 joystickOriginalScreenPos;
    private float joystickRadius;

    void Start()
    {
        Debug.Log("joystick operating");
        joystickOriginalWorldPos = joystickBackground.transform.position;
        joystickOriginalScreenPos = playerCamera.WorldToScreenPoint(joystickOriginalWorldPos); // 변경된 부분
        Debug.Log(joystickOriginalWorldPos);
        joystickRadius = joystickBackground.GetComponent<RectTransform>().sizeDelta.y / 2;
    }

    public void OnDrag(BaseEventData eventData)
    {
        joystickOriginalWorldPos = joystickBackground.transform.position;
        Debug.Log("Joystick drag");
        PointerEventData pointerEventData = eventData as PointerEventData;
        Vector2 dragScreenPos = pointerEventData.position;
        Vector3 dragWorldPos = playerCamera.ScreenToWorldPoint(new Vector3(dragScreenPos.x, dragScreenPos.y, joystickOriginalWorldPos.z));

        // 조이스틱 핸들을 월드 좌표 기준으로 이동
        joystickHandle.transform.position = dragWorldPos;
        joystickVec = (dragWorldPos - joystickOriginalWorldPos).normalized;

        float joystickDist = Vector2.Distance(dragWorldPos, joystickOriginalWorldPos);

        Debug.Log(dragWorldPos);

        if (joystickDist < joystickRadius)
        {
            joystickHandle.transform.position = joystickOriginalWorldPos + joystickVec * joystickDist;
        }
        else
        {
            joystickHandle.transform.position = joystickOriginalWorldPos + joystickVec * joystickRadius;
        }
    }

    public void OnPointerUp()
    {
        joystickVec = Vector2.zero;
        joystickHandle.transform.position = joystickBackground.transform.position;
    }

    // 조이스틱의 입력 값을 게임에 전달하는 메소드
    public Vector2 GetInputDirection()
    {
        return joystickVec;
    }
}

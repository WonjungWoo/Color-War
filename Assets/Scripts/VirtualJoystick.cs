using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour
{
    [SerializeField] private GameObject joystickBackground;
    [SerializeField] private GameObject joystickHandle;
    public Vector2 joystickVec;
    private Vector2 joystickTouchPos;
    private Vector2 joystickOriginalPos;
    private float joystickRadius;

    void Start()
    {
        Debug.Log("joystick operating");
        joystickOriginalPos = joystickBackground.transform.position;
        joystickRadius = joystickBackground.GetComponent<RectTransform>().sizeDelta.y / 4;
    }

    public void OnDrag(BaseEventData eventData)
    {
        Debug.Log("Joystick drag");
        PointerEventData pointerEventData = eventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        joystickVec = (dragPos - joystickTouchPos).normalized;

        float joystickDist = Vector2.Distance(dragPos, joystickTouchPos);

        if (joystickDist < joystickRadius)
        {
            joystickHandle.transform.position = joystickTouchPos + joystickVec * joystickDist;
        }
        else
        {
            joystickHandle.transform.position = joystickTouchPos + joystickVec * joystickRadius;
        }
    }

    public void OnPointerDown()
    {   
        Debug.Log("Joystick clicked");
        joystickHandle.transform.position = Input.mousePosition;
        joystickTouchPos = Input.mousePosition;
    }

    public void OnPointerUp()
    {
        joystickVec = Vector2.zero;
        joystickHandle.transform.position = joystickOriginalPos;
        joystickBackground.transform.position = joystickOriginalPos;
    }

    // 조이스틱의 입력 값을 게임에 전달하는 메소드
    public Vector2 GetInputDirection()
    {
        return joystickVec;
    }
}

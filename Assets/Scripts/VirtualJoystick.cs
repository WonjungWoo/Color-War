using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;

    private void Start()
    {
        joystickBackground = GetComponent<RectTransform>();
        joystickHandle = transform.GetChild(0).GetComponent<RectTransform>(); // 핸들은 첫 번째 자식으로 가정
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(Camera.main, joystickBackground.position);
        Vector2 radius = joystickBackground.sizeDelta / 2;
        Vector2 input = (eventData.position - position) / radius;

        input = (input.magnitude > 1.0f) ? input.normalized : input;

        // 핸들의 위치를 조정
        joystickHandle.anchoredPosition = input * radius;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    // 조이스틱의 입력 값을 게임에 전달하는 메소드
    public Vector2 GetInputDirection()
    {
        return joystickHandle.anchoredPosition / (joystickBackground.sizeDelta / 2);
    }
}

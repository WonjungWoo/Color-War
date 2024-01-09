using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>  ColorGameManager.Instance.SelectColor(button.image.color, button));
    }


}
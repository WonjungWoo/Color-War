using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerAction : MonoBehaviour
{
    [SerializeField] private Player player;
    private bool isButtonPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonDownRun()
    {
        Debug.Log("Press");
        isButtonPressed = true;
        player.ChangeSpeed(10);
    }

    public void OnButtonUpRun()
    {
        Debug.Log("Up");
        isButtonPressed = false;
        player.ChangeSpeed(5);
    }
}

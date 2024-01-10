using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField]
    private PaintUI paintUI;
    public PaintUI PaintUI { get { return paintUI; } }
    [SerializeField]
    private KillUI killUI;
    public KillUI KillUI { get { return KillUI; } }
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

    public void OnClickColorButton()
    {
        // 거리가 가까운 경우 색칠
        float distance = player.CheckDistanceWithClosestNPC();

        Debug.Log("distance:"+distance);

        if (distance <= 4000f)
        {
            // 가장 가까운 캐릭터가 플레이어가 아닐 경우
            if (!player.GetIsClosestPlayer()) {
                GameObject npcObject = player.getClosestNPC();
            
                if (npcObject != null)
                {
                    Debug.Log("ch1");
                    Npc npc = npcObject.GetComponent<Npc>();
                    Debug.Log(npc);

                    if (npc.GetWhichColored() == false)
                    {
                        PaintUI.Open(player, npc);
                        // 해당 NPC의 whichColored 값을 변경
                        npc.SetWhichColored(true);
                        Debug.Log("colered!");

                        npc.setnpcColor(player.GetComponentInChildren<SpriteRenderer>().color);
                    }
                    else {
                        PaintUI.Open(player, npc);
                        Debug.Log("failed to color: already colered");
                    }
                }
            }
            else {
                Debug.Log("failed to color: he is a player!");
            }
        }
        else {
            Debug.Log("failed to color: too far to color");
        }
    }

    public void OnClickEatButton()
    {
        // 거리가 가까운 경우 색칠
        float distance = player.CheckDistanceWithClosestNPC();

        Debug.Log("distance:"+distance);

        if (distance <= 4000f)
        {
            // 가장 가까운 캐릭터가 플레이어일 경우 - 게임 종료
            if (player.GetIsClosestPlayer()) {
                Debug.Log("Eat!! Game over.");
            }
            else {
                GameObject npcObject = player.getClosestNPC();
                Npc npc = npcObject.GetComponent<Npc>();
                KillUI.Open(player,npc);
                Debug.Log("failed to eat: he is NPC");
            }
        }
        else {
            Debug.Log("failed to eat: too far to eat");
        }
    }
}

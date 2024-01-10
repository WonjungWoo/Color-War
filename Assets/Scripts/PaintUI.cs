using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintUI : MonoBehaviour
{
    [SerializeField]
    private Image playerImg;
    [SerializeField]
    private Image npcImg;
    

    public void Open(Player player, Npc npc)
    {
        playerImg.color = player.GetComponentInChildren<SpriteRenderer>().color;
        npcImg.color = npc.GetComponentInChildren<SpriteRenderer>().color;
        gameObject.SetActive(true);
        Invoke("Close", 3f);
    }
    
    public void Close()
    {
        gameObject.SetActive(false);
        
    }
}

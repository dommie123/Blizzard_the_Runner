using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboMeter : MonoBehaviour
{
    public PlayerController player;
    private TMP_Text text;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void FixedUpdate()
    {
        text.enabled = (player.combo >= 2); //slightly more efficient than an if statement
        text.text = "x" + Mathf.Clamp(player.combo,1,player.comboCap).ToString();
    }
}

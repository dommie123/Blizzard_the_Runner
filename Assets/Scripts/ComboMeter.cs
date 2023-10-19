using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboMeter : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void FixedUpdate()
    {
        string currentCombo = Mathf.Clamp(player.GetCombo(), 1, player.GetComboCap()).ToString();

        text.enabled = (player.GetCombo() >= 2);     // slightly more efficient than an if statement
        text.text = $"x{currentCombo}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    [SerializeField] private PlayerController player;

    private int coins;

    public TMP_Text coinText;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateCoins()
    {
        coins = player.Coins;
        coinText.text = $"x {coins}";
    }
}

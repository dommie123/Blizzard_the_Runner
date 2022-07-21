using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static CoinManager instance;
    private int coins;
    public TMP_Text coinText;
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCoins()
    {
        coins = PlayerController.instance.Coins;
        coinText.text = $"x {coins}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenBehavior : MonoBehaviour
{
    public Button btnStart;
    public GameBehavior game;

    // Start is called before the first frame update
    void Start()
    {
        btnStart.onClick.AddListener(StartGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartGame()
    {
        game.gameStarted = true;
    }
}

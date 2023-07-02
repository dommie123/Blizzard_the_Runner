using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuBehavior : MonoBehaviour
{
    public Button btnMainMenu;
    
    private AudioSource menuSFX;

    // Start is called before the first frame update
    void Start()
    {
        menuSFX = GameObject.Find("Menu SFX").GetComponent<AudioSource>();

        btnMainMenu.onClick.AddListener(GoToMainMenu);
    }

    private void GoToMainMenu()
    {
        menuSFX.Play();
        SceneManager.LoadSceneAsync("Title");
    }
}

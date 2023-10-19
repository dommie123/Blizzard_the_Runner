using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuBehavior : MonoBehaviour
{
    private AudioSource menuSFX;

    private void Awake()
    {
        menuSFX = GameObject.Find("Menu SFX").GetComponent<AudioSource>();
    }
    public void GoToMainMenu()
    {
        menuSFX.Play();
        SceneManager.LoadSceneAsync("Title");
    }
}

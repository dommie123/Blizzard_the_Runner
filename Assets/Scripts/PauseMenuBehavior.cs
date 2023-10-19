using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuBehavior : MonoBehaviour
{
    private AudioSource menuSFX;

    public void GoToMainMenu()
    {
        menuSFX.Play();
        SceneManager.LoadSceneAsync("Title");
    }
}

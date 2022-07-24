using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuBehavior : MonoBehaviour
{
    public Button btnMainMenu;
    // Start is called before the first frame update
    void Start()
    {
        btnMainMenu.onClick.AddListener(GoToMainMenu);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync("Main");
    }
}

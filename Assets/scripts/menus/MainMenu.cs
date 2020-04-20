using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Scene scene;

    private void Awake()
    {
        scene = SceneManager.GetActiveScene();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(scene.buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit the game fromm the Main Menu");
    }
}

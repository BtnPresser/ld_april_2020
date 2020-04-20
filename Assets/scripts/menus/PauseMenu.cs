using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenu;

    private Keyboard kb;

    private void Awake()
    {
        kb = InputSystem.GetDevice<Keyboard>();
    }
    // Update is called once per frame
    void Update()
    {
        // Shortcuts if Game is already paused
        if (GamePaused)
        {
            if (kb.escapeKey.wasPressedThisFrame)
            {
                Resume();
            }
        }
        // normal keybindings when game is running
        else
        {
            if (kb.escapeKey.wasPressedThisFrame)
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
    }

    public void LoadMenu()
    {
        Debug.Log("Loading Main Menu Scene");
        SceneManager.LoadScene("MenuScene");
        Resume();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game was Pressed from Pause Menu");
    }
}

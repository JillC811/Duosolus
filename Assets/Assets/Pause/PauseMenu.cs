using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameStateManager.Instance.GameIsPaused) {
                Resume();
            }
            else {
               Pause(); 
            }  
        }
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameStateManager.Instance.GameIsPaused = false;
    }

    void Pause() {
        if (GameStateManager.Instance.Cleared) return;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameStateManager.Instance.GameIsPaused = true;
    }

    public void Settings() {
        GameStateManager.Instance.GameIsPaused = false;
        Time.timeScale = 1f;
        // SceneManager.LoadScene("Insert Settings Scene Here");
        Debug.Log("Loading Settings...");
    }

    public void LoadMap() {
        GameStateManager.Instance.GameIsPaused = false;
        Time.timeScale = 1f;
        // SceneManager.LoadScene("Insert World Map Scene Here");
        Debug.Log("Loading World Map...");
    }

    public void QuitGame() {
        GameStateManager.Instance.GameIsPaused = false;
        Time.timeScale = 1f;
        // SceneManager.LoadScene("Insert Main Menu Scene Here");
        Debug.Log("Loading Main Menu...");
    }
}

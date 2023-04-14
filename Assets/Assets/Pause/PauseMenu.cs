using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject deadScreenUI;
    public GameObject settingsMenuUI;
    public bool wasPaused = false;

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
    
    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameStateManager.Instance.GameIsPaused = false;

        if(wasPaused) wasPaused = true;
    }


    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameStateManager.Instance.GameIsPaused = false;

        if(wasPaused)
        {
            deadScreenUI.SetActive(true);
            wasPaused = false;
        }
    }

    void Pause() {
        if (GameStateManager.Instance.Cleared) return;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameStateManager.Instance.GameIsPaused = true;

        
        if(deadScreenUI.activeSelf)
        {
            deadScreenUI.SetActive(false);
            wasPaused = true;
        }
    }

    public void Settings() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 0f;
        settingsMenuUI.SetActive(true);
        GameStateManager.Instance.GameIsPaused = false;
    }

    public void LoadMap() {
        Time.timeScale = 1f;
        GlobalGameStateManager.Instance.curLevel = SceneNameToLevel(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("LevelMenuScene", LoadSceneMode.Single);
        Debug.Log("Loading World Map out of " + name + "...");
    }

    public void QuitGame() {
        Time.timeScale = 1f;
        GlobalGameStateManager.Instance.curLevel = SceneNameToLevel(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
        Debug.Log("Loading Main Menu out of " + name + "...");
    }

    public static int SceneNameToLevel(string sceneName)
    {
        int levelNumber;
        if (!TryParseLevelNumber(sceneName, out levelNumber))
        {
            Debug.LogError("Invalid scene name: " + sceneName + ". Scene name must be in the format 'LevelXScene'");
            return -1;
        }
        return levelNumber;
    }

    private static bool TryParseLevelNumber(string sceneName, out int levelNumber)
    {
        levelNumber = -1;
        if (sceneName.StartsWith("Level") && sceneName.EndsWith("Scene"))
        {
            string levelNumberString = sceneName.Substring("Level".Length, sceneName.Length - "Level".Length - "Scene".Length);
            if (int.TryParse(levelNumberString, out levelNumber))
            {
                return true;
            }
        }
        return false;
    }
}

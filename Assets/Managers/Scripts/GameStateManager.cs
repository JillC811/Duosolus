using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface InterfaceUndo
{
    void undo(Vector3 coord);
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public struct History {
        public GameObject target;
        public Vector3 coord;

        public History(GameObject obj, Vector3 vec)
        {
            target = obj;
            coord = vec;
        }
    }

    public Stack<History> PreviousMoves = new Stack<History>();

    public bool EventOccurance = false;
    public bool GameIsPaused = false;
    public bool PlayerMoving = false;
    public bool Cleared = false;
    public bool heroDuplicateActive = false;
    public bool villainDuplicateActive = false;
    public int HeroMoving = 0;
    public int VillainMoving = 0;
    public int HeroCloneMoving = 0;
    public int VillainCloneMoving = 0;
    public GameObject clearScreenUI;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called per frame
    void Update() {
        // Check if players moved, update PlayerMoving variable accordingly
        if (!heroDuplicateActive && !villainDuplicateActive) {
            if (PlayerMoving && HeroMoving == 0 && VillainMoving == 0)
            {
                PreviousMoves.Push(new History(null, new Vector3(0f, 0f, 0f)));
                PlayerMoving = false;
            }
            else if (!PlayerMoving && (HeroMoving > 0 || VillainMoving > 0))
            {
                PlayerMoving = true;
            } 
        }
        else if (heroDuplicateActive) {
            if (PlayerMoving && HeroMoving == 0 && VillainMoving == 0 && HeroCloneMoving == 0)
            {
                PreviousMoves.Push(new History(null, new Vector3(0f, 0f, 0f)));
                PlayerMoving = false;
            }
            else if (!PlayerMoving && (HeroMoving > 0 || VillainMoving > 0 || HeroCloneMoving > 0))
            {
                PlayerMoving = true;
            } 
        }
       else if (villainDuplicateActive) {
            if (PlayerMoving && HeroMoving == 0 && VillainMoving == 0 && VillainCloneMoving == 0)
            {
                PreviousMoves.Push(new History(null, new Vector3(0f, 0f, 0f)));
                PlayerMoving = false;
            }
            else if (!PlayerMoving && (HeroMoving > 0 || VillainMoving > 0 || VillainCloneMoving > 0))
            {
                PlayerMoving = true;
            } 
        }

        // Undo
        if(!PlayerMoving && !Cleared && Input.GetKeyDown(KeyCode.Backspace))
        {
            if(PreviousMoves.Count > 0)
            {
                foreach (History item in PreviousMoves)
                {
                    Debug.Log(item.target);
                }

                History hist = PreviousMoves.Pop();
                hist = PreviousMoves.Pop();
                
                do
                {
                    InterfaceUndo obj = hist.target.GetComponent<InterfaceUndo>();
                    obj.undo(hist.coord);
                    if(PreviousMoves.Count == 0) break;
                    hist = PreviousMoves.Pop();
                } while(hist.target != null);

                PreviousMoves.Push(new History(null, new Vector3(0f,0f,0f)));
            }
        }
    }

    public void Clear() {
        Cleared = true;
        Time.timeScale = 0f;
        clearScreenUI.SetActive(true);
        int level = SceneNameToLevel(SceneManager.GetActiveScene().name);
        GlobalGameStateManager.Instance.clearedLevels[level] = true;
    }

    public void LoadMap() {
        Time.timeScale = 1f;
        GlobalGameStateManager.Instance.curLevel = SceneNameToLevel(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("LevelMenuScene", LoadSceneMode.Single);
        Debug.Log("Loading World Map out of " + name + "...");
    }

    public void RestartLevel() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        Debug.Log("Loading Main Menu...");
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

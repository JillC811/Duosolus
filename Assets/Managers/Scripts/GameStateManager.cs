using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

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
    public List<DoorActivate> SwitchObject = new List<DoorActivate>();
    public List<int> SwitchTime = new List<int>();
    public List<GameObject> MonsterList = new List<GameObject>();

    public bool EventOccurance = false;
    public bool GameIsPaused = false;
    public bool PlayerMoving = false;
    public bool Cleared = false;
    public bool heroDuplicateActive = false;
    public bool villainDuplicateActive = false;
    public bool skeletonActive = false;
    public bool cyclopeActive = false;
    public bool cyclope2Active = false;

    public Dictionary<GameObject, int> ObjectsInMotion = new Dictionary<GameObject, int>();
    
    public int SkeletonMoving = 0;
    public int CyclopeMoving = 0;
    public int Cyclope2Moving = 0;
    public GameObject clearScreenUI;

    public int TurnCount = 0;

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
        if (PlayerMoving && ObjectsInMotion.Count == 0) 
        {
            // Check for Timed Doors
            for(int i = 0; i < SwitchTime.Count; i++)
            {
                if(SwitchTime[i] == TurnCount)
                {
                    SwitchObject[i].ResetSwitch();
                    SwitchObject.RemoveAt(i);
                    SwitchTime.RemoveAt(i);
                    i--;
                }
            }

            PreviousMoves.Push(new History(null, new Vector3(0f, 0f, 0f)));
            PlayerMoving = false;
            TurnCount++;
        }
        else if (!PlayerMoving && ObjectsInMotion.Count > 0)
        {
            PlayerMoving = true;

            // Move Monsters
            foreach(GameObject m in MonsterList)
            {
                MonsterMovement mmove = m.GetComponent<MonsterMovement>();
                mmove.Move();
            }
        }
        
        // Undo
        if(!PlayerMoving && !Cleared && Input.GetKeyDown(KeyCode.Backspace))
        {
            if(PreviousMoves.Count > 0)
            {
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

                TurnCount--;

                for(int i = 0; i < SwitchTime.Count; i++)
                {
                    if(TurnCount == SwitchObject[i].InitTurn)
                    {
                        SwitchObject[i].ResetSwitch();
                        SwitchObject.RemoveAt(i);
                        SwitchTime.RemoveAt(i);
                        i--;
                    }
                }
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

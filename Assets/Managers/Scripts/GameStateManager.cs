using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 
using UnityEngine.UI;

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
    public int SwappedTurn = -1;
    public bool GameIsPaused = false;
    public bool PlayerMoving = false;
    public bool Cleared = false;

    public Dictionary<GameObject, int> ObjectsInMotion = new Dictionary<GameObject, int>();
    
    public GameObject clearScreenUI;

    public int TurnCount = 0;

    // Instruction Display
    public static float timeOut = 20.0f; // Time in seconds before the image is displayed
    private const string IMAGEPATH = "Images/undoInstruction"; // Name of the image to display (without the extension)

    private Image image; // Reference to the Image component
    private float timeElapsed = 0.0f; // Time elapsed since the last input
    private GameObject imageObject;

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

    void Start()
    {
        // Instruction Display
        Sprite sprite = Resources.Load<Sprite>(IMAGEPATH);
        imageObject = new GameObject("Image");
        //imageObject.transform.SetParent(transform);
        RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
        imageObject.transform.localScale = new Vector3(0.34f, 0.34f, 1f);
        imageObject.transform.position = new Vector3(0f,0.87f,0f);
        image = imageObject.AddComponent<Image>();
        image.sprite = sprite;
        SpriteRenderer spriteRenderer = imageObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;
        spriteRenderer.sortingLayerName = "Instruction";
        imageObject.SetActive(false);
    }

    // Called per frame
    void Update() {
        // Instruction Display
        if (Input.anyKey)
        {
            timeElapsed = 0.0f;
            imageObject.SetActive(false);
        }
        else
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= timeOut)
            {
                imageObject.SetActive(true);
                SpriteRenderer spriteRenderer = imageObject.GetComponent<SpriteRenderer>();
                Color color = spriteRenderer.color;
                color.a = Mathf.Max(0, Mathf.Min(1.0f, Mathf.Sin(timeElapsed) + 0.5f));
                spriteRenderer.color = color;

            }
        }

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

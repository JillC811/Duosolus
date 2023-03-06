using System.Collections.Generic;
using UnityEngine;

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
    public int HeroMoving = 0;
    public int VillainMoving = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called per frame
    void Update() {
        // Check if players moved, update PlayerMoving variable accordingly
        if (PlayerMoving && HeroMoving == 0 && VillainMoving == 0)
        {
            PreviousMoves.Push(new History(null, new Vector3(0f,0f,0f)));
            PlayerMoving = false;
        }
        else if (!PlayerMoving && (HeroMoving > 0 || VillainMoving > 0))
        {
            PlayerMoving = true;
        } 

        // Undo
        if(!PlayerMoving && Input.GetKeyDown(KeyCode.Backspace))
        {
            if(PreviousMoves.Count > 0)
            {
                History hist = PreviousMoves.Pop();
                if(hist.target == null) hist = PreviousMoves.Pop();
                
                do
                {
                    InterfaceUndo obj = hist.target.GetComponent<InterfaceUndo>();
                    obj.undo(hist.coord);
                    if(PreviousMoves.Count == 0) break;
                    hist = PreviousMoves.Pop();
                } while(hist.target != null);
            }
            
            //PlayerHero.Instance.undo();
            //PlayerVillain.Instance.undo();
        }
    }
}

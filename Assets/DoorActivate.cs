using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorActivate : MonoBehaviour, InterfaceUndo
{
    public GameObject Wall;
    public int Turn = 3;

    private Tilemap tilemap;
    public Tile initTile;
    public Tile newTile;

    [HideInInspector]
    public int InitTurn = 0;
    
    private Vector3Int tilePosition;

    private const string SWITCH_ACTIVATE_SFX_FILEPATH = "SFX/Strange";


    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++) {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(pos) == initTile) {
                    tilePosition = pos;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Activate, undo is true if this function was called from undo function
    public void ActivateSwitch(bool fromundo)
    {
        if(!Wall.activeSelf) return;

        // SFX
        // Load the audio clip from the "Resources" folder
        AudioClip clip = Resources.Load<AudioClip>(SWITCH_ACTIVATE_SFX_FILEPATH);
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        // Switch Activation
        Wall.SetActive(false);
        GameStateManager.Instance.SwitchObject.Add(this);
        
        if(fromundo) GameStateManager.Instance.SwitchTime.Add(InitTurn + Turn + 1); // If switch is activated via Undo
        else
        {
            GameStateManager.Instance.SwitchTime.Add(GameStateManager.Instance.TurnCount + Turn);
            InitTurn = GameStateManager.Instance.TurnCount;
        }
        tilemap.SetTile(tilePosition, newTile);
        
    }

    // Reset
    public void ResetSwitch()
    {
        Wall.SetActive(true);
        tilemap.SetTile(tilePosition, initTile);

        // If Reset was not the result of Undo
        if(GameStateManager.Instance.TurnCount != InitTurn)
        {
            GameStateManager.Instance.PreviousMoves.Push(new GameStateManager.History(this.gameObject, new Vector3((float)InitTurn, 0.0f, 0.0f)));
        }
    }

    // Undo
    public void undo(Vector3 coord)
    {
        InitTurn = (int)coord.x;
        ActivateSwitch(true);
    }
}

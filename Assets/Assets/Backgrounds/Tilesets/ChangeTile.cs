using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class ChangeTile : MonoBehaviour, InterfaceUndo
{
    public Tilemap tilemap; 
    public Stack<TileBase> deletedTiles = new Stack<TileBase>();
    public Dictionary<Vector3Int, Vector3Int> switchToWall = new Dictionary<Vector3Int, Vector3Int>();

    // Start is called before the first frame update
    void Start()
    {
        // Switch to wall
        int level = GameStateManager.SceneNameToLevel(SceneManager.GetActiveScene().name);
        switch(level)
        {
            case 0:
                switchToWall.Add(new Vector3Int(4, -3, 0), new Vector3Int(-2, -3, 0));
                break;
            case 1:
                switchToWall.Add(new Vector3Int(0, -3, 0), new Vector3Int(0, 1, 0));
                break;
            case 2:
                switchToWall.Add(new Vector3Int(4, -2, 0), new Vector3Int(-1, 2, 0));
                break;
            case 3:
                switchToWall.Add(new Vector3Int(0, -1, 0), new Vector3Int(-3, 2, 0));
                break;
            case 4:
                switchToWall.Add(new Vector3Int(2, -3, 0), new Vector3Int(3, -1, 0));
                break;
            case 5:
                switchToWall.Add(new Vector3Int(1, -2, 0), new Vector3Int(1, 2, 0));
                break;
            case 9:
                switchToWall.Add(new Vector3Int(4, -2, 0), new Vector3Int(3, -4, 0));
                break;
            case 10:
                switchToWall.Add(new Vector3Int(-5, -1, 0), new Vector3Int(3, -4, 0));
                break;
            case 12:
                switchToWall.Add(new Vector3Int(3, -4, 0), new Vector3Int(4, 0, 0));
                break;
            case 13:
                switchToWall.Add(new Vector3Int(-2, -2, 0), new Vector3Int(-5, 1, 0));
                break;
            case 14:
                switchToWall.Add(new Vector3Int(-4, 3, 0), new Vector3Int(1, 1, 0));
                break;

        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void deleteTile(Vector3 coord)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(coord);
        deletedTiles.Push(tilemap.GetTile(switchToWall[cellPosition]));
        GameStateManager.Instance.PreviousMoves.Push(new GameStateManager.History(this.gameObject, tilemap.CellToWorld(switchToWall[cellPosition])));
        tilemap.SetTile(switchToWall[cellPosition], null);
    }

    public void undo(Vector3 coord)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(coord);
        tilemap.SetTile(cellPosition, deletedTiles.Pop());
    }
}

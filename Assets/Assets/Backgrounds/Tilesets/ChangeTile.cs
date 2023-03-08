using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChangeTile : MonoBehaviour, InterfaceUndo
{
    public Tilemap tilemap; 
    public Stack<TileBase> deletedTiles = new Stack<TileBase>();
    public Dictionary<Vector3Int, Vector3Int> switchToWall = new Dictionary<Vector3Int, Vector3Int>();

    // Start is called before the first frame update
    void Start()
    {
        // Switch to wall
        switchToWall.Add(new Vector3Int(3, -1, 0), new Vector3Int(0, -4, 0));

        
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerLevelSelect : MonoBehaviour
{
    Vector2[] coords = new Vector2[]{
        // World 1
        new Vector2(-0.72f, -0.72f),
        new Vector2(-0.4f, -0.56f),
        new Vector2(-0.08f, -0.4f),
        new Vector2(-0.4f, -0.08f),
        new Vector2(-0.08f, 0.08f),
        new Vector2(0.24f, 0.24f),
        
        // World 2
        new Vector2(0.56f, 1.36f),
        new Vector2(0.88f, 1.52f),
        new Vector2(0.72f, 1.84f),
        new Vector2(0.4f, 2.00f),

        // World 3
        new Vector2(-0.08f, 3.12f),
        new Vector2(-0.4f, 3.28f),
        new Vector2(-0.08f, 3.44f),
        new Vector2(-0.4f, 3.60f),

        // Final World
        new Vector2(-0.08f, 4.72f),
        new Vector2(0.24f, 4.88f),
        new Vector2(-0.08f, 5.04f),
        new Vector2(0.24f, 5.20f),
        new Vector2(-0.24f, 5.36f),
        new Vector2(0.08f, 5.52f)
    };
    int index;

    public Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        index = GlobalGameStateManager.Instance.curLevel;
        transform.position = coords[index];

        // Lock levels that you can't access
        for(int i = 0; i < coords.Length - 1; i++)
        {
            Vector3Int cellPosition; 
            
            //First level is always unlocked
            cellPosition = tilemap.WorldToCell(coords[0]);
            tilemap.SetTile(cellPosition, null);

            if(GlobalGameStateManager.Instance.clearedLevels[i])
            {
                cellPosition = tilemap.WorldToCell(coords[i+1]);
                tilemap.SetTile(cellPosition, null);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("left") || Input.GetKeyDown("down")
            || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S))
        {
            if(index > 0) transform.position = coords[--index];
        }
        else if (Input.GetKeyDown("right") || Input.GetKeyDown("up")
                || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W))
        {
            if(index + 1 < coords.Length && GlobalGameStateManager.Instance.clearedLevels[index])  transform.position = coords[++index];
        }

        if (Input.GetKeyDown("space"))
        {
            string scene = "Level" + index + "Scene";
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
    }
}

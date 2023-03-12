using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerLevelSelect : MonoBehaviour
{
    Vector2[] coords = new Vector2[]{
        new Vector2(-0.4f, -0.72f),
        new Vector2(-0.72f, -0.72f),
        new Vector2(-0.72f, -0.4f),
        new Vector2(-0.4f, -0.4f),
        
        new Vector2(0.24f + 0.16f, -0.56f),
        new Vector2(0.56f + 0.16f, -0.56f),
        new Vector2(0.56f + 0.16f, -0.24f),
        new Vector2(0.24f + 0.16f, -0.24f)
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
        if (Input.GetKeyDown("left") || Input.GetKeyDown("down"))
        {
            if(index > 0) transform.position = coords[--index];
        }
        else if (Input.GetKeyDown("right") || Input.GetKeyDown("up"))
        {
            if(index + 1 < coords.Length && GlobalGameStateManager.Instance.clearedLevels[index]) transform.position = coords[++index];
        }

        if (Input.GetKeyDown("space"))
        {
            string scene = "Level" + index + "Scene";
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
    }
}

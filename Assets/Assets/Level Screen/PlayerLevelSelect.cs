using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Start is called before the first frame update
    void Start()
    {
        transform.position = coords[0];
        index = GlobalGameStateManager.Instance.curLevel;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Skip : MonoBehaviour
{
    public string LevelName;
    public float changeTime;

    // Update is called once per frame
    void Update()
    {
        changeTime -= Time.deltaTime;
        if(changeTime <= 0f)
        {
            SceneManager.LoadScene(LevelName);
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            SceneManager.LoadScene(LevelName);
        }
    }
}

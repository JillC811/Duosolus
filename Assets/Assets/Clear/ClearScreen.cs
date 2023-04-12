using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearScreen : MonoBehaviour
{

    void Start() {
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameStateManager.Instance.LoadMap();
        }
    }
}

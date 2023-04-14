using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInstructionScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GlobalGameStateManager.Instance.clearedLevels[3]) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

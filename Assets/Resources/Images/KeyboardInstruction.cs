using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInstruction : MonoBehaviour
{
    public GameObject secondInstruction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy if anything valid is pressed
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || 
            Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) 
        {
            secondInstruction.SetActive(true);
            Destroy(gameObject);
        }
    }
}

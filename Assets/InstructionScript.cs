using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionScript : MonoBehaviour
{
    private float minY, maxY;
    private bool up = false;
    private const float MOVESPEED = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        minY = transform.position.y - 200.0f;
        maxY = transform.position.y + 200.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) gameObject.SetActive(false);
        Vector3 pos = transform.position;
        pos.y += (up?MOVESPEED:-MOVESPEED);
        transform.position = pos;
        if(pos.y >= maxY || pos.y <= minY) up = !up;
    }
}

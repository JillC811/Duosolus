using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterMovement : MonoBehaviour, InterfaceUndo
{
    public LayerMask Wall;
    public Transform MovePoint;

    public Animator animator;

    public float MoveSpeed = 1;
    public int hMove = 0;
    public int vMove = 0;
    
    // private const string Cyclope_DEATH_SFX_FILEPATH = "SFX/Hit7";

    // Start is called before the first frame update
    void Start()
    {
        MovePoint.parent = null;
        animator = GetComponent<Animator>();

        GameStateManager.Instance.MonsterList.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if this object is considered in motion, check if any arrow keys are pressed
        if (GameStateManager.Instance.ObjectsInMotion.ContainsKey(gameObject)) {
            transform.position = Vector3.MoveTowards(transform.position, MovePoint.position, MoveSpeed * Time.deltaTime);
            animator.SetBool("isMoving", true);   
        }
        else animator.SetBool("isMoving", false); // Else set the animation variable to false to disable animation
        
        //Animation Purpose DON'T TOUCH
        if (vMove != 0) animator.SetInteger("direction", 1 + vMove);
        else if (hMove != 0) animator.SetInteger("direction", 2 + hMove);

        // Check if instance is done moving
        if(transform.position == MovePoint.position && GameStateManager.Instance.ObjectsInMotion.ContainsKey(gameObject))
        {
            GameStateManager.Instance.ObjectsInMotion.Remove(gameObject);

            // Check for wall and turn
            if(Physics2D.OverlapPoint(new Vector3(transform.position.x + hMove*0.16f, transform.position.y + vMove*0.16f, 0f), LayerMask.GetMask("Wall")))
            {
                hMove = -hMove;
                vMove = -vMove;
            }
        }
        
    }

    public void Move()
    {
        GameStateManager.Instance.ObjectsInMotion.Add(gameObject, 1);
        GameStateManager.Instance.PreviousMoves.Push(new GameStateManager.History(this.gameObject, transform.position));
        if(!Physics2D.OverlapPoint(new Vector3(transform.position.x + hMove*0.16f, transform.position.y + vMove*0.16f, 0f), LayerMask.GetMask("Wall"))) 
            MovePoint.position += new Vector3(hMove * 0.16f, vMove * 0.16f, 0f);
    }

    // Undo last move, called by GameStateManager
    public void undo(Vector3 coord)
    {
        // Turn if last movement was not the expected
        float epsilon = 0.01f;
            Debug.Log(Mathf.Abs(coord.x - (transform.position.x - hMove*0.16f)));
        if(Mathf.Abs(coord.x - (transform.position.x - hMove*0.16f)) > epsilon || Mathf.Abs(coord.y - (transform.position.y - vMove*0.16f)) > epsilon)
        {
            Debug.Log(GameStateManager.Instance.ObjectsInMotion.ContainsKey(gameObject));
            hMove = -hMove;
            vMove = -vMove;
        }

        transform.position = coord;
        MovePoint.position = coord;
    }
}

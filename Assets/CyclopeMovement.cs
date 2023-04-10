using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CyclopeMovement : MonoBehaviour, InterfaceUndo
{
    public float MoveSpeed;
    public bool isDead = false;
    public Transform MovePoint;
    public LayerMask Wall;
    public LayerMask Death;

    public Animator animator;
    
    private const string Cyclope_DEATH_SFX_FILEPATH = "SFX/Hit7";

    // Start is called before the first frame update
    void Start()
    {
        GameStateManager.Instance.cyclopeActive = true;
        MovePoint.parent = null;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if space is free, move on if so
        if (GameStateManager.Instance.CyclopeMoving > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f || Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f) {
            transform.position = Vector3.MoveTowards(transform.position, MovePoint.position, MoveSpeed * Time.deltaTime);
            animator.SetBool("isMoving", true);   
        }
        else animator.SetBool("isMoving", false);
        
         //Animation Purpose
        if (MovePoint.position.y > transform.position.y)
            animator.SetInteger("direction", 2);
        else if (MovePoint.position.y < transform.position.y) animator.SetInteger("direction", 0);

        if (MovePoint.position.x > transform.position.x)
            animator.SetInteger("direction", 3);
        else if (MovePoint.position.x < transform.position.x) animator.SetInteger("direction", 1);

        // Check if instance is done moving
        if(transform.position == MovePoint.position && GameStateManager.Instance.CyclopeMoving > 0)
        {
            GameStateManager.Instance.CyclopeMoving = 0;
            
            // Check if on top of death tile
            if(Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), Death))
            {
                gameObject.SetActive(false);
                isDead = true;
                
                // SFX
                AudioClip clip = Resources.Load<AudioClip>(Cyclope_DEATH_SFX_FILEPATH);
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.volume = 0.3f;
                audioSource.Play();
            }

        }

        // If Cyclope dead, don't do anything
        if(GameStateManager.Instance.EventOccurance)
        {
            return;
        }

        if(!GameStateManager.Instance.PlayerMoving)
        {
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                // Not wall
                if(!Physics2D.OverlapPoint(transform.position + new Vector3(Input.GetAxisRaw("Horizontal")*0.16f, 0f, 0f), Wall))
                {
                    GameStateManager.Instance.CyclopeMoving += 1;
                    GameStateManager.Instance.PreviousMoves.Push(new GameStateManager.History(this.gameObject, transform.position));
                    MovePoint.position += new Vector3(Input.GetAxisRaw("Horizontal")*0.16f, 0f, 0f);
                }
            }

            else if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                if(!Physics2D.OverlapPoint(transform.position + new Vector3(0f, Input.GetAxisRaw("Vertical")*0.16f, 0f), Wall))
                {
                    GameStateManager.Instance.CyclopeMoving += 1;
                    GameStateManager.Instance.PreviousMoves.Push(new GameStateManager.History(this.gameObject, transform.position));
                    MovePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical")*0.16f, 0f);
                }
            }

            MovePoint.position = new Vector3(Mathf.Round(MovePoint.position.x / 0.08f) * 0.08f, Mathf.Round(MovePoint.position.y / 0.08f) * 0.08f, MovePoint.position.z);
        }
    }

    // Undo last move, called by GameStateManager
    public void undo(Vector3 coord)
    {
        transform.position = coord;
        MovePoint.position = coord;

        if(isDead)
        {
            GameStateManager.Instance.EventOccurance = false;
            animator.SetBool("isDead", false);
            isDead = false;
            gameObject.SetActive(true);
        }
    }
}

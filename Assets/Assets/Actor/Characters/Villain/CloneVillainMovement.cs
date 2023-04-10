using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneVillainMovement : MonoBehaviour, InterfaceUndo
{
    public float MoveSpeed;
    public Transform MovePoint;
    public LayerMask Wall;
    public LayerMask Death;
    public ChangeTile changeTileScript;
    public GameObject orangeTile;
    public GameObject blueTile;
    private Vector3 orangePosition;
    private Vector3 bluePosition;
    private bool invertedActive = false;

    public Animator animator;

    private const string VILLAIN_DEATH_SFX_FILEPATH = "SFX/Success3";
    private const string SWAP_SFX_FILEPATH = "SFX/Spirit";
    private const string TELEPORT_SFX_FILEPATH = "SFX/PowerUp1";
    private const string INVERT_SFX_FILEPATH = "SFX/Hit2";
    

    // Start is called before the first frame update
    void Start()
    {
        MovePoint.parent = null;
        animator = GetComponent<Animator>();

        orangePosition = orangeTile.transform.position;
        bluePosition = blueTile.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // If hero dead, don't do anything - or if an event is happening
        if(GameStateManager.Instance.EventOccurance)
        {
            MovePoint.position = transform.position;
            animator.SetBool("isMoving", false);
            return;
        }

        // Check if space is free, move on if so
        if (GameStateManager.Instance.VillainCloneMoving > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f || Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
        {
            if (!invertedActive) {
                transform.position = Vector3.MoveTowards(transform.position, MovePoint.position, MoveSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 oppositeDirection = new Vector3(-Input.GetAxisRaw("Horizontal"), -Input.GetAxisRaw("Vertical"), 0f);
                MovePoint.position = transform.position + oppositeDirection;
                transform.position = Vector3.MoveTowards(transform.position, MovePoint.position, MoveSpeed * Time.deltaTime);
            }
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
        if(transform.position == MovePoint.position && GameStateManager.Instance.VillainCloneMoving > 0)
        {
            GameStateManager.Instance.VillainCloneMoving = 0;

            // Check if on top of oneway
            if(Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Oneway")))
            {
                GameObject obj = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Oneway")).gameObject;
                switch(obj.name)
                {
                    case "Oneway_down":
                        MovePoint.position = new Vector3(MovePoint.position.x, MovePoint.position.y - 0.16f, MovePoint.position.z);
                        transform.position = new Vector3(transform.position.x, transform.position.y - 0.16f, transform.position.z);
                        break;
                    case "Oneway_up":
                        MovePoint.position = new Vector3(MovePoint.position.x, MovePoint.position.y + 0.16f, MovePoint.position.z);
                        transform.position = new Vector3(transform.position.x, transform.position.y + 0.16f, transform.position.z);
                        break;
                    case "Oneway_left":
                        MovePoint.position = new Vector3(MovePoint.position.x - 0.16f, MovePoint.position.y, MovePoint.position.z);
                        transform.position = new Vector3(transform.position.x - 0.16f, transform.position.y, transform.position.z);
                        break;
                    case "Oneway_right":
                        MovePoint.position = new Vector3(MovePoint.position.x + 0.16f, MovePoint.position.y, MovePoint.position.z);
                        transform.position = new Vector3(transform.position.x + 0.16f, transform.position.y, transform.position.z);
                        break;
                }
            }
            
            // Check if on top of death tile
            if((Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), Death) || Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Villain_Enemy")) || Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Neutral_Enemy"))) && !GameStateManager.Instance.EventOccurance)
            {
                animator.SetBool("isDead", true);
                GameStateManager.Instance.EventOccurance = true;
                GameStateManager.Instance.Clear();

                // SFX
                AudioClip clip = Resources.Load<AudioClip>(VILLAIN_DEATH_SFX_FILEPATH);
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.volume = 0.3f;
                audioSource.Play();
            }

            // Check if on top of switch tile
            if (Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Switch")))
            {
                Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position, LayerMask.GetMask("Switch"));
                foreach (Collider2D collider in colliders)
                {
                    changeTileScript = collider.GetComponent<ChangeTile>();
                    if (changeTileScript != null)
                    {
                        changeTileScript.deleteTile(transform.position);
                    }
                }
            }

            // Check if on top of a teleportation tile
            if(Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Teleport")))
            {
                GameObject obj = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Teleport")).gameObject;
                switch(obj.name)
                {
                    case "Teleport_Blue":
                        transform.position = orangePosition;
                        MovePoint.position = transform.position;
                        break;
                    case "Teleport_Orange":
                        transform.position = bluePosition;
                        MovePoint.position = transform.position;
                        break;
                }

                // SFX
                 AudioClip clip = Resources.Load<AudioClip>(TELEPORT_SFX_FILEPATH);
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.volume = 0.3f;
                audioSource.Play();
            }

            // Check if on top of timed door switch
            if(Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Timer")))
            {
                DoorActivate timedDoor = Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Timer")).GetComponent<DoorActivate>();
                timedDoor.ActivateSwitch(false);
            }

             // Check if on top of a control inverter tile
            if(Physics2D.OverlapPoint(new Vector3(transform.position.x, transform.position.y), LayerMask.GetMask("Invert"))) {
                if (invertedActive) {
                    invertedActive = false;
                }
                else {
                    invertedActive = true;
                }
                // SFX
                AudioClip clip = Resources.Load<AudioClip>(INVERT_SFX_FILEPATH);
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.volume = 0.3f;
                audioSource.Play();
            }
        }

        if(!GameStateManager.Instance.PlayerMoving)
        {
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                // Not wall
                if(!Physics2D.OverlapPoint(transform.position + new Vector3(Input.GetAxisRaw("Horizontal")*0.16f, 0f, 0f), Wall))
                {
                    GameStateManager.Instance.VillainCloneMoving += 1;
                    GameStateManager.Instance.PreviousMoves.Push(new GameStateManager.History(this.gameObject, transform.position));
                    MovePoint.position += new Vector3(Input.GetAxisRaw("Horizontal")*0.16f, 0f, 0f);
                }
            }

            else if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                if(!Physics2D.OverlapPoint(transform.position + new Vector3(0f, Input.GetAxisRaw("Vertical")*0.16f, 0f), Wall))
                {
                    GameStateManager.Instance.VillainCloneMoving += 1;
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
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeroMovement : MonoBehaviour, InterfaceUndo
{
    public float MoveSpeed;
    public bool isDead = false;
    public Transform MovePoint;
    public LayerMask Wall;
    public LayerMask Death;
    public LayerMask Villain;
    public ChangeTile changeTileScript;
    public GameObject deadScreenUI;
    public GameObject orangeTile;
    public GameObject blueTile;
    private Vector3 orangePosition;
    private Vector3 bluePosition;
    public GameObject duplicationDestination;
    private bool invertedActive = false;

    public Animator animator;
    
    private const string HERO_DEATH_SFX_FILEPATH = "SFX/Hit7";
    private const string SWITCH_PRESS_SFX_FILEPATH = "SFX/Magic2";
    private const string SWAP_SFX_FILEPATH = "SFX/Spirit";
    private const string TELEPORT_SFX_FILEPATH = "SFX/PowerUp1";
    private const string INVERT_SFX_FILEPATH = "SFX/Hit2";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if(orangeTile != null) orangePosition = orangeTile.transform.position;
        if(blueTile != null) bluePosition = blueTile.transform.position;

        // Free the MovePoint
        MovePoint.parent = null;    
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if this object is considered in motion, check if any arrow keys are pressed
        if (GameStateManager.Instance.ObjectsInMotion.ContainsKey(gameObject) || Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f || Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f) {
            Debug.Log(MovePoint.position);
            // Move the object towards "MovePoint" object
            transform.position = Vector3.MoveTowards(transform.position, MovePoint.position, MoveSpeed * Time.deltaTime);
            // Set the animation variable to true
            animator.SetBool("isMoving", true);   
        }
        else animator.SetBool("isMoving", false); // Else set the animation variable to false to disable animation
        
        //Animation Purpose DON'T TOUCH
        if (MovePoint.position.y > transform.position.y)
            animator.SetInteger("direction", 2);
        else if (MovePoint.position.y < transform.position.y) animator.SetInteger("direction", 0);

        if (MovePoint.position.x > transform.position.x)
            animator.SetInteger("direction", 3);
        else if (MovePoint.position.x < transform.position.x) animator.SetInteger("direction", 1);

        // Check if instance is done moving
        if(transform.position == MovePoint.position && GameStateManager.Instance.ObjectsInMotion.ContainsKey(gameObject))
        {
            GameStateManager.Instance.ObjectsInMotion.Remove(gameObject);

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
            if(Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), Death) || Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Hero_Enemy")) || Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Neutral_Enemy")))
            {
                deadScreenUI.SetActive(true);
                isDead = true;
                animator.SetBool("isDead", true);
                GameStateManager.Instance.EventOccurance = true;
                GameStateManager.Instance.ObjectsInMotion.Clear();
                // GameStateManager.Instance.VillainMoving = 0;
                
                // SFX
                AudioClip clip = Resources.Load<AudioClip>(HERO_DEATH_SFX_FILEPATH);
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
                        // SFX
                        AudioClip clip = Resources.Load<AudioClip>(SWITCH_PRESS_SFX_FILEPATH);
                        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                        audioSource.clip = clip;
                        audioSource.volume = 0.3f;
                        audioSource.Play();
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

            // Check if on top of a swap tile
            if(Physics2D.OverlapPoint(new Vector3(transform.position.x, transform.position.y), LayerMask.GetMask("Swap")))
            {   
                GameObject[] villains = GameObject.FindGameObjectsWithTag("Player_Villain");
                float minDistance = Mathf.Infinity;
                GameObject closestVillain = null;
                Vector3 currentPosition = transform.position;

                foreach (GameObject villain in villains) {
                    float distance = Vector3.Distance(villain.transform.position, currentPosition);
                    if (distance < minDistance) {
                        minDistance = distance;
                        closestVillain = villain;
                    }
                }

                if (closestVillain != null) {
                    Vector3 HeroPosition = transform.position;
                    Vector3 HeroMovePoint = MovePoint.position;
                    MovePoint.position = closestVillain.transform.position;
                    closestVillain.transform.position = HeroMovePoint;
                    transform.position = closestVillain.transform.position;
                    closestVillain.transform.position = HeroPosition;
                    
                    // SFX
                    AudioClip clip = Resources.Load<AudioClip>(SWAP_SFX_FILEPATH);
                    AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.clip = clip;
                    audioSource.volume = 0.3f;
                    audioSource.Play();
                }


                
            }

            // Check if on top of a duplicator
            
            if(Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Duplicator")))
            {
                GameObject obj = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Duplicator")).gameObject;
                // Bug - duplicator works even when there's already an object at the destination
                // if (Physics2D.OverlapPoint(new Vector2(duplicationDestination.transform.position.x, duplicationDestination.transform.position.y), LayerMask.GetMask("Player_Hero", "Player_Villain")) == null) 
                if(true)
                {
                    Debug.Log(Physics2D.OverlapPoint(new Vector2(duplicationDestination.transform.position.x, duplicationDestination.transform.position.y), LayerMask.GetMask("Player_Hero", "Player_Villain")));

                    GameObject duplicate = Instantiate(gameObject);
                    duplicate.transform.position = duplicationDestination.transform.position;

                    GameObject duplicateMP = Instantiate(MovePoint.gameObject);
                    duplicateMP.transform.position = duplicate.transform.position;

                    HeroMovement hm = duplicate.GetComponent<HeroMovement>();
                    Transform ht = duplicateMP.GetComponent<Transform>();
                    hm.MovePoint = ht;

                    // obj.SetActive(false);
                    
                    GameStateManager.Instance.PreviousMoves.Push(new GameStateManager.History(duplicate, new Vector3(-1, -1, -1)));
                }
            }

            // Check if on top of timed door switch
            if(Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Timer")))
            {
                DoorActivate timedDoor = Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask.GetMask("Timer")).GetComponent<DoorActivate>();
                timedDoor.ActivateSwitch(false);
            }

            // Check if on top of a control inverter tile
            if(Physics2D.OverlapPoint(new Vector3(transform.position.x, transform.position.y), LayerMask.GetMask("Invert"))) {
                invertedActive = !invertedActive;
                // SFX
                AudioClip clip = Resources.Load<AudioClip>(INVERT_SFX_FILEPATH);
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.volume = 0.3f;
                audioSource.Play();
            }
        }

        // If hero dead, don't do anything
        if(GameStateManager.Instance.EventOccurance)
        {
            return;
        }

        if(!GameStateManager.Instance.PlayerMoving)
        {
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                // Not wall
                if(!Physics2D.OverlapPoint(transform.position + new Vector3((invertedActive?-1:1) * Input.GetAxisRaw("Horizontal")*0.16f, 0f, 0f), Wall))
                {
                    GameStateManager.Instance.ObjectsInMotion.Add(gameObject, 1);
                    GameStateManager.Instance.PreviousMoves.Push(new GameStateManager.History(this.gameObject, transform.position));
                    MovePoint.position += new Vector3((invertedActive?-1:1) * Input.GetAxisRaw("Horizontal")*0.16f, 0f, 0f);
                }
            }

            else if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                if(!Physics2D.OverlapPoint(transform.position + new Vector3(0f, (invertedActive?-1:1) * Input.GetAxisRaw("Vertical")*0.16f, 0f), Wall))
                {
                    GameStateManager.Instance.ObjectsInMotion.Add(gameObject, 1);
                    GameStateManager.Instance.PreviousMoves.Push(new GameStateManager.History(this.gameObject, transform.position));
                    MovePoint.position += new Vector3(0f, (invertedActive?-1:1) * Input.GetAxisRaw("Vertical")*0.16f, 0f);
                }
            }

            MovePoint.position = new Vector3(Mathf.Round(MovePoint.position.x / 0.08f) * 0.08f, Mathf.Round(MovePoint.position.y / 0.08f) * 0.08f, MovePoint.position.z);
        }
    }

    // Undo last move, called by GameStateManager
    public void undo(Vector3 coord)
    {
        if(coord.x == -1 && coord.y == -1 && coord.z == -1)
        {
            Destroy(MovePoint.gameObject);
            Destroy(gameObject);
            return;
        }

        transform.position = coord;
        MovePoint.position = coord;

        if(isDead)
        {
            GameStateManager.Instance.EventOccurance = false;
            animator.SetBool("isDead", false);
            isDead = false;
            deadScreenUI.SetActive(false);
        }
    }
}


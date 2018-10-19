using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	public float fSpeed = 4.0f;
	[Range(1,10)]
	public float jumpForce = 6.5f;

	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;

	public int isGrounded = 2;
	private Rigidbody rb;
    public Vector3 currentVelocity;

    public GameObject SwordSwipe;
    private GameObject MainCamera;

    public int health = 100;
    public Slider healthSlider;

    private int hurtBoxDamage = 10;
    private Animator karaAnim;
    private Transform panelTrans;
    private Animator panelAnim;

    public string previousScene;

    //Leave public until further testing
    private const float tDamageInvulnMAX = 2.5f;
    public float tDamageInvuln = 0.0f;
    private const float tComboMAX = 1.0f;
    public float tCombo = 0.0f;
    public int combo = 0;

    public bool bDoorLockout = false;

    // Use this for initialization
    void Start () {
        Debug.Log("Start!");
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        healthSlider = MainCamera.GetComponentInChildren<Slider>();
        healthSlider.value = health;

        karaAnim = GetComponentInChildren<Animator>();

        rb = this.GetComponent<Rigidbody>();

        DontDestroyOnLoad(this.gameObject);
    }

	void OnCollisionEnter(Collision other){
        Debug.Log("Player Hit something!");
        if (other.collider.tag == "Environment")
        {
            Debug.Log("PlayerOnCollisionEnterEnvironment");
            
            isGrounded = 2;
        }
        if(other.collider.tag == "HurtBox")
        {
            //Debug.Log("PlayerOnCollisionEnterHurtBox");
            SendMessage("ApplyDamage", hurtBoxDamage);
        }
	}

    void OnCollisionExit(Collision other)
    {
        if(other.collider.tag == "Environment")
        {
            
        }
    }
	
	// Update is called once per frame
	void Update () {
        //Capture current velocity
        currentVelocity = rb.velocity;
        //Left Right movement
        Movement();

        //Attack thingy
        Attack();

        //Basic jump
        Jump();

        //Many many timers
        Timers();

        //Send velocity update
        rb.velocity = currentVelocity;
	}

    void ApplyDamage(int damage)
    {
        if(tDamageInvuln >= 0.0f)
        {
            health -= damage;
            healthSlider.value = health;
            tDamageInvuln = tDamageInvulnMAX;
        }
    }

    //Movement
    void Movement()
    {
        float InputX = Input.GetAxis("Horizontal");
        currentVelocity.x = InputX  * fSpeed;

        if (InputX != 0.0f)
        {
            karaAnim.SetFloat("InputX", InputX);
        }
        else
        {
            karaAnim.SetFloat("InputX", 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.A) ||
            Input.GetAxis("Horizontal") < 0.0f)
        {
            transform.forward = Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.D) ||
            Input.GetAxis("Horizontal") > 0.0f)
        {
            transform.forward = Vector3.right;
        }
    }
    //Jump
    void Jump()
    {
        if(Input.GetButtonDown ("Jump") && isGrounded != 0 )
        {  
		    currentVelocity.y = jumpForce;
		    isGrounded--;
        }
        karaAnim.SetInteger("Jump", isGrounded);
    }
    //Attack
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) ||
            Input.GetKeyDown(KeyCode.RightControl) ||
            Input.GetButtonDown("Fire1"))
        {
            //Player swung, do stuff
            GameObject sword = Instantiate(SwordSwipe) as GameObject;
            sword.transform.parent = this.transform;
            sword.transform.localPosition = Vector3.zero + new Vector3(0.0f, 1.0f, 0.0f);
            Destroy(sword, .25f);

            if (combo == 4)
            {
                combo = 0;
            }
            else
            {
                combo++;
            }

            if (karaAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack 2"))
            {
                //Third attack of combo
                karaAnim.SetInteger("Combo", combo);
                tCombo = tComboMAX;
            }
            else if (karaAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack 1"))
            {
                //Second attack of combo
                karaAnim.SetInteger("Combo", combo);
                tCombo = tComboMAX;
            }
            else
            {
                //First attack of combo
                karaAnim.SetInteger("Combo", combo);
                tCombo = tComboMAX;
            }
        }
        else
        {
            //Player did not swing, do housekeeping
            if(tCombo <= 0.0f)
            {
                combo = 0;
                karaAnim.SetInteger("Combo", combo);
            }
            if (combo == 4)
            {
                combo = 0;
            }
        }
    }

    //Obsolete function
    void BetterJump()
	{
		if (currentVelocity.y < 0) {
			currentVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		} 
		else if(currentVelocity.y > 0 && !Input.GetButton("Jump")) {
			currentVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
		}
	}

    void ChangeScene(DoorTransporter exitdoor)
    {
        if(bDoorLockout == false)
        {
            bDoorLockout = true;
            //coroutine because reasons
            previousScene = exitdoor.currentScene;
            StartCoroutine(LoadScene(exitdoor.destinationScene));
        }
        else
        {
            //Do Nothing
        }
    }

    IEnumerator LoadScene(string destinationScene)
    {
        //Fade
        panelAnim.SetBool("end", true);
        panelAnim.SetBool("start", false);
        //Transition and loop until done
        AsyncOperation async = SceneManager.LoadSceneAsync(destinationScene);
        while(!async.isDone)
        {
            yield return null;
        }
        if(async.isDone)
        {
            //Find Spot
            FindDoorMat();
            
            //Reveal
            panelAnim.SetBool("start", true);
            panelAnim.SetBool("end", false);
            bDoorLockout = false;
        }
    }

    void FindDoorMat()
    {
        //Find right doormat
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        string doorDestination;
        foreach (GameObject door in doors)
        {
            //Compare previous scene vs door to find right spot
            doorDestination = door.GetComponent<DoorTransporter>().destinationScene;
            if (doorDestination == this.previousScene)
            {
                transform.position = door.transform.Find("DoorMat").position;
            }
        }
    }

    void Timers()
    {
        float timeChange = Time.deltaTime;
        TimerDamageInvuln(timeChange);
        TimerCombo(timeChange);
    }

    void TimerDamageInvuln(float timeChange)
    {
        if (tDamageInvuln >= 0.0f)
        {
            tDamageInvuln -= timeChange;
        }
    }

    void TimerCombo(float timeChange)
    {
        if (tCombo >= 0.0f)
        {
            tCombo -= timeChange;
        }
    }
}

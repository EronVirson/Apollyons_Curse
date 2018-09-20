using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	float fSpeed = 400.0f;
	[Range(1,10)]
	public float jumpForce = 6.5f;

	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;

	public int isGrounded = 2;
	public Rigidbody rb;
    public Vector3 currentVelocity;

    public float tapSpeed = 0.5f;
    private float lastTapTime = 0.0f;

    public GameObject SwordSwipe;

    public int health = 100;
    public Slider healthSlider;

    private int hurtBoxDamage = 10;
    public Animator anim;

    public enum animState
    {
        Idle,
        Combat,
        Running,
        Jump,
        SecondJump
    }
    public animState currentAnim;
    public animState lastAnim;

	// Use this for initialization
	void Start () {
        Debug.Log("Start!");
        healthSlider = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Slider>();
        rb = this.GetComponent<Rigidbody>();
        healthSlider.value = health;
        //anim = GetComponentInChildren<Animator>();
        
    }

	void OnCollisionEnter(Collision other){
        Debug.Log("Player Hit something!");
        if (other.collider.tag == "Environment")
        {
            Debug.Log("PlayerOnCollisionEnterEnvironment");
            //rb.useGravity = false;
            //isGrounded = true;
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
            //rb.useGravity = true;
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

        //Send velocity update
        rb.velocity = currentVelocity;
	}

    void ApplyDamage(int damage)
    {
        health -= damage;
        healthSlider.value = health;
    }

    //Movement
    void Movement()
    {
        currentVelocity.x = Input.GetAxis("Horizontal") * Time.deltaTime * fSpeed;

        if (Input.GetAxis("Horizontal") != 0.0f)
        {
            //anim.SetBool("IsRunning", true);
        }
        else
        {
            //anim.SetBool("IsRunning", false);
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
        if(Input.GetButtonDown ("Jump") && isGrounded != 0 ) {
            
			currentVelocity.y = jumpForce;
			isGrounded--;
            //anim.SetInteger("IsJumping", isGrounded);
        }
    }
    //Attack
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) ||
            Input.GetKeyDown(KeyCode.RightControl) ||
            Input.GetButtonDown("Fire1"))
        {
            //anim.SetBool("InCombat", true);
            GameObject sword = Instantiate(SwordSwipe) as GameObject;
            sword.transform.parent = this.transform;
            sword.transform.localPosition = Vector3.zero + new Vector3(0.0f, 1.0f, 0.0f);
            Destroy(sword, .25f);
        }
        else
        {
            //anim.SetBool("InCombat", false);
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
}

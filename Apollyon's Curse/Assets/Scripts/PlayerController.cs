using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	float fSpeed = 400.0f;
	[Range(1,10)]
	public float jumpForce = 9.0f;

	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;

	public int isGrounded = 2;
	Rigidbody rb;
    public Vector3 currentVelocity;

    public float tapSpeed = 0.5f;
    private float lastTapTime = 0.0f;

    public GameObject SwordSwipe;

    public int health = 100;
    public Slider healthSlider;

    private int hurtBoxDamage = 10;
    public Animator anim;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
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
        currentVelocity.x =  Input.GetAxis("Horizontal") * Time.deltaTime * fSpeed;

        if (Input.GetAxis("Horizontal") != 0.0f )
        {
            //anim.Play("Running");
        }
        if(Input.GetAxis("Horizontal") == 0.0f)
        {
            //anim.Play("Idle");
        }

        //Attack thingy
		if(Input.GetKeyDown(KeyCode.Mouse0) || 
            Input.GetKeyDown(KeyCode.RightControl) ||
            Input.GetButtonDown("Fire1"))
        {
            GameObject sword = Instantiate(SwordSwipe) as GameObject;
            sword.transform.parent = this.transform;
            sword.transform.localPosition = Vector3.zero + new Vector3(0.0f, 1.0f, 0.0f);
            Destroy(sword, .25f);
        }
        
        //Double Tap Dash
        if (Input.GetKeyDown(KeyCode.A) ||
			Input.GetAxis("Horizontal") < 0.0f)
        {
            transform.forward = Vector3.left;
            if((Time.time - lastTapTime) < tapSpeed)
            {
                //Tap Success
                Debug.Log("Double A");
                //currentVelocity += Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime * fSpeed * 100;
            }
            lastTapTime = Time.time;
        }
		if (Input.GetKeyDown(KeyCode.D) ||
			Input.GetAxis("Horizontal") > 0.0f)
        {
            transform.forward = Vector3.right;
            if ((Time.time - lastTapTime) < tapSpeed)
            {
                //Tap Success
                Debug.Log("Double D");
                //currentVelocity += Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime * fSpeed * 100;
            }
            lastTapTime = Time.time;
        }
        

        //Basic jump
        if (Input.GetButtonDown ("Jump") && isGrounded != 0 ) {
            //anim.Play("Jump");
			currentVelocity.y = jumpForce;
			//isGrounded = false;
			isGrounded--;
		}
		//Better Jump
		//BetterJump();

        //Send velocity update
        rb.velocity = currentVelocity;
	}

    void ApplyDamage(int damage)
    {
        health -= damage;
        healthSlider.value = health;
    }

   
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

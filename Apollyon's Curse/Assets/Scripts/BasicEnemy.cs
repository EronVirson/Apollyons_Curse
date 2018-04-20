using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {

	public GameObject self;
	public GameObject player;

	public float fSpeed = 10.0f;
    [Range(1, 10)]
    public float jumpForce = 9.0f;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public int isGrounded = 2;
    Rigidbody rb;
    public Vector3 currentVelocity;

    public GameObject SwordSwipe;

    public int health = 10;

    public int hurtBoxDamage = 10;

	public enum EnemyState
	{
		Idle,
		Chase,
		Attack,
		Broken
	}
	public EnemyState state;

	public float attackDelay = 2.0f;
	private float attackDelayDefault = 2.0f;
	public float attackRange = 2.0f;

	public float triggerDistance = 5.0f;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
		state = EnemyState.Idle;
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "HurtBox")
        {
            //Debug.Log("PlayerOnCollisionEnterHurtBox");
            SendMessage("ApplyDamage", hurtBoxDamage);
        }
        if (other.collider.tag == "HitBox")
        {
            //Debug.Log("PlayerOnCollisionEnterHurtBox");
            SendMessage("ApplyDamage", hurtBoxDamage);
        }
    }

    // Update is called once per frame
    void Update () {
        //Capture current velocity
        currentVelocity = rb.velocity;
        
        if(health <= 0)
        {
            state = EnemyState.Broken;
        }

		switch (state)
		{
		case EnemyState.Idle:
			Idle();
			break;
		case EnemyState.Chase:
			Chase();
			break;
		case EnemyState.Attack:
			Attack();
			break;
		case EnemyState.Broken:
			Broken ();
			break;
		}

		//Better Jump
        /* 
        if (currentVelocity.y < 0)
        {
            currentVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (currentVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            currentVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        } 
        */
        //Send velocity update
        rb.velocity = currentVelocity;
    }

	void Idle()
	{
        
		//Is the player nearby?
		if((Vector3.Distance(transform.position, player.transform.position) <= triggerDistance))
		{
			//Yes, trigger the trap
			state = EnemyState.Chase;
		}
	}

	void Chase()
	{
		//Is player close enough to attack?
		if((Vector3.Distance(transform.position, player.transform.position) <= attackRange))
		{
			if (attackDelay <= 0.0f) {
				state = EnemyState.Attack;
				return;
			}
			if (attackDelay > 0.0f) {
				attackDelay -= Time.deltaTime;
			}
		}
		//Is the player to the left?
		if (player.transform.position.x < transform.position.x) {
			MoveLeft ();
		}

		//Is the player to the right?
		if (player.transform.position.x > transform.position.x) {
			MoveRight ();
		}
	}

	void Attack()
	{
		Debug.Log ("Attacking Player!");
        SwingSword();
		attackDelay = attackDelayDefault;
        state = EnemyState.Idle;
	}

	void Broken()
	{
		Destroy (self);
	}

    void Jump()
    {
        if (isGrounded != 0)
        {
            currentVelocity = Vector3.up * jumpForce;
            //isGrounded = false;
            isGrounded--;
        }
    }

    void MoveLeft()
    {
		transform.forward = Vector3.left;
        currentVelocity += Vector3.left * Time.deltaTime * fSpeed;
    }
    void MoveRight()
    {
		transform.forward = Vector3.right;
        currentVelocity += Vector3.right * Time.deltaTime * fSpeed;
    }

    void SwingSword()
    {
        GameObject sword = Instantiate(SwordSwipe) as GameObject;
        sword.transform.parent = this.transform;
        sword.transform.localPosition = Vector3.zero + new Vector3(0.0f, 1.0f, 0.0f);
        Destroy(sword, .25f);
    }

    void ApplyDamage(int damage)
    {
        health -= damage;
    }
}

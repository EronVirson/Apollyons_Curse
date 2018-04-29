using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {

    public GameObject self;
    public GameObject player;

    float fSpeed = 300.0f;
    [Range(1, 10)]
    public float jumpForce = 9.0f;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public int isGrounded = 1;
    Rigidbody rb;
    public Vector3 currentVelocity;

    public GameObject SwordSwipe;

    public int health = 10;

    public int hurtBoxDamage = 10;
    public float distanceToPlayer;

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

    GenericTrigger ChaseRange;
    GenericTrigger AttackRange;
    GenericTrigger PersonalSpaceRange;
    GenericTrigger BufferSpaceRange;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();

        //Collect scripts
        ChaseRange = transform.Find("ChaseRange").GetComponent<GenericTrigger>();
        AttackRange = transform.Find("AttackRange").GetComponent<GenericTrigger>();
        PersonalSpaceRange = transform.Find("PersonalSpaceRange").GetComponent<GenericTrigger>();
        BufferSpaceRange = transform.Find("BufferSpaceRange").GetComponent<GenericTrigger>();

        player = GameObject.FindGameObjectWithTag("Player");
        state = EnemyState.Idle;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Environment")
        {
            isGrounded = 1;
        }
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
    void Update() {
        //Capture current velocity
        currentVelocity = rb.velocity;
        currentVelocity.x = 0.0f;

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > triggerDistance)
        {
            state = EnemyState.Idle;
        }

        if (health <= 0)
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
                Broken();
                break;
        }


        //Send velocity update
        rb.velocity = currentVelocity;
    }

    void Idle()
    {

        //Is the player nearby?
        if (ChaseRange.isTouchingPlayer)
        {
            //Yes, chase them
            state = EnemyState.Chase;
        }
    }

    void Chase()
    {
        //Is player close enough to attack?
        if (AttackRange.isTouchingPlayer)
        {
            if (attackDelay <= 0.0f) {
                state = EnemyState.Attack;
                return;
            }
            if (attackDelay > 0.0f) {
                attackDelay -= Time.deltaTime;
            }
        }
        //Is player too far away?
        if (!ChaseRange.isTouchingPlayer)
        {
            state = EnemyState.Idle;
        }
        //Is player above?
        if (player.transform.position.y > transform.position.y)
        {
            Jump();
        }
        //Is player within Personal Space aka bubble?
        if (PersonalSpaceRange.isTouchingPlayer)
        {
            //Respect distance and exit
            MoveAway();
            return;
        }
        if(BufferSpaceRange.isTouchingPlayer)
        {
            //Don't move and exit
            return;
        }
        MoveTowards();

    }

    void MoveTowards()
    {
        //Is the player to the left?
        if (player.transform.position.x < transform.position.x)
        {
            MoveLeft();
        }

        //Is the player to the right?
        if (player.transform.position.x > transform.position.x)
        {
            MoveRight();
        }
    }
    void MoveAway()
    {
        //Is the player to the left?
        if (player.transform.position.x < transform.position.x)
        {
            MoveRight();
        }

        //Is the player to the right?
        if (player.transform.position.x > transform.position.x)
        {
            MoveLeft();
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
        MoveTowards();
        if (isGrounded != 0)
        {
            currentVelocity = Vector3.up * jumpForce;
            
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

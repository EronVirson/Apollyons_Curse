using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {

    float fSpeed = 400.0f;
    [Range(1, 10)]
    public float jumpForce = 9.0f;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public int isGrounded = 2;
    Rigidbody rb;
    public Vector3 currentVelocity;

    public GameObject SwordSwipe;

    public int health = 100;

    public int hurtBoxDamage = 10;

    // Use this for initialization
    void Start () {
		
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
        
        //Better Jump
        if (currentVelocity.y < 0)
        {
            currentVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (currentVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            currentVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        //Send velocity update
        rb.velocity = currentVelocity;
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
        currentVelocity += Vector3.left * Time.deltaTime * fSpeed;
    }
    void MoveRight()
    {
        currentVelocity += Vector3.right * Time.deltaTime * fSpeed;
    }

    void ApplyDamage(int damage)
    {
        health -= damage;
    }
}

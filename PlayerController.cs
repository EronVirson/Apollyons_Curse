﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private int hurtBoxDamage = 10;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	void OnCollisionEnter(Collision other){
        Debug.Log("Player Hit something!");
        if (other.collider.tag == "Environment")
        {
            Debug.Log("PlayerOnCollisionEnterEnvironment");
            //isGrounded = true;
            isGrounded = 2;
        }
        if(other.collider.tag == "HurtBox")
        {
            //Debug.Log("PlayerOnCollisionEnterHurtBox");
            SendMessage("ApplyDamage", hurtBoxDamage);
        }
	}
	
	// Update is called once per frame
	void Update () {
        //Capture current velocity
        currentVelocity = rb.velocity;
        //Left Right movement
        currentVelocity.x = 0.0f;
        currentVelocity += Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime * fSpeed;

        //Attack thingy
		if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.RightControl))
        {
            GameObject sword = Instantiate(SwordSwipe) as GameObject;
            sword.transform.parent = this.transform;
            sword.transform.localPosition = Vector3.zero + new Vector3(0.0f, 1.0f, 0.0f);
            Destroy(sword, .25f);
        }
        
        //Double Tap Dash
        if (Input.GetKeyDown(KeyCode.A))
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
        if (Input.GetKeyDown(KeyCode.D))
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
			currentVelocity = Vector3.up * jumpForce;
			//isGrounded = false;
			isGrounded--;
		}
		//Better Jump
		if (currentVelocity.y < 0) {
			currentVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		} 
		else if(currentVelocity.y > 0 && !Input.GetButton("Jump")) {
			currentVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
		}
        //Send velocity update
        rb.velocity = currentVelocity;
    }

    void ApplyDamage(int damage)
    {
        health -= damage;
    }
}
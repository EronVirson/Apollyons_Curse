using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControls : MonoBehaviour {

	float fSpeed = 400.0f;
	[Range(1,10)]
	public float jumpForce = 9.0f;

	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;

	public bool isGrounded;
	Rigidbody rb;
    public Vector3 currentVelocity;

    public float tapSpeed = 0.5f;
    private float lastTapTime = 0.0f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	void OnCollisionEnter(){
		isGrounded = true;
	}
	
	// Update is called once per frame
	void Update () {
        //Capture current velocity
        currentVelocity = rb.velocity;
        //Left Right movement
        currentVelocity.x = 0.0f;
        currentVelocity += Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime * fSpeed;

        //Double Tap Dash
        if (Input.GetKeyDown(KeyCode.A))
        {
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
            if ((Time.time - lastTapTime) < tapSpeed)
            {
                //Tap Success
                Debug.Log("Double D");
                //currentVelocity += Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime * fSpeed * 100;
            }
            lastTapTime = Time.time;
        }
        

        //Basic jump
        if (Input.GetButtonDown ("Jump") && isGrounded ) {
			currentVelocity = Vector3.up * jumpForce;
			isGrounded = false;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	float fSpeed = 40.0f;
	public Vector3 jump;
	[Range(1,10)]
	public float jumpForce = 5.0f;

	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;

	public bool isGrounded;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		jump = new Vector3 (0.0f, 2.0f, 0.0f);
	}

	void OnCollisionStay(){
		isGrounded = true;
	}
	
	// Update is called once per frame
	void Update () {
        //Left Right movement
        rb.velocity += Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime * fSpeed;

        /*
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * fSpeed;
        transform.Rotate(0, 0, 0);
        transform.Translate(x, 0, 0);
        */
		//Basic jump
		if (Input.GetButtonDown ("Jump") && isGrounded ) {
			rb.velocity = jump * jumpForce;
			isGrounded = false;
		}
		//Better Jump
		if (rb.velocity.y < 0) {
			rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		} 
		else if(rb.velocity.y > 0 && !Input.GetButton("Jump")) {
			rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
		}
    }
}

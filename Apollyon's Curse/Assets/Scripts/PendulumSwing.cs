using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumSwing : MonoBehaviour {

	public Rigidbody rb;
	public float leftPushRange;
	public float rightPushRange;
	public Vector3 velocityThreshold = new Vector3(0.0f, 0.0f, 120.0f);
	public Vector3 AngularCapture;
	public float AngMagnitude;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.angularVelocity = velocityThreshold;
	}
	
	// Update is called once per frame
	void Update () {
		Push ();
		AngularCapture = rb.angularVelocity;
		AngMagnitude = rb.angularVelocity.magnitude;
	}

	public void Push()
	{
		if (transform.rotation.z > 90
		    && transform.rotation.z < rightPushRange
		    && (rb.angularVelocity.z > 0)
			&& rb.angularVelocity.magnitude < velocityThreshold.magnitude) {
			Debug.Log ("Right-ish");
			rb.angularVelocity = velocityThreshold;
		} else if (transform.rotation.z < 90
		         && transform.rotation.z > leftPushRange
		         && (rb.angularVelocity.z < 0)
		         && rb.angularVelocity.magnitude > velocityThreshold.magnitude * -1) {
			Debug.Log ("Left-ish");
			rb.angularVelocity = velocityThreshold * -1;
		}
	}
}

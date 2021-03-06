﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;
	public float smoothSpeed = 0.125f;

	public Vector3 offset;

	// Use this for initialization
	void Start () {
		//Calculate and store the offset value by getting the distance between the player's position and camera's position.
		offset = new Vector3(0, 1, -10);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    

    // Update is called once per frame
    void FixedUpdate () {
		// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
		Vector3 desiredPosition = player.transform.position + offset;
        //Smoothly move camera towards player
		Vector3 smoothedPosition = Vector3.Lerp (transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;

		transform.LookAt (player.transform.position);
	}
}

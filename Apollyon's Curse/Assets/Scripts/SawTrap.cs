using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrap : MonoBehaviour {

	public GameObject player;
	public GameObject startPosition;
	public GameObject endPosition;
	public GameObject Saw;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		Saw.transform.position = startPosition.transform.position;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			//Player is here, fire the Saw

		}
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Player") {
			//Player left, reset the trap
			Saw.transform.position = startPosition.transform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

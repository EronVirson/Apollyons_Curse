using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTrigger : MonoBehaviour {

    public bool isTouchingPlayer = false;
    public bool isTouchingCamera = false;
    

	// Use this for initialization
	void Start () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isTouchingPlayer = true;
        }
        if(other.gameObject.tag == "MainCamera")
        {
            isTouchingCamera = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            isTouchingPlayer = false;
        }
        if (other.gameObject.tag == "MainCamera")
        {
            isTouchingCamera = false;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}

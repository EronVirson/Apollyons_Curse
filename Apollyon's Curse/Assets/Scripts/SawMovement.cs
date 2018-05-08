using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour {


    
    public float MoveSpeed = 30f;
    public GameObject startPosition;
    public GameObject endPosition;

    private bool toStart = false;
    private bool toEnd = false;

	// Use this for initialization
	void Start () {

        transform.position = startPosition.transform.position;
        toEnd = true;
		
	}
	
	// Update is called once per frame
	void Update () {

        //transform.Rotate(Vector3.forward, SpinSpeed * Time.deltaTime);

        if (transform.position == endPosition.transform.position )
        {
            toEnd = false;
            toStart = true;
        }

        if (transform.position == startPosition.transform.position)
        {
            toStart = false;
            toEnd = true;
        }

        if (toEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition.transform.position, MoveSpeed * Time.deltaTime);
        }

        if (toStart)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition.transform.position, MoveSpeed * Time.deltaTime);
        }
	}
}

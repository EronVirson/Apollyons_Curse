using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrap : MonoBehaviour {

	public GameObject player;
	public GameObject startPosition;
	public GameObject endPosition;
	public GameObject Saw;
    public bool gotoEnd = true; //false indicates that it is moving toward startPosition

    public float speed = 10;
    public float reloadSpeed = 5;


    public enum TrapState
    {
        Idle,
        Active,
        Reload,
        Broken
    }
    public TrapState state;

    // Use this for initialization
    void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		Saw.transform.position = startPosition.transform.position;

        state = TrapState.Idle;
	}

	void OnTriggerEnter(Collider other){
        Debug.Log("SawBlade Triggered!");
        if (other.tag == "Player") {
            //Player is here, fire the Saw
            state = TrapState.Active;
		}
	}

	void OnTriggerExit(Collider other){
        Debug.Log("SawBlade Exit");
        if (other.tag == "Player") {
			//Player left, reset the trap
		    //Saw.transform.position = startPosition.transform.position;
            state = TrapState.Reload;
		}
	}
	
	// Update is called once per frame
	void Update () {
        //FSM
        switch (state)
        {
            case TrapState.Idle:
                Idle();
                break;
            case TrapState.Active:
                Active();
                break;
            case TrapState.Reload:
                Reload();
                break;
            case TrapState.Broken:
                //Broken Action
                break;
        }
    }

    void Idle()
    {
        //Wait
    }

    void Active()
    {
        if(gotoEnd == true)
        {
            if(Saw.transform.position == endPosition.transform.position)
            {
                gotoEnd = false; 
            }
            else
            {
                Saw.transform.position = Vector3.MoveTowards(Saw.transform.position, endPosition.transform.position, speed * Time.deltaTime);
            }
        }
        if(gotoEnd == false)
        {
            if (Saw.transform.position == startPosition.transform.position)
            {
                gotoEnd = true;
            }
            else
            {
                Saw.transform.position = Vector3.MoveTowards(Saw.transform.position, startPosition.transform.position, speed * Time.deltaTime);
            }
        }
    }

    void Reload()
    {
        if (Saw.transform.position == startPosition.transform.position)
        {
            gotoEnd = true;
            state = TrapState.Idle;
        }
        else
        {
            Saw.transform.position = Vector3.MoveTowards(Saw.transform.position, startPosition.transform.position, speed * Time.deltaTime);
        }
    }

    void Broken()
    {

    }
}

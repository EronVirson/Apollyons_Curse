using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smasher : MonoBehaviour {

    public Vector3 startPosition;
    public GameObject player;
    public float triggerDistance = 2.5f;
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
        startPosition = this.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        state = TrapState.Idle;
	}
	
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("WE HIT LAND!");
        if(state == TrapState.Active)
        {
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
        //Is the player nearby?
        if((Vector3.Distance(transform.position, player.transform.position) <= triggerDistance) && transform.position.y >= player.transform.position.y)
        {
            //Yes, trigger the trap
            state = TrapState.Active;
            this.tag = "HurtBox";
        }
    }

    void Active()
    {
        //Smash the player! Keep going until things go smoosh! (Check Collision)
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    void Reload()
    {
        if (transform.position == startPosition)
        {
            state = TrapState.Idle;
            this.tag = "Environment";
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, reloadSpeed * Time.deltaTime);
        }
    }
}

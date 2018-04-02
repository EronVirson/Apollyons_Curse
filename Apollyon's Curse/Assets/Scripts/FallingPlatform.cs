using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour {

    public Vector3 startPosition;
    public GameObject player;
    public float triggerDistance = 0.5f;
    public float speed = 10;
    public float reloadSpeed = 5;

    public float timer = 5.0f;

    // Use this for initialization
    void Start()
    {
        startPosition = this.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            timer = 5.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == startPosition)
        {
            //Nothing
        }
        else
        {
            if(timer > 0.0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, startPosition, reloadSpeed * Time.deltaTime);
            }
        }


    }
    
}

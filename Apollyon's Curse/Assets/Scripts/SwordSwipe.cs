using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwipe : MonoBehaviour {

    GameObject parent;
    public float speed = 1000;
    public float timer = 1.0f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        //this.transform.localPosition = Vector3.Slerp(this.transform.localPosition, new Vector3(1, 0, 0) + this.transform.localPosition, 0.01f);
        transform.RotateAround(transform.parent.position, new Vector3(0, 0, -1), Time.deltaTime * speed);

        //Despawn timer
        timer -= Time.deltaTime;
        if(timer <= 0.0f)
        {
            Destroy(this);
        }
	}
}

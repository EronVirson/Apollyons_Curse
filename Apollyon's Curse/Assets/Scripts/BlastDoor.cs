using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastDoor : MonoBehaviour {
    public GameObject self;

	// Use this for initialization
	void Start () {
		
	}

    void OnCollisionEnter(Collision other)
    {
        //Player Weapon
        if (other.collider.tag == "HitBox")
        {
            //Send message to parent, disable self
            //Wait for timer before parent re-enables
            transform.parent.SendMessage("BlastHit");
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    void ApplyDamage(int damage)
    {
        transform.parent.SendMessage("BlastHit");
    }
}

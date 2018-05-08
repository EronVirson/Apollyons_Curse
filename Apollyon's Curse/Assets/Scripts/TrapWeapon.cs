using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapWeapon : MonoBehaviour {
    public int damage = 5;

    // Use this for initialization
    void Start () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.SendMessage("ApplyDamage", damage);
        }

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.SendMessage("ApplyDamage", damage);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}

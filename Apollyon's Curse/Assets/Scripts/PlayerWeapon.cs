using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {

    public int damage = 5;

    // Use this for initialization
    void Start () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        other.gameObject.SendMessage("ApplyDamage", damage);   
    }

    // Update is called once per frame
    void Update () {
		
	}
}

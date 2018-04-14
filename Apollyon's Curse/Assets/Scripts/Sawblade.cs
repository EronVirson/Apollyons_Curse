using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawblade : MonoBehaviour {

    public int damage = 5;

    // Use this for initialization
    void Start () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("SawBlade::OnTriggerEnter");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Hurting the player");
            other.gameObject.SendMessage("ApplyDamage", damage);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}

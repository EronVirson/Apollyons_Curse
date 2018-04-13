using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAiming : MonoBehaviour {
    public GameObject player;
    public float timer = 2.5f;
    public GameObject muzzle;
    public float triggerDistance = 5.0f;
    public float reloadSpeed = 1.0f;
    public GameObject bullet;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        //Player nearby and below?
        if ((Vector3.Distance(transform.position, player.transform.position) <= triggerDistance) && transform.position.y >= player.transform.position.y)
        {
            //Shoot em!
            this.transform.LookAt(player.transform);
            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
				timer = reloadSpeed;
                GameObject bang = Instantiate(bullet);
                bang.transform.position = muzzle.transform.position;
                bang.transform.forward = this.transform.forward;
            }
        }
    }
}

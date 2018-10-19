using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour {

    public GameObject Player;
    public GameObject SpawnPoint;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Player.transform.position = SpawnPoint.transform.position;
        SceneManager.LoadScene("AC_RoomTest");
    }
}

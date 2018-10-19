using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTransporter : MonoBehaviour {
    
    public string destinationScene;
    public string currentScene;

    GenericTrigger TransitionTrigger;
    public GameObject Blast;
    public GameObject Trigger;
    public GameObject DoorMat;

    public float fBlastRespawn = 5.0f;
    public float fBlastRespawnTimer = 0.0f;

    public GameObject player;

	// Use this for initialization
	void Start () {
        TransitionTrigger = Trigger.GetComponent<GenericTrigger>();
        player = GameObject.FindGameObjectWithTag("Player");
        currentScene = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        BlastRespawnTimer();
        LookingForTransitions();
    }

    void BlastHit()
    {
        Blast.SetActive(false);
        fBlastRespawnTimer = fBlastRespawn;
    }

    void BlastRespawnTimer()
    {
        if(fBlastRespawnTimer > 0.0f)
        {
            fBlastRespawnTimer -= Time.deltaTime;
        }
        if(fBlastRespawnTimer <= 0.0f)
        {
            Blast.SetActive(true);
        }
    }

    void LookingForTransitions()
    {
        if(TransitionTrigger.isTouchingPlayer)
        {
            Debug.Log("===NOW ENTERING NEXT ZONE!===");
            player.SendMessage("ChangeScene", this);
            Trigger.SetActive(false);
        }
    }
}

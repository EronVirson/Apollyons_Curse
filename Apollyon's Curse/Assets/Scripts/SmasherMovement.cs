using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmasherMovement : MonoBehaviour {

    public GameObject startPosition;
    public GameObject endPosition;
    public AudioClip targetSound;
    public float Speed = 10f;

    private AudioSource audioSource;
    private bool toStart = false;
    private bool toEnd = false;
    private bool active = true;
    public float Delay = 0f;

    // Use this for initialization
    void Start () {

        transform.position = startPosition.transform.position;
        toEnd = true;
        active = true;
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {

        if (active)
        {
            if (transform.position == endPosition.transform.position)
            {
                active = false;
                toEnd = false;
                toStart = true;
                audioSource.PlayOneShot(targetSound);

                StartCoroutine(Wait());
            }

            if (transform.position == startPosition.transform.position)
            {
                active = false;
                toStart = false;
                toEnd = true;
                audioSource.PlayOneShot(targetSound);

                StartCoroutine(Wait());
            }

            if (toEnd)
            {
                transform.position = Vector3.MoveTowards(transform.position, endPosition.transform.position, Speed * Time.deltaTime);
            }

            if (toStart)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPosition.transform.position, Speed * Time.deltaTime);
            }
        }
    }

    void MoveObject()
    {
        if (toEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition.transform.position, Speed * Time.deltaTime);
        }

        if (toStart)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition.transform.position, Speed * Time.deltaTime);
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(Delay);
        active = true;
    }
}

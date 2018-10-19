using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour
{

    public GameObject player;       //Public variable to store a reference to the player game object
    public float minXPosition = 1000;
    public float minYPosition = 1000;
    public float maxXPosition = 1000;
    public float maxYPosition = 1000;


    private Vector3 offset;         //Private variable to store the offset distance between the player and camera

    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;

        DontDestroyOnLoad(this.gameObject);
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        // Clamp the camera bounds so that it stops at the edges of the screen
        float x = Mathf.Clamp(player.transform.position.x + offset.x, minXPosition, maxXPosition);
        float y = Mathf.Clamp(player.transform.position.y + offset.y, minYPosition, maxYPosition);
        float z = player.transform.position.z + offset.z;

        transform.position = new Vector3(x, y, z);
    }
}
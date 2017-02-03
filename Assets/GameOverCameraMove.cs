using UnityEngine;
using System.Collections;

public class GameOverCameraMove : MonoBehaviour {

    public GameObject player;
    public float moveSpeed;
    private Vector3 playerPosition;

	// Use this for initialization
	void Start () {
        playerPosition = player.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(playerPosition, Vector3.up, moveSpeed);
	
	}
}

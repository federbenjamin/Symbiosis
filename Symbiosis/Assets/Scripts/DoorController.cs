using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {

	public float nextRoomNum;

	private GameObject nextRoom;
	private GameObject playerCamera;
	private float cameraOffset = -7.2f;

	private Vector3 nextRoomPos;

	// Use this for initialization
	void Start () {
		nextRoom = GameObject.Find ("Room" + nextRoomNum);
		nextRoomPos = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, nextRoom.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			other.transform.position = new Vector3 (nextRoomPos.x, other.transform.position.y, nextRoomPos.z);
			playerCamera = GameObject.Find ("Camera" + other.name);
			playerCamera.transform.position = new Vector3 (nextRoomPos.x, playerCamera.transform.position.y, nextRoomPos.z + cameraOffset); 
		}
	}
}

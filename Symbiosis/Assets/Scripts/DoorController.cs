using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {

	public float nextRoomNum;

	private GameObject nextRoom;
	private GameObject playerCamera;
	private float cameraOffset = -3f;

	private Vector3 nextRoomPos;
	private GameObject player1;
	private GameObject player2;
	private GameObject playersCamera;

	private RoomController roomController;

	// Use this for initialization
	void Start () {
		nextRoom = GameObject.Find ("Room" + nextRoomNum);
		nextRoomPos = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, nextRoom.transform.position.z);
		roomController = transform.parent.GetComponent<RoomController> ();
		player1 = GameObject.Find ("P1");
		player2 = GameObject.Find ("P2");
		playersCamera = GameObject.Find ("CameraP1");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (roomController.roomCleared == true) {
			if (other.tag == "Player") {
				if (roomController.getPlayersTogether() == true) {
					player1.transform.position = new Vector3 (nextRoomPos.x - 1, player1.transform.position.y, nextRoomPos.z);
					player2.transform.position = new Vector3 (nextRoomPos.x + 1, player2.transform.position.y, nextRoomPos.z);
					playersCamera.transform.position = new Vector3 (nextRoomPos.x, playersCamera.transform.position.y, nextRoomPos.z + cameraOffset); 
				} else {
					other.transform.position = new Vector3 (nextRoomPos.x, other.transform.position.y, nextRoomPos.z);
					playerCamera = GameObject.Find ("Camera" + other.name);
					playerCamera.transform.position = new Vector3 (nextRoomPos.x, playerCamera.transform.position.y, nextRoomPos.z + cameraOffset); 
				}
			}
		}
	}
}

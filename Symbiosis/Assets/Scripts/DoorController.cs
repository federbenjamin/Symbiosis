using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {

	public float nextRoomNum;
	public char outDoor;
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
					Vector3 outPosition_1 = new Vector3 (nextRoomPos.x - 1, player1.transform.position.y, nextRoomPos.z);
					outPosition_1 = outPosition_1 + OutPlacer (outDoor) + Offsetter(outDoor, true);
					player1.transform.position = outPosition_1;

					Vector3 outPosition_2 = new Vector3 (nextRoomPos.x - 1, player2.transform.position.y, nextRoomPos.z);
					outPosition_2 = outPosition_2 + OutPlacer (outDoor) + Offsetter(outDoor, false);
					player2.transform.position = outPosition_2;

					playersCamera.transform.position = new Vector3 (nextRoomPos.x, playersCamera.transform.position.y, nextRoomPos.z + cameraOffset); 
				} else {
					Vector3 outPosition = new Vector3 (nextRoomPos.x, other.transform.position.y, nextRoomPos.z);
					outPosition = outPosition + OutPlacer (outDoor);
					other.transform.position = outPosition;
					playerCamera = GameObject.Find ("Camera" + other.name);
					playerCamera.transform.position = new Vector3 (nextRoomPos.x, playerCamera.transform.position.y, nextRoomPos.z + cameraOffset); 
				}
			}
		}
	}

	Vector3 OutPlacer(char dirChar){
		Vector3 retval;
		if (dirChar == 'n'){
			retval = new Vector3 ( 0, 0, 3);
		}else if (dirChar == 's'){
			retval = new Vector3 ( 0, 0,-3);
		}else if (dirChar == 'e'){
			retval = new Vector3 ( 7, 0,0);
		}else{
			retval = new Vector3 (-7, 0,0);
		}
		return retval;
	}
	Vector3 Offsetter(char dirChar, bool isPlayer_1){
		Vector3 retval;
		float x = 0.5f;
		if (isPlayer_1){
			x = x*-1;
		}
		if (dirChar == 'n' || dirChar == 's'){
			retval = new Vector3 ( x, 0, 0);
		}else {

			retval = new Vector3 ( 0, 0, 2*x);
		}
		return retval;
	}
}

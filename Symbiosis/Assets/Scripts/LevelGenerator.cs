using UnityEngine;
using System.Collections;
using System;

public class LevelGenerator : MonoBehaviour {

	public static LevelGenerator Instance;

	private int seed;
	private bool useRandomSeed = true;
	private int size = 2;
	private Transform roomParent;

	void Awake () {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		roomParent = GameObject.Find("Rooms").transform;

		if (useRandomSeed) {
			seed = (int)System.DateTime.Now.Ticks;//Time.time.ToString();
		}

		Debug.Log(seed);
		System.Random pseudoRandom = new System.Random(seed);

		string[] players = new string[] {"P1", "P2"};
		foreach (string player in players) {
			int offset = 0;
			if (player == "P2") {
				offset = (size + 1) * 40;
			}
			GenerateInitalRooms(player, offset);
		}

		// Generate gauntlet
		Vector3 roomPosition = new Vector3(size * 40, 0, (size - 1) * 32);
		GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Gauntlet"), roomPosition, Quaternion.identity) as GameObject;
		newObj.name = "RoomGauntlet";
		newObj.transform.SetParent(roomParent);

		GameObject.Find("DoorSwitchEnterRight").GetComponent<DoorController>().nextRoomNum = "P2-" + (size * (size - 1));
		GameObject.Find("DoorSwitchEnterLeft").GetComponent<DoorController>().nextRoomNum = "P1-" + ((size * size) - 1);

		LoadRemainingAssets();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GenerateInitalRooms(string player, int offset) {
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				int roomNumber = (i * size) + j;
				int xOffset = j * 40 + offset;
				int zOffset = i * 32;
				Vector3 roomPosition = new Vector3(xOffset, 0, zOffset);
				GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/RoomBlank"), roomPosition, Quaternion.identity) as GameObject;
				newObj.name = "Room" + player + "-" + roomNumber;
				newObj.transform.SetParent(roomParent);

				GenerateDoors(newObj, player, roomNumber, xOffset, zOffset);
			}
		}
	}

	void GenerateDoors(GameObject room, string player, int roomNum, int offsetX, int offsetZ) {
		Transform doorParent = room.transform;

		bool p1SwitchDoor = false;
		bool p2SwitchDoor = false;
		if (player == "P1" && (roomNum == (size * size - 1))) {
			p1SwitchDoor = true;
		} else if (player == "P2" && (roomNum == size * (size - 1))) {
			p2SwitchDoor = true;
		}

		GameObject newObj;
		Vector3 objectPos;
		Quaternion objectRot;
		DoorController doorControl;
		// Left/West
		objectPos = new Vector3(-8f + offsetX, 0, 0 + offsetZ);
		if ((roomNum % size == 0) && !p2SwitchDoor) {
			objectRot = Quaternion.Euler(-90, 90, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/WallBlank"), objectPos, objectRot) as GameObject;
			newObj.name = "WallWest";
		} else {
			objectRot = Quaternion.Euler(0, 90, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/DoorBlank"), objectPos, objectRot) as GameObject;
			newObj.name = "DoorWest";
			doorControl = newObj.GetComponent<DoorController>();
			doorControl.outDoor = 'e';
			if (!p2SwitchDoor) {
				doorControl.nextRoomNum = player + "-" + (roomNum - 1);
			} else {
				doorControl.nextRoomNum = "100";
			}
		}
		newObj.transform.SetParent(doorParent);

		// Up/North
		objectPos = new Vector3(0 + offsetX, 0, 4f + offsetZ);
		if (roomNum >= (size * (size - 1))) {
			objectRot = Quaternion.Euler(-90, -180, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/WallBlank"), objectPos, objectRot) as GameObject;
			newObj.name = "WallNorth";
		} else {
			objectRot = Quaternion.Euler(0, -180, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/DoorBlank"), objectPos, objectRot) as GameObject;
			newObj.name = "DoorNorth";
			doorControl = newObj.GetComponent<DoorController>();
			doorControl.outDoor = 's';
			doorControl.nextRoomNum = player + "-" + (roomNum + size);
		}
		newObj.transform.SetParent(doorParent);

		// Right/East
		objectPos = new Vector3(8f + offsetX, 0, 0 + offsetZ);
		if ((roomNum % size == (size - 1)) && !p1SwitchDoor) {
			objectRot = Quaternion.Euler(-90, -90, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/WallBlank"), objectPos, objectRot) as GameObject;
			newObj.name = "WallEast";
		} else {
			objectRot = Quaternion.Euler(0, -90, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/DoorBlank"), objectPos, objectRot) as GameObject;
			newObj.name = "DoorEast";
			doorControl = newObj.GetComponent<DoorController>();
			doorControl.outDoor = 'w';
			if (!p1SwitchDoor) {
				doorControl.nextRoomNum = player + "-" + (roomNum + 1);
			} else {
				doorControl.nextRoomNum = "100";
			}
		}
		newObj.transform.SetParent(doorParent);

		// Down/South
		objectPos = new Vector3(0 + offsetX, 0, -4f + offsetZ);
		if (roomNum < size) {
			objectRot = Quaternion.Euler(-90, 0, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/WallBlank"), objectPos, objectRot) as GameObject;
			newObj.name = "WallSouth";
		} else {
			objectRot = Quaternion.Euler(0, 0, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/DoorBlank"), objectPos, objectRot) as GameObject;
			newObj.name = "DoorSouth";
			doorControl = newObj.GetComponent<DoorController>();
			doorControl.outDoor = 'n';
			doorControl.nextRoomNum = player + "-" + (roomNum - size);
		}
		newObj.transform.SetParent(doorParent);
	}

	void LoadRemainingAssets() {
		Transform rootTransform = GameObject.Find("Root").transform;
		string[] objectNameList = new string[] {"Divider", "DividerHealth", "Canvas", "P1", "P2", "CameraParentP1", "CameraParentP2", "AudioListener"};
		foreach (string objectName in objectNameList) {
			GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/" + objectName) as GameObject);
			newObj.name = objectName;
			newObj.transform.SetParent(rootTransform);
		}
	}
}

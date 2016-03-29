using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class LevelGenerator : MonoBehaviour {

	private int seed;
	private int size;
	private System.Random pseudoRandom;
	private Transform roomParent;

	private int numRedRoomPrefabs = 1;
	private int numGreenRoomPrefabs = 1;
	private int numBlueRoomPrefabs = 1;

	// 0=bottom-left, 1=bottom-right, 2=top-left, 3=top-right
	private int p1Quadrant = 0;
	private int p2Quadrant = 0;

	private string switchRoomNumber = "100";

	void Awake () {
		roomParent = GameObject.Find("Rooms").transform;
	}

	// Use this for initialization
	void Start () {

		// Get level inputs - size and seed
		if (LevelData.randomLevel) {
			LevelData.GenerateRandomSeed();
		}
		size = LevelData.levelSize;
		seed = LevelData.levelSeed;
		Debug.Log(seed);
		pseudoRandom = new System.Random(seed);

		// Randomize level positioning
		int rightSidePlayer = pseudoRandom.Next(2);
		int topHalfPlayer = pseudoRandom.Next(2);

		// Generate gauntlet
		Vector3 roomPosition = new Vector3((size - 1) * 40, 0, (size - 1) * 32);
		GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Gauntlet"), roomPosition, Quaternion.identity) as GameObject;
		newObj.name = "RoomGauntlet";
		newObj.transform.SetParent(roomParent);

		string[] players = new string[] {"P1", "P2"};
		foreach (string player in players) {
			int xOffset = 0;
			if (player == "P1" && rightSidePlayer == 0) {
				xOffset = (size - 1) * 40;
				p1Quadrant++;
			}
			else if (player == "P2" && rightSidePlayer == 1) {
				xOffset = (size - 1) * 40;
				p2Quadrant++;
			}

			int zOffset = 0;
			if (player == "P1" && topHalfPlayer == 0) {
				zOffset = (size - 1) * 32;
				p1Quadrant = p1Quadrant + 2;
			}
			else if (player == "P2" && topHalfPlayer == 1) {
				zOffset = (size - 1) * 32;
				p2Quadrant = p2Quadrant + 2;
			}
			GenerateInitalRooms(player, xOffset, zOffset);
		}

		LoadRemainingAssets();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GenerateInitalRooms(string player, int levelXOffset, int levelZOffset) {
		int quadrant;
		if (player == "P1") {
			quadrant = p1Quadrant;
		} else {
			quadrant = p2Quadrant;
		}

		int switchRoomAdjacentNum;
		int skipRoom;
		int switchRoomBlocking;
		if (quadrant == 0) {
			switchRoomAdjacentNum = (size * size) - 2;
			skipRoom = (size * size - 1);
			switchRoomBlocking = size * (size - 1) - 1;
		} else if (quadrant == 1) {
			switchRoomAdjacentNum = size * (size - 1) + 1;
			skipRoom = size * (size - 1);
			switchRoomBlocking = size * (size - 2);
		} else if (quadrant == 2) {
			switchRoomAdjacentNum = size - 2;
			skipRoom = size - 1;
			switchRoomBlocking = (size * 2) - 1;
		} else {
			switchRoomAdjacentNum = 1;
			skipRoom = 0;
			switchRoomBlocking = size;
		}

		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				int roomNumber = (i * size) + j;

				if (roomNumber != skipRoom) {
					int xOffset = j * 40 + levelXOffset;
					int zOffset = i * 32 + levelZOffset;
					Vector3 roomPosition = new Vector3(xOffset, 0, zOffset);

					string roomSuffix = RandomRoomPrefab(roomNumber);
					string roomColor = Regex.Match(roomSuffix, @"\D+").Groups[0].Value;
					GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Room" + roomSuffix), roomPosition, Quaternion.identity) as GameObject;
					newObj.name = "Room" + player + "-" + roomNumber;
					newObj.transform.SetParent(roomParent);
					newObj.GetComponent<RoomController>().RoomColor = roomColor;

					bool switchDoor = false;
					bool switchWall = false;
					if (roomNumber == switchRoomAdjacentNum) {
						switchDoor = true;
					} else if (roomNumber == switchRoomBlocking) {
						switchWall = true;
					}
					GenerateDoorOrWall(newObj, player, roomNumber, roomColor, xOffset, zOffset, quadrant, switchDoor, switchWall);
				}
			}
		}
	}

	string RandomRoomPrefab(int roomNum) {
		int colorNum = pseudoRandom.Next(3);

		// Red
		if (colorNum == 0) {
			int prefabNum = pseudoRandom.Next(numRedRoomPrefabs);
			return "Red" + prefabNum;
		}
		//Green
		else if (colorNum == 1) {
			int prefabNum = pseudoRandom.Next(numGreenRoomPrefabs);
			return "Green" + prefabNum;
		}
		//Blue
		else {
			int prefabNum = pseudoRandom.Next(numBlueRoomPrefabs);
			return "Blue" + prefabNum;
		}
	}

	void GenerateDoorOrWall(
			GameObject room, 
			string player, 
			int roomNum, 
			string roomColor, 
			int offsetX, 
			int offsetZ, 
			int quadrant, 
			bool switchDoorAdjacent,
			bool switchWallAdjacent
		) {

		Transform doorParent = room.transform;
		GameObject newObj;
		Vector3 objectPos;
		Quaternion objectRot;
		DoorController doorControl;

		// Left/West
		objectPos = new Vector3(-8f + offsetX, 0, offsetZ);
		if (roomNum % size == 0 && !switchDoorAdjacent) {
			objectRot = Quaternion.Euler(-90, 90, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Wall" + roomColor), objectPos, objectRot) as GameObject;
			newObj.name = "WallWest";
		} else {
			objectRot = Quaternion.Euler(0, 90, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Door" + roomColor), objectPos, objectRot) as GameObject;
			newObj.name = "DoorWest";
			doorControl = newObj.GetComponent<DoorController>();
			doorControl.outDoor = 'e';
			
			if (switchDoorAdjacent && (quadrant == 1 || quadrant == 3)) {
				doorControl.nextRoomNum = switchRoomNumber;
				GameObject.Find("DoorSwitchEnterRight").GetComponent<DoorController>().nextRoomNum = player + "-" + roomNum;
			} else {
				doorControl.nextRoomNum = player + "-" + (roomNum - 1);
			}
		}
		newObj.transform.SetParent(doorParent);

		// Right/East
		objectPos = new Vector3(8f + offsetX, 0, offsetZ);
		if ((roomNum % size == (size - 1)) && !switchDoorAdjacent) {
			objectRot = Quaternion.Euler(-90, -90, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Wall" + roomColor), objectPos, objectRot) as GameObject;
			newObj.name = "WallEast";
		} else {
			objectRot = Quaternion.Euler(0, -90, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Door" + roomColor), objectPos, objectRot) as GameObject;
			newObj.name = "DoorEast";
			doorControl = newObj.GetComponent<DoorController>();
			doorControl.outDoor = 'w';

			if (switchDoorAdjacent && (quadrant == 0 || quadrant == 2)) {
				doorControl.nextRoomNum = switchRoomNumber;
				GameObject.Find("DoorSwitchEnterLeft").GetComponent<DoorController>().nextRoomNum = player + "-" + roomNum;
			} else {
				doorControl.nextRoomNum = player + "-" + (roomNum + 1);
			}
		}
		newObj.transform.SetParent(doorParent);

		bool isSwitchWall;
		// Up/North
		isSwitchWall = switchWallAdjacent && quadrant <= 1;
		objectPos = new Vector3(0 + offsetX, 0, 4f + offsetZ);
		bool topWall = roomNum >= (size * (size - 1));
		if (topWall || isSwitchWall) {
			objectRot = Quaternion.Euler(-90, -180, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Wall" + roomColor), objectPos, objectRot) as GameObject;
			newObj.name = "WallNorth";
		} else {
			objectRot = Quaternion.Euler(0, -180, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Door" + roomColor), objectPos, objectRot) as GameObject;
			newObj.name = "DoorNorth";
			doorControl = newObj.GetComponent<DoorController>();
			doorControl.outDoor = 's';
			doorControl.nextRoomNum = player + "-" + (roomNum + size);
		}
		newObj.transform.SetParent(doorParent);

		// Down/South
		isSwitchWall = switchWallAdjacent && quadrant >= 2;
		bool bottomWall = roomNum < size;
		objectPos = new Vector3(0 + offsetX, 0, -4f + offsetZ);
		if (bottomWall || isSwitchWall) {
			objectRot = Quaternion.Euler(-90, 0, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Wall" + roomColor), objectPos, objectRot) as GameObject;
			newObj.name = "WallSouth";
		} else {
			objectRot = Quaternion.Euler(0, 0, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Door" + roomColor), objectPos, objectRot) as GameObject;
			newObj.name = "DoorSouth";
			doorControl = newObj.GetComponent<DoorController>();
			doorControl.outDoor = 'n';
			doorControl.nextRoomNum = player + "-" + (roomNum - size);
		}
		newObj.transform.SetParent(doorParent);
	}

	void ReplaceDoorWithWall(string player, string roomNum, string direction) {
		GameObject room = GameObject.Find("Room" + player + "-" + roomNum);
		foreach (Transform child in room.transform) {
			if (child.name == ("Door" + direction)) {
				Vector3 wallPos = new Vector3(child.position.x, 0, child.position.z);
				Quaternion wallRot = Quaternion.Euler(-90, child.rotation.eulerAngles.y, 0);
				string roomColor = room.GetComponent<RoomController>().RoomColor;
				GameObject newWall = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Wall" + roomColor), wallPos, wallRot) as GameObject;
				newWall.name = ("Wall" + direction);
				Destroy(child.gameObject);
				break;
			}
		}
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

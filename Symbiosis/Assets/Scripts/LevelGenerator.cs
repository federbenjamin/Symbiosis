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
			int quadrant, xOffset, zOffset;
			quadrant = xOffset = zOffset = 0;

			bool shiftRight = (player == "P1" && rightSidePlayer == 0) || (player == "P2" && rightSidePlayer == 1);
			if (shiftRight) {
				xOffset = (size - 1) * 40;
				quadrant++;
			}
			bool shiftUp = (player == "P1" && topHalfPlayer == 0) || (player == "P2" && topHalfPlayer == 1);
			if (shiftUp) {
				zOffset = (size - 1) * 32;
				quadrant = quadrant + 2;
			}

			GenerateInitalRooms(player, quadrant, xOffset, zOffset);
		}

		LoadRemainingAssets();
	}

	private void GenerateInitalRooms(string player, int quadrant, int levelXOffset, int levelZOffset) {
		int[] quadrantRooms;
		quadrantRooms = GetRoomExceptionsByQuadrant(quadrant);

		GenerateTutorialRoom(player, quadrantRooms);

		int skipRoom = quadrantRooms[1];
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
					
					Vector3 roomOffset = new Vector3(xOffset, 0, zOffset);
					GenerateDoorsAndWalls(newObj.transform, player, roomNumber, roomColor, roomOffset, quadrantRooms);
				}
			}
		}
	}

	private void GenerateTutorialRoom(string player, int[] quadrantRooms) {
		int quadrant = quadrantRooms[0];
		int tutorialRoomAdjacent = quadrantRooms[4];

	}

	/*
		Return a list of 4 integers.
		Index 0: Quadrant to return unique room numbers for
		Index 1: Room number to not generate (switch room takes its location, central room)
		Index 2: Room number with a door exiting into the switch room
		Index 3: Room number with a wall north/south of the switch room
		Index 4: Room number the tutorial room exits into
	*/
	private int[] GetRoomExceptionsByQuadrant(int quadrant) {
		int switchRoomAdjacentNum, skipRoom, switchRoomBlocking, tutorialRoomAdjacent;
		if (quadrant == 0) {
			switchRoomAdjacentNum = (size * size) - 2;
			skipRoom = (size * size - 1);
			switchRoomBlocking = size * (size - 1) - 1;
			tutorialRoomAdjacent = 0;
		} else if (quadrant == 1) {
			switchRoomAdjacentNum = size * (size - 1) + 1;
			skipRoom = size * (size - 1);
			switchRoomBlocking = size * (size - 2);
			tutorialRoomAdjacent = size - 1;
		} else if (quadrant == 2) {
			switchRoomAdjacentNum = size - 2;
			skipRoom = size - 1;
			switchRoomBlocking = (size * 2) - 1;
			tutorialRoomAdjacent = size * (size - 1);
		} else {
			switchRoomAdjacentNum = 1;
			skipRoom = 0;
			switchRoomBlocking = size;
			tutorialRoomAdjacent = (size * size) - 1;
		}

		return new int[] {quadrant, skipRoom, switchRoomAdjacentNum, switchRoomBlocking, tutorialRoomAdjacent};
	}

	private string RandomRoomPrefab(int roomNum) {
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

	private void GenerateDoorsAndWalls(Transform doorParent, string player, int roomNum, string roomColor, Vector3 offset, int[] quadrantRooms) {

		int quadrant = quadrantRooms[0];
		int switchRoomNumber = quadrantRooms[2];
		bool switchDoor, switchWall, tutorialDoor;
		switchDoor = switchWall = tutorialDoor = false;
		if (roomNum == switchRoomNumber) {
			switchDoor = true;
		} else if (roomNum == quadrantRooms[3]) {
			switchWall = true;
		} else if (roomNum == quadrantRooms[4]) {
			tutorialDoor = true;
		}

		bool spawnWall;
		GameObject newObj;
		string[] directions = new string[] {"West", "East", "North", "South"};
		foreach (string direction in directions) {
			if (direction == "East") {
				spawnWall = (roomNum % size == (size - 1)) && !switchDoor && !tutorialDoor;
			} else if (direction == "West") {
				spawnWall = (roomNum % size == 0) && !switchDoor && !tutorialDoor;
			} else if (direction == "North") {
				spawnWall = (roomNum >= (size * size - 1)) || (switchWall && quadrant <= 1);
			} else {
				spawnWall = (roomNum < size) || (switchWall && quadrant >= 2);
			}

			newObj = GenerateDoorOrWall(direction, player, roomNum, roomColor, offset, spawnWall);
			newObj.transform.SetParent(doorParent);

			if (direction == "East") {
				if (switchDoor && (quadrant == 0 || quadrant == 2)) {
					newObj.GetComponent<DoorController>().nextRoomNum = switchRoomNumber.ToString();
					GameObject.Find("DoorSwitchEnterLeft").GetComponent<DoorController>().nextRoomNum = player + "-" + roomNum;
				} else if (tutorialDoor && (quadrant == 1 || quadrant == 3)) {
					newObj.GetComponent<DoorController>().nextRoomNum = player + "Tutorial";
				} 
			} else if (direction == "West") {
				if (switchDoor && (quadrant == 1 || quadrant == 3)) {
					newObj.GetComponent<DoorController>().nextRoomNum = switchRoomNumber.ToString();
					GameObject.Find("DoorSwitchEnterRight").GetComponent<DoorController>().nextRoomNum = player + "-" + roomNum;
				} else if (tutorialDoor && (quadrant == 0 || quadrant == 2)) {
					newObj.GetComponent<DoorController>().nextRoomNum = player + "Tutorial";
				} 
			}
		}
	}

	private GameObject GenerateDoorOrWall(string direction, string player, int roomNum, string roomColor, Vector3 posOffset, bool spawnWall) {

		GameObject newObj;
		Vector3 objectPos;
		Quaternion objectRot;
		DoorController doorControl;

		const string switchRoomNumber = "100";
		Vector3 dirOffset;
		int yRotation;
		char newOutDoor;
		int nextRoomNum;

		if (direction == "East") {
			newOutDoor = 'w';
			yRotation = -90;
			dirOffset = new Vector3(8f, 0, 0);
			nextRoomNum = roomNum + 1;
		} 
		else if (direction == "West") {
			newOutDoor = 'e';
			yRotation = 90;
			dirOffset = new Vector3(-8f, 0, 0);
			nextRoomNum = roomNum - 1;
		} 
		else if (direction == "North") {
			newOutDoor = 's';
			yRotation = -180;
			dirOffset = new Vector3(0, 0, 4f);
			nextRoomNum = roomNum + size;
		} 
		else {
			newOutDoor = 'n';
			yRotation = 0;
			dirOffset = new Vector3(0, 0, -4f);
			nextRoomNum = roomNum - size;
		}

		// Generate Door or Wall object
		objectPos = dirOffset + posOffset;
		if (spawnWall) {
			// Generate Wall
			objectRot = Quaternion.Euler(-90, yRotation, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Wall" + roomColor), objectPos, objectRot) as GameObject;
			newObj.name = "Wall" + direction;
		} else {
			// Generate Door
			objectRot = Quaternion.Euler(0, yRotation, 0);
			newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Door" + roomColor), objectPos, objectRot) as GameObject;
			newObj.name = "Door" + direction;
			doorControl = newObj.GetComponent<DoorController>();
			doorControl.outDoor = newOutDoor;
			doorControl.nextRoomNum = player + "-" + nextRoomNum;
		}

		return newObj;
	}

	private void ReplaceDoorWithWall(string player, string roomNum, string direction) {
		GameObject room = GameObject.Find("Room" + player + "-" + roomNum);
		foreach (Transform child in room.transform) {
			// Find the door to replace in the room object
			if (child.name == ("Door" + direction)) {

				// Save the door's position/rotation/color and generate a wall in its place
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

	private void ReplaceWallWithDoor(string player, string roomNum, string direction, string connectingRoom) {
		char newOutDoor;
		if (direction == "North") {
			newOutDoor = 's';
		} else if (direction == "South") {
			newOutDoor = 'n';
		} else if (direction == "East") {
			newOutDoor = 'w';
		} else {
			newOutDoor = 'e';
		}

		GameObject room = GameObject.Find("Room" + player + "-" + roomNum);
		foreach (Transform child in room.transform) {
			// Find the door to replace in the room object
			if (child.name == ("Wall" + direction)) {

				// Save the door's position/rotation/color and generate a wall in its place
				Vector3 roomPos = new Vector3(child.position.x, 0, child.position.z);
				Quaternion roomRot = Quaternion.Euler(0, child.rotation.eulerAngles.y, 0);
				string roomColor = room.GetComponent<RoomController>().RoomColor;
				GameObject newDoor = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Door" + roomColor), roomPos, roomRot) as GameObject;
				newDoor.name = ("Door" + direction);
				DoorController doorController = newDoor.GetComponent<DoorController>();
				doorController.outDoor = newOutDoor;
				doorController.nextRoomNum = connectingRoom;
				Destroy(child.gameObject);
				break;

			}
		}
	}

	private void LoadRemainingAssets() {
		Transform rootTransform = GameObject.Find("Root").transform;
		string[] objectNameList = new string[] {"Divider", /*"DividerHealth",*/ "Canvas", "P1", "P2", "CameraParentP1", "CameraParentP2", "AudioListener"};
		foreach (string objectName in objectNameList) {
			GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/" + objectName) as GameObject);
			newObj.name = objectName;
			newObj.transform.SetParent(rootTransform);
		}
	}
}

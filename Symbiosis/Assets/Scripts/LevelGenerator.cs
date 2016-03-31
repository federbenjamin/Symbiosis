using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class LevelGenerator : MonoBehaviour {

	private int seed;
	private int size;
	private int maxDifficulty;
	private System.Random pseudoRandom;

	private string[] directions = new string[] {"West", "East", "North", "South"};
	private string[] colors = new string[] {"Red", "Green", "Blue"};

	private Transform roomParent;
	private Vector3 gauntletPosition;
	private float tutorialRoomDistance;

	private List<string> remainingRoomColors = new List<string>();
	// List order: Red, Green, Blue
	private int[] numRoomTypePrefabs = new int[] {5, 5, 5};
	private Dictionary<string, List<int>> remainingRoomPrefabs = new Dictionary<string, List<int>>();
	private Dictionary<string, int[]> enemyTypeDifficulty = new Dictionary<string, int[]>();
	private int tutorialCorridorLength = 4;
	private int roomDifficultyRange = 2;


	void Awake () {
		roomParent = GameObject.Find("Rooms").transform;
	}

	// Use this for initialization
	void Start () {

		// Get level inputs - difficulty, size, and seed
		maxDifficulty = LevelData.levelDifficulty;
		size = LevelData.levelSize;
		if (LevelData.randomLevel) {
			LevelData.GenerateRandomSeed();
		}
		seed = LevelData.levelSeed;
		Debug.Log("Using Seed: " + seed);
		pseudoRandom = new System.Random(seed);

		// Instantiate Room Prefabs and Room Colors Remaning Lists
		//FillEmptyRoomColorList();
		foreach (string color in colors) {
			ResetPrefabsRemainingByColor(color);
		}

		// Add the number and difficulty of each enemy color
		enemyTypeDifficulty.Add("Red", new int[] {1, 2});
		enemyTypeDifficulty.Add("Green", new int[] {1, 2});
		enemyTypeDifficulty.Add("Blue", new int[] {1, 2, 3});

		// Randomize level positioning
		int rightSidePlayer = pseudoRandom.Next(2);
		int topHalfPlayer = pseudoRandom.Next(2);

		// Generate gauntlet
		gauntletPosition = new Vector3((size - 1) * 40, 0, (size - 1) * 32);
		GenerateRoom("Gauntlet", "RoomGauntlet", gauntletPosition);

		string[] players = new string[] {"P1", "P2"};
		foreach (string player in players) {
			// Quadrant: 0=bottom-left, 1=bottom-right, 2=top-left, 3=top-right
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

		GenerateTutorialRooms(player, quadrantRooms);

		int skipRoom = quadrantRooms[1];
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {

				int roomNumber = (i * size) + j;
				if (roomNumber != skipRoom) {

					int xOffset = j * 40 + levelXOffset;
					int zOffset = i * 32 + levelZOffset;
					Vector3 roomPosition = new Vector3(xOffset, 0, zOffset);

					string roomSuffix = RandomRoomPrefab();
					string roomColor = Regex.Match(roomSuffix, @"\D+").Groups[0].Value;
					string name = "Room" + player + "-" + roomNumber;
					GameObject newRoom = GenerateRoom(roomSuffix, name, roomPosition, roomColor);
					
					GenerateDoorsAndWalls(newRoom.transform, player, roomNumber, roomColor, roomPosition, quadrantRooms);
					SpawnEnemies(newRoom.transform, roomColor);
				}
			}
		}
	}

	private void SpawnEnemies(Transform newRoom, string roomColor) {
		int roomDifficulty = GetNormalizedRoomDifficulty(newRoom.position);

		// Generate a random subset of possible enemy types
		List<Transform> spawnerTransforms = new List<Transform>();
		foreach (Transform child in newRoom) {
			if (child.tag == "EnemySpawnerBlank") {
				spawnerTransforms.Add(child);
			}
		}
		int enemySpawnCount = pseudoRandom.Next(spawnerTransforms.Count - 1) + 1;
		int[] enemiesToSpawn = GenerateEnemySet(roomDifficulty, roomColor, enemySpawnCount);

		// Spawn choosen enemy types in random locations in the room
		for (int i = 0; i < enemiesToSpawn.Length; i++) {
			int spawnObjectIndex = pseudoRandom.Next(spawnerTransforms.Count);
			Transform spawnObject = spawnerTransforms[spawnObjectIndex];
			spawnerTransforms.RemoveAt(spawnObjectIndex);
			ReplaceSpawnWithObject(spawnObject, roomColor, newRoom, enemiesToSpawn[i]);
		}
		// Clean up the rest of the spawn objects
		foreach (Transform remainingSpawns in spawnerTransforms) {
			ReplaceSpawnWithObject(remainingSpawns, newRoom);
			Destroy(remainingSpawns.gameObject);
		}
	}

	private int[] GenerateEnemySet(int roomDifficultyUpper, string roomColor, int numOfSpawnLocations) {
		// Replace with enemy subset generation algorithm
		int roomDifficultyLower = roomDifficultyUpper - roomDifficultyRange;
		int[] enemyColorTypes = enemyTypeDifficulty[roomColor];

		// Initialize enemy set to random enemies
		int[] enemySet = new int[numOfSpawnLocations];
		for (int i = 0; i < numOfSpawnLocations; i++) {
			int randomEnemyIndex = pseudoRandom.Next(enemyColorTypes.Length);
			enemySet[i] = enemyColorTypes[randomEnemyIndex];
		}
		int enemyDifficultyTotal = SumArray(enemySet);

		return enemySet;
	}

	private int SumArray(int[] array) {
		int sum = 0;
		for (int i = 0; i < array.Length; i++) {
			sum += array[i];
		}
		return sum;
	}

	private void ReplaceSpawnWithObject(Transform spawn, Transform newRoom) {
		string objectSpawnFile = "";
		int objectToSpawn = pseudoRandom.Next(100);

		// Spawn health pick-ups
		if (objectToSpawn >= 98) {
			objectSpawnFile = "FullHeart";
		}
		else if (objectToSpawn >= 94) {
			objectSpawnFile = "HalfHeart";
		}

		if (objectSpawnFile != "") {
			GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/" + objectSpawnFile)) as GameObject;
			newObj.transform.SetParent(newRoom);
			newObj.transform.position = spawn.position;
		}
		Destroy(spawn.gameObject);
	}

	private void ReplaceSpawnWithObject(Transform spawn, string roomColor, Transform newRoom, int enemyNumber) {
		string objectSpawnFile;
		int objectToSpawn = pseudoRandom.Next(100);

		// Spawn enemy as intended
		if (objectToSpawn <= 95) {
			objectSpawnFile = "EnemySpawns/SpawnEnemy" + roomColor + enemyNumber;
		}
		// Spawn health pick-ups
		else if (objectToSpawn >= 99) {
			objectSpawnFile = "FullHeart";
		}
		else {
			objectSpawnFile = "HalfHeart";
		}

		GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/" + objectSpawnFile)) as GameObject;
		newObj.transform.SetParent(newRoom);
		newObj.transform.position = spawn.position;
		Destroy(spawn.gameObject);
	}

	private int GetNormalizedRoomDifficulty(Vector3 roomPosition) {
		float roomDistance = Vector3.Distance(roomPosition, gauntletPosition);
		float unNormalizedDifficulty = (1 - roomDistance / tutorialRoomDistance) * maxDifficulty;
		return Mathf.CeilToInt(unNormalizedDifficulty);
	}

	private GameObject GenerateRoom(string roomSuffix, string roomName, Vector3 roomPosition) {
		GameObject newRoom = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Rooms/Room" + roomSuffix), roomPosition, Quaternion.identity) as GameObject;
		newRoom.name = roomName;
		newRoom.transform.SetParent(roomParent);
		return newRoom;
	}

	private GameObject GenerateRoom(string roomSuffix, string roomName, Vector3 roomPosition, string roomColor) {
		GameObject newRoom = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Rooms/Room" + roomSuffix), roomPosition, Quaternion.identity) as GameObject;
		newRoom.name = roomName;
		newRoom.transform.SetParent(roomParent);
		newRoom.GetComponent<RoomController>().RoomColor = roomColor;
		return newRoom;
	}

	private void GenerateTutorialRooms(string player, int[] quadrantRooms) {
		int quadrant = quadrantRooms[0];

		int roomNum, offsetX, offsetZ;
		Vector3 offset;
		if (quadrant == 0) {
			roomNum = -1;
			offsetX = -1;
			offsetZ = 0;
		} else if (quadrant == 1) {
			roomNum = size;
			offsetX = size * 2 - 1;
			offsetZ = 0;
		} else if (quadrant == 2) {
			roomNum = size * (size - 1) - 1;
			offsetX = -1;
			offsetZ = size * 2 - 2;
		} else {
			roomNum = size * size;
			offsetX = size * 2 - 1;
			offsetZ = size * 2 - 2;
		}
		offset = new Vector3(offsetX * 40, 0, (offsetZ - (tutorialCorridorLength - 1)) * 32);

		string roomName = "Room" + player + "Tutorial";
		GenerateRoom(player + "Tutorial", roomName, offset);
		Transform finalTutorialRoom = GameObject.Find(roomName + "Exit").transform;

		offset = new Vector3(offsetX * 40, 0, offsetZ * 32);
		tutorialRoomDistance = Vector3.Distance(offset, gauntletPosition);

		bool spawnWall;
		GameObject newObj;
		foreach (string direction in directions) {

			spawnWall = true;
			bool rightHalfDoor = direction == "West" && (quadrant == 1 || quadrant == 3);
			bool leftHalfDoor = direction == "East" && (quadrant == 0 || quadrant == 2);
			if (rightHalfDoor || leftHalfDoor) {
				spawnWall = false;
			}

			newObj = GenerateDoorOrWall(direction, player, roomNum, "Blank", offset, spawnWall);
			newObj.transform.SetParent(finalTutorialRoom);
		}
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

	private string RandomRoomPrefab() {
		if (remainingRoomColors.Count == 0) {
			FillEmptyRoomColorList();
		}

		// Pick one of the three colors
		int colorIndexNum = pseudoRandom.Next(remainingRoomColors.Count);
		string color = remainingRoomColors[colorIndexNum];
		remainingRoomColors.RemoveAt(colorIndexNum);

		if (remainingRoomPrefabs[color].Count == 0) {
			ResetPrefabsRemainingByColor(color);
		}

		// Pick one of the prefabs of the color chosen above
		List<int> prefabList = remainingRoomPrefabs[color];
		int indexNum = pseudoRandom.Next(prefabList.Count);
		int prefabNum = prefabList[indexNum];
		prefabList.RemoveAt(indexNum);

		return color + prefabNum;
	}

	private void FillEmptyRoomColorList() {
		foreach (string color in colors) {
			for (int i = 0; i < size; i++) {
				remainingRoomColors.Add(color);
			}
		}
	}

	private void ResetPrefabsRemainingByColor(string color) {
		int numOfPrefabs;
		if (color == "Red") {
			numOfPrefabs = numRoomTypePrefabs[0];
		} else if (color == "Green") {
			numOfPrefabs = numRoomTypePrefabs[1];
		} else {
			numOfPrefabs = numRoomTypePrefabs[2];
		}

		List<int> prefabList = new List<int>(numOfPrefabs);
		for (int i = 0; i < numOfPrefabs; i++) {
			prefabList.Add(i);
		}
		if (remainingRoomPrefabs.ContainsKey(color)) {
			remainingRoomPrefabs[color] = prefabList;
		} else {
			remainingRoomPrefabs.Add(color, prefabList);
		}
	}

	private void GenerateDoorsAndWalls(Transform doorParent, string player, int roomNum, string roomColor, Vector3 offset, int[] quadrantRooms) {
		const string switchRoomNumber = "100";

		int quadrant = quadrantRooms[0];
		bool switchDoor, switchWall, tutorialDoor;
		switchDoor = switchWall = tutorialDoor = false;
		if (roomNum == quadrantRooms[2]) {
			switchDoor = true;
		} else if (roomNum == quadrantRooms[3]) {
			switchWall = true;
		} else if (roomNum == quadrantRooms[4]) {
			tutorialDoor = true;
		}

		bool spawnWall;
		GameObject newObj;
		foreach (string direction in directions) {
			if (direction == "East") {
				spawnWall = (roomNum % size == (size - 1)) && !switchDoor && !tutorialDoor;
			} else if (direction == "West") {
				spawnWall = (roomNum % size == 0) && !switchDoor && !tutorialDoor;
			} else if (direction == "North") {
				spawnWall = (roomNum >= (size * (size - 1))) || (switchWall && quadrant <= 1);
			} else {
				spawnWall = (roomNum < size) || (switchWall && quadrant >= 2);
			}

			newObj = GenerateDoorOrWall(direction, player, roomNum, roomColor, offset, spawnWall);
			newObj.transform.SetParent(doorParent);

			if (direction == "East") {
				if (switchDoor && (quadrant == 0 || quadrant == 2)) {
					newObj.GetComponent<DoorController>().nextRoomNum = switchRoomNumber;
					GameObject.Find("DoorSwitchEnterLeft").GetComponent<DoorController>().nextRoomNum = player + "-" + roomNum;
				} else if (tutorialDoor && (quadrant == 1 || quadrant == 3)) {
					newObj.GetComponent<DoorController>().nextRoomNum = player + "TutorialExit";
				} 
			} else if (direction == "West") {
				if (switchDoor && (quadrant == 1 || quadrant == 3)) {
					newObj.GetComponent<DoorController>().nextRoomNum = switchRoomNumber;
					GameObject.Find("DoorSwitchEnterRight").GetComponent<DoorController>().nextRoomNum = player + "-" + roomNum;
				} else if (tutorialDoor && (quadrant == 0 || quadrant == 2)) {
					newObj.GetComponent<DoorController>().nextRoomNum = player + "TutorialExit";
				} 
			}
		}
	}

	private GameObject GenerateDoorOrWall(string direction, string player, int roomNum, string roomColor, Vector3 posOffset, bool spawnWall) {

		GameObject newObj;
		Vector3 objectPos;
		Quaternion objectRot;
		DoorController doorControl;

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
		string[] objectNameList = new string[] {"Divider", "HUD", "Pause", "P1", "P2", "CameraParentP1", "CameraParentP2", "AudioListener"};
		foreach (string objectName in objectNameList) {
			GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/" + objectName) as GameObject);
			newObj.name = objectName;
			newObj.transform.SetParent(rootTransform);
		}

		Destroy(this.gameObject);
	}
}

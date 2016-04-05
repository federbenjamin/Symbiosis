using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class LevelGenerator : MonoBehaviour {

	private int seed;
	private int size;
	// private int maxDifficulty;  // depreciated, may be used later..
	private System.Random pseudoRandom;

	private string[] directions = new string[] {"West", "East", "North", "South"};
	private string[] colors = new string[] {"Red", "Green", "Blue"};

	private LevelGraph player1Level;
	private LevelGraph player2Level;

	private Transform roomParent;
	private Vector3 gauntletPosition;

	private List<string> remainingRoomColors = new List<string>();
	// List order: Red, Green, Blue
	private int[] numRoomTypePrefabs = new int[] {5, 5, 5};
	private Dictionary<string, List<int>> remainingRoomPrefabs = new Dictionary<string, List<int>>();
	private Dictionary<string, int[]> enemyTypeDifficulty = new Dictionary<string, int[]>();
	private int tutorialCorridorLength = 4;
	private int maxEnemiesPerRoom = 4;


	// Use this for initialization
	void Start () {
		GameObject rootNode = new GameObject("Root");
		GameObject roomsObject = new GameObject("Rooms");
		roomParent = roomsObject.transform;
		roomParent.SetParent(rootNode.transform);

		// Get level inputs - difficulty, size, and seed
		// maxDifficulty = LevelData.levelDifficulty;  // depreciated, may be used later..
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
		enemyTypeDifficulty.Add("Red", new int[] {1, 2, 3});
		enemyTypeDifficulty.Add("Green", new int[] {1, 2, 4});
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

			// Instantiate individual player level's
			LevelGraph playerLevel;
			if (player == "P1") {
				player1Level = new LevelGraph(player);
				playerLevel = player1Level;
			} else {
				player2Level = new LevelGraph(player);
				playerLevel = player2Level;
			}

 			GenerateInitalRooms(playerLevel, player, quadrant, xOffset, zOffset);
		}

		// Perform graph traversal algorithms on both player's level graphs
		foreach (string player in players) {
			LevelGraph playerLevel;
			if (player == "P1") {
				playerLevel = player1Level;
			} else {
				playerLevel = player2Level;
			}

			playerLevel.AddAllAdjacentRooms();

			playerLevel.GenerateLevel(pseudoRandom);

			// Calculate each room's distance from the switch room and spawn enemies based on that
			playerLevel.CalculateRoomDistances();

			// Do a final level update to remove all distant deadend room chains
			playerLevel.PruneDistantDeadends();

			// Modify in-game model of level to what the graph structure represents
			UpdateLevelUsingGraph(playerLevel, player);

			int maxDistance = playerLevel.MaxDistance;
			foreach (Node roomNode in playerLevel.RoomList) {
				string roomColor = roomNode.RoomObject.GetComponent<RoomController>().RoomColor;
				SpawnEnemies(roomNode, maxDistance, roomColor);
			}

			SetAllDoorLightColors(playerLevel, player);

			// playerLevel.Print();
		}

		LoadRemainingAssets();
	}

	private void SetAllDoorLightColors(LevelGraph playerLevel, string player) {
		// Set tutorial room light
		GameObject tutorialRoom = GameObject.Find("Room" + player + "TutorialExit");
		string roomColor = playerLevel.TutorialAdjRoom.RoomColor;
		int tutorialAdjRoomNumber = playerLevel.TutorialAdjRoom.RoomNumber;
		string doorName = "DoorWest";
		if (tutorialAdjRoomNumber == 0 || tutorialAdjRoomNumber == size * (size - 1)) {
			doorName = "DoorEast";
		}
		ChangeDoorColor(tutorialRoom, doorName, roomColor);

		// Set switch room light
		GameObject switchRoom = GameObject.Find("Room100");
		roomColor = playerLevel.SwitchAdjRoom.RoomColor;
		int switchAdjRoomNumber = playerLevel.SwitchAdjRoom.RoomNumber;
		doorName = "DoorSwitchEnterLeft";
		if (switchAdjRoomNumber == 1 || switchAdjRoomNumber == size - 2) {
			doorName = "DoorSwitchEnterRight";
		}
		ChangeDoorColor(switchRoom, doorName, roomColor);

		// Set core dungeon lights
		foreach (Edge edge in playerLevel.DoorList) {
			GameObject roomObject1 = edge.door1.RoomInside.RoomObject;
			GameObject roomObject2 = edge.door2.RoomInside.RoomObject;
			string roomColor1 = edge.door1.RoomInside.RoomColor;
			string roomColor2 = edge.door2.RoomInside.RoomColor;
			string doorName1 = "Door" + edge.door1.Direction;
			string doorName2 = "Door" + edge.door2.Direction;

			ChangeDoorColor(roomObject1, doorName1, roomColor2);
			ChangeDoorColor(roomObject2, doorName2, roomColor1);
		}
	}

	private void ChangeDoorColor(GameObject roomObject, string doorName, string newDoorColor) {
		Color colorObject = GetColorObject(newDoorColor);
		foreach (Transform child in roomObject.transform) {
			if (child.name == doorName) {
				foreach (Transform doorChild in child) {
					if (doorChild.name == "NextRoomLight") {
						Light doorLight = doorChild.gameObject.GetComponent<Light>();
						doorLight.color = colorObject;
						break;
					}
				}
				break;
			}
		}
	}

	private Color GetColorObject(string roomColor) {
		if (roomColor == "Red") {
			return Color.red;
		} else if (roomColor == "Green") {
			return Color.green;
		} else {
			return Color.blue;
		}
	}

	private void UpdateLevelUsingGraph(LevelGraph playerLevel, string player) {
		// Replace doors that have been removed with walls
		foreach (Edge edge in playerLevel.RemovedDoorList) {
			ReplaceDoorWithWall(player, edge);
		}
		playerLevel.ClearRemovedDoors();

		// Find all rooms with no doors attached and remove them;
		List<Node> isolatedRooms = playerLevel.GetIsolatedRooms();
		foreach (Node room in isolatedRooms) {
			Destroy(room.RoomObject);
		}
	}

	private void GenerateInitalRooms(LevelGraph playerLevel, string player, int quadrant, int levelXOffset, int levelZOffset) {
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

					// Create room in scene hierarchy 
					string roomSuffix = RandomRoomPrefab();
					string roomColor = Regex.Match(roomSuffix, @"\D+").Groups[0].Value;
					string name = "Room" + player + "-" + roomNumber;
					GameObject newRoom = GenerateRoom(roomSuffix, name, roomPosition, roomColor);

					// Add room to graph
					Node newRoomNode = AddGraphNode(playerLevel, newRoom, roomNumber, quadrantRooms, roomColor);
					
					GenerateDoorsAndWalls(playerLevel, newRoomNode, newRoom.transform, player, roomNumber, roomColor, roomPosition, quadrantRooms);
				}
			}
		}
	}

	private Node AddGraphNode(LevelGraph playerLevel, GameObject newRoom, int roomNumber, int[] quadrantRooms, string roomColor) {
		Node newRoomNode = new Node(newRoom, roomNumber, roomColor);

		if (roomNumber == quadrantRooms[4]) {
			// Tutorial Adjacent Room
			playerLevel.TutorialAdjRoom = newRoomNode;
		} else if (roomNumber == quadrantRooms[2]) {
			// Switch Adjacent Room
			playerLevel.SwitchAdjRoom = newRoomNode;
		}

		// All other core level rooms
		playerLevel.RoomList.Add(newRoomNode);
		return newRoomNode;
	}

	private void SpawnEnemies(Node roomNode, int maxDistance, string roomColor) {
		Transform room = roomNode.RoomObject.transform;
		float roomDifficulty = GetNormalizedRoomDifficulty(roomNode.Distance, maxDistance);

		// Generate a random subset of possible enemy types
		List<Transform> spawnerTransforms = new List<Transform>();
		foreach (Transform child in room) {
			if (child.tag == "EnemySpawnerBlank") {
				spawnerTransforms.Add(child);
			}
		}

		// Establish upper and lower enemy count limits
		int spawnCountUpper, spawnCountLower;
		if (roomDifficulty <= 0.15f) {
			spawnCountLower = 0;
			spawnCountUpper = 1;
		} else if (roomDifficulty >= 0.96f) {
			spawnCountLower = 4;
			spawnCountUpper = 4;
		} else if (roomDifficulty <= 0.55f) {
			spawnCountLower = 1;
			spawnCountUpper = 2;
		} else if (roomDifficulty >= 0.85f) {
			spawnCountLower = 3;
			spawnCountUpper = 4;
		} else {
			spawnCountLower = 2;
			spawnCountUpper = 3;
		}

		// Generate random number between boundaries for number of enemies to spawn
		int spawnCount = pseudoRandom.Next(spawnCountLower, spawnCountUpper + 1);
		int[] enemiesToSpawn = GenerateEnemySet(roomDifficulty, roomColor, spawnCount);

		// Spawn choosen enemy types in random locations in the room
		for (int i = 0; i < enemiesToSpawn.Length; i++) {
			int spawnObjectIndex = pseudoRandom.Next(spawnerTransforms.Count);
			Transform spawnObject = spawnerTransforms[spawnObjectIndex];
			spawnerTransforms.RemoveAt(spawnObjectIndex);
			ReplaceSpawnWithObject(spawnObject, roomColor, room, enemiesToSpawn[i]);
		}
		// Clean up the rest of the spawn objects
		int maxHealthItems = 2;
		foreach (Transform remainingSpawns in spawnerTransforms) {
			if (maxHealthItems > 0) {
				ReplaceSpawnWithHeart(remainingSpawns, room);
			}
			Destroy(remainingSpawns.gameObject);
			maxHealthItems--;
		}
	}

	private int[] GenerateEnemySet(float roomDifficulty, string roomColor, int numOfSpawnLocations) {
		// Get random index of enemy type list with hardest enemy based on room difficulty
		int[] enemyTypes = enemyTypeDifficulty[roomColor];
		int enemyTypeMax = enemyTypes.Length - 1;
		if (roomDifficulty < 0.2f) {
			enemyTypeMax = 0;
		} else if (roomDifficulty < 0.5f) {
			enemyTypeMax = 1;
		}

		// Initialize enemy set to random enemies
		int[] enemySet = new int[numOfSpawnLocations];
		for (int i = 0; i < numOfSpawnLocations; i++) {
			int randomEnemyIndex = pseudoRandom.Next(enemyTypeMax + 1);
			enemySet[i] = enemyTypes[randomEnemyIndex];
		}

		return enemySet;
	}

	// Unneccessary ??
	private int SumArray(int[] array) {
		int sum = 0;
		for (int i = 0; i < array.Length; i++) {
			sum += array[i];
		}
		return sum;
	}

	private void ReplaceSpawnWithHeart(Transform spawn, Transform newRoom) {
		string objectSpawnFile = "";
		int objectToSpawn = pseudoRandom.Next(100);

		// Spawn health pick-ups
		if (objectToSpawn >= 98) {
			objectSpawnFile = "FullHeart";
		}
		else if (objectToSpawn >= 95) {
			objectSpawnFile = "HalfHeart";
		}

		if (objectSpawnFile != "") {
			GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/" + objectSpawnFile)) as GameObject;
			newObj.transform.SetParent(newRoom);
			Vector3 heartPosition = spawn.position;
			heartPosition.y = 0.48f;
			newObj.transform.position = heartPosition;
		}
		Destroy(spawn.gameObject);
	}

	private void ReplaceSpawnWithObject(Transform spawn, string roomColor, Transform newRoom, int enemyNumber) {
		string objectSpawnFile;
		int objectToSpawn = pseudoRandom.Next(100);

		objectSpawnFile = "EnemySpawns/SpawnEnemy" + roomColor + enemyNumber;

		GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/" + objectSpawnFile)) as GameObject;
		newObj.transform.SetParent(newRoom);
		newObj.transform.position = spawn.position;
		Destroy(spawn.gameObject);
	}

	private float GetNormalizedRoomDifficulty(int roomDistance, int maxDistance) {
		float normalizedDifficulty = (1f - (float)roomDistance / (float)maxDistance);
		return normalizedDifficulty;
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

		bool spawnWall;
		GameObject newObj;
		foreach (string direction in directions) {
			if (direction == "West" || direction == "East") {

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

	private void GenerateDoorsAndWalls(LevelGraph playerLevel, Node roomNode, Transform doorParent, string player, int roomNum, string roomColor, Vector3 offset, int[] quadrantRooms) {
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
					continue;
				} else if (tutorialDoor && (quadrant == 1 || quadrant == 3)) {
					newObj.GetComponent<DoorController>().nextRoomNum = player + "TutorialExit";
					continue;
				} 
			} else if (direction == "West") {
				if (switchDoor && (quadrant == 1 || quadrant == 3)) {
					newObj.GetComponent<DoorController>().nextRoomNum = switchRoomNumber;
					GameObject.Find("DoorSwitchEnterRight").GetComponent<DoorController>().nextRoomNum = player + "-" + roomNum;
					continue;
				} else if (tutorialDoor && (quadrant == 0 || quadrant == 2)) {
					newObj.GetComponent<DoorController>().nextRoomNum = player + "TutorialExit";
					continue;
				} 
			}

			// Add to graph
			if (!spawnWall) {
				int connectingRoom = GetConnectingRoom(roomNum, direction);
				// Only add doors connected to rooms with a lower number (avoid duplicates)
				if (connectingRoom < roomNum) {
					Node adjacentNode = playerLevel.GetRoomByNumber(connectingRoom);

					// Debug to make sure no null adjacent nodes are found
					if (adjacentNode == null) {
						Debug.LogError(player + ": room #" + roomNode.RoomNumber + " returned NULL adjacent room node #" + connectingRoom);
					}

					Edge newEdge = new Edge(roomNode, direction, adjacentNode);
					playerLevel.DoorList.Add(newEdge);
				}
			}
		}
	}

	private int GetConnectingRoom(int roomNum, string doorDirection){
		int nextRoomNum;
		if (doorDirection == "East") {
			nextRoomNum = roomNum + 1;
		} else if (doorDirection == "West") {
			nextRoomNum = roomNum - 1;
		} else if (doorDirection == "North") {
			nextRoomNum = roomNum + size;
		} else {
			nextRoomNum = roomNum - size;
		}
		return nextRoomNum;
	}

	private GameObject GenerateDoorOrWall(string direction, string player, int roomNum, string roomColor, Vector3 posOffset, bool spawnWall) {

		GameObject newObj;
		Vector3 objectPos;
		Quaternion objectRot;
		DoorController doorControl;

		int nextRoomNum = GetConnectingRoom(roomNum, direction);
		Vector3 dirOffset;
		int yRotation;
		char newOutDoor;

		if (direction == "East") {
			newOutDoor = 'w';
			yRotation = -90;
			dirOffset = new Vector3(8f, 0, 0);
		} 
		else if (direction == "West") {
			newOutDoor = 'e';
			yRotation = 90;
			dirOffset = new Vector3(-8f, 0, 0);
		} 
		else if (direction == "North") {
			newOutDoor = 's';
			yRotation = -180;
			dirOffset = new Vector3(0, 0, 4f);
		} 
		else {
			newOutDoor = 'n';
			yRotation = 0;
			dirOffset = new Vector3(0, 0, -4f);
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

	// Swap parameters with edge to replace 2 doors at once
	private void ReplaceDoorWithWall(string player, Edge connectingDoors) {
		foreach (Door door in new Door[] {connectingDoors.door1, connectingDoors.door2}) {
			int roomNum = door.RoomInside.RoomNumber;
			string direction = door.Direction;

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
					newWall.transform.SetParent(room.transform);
					Destroy(child.gameObject);
					break;

				}
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
				newDoor.transform.SetParent(room.transform);
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

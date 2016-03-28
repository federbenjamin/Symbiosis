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

		Vector3 roomPosition = new Vector3(size * 40, 0, (size - 1) * 32);
		GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/Gauntlet"), roomPosition, Quaternion.identity) as GameObject;
		newObj.name = "RoomGauntlet";
		newObj.transform.SetParent(roomParent);
		
		LoadRemainingAssets();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GenerateInitalRooms(string player, int offset) {
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				int roomNumber = (i * size) + j;
				int xOffset = i * 40 + offset;
				int zOffset = j * 32;
				Vector3 roomPosition = new Vector3(xOffset, 0, zOffset);
				GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/RoomBlank"), roomPosition, Quaternion.identity) as GameObject;
				newObj.name = "Room" + player + roomNumber;
				newObj.transform.SetParent(roomParent);
			}
		}
	}

	void LoadRemainingAssets() {
		Transform rootTransform = GameObject.Find("Root").transform;
		string[] objectNameList = new string[] {"Divider", "DividerHealth", "Canvas", "P1", "P2", "CameraParentP1", "CameraParentP2"};
		foreach (string objectName in objectNameList) {
			GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/" + objectName) as GameObject);
			newObj.name = objectName;
			newObj.transform.SetParent(rootTransform);
		}
	}
}

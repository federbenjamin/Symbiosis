using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomController : MonoBehaviour {

	public GameObject enemy1;


	public List<GameObject> spawnpoints;
	public List<GameObject> enemies;
	public int players = 0;
	public bool playersTogether = false;

	private bool hasTriggered = false;
	public bool roomCleared = false;

	public int switchesActive = 0;

	private CameraController cameraController;

	// Use this for initialization
	void Start () {

		//Add spawnPoints in the room to the array
		foreach (Transform child in transform) {
			if (child.tag == "Enemy1Spawn") {
				spawnpoints.Add (child.gameObject);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		//Check if player has enetered room and count enemies
		if (transform.name == "Room11") {
			if (switchesActive == 2) {
				roomCleared = true;
			}
		} else {
			if (hasTriggered == true) {
				if (roomCleared == false) {
					CheckIfEnemies ();
					if (enemies.Count == 0) {
						roomCleared = true;
					}
				}
			}
		}
	}

	void OnTriggerEnter(Collider other) {

		//If player enters room, spawn enemies and lock doors
		if (other.tag == "Player") {
			players += 1;

			if (players == 2 && playersTogether == false) {
				playersTogether = true;
				cameraController = GameObject.Find ("CameraP1").GetComponent<CameraController> ();
				cameraController.MergeCamera ();
			}

			if (hasTriggered == false) {
				hasTriggered = true;
				foreach (GameObject spawnpoint in spawnpoints) {
					GameObject enemyChild = Instantiate (enemy1, spawnpoint.transform.position, spawnpoint.transform.rotation) as GameObject;
					enemyChild.transform.parent = transform;
				}
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			players -= 1;
		}
	}

	//Add enemies in the current room to the enemies List
	void CheckIfEnemies() {
		enemies.Clear();
		foreach (Transform child in transform) {
			if (child.tag == "Enemy") {
				enemies.Add (child.gameObject);
			}
		}
	}
}

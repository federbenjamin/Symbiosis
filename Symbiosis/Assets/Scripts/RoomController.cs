﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviour {

	public GameObject enemy1;
	public GameObject enemy2;
	public GameObject boss;
	public GameObject greenZomb;
	public GameObject redZomb;
	public GameObject blueZomb;

	public List<GameObject> spawnpoints;
	public List<GameObject> enemies;
	public List<GameObject> doors;
	public List<GameObject> switches;

	public int players = 0;
	public static bool playersTogether = false;

	private bool hasTriggered = false;
	public bool roomCleared = false;

	public int switchesActive = 0;

	private CameraController cameraController;

	private Transform doorLeft;
	private Transform doorRight;

	// Use this for initialization
	void Start () {
		playersTogether = false;

		//Add spawnPoints in the room to the array
		foreach (Transform child in transform) {
			if (child.tag == "EnemySpawn") {
				spawnpoints.Add (child.gameObject);
			} else if (child.tag == "Door") {
				doors.Add (child.gameObject);
			} else if (child.tag == "Switch") {
				switches.Add (child.gameObject);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		//Check if player has enetered room and count enemies
		if (transform.name == "Room11") {
			if (!hasTriggered && switchesActive == 2) {
				hasTriggered = true;
				SpawnEnemies();
				foreach (GameObject button in switches) {
					Destroy (button);
					switchesActive = 0;
				}
			}

			if (hasTriggered && !roomCleared) {
				if (CountEnemies() == 0) {
					roomCleared = true;
					foreach (GameObject door in doors) {
						Animator doorAnimator = door.GetComponent<Animator> ();
						doorAnimator.SetTrigger ("Open");
					//	doorLeft = door.transform.Find ("LeftDoor");
					//	doorLeft.transform.Rotate (0, 0, 120);
					//	doorRight = door.transform.Find ("RightDoor");
					//	doorRight.transform.Rotate (0, 0, -120);
					}
					SceneManager.LoadScene ("WinScreen");
				}
			}
		} else {
			if (hasTriggered == true) {
				if (roomCleared == false) {
					if (CountEnemies() == 0) {
						roomCleared = true;

						//Leftdoor +, RightDoor - Rotations in Y
						foreach (GameObject door in doors) {
							Animator doorAnimator = door.GetComponent<Animator> ();
							doorAnimator.SetTrigger ("Open");
						//	doorLeft = door.transform.Find ("LeftDoor");
						//	doorLeft.transform.Rotate (0, 0, 120);
						//	doorRight = door.transform.Find ("RightDoor");
						//	doorRight.transform.Rotate (0, 0, -120);
						}
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

			if (hasTriggered == false && transform.name != "Room11") {
				hasTriggered = true;
				SpawnEnemies();
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			players -= 1;
		}
	}

	void SpawnEnemies() {
		/*
		 * 10's digits are Green Enemies
		 * 20's digits are Red Enemies
		 * 30's digits are Blue Enemies
		 *
		 * _0 is walking zombie
		 * _1 is walking with shooting scientist
		 * _2 is special lab enemy
		 */

		foreach (GameObject spawnpoint in spawnpoints) {
			string enemyType = spawnpoint.name.Substring(10);
			GameObject enemyChild;
			if (enemyType == "1") {
				enemyChild = Instantiate (enemy1, spawnpoint.transform.position, spawnpoint.transform.rotation) as GameObject;
			} else if (enemyType == "2") {
				enemyChild = Instantiate (enemy2, spawnpoint.transform.position, spawnpoint.transform.rotation) as GameObject;
				enemyChild.GetComponent<Enemy2Behavior> ().addShootingOffset (100);
			} else if (enemyType == "10") {
				enemyChild = Instantiate (greenZomb, spawnpoint.transform.position, spawnpoint.transform.rotation) as GameObject;
			} else if (enemyType == "20") {
				enemyChild = Instantiate (redZomb, spawnpoint.transform.position, spawnpoint.transform.rotation) as GameObject;
			} else if (enemyType == "30") {
				enemyChild = Instantiate (blueZomb, spawnpoint.transform.position, spawnpoint.transform.rotation) as GameObject;
			} else {
				enemyChild = Instantiate (boss, spawnpoint.transform.position, spawnpoint.transform.rotation) as GameObject;
			}
			enemyChild.transform.parent = transform;
		}
	}

	//Add enemies in the current room to the enemies List
	int CountEnemies() {
		int total = 0;
		enemies.Clear();
		foreach (Transform child in transform) {
			if (child.tag == "Enemy") {
				total++;
			}
		}
		return total;
	}

	public bool getPlayersTogether() {
		return playersTogether;
	}
}

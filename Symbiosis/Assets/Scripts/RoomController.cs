using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class RoomController : MonoBehaviour {

	private string roomColor;
	public string RoomColor {
		get{return roomColor;}
		set{roomColor = value;}
	}

	public GameObject boss;
	public GameObject greenZomb;
	public GameObject redZomb;
	public GameObject blueZomb;
	public GameObject blueSpecial;
	public GameObject blueTurret;
	public GameObject greenSpecial;
	public GameObject redSpecial;

	public List<GameObject> spawnpoints;
	public List<GameObject> enemies;
	public List<GameObject> doors;
	public List<GameObject> switches;

	public int players = 0;
	public static bool playersTogether;

	private bool hasTriggered = false;
	private bool enemiesActive = false;
	public bool EnemiesActive {
		get{return enemiesActive;}
		set{enemiesActive = value;}
	}
	public bool roomCleared = false;

	private bool switch1Active = false;
	public bool Switch1Active {
		get{return switch1Active;}
		set{switch1Active = value;}
	}
	private bool switch2Active = false;
	public bool Switch2Active {
		get{return switch2Active;}
		set{switch2Active = value;}
	}

	private CameraController cameraController;

	private Transform doorLeft;
	private Transform doorRight;

	// Use this for initialization
	void Start () {
		playersTogether = GameObject.FindWithTag("Canvas").GetComponent<GameStats>().PlayersTogether;

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

		if (transform.name == "Room100") roomCleared = true;
	}
	
	// Update is called once per frame
	void Update () {

		// Logic for switch room
		if (transform.name == "Room100") {
			if (!hasTriggered && switch1Active && switch2Active) {
				hasTriggered = true;
				foreach (GameObject button in switches) {
					button.GetComponent<SwitchesController> ().PermanentlySwitchedOff = true;
				}
				foreach (GameObject door in doors) {
					if (door.name == "DoorSwitchExit") {
						Animator doorAnimator = door.GetComponent<Animator> ();
						StartCoroutine (OpenDoor(1f, doorAnimator));
					} else if (door.name == "DoorSwitchEnterLeft" || door.name == "DoorSwitchEnterRight") {
						Animator doorAnimator = door.GetComponent<Animator> ();
						doorAnimator.SetTrigger ("Close");
					}
				}
			}
		//Check if player has enetered room and count enemies
		} else {
			if (hasTriggered == true) {
				if (roomCleared == false) {
					if (CountEnemies() == 0) {
						roomCleared = true;
						if (transform.name == "Room200") {
							StartCoroutine ("Wait");
						}
						//Leftdoor +, RightDoor - Rotations in Y
						if (transform.name != "RoomP1Tutorial1" && transform.name != "RoomP2Tutorial1") {
							foreach (GameObject door in doors) {
								if (door != null) {
									Animator doorAnimator = door.GetComponent<Animator> ();
									doorAnimator.SetTrigger ("Open");
								}
							}
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
				GameObject.FindWithTag("Canvas").GetComponent<GameStats>().PlayersTogether = true;
				cameraController = GameObject.Find ("CameraParentP1").GetComponent<CameraController> ();
				cameraController.MergeCamera ();
			}

			if (hasTriggered == false && transform.name != "Room100") {
				hasTriggered = true;
				SpawnEnemies();
			}

			other.gameObject.GetComponent<StatsManager>().RoomIn = this.gameObject;
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
		 * _3 is turret enemy
		 */

		foreach (GameObject spawnpoint in spawnpoints) {
			string enemyType = spawnpoint.name.Substring(10);
			enemyType = Regex.Match(enemyType, @"\D+[1-3]").Groups[0].Value;
			GameObject enemyChild;

			Vector3 spawnVector = spawnpoint.transform.position;
			spawnVector.y = 0.055f;
			Debug.Log(enemyType);
			if (enemyType == "Green1" || enemyType == "10") {
				enemyChild = Instantiate (greenZomb, spawnVector, spawnpoint.transform.rotation) as GameObject;
			} else if (enemyType == "Green2" || enemyType == "12") {
				enemyChild = Instantiate (greenSpecial, spawnVector, spawnpoint.transform.rotation) as GameObject;
			} else if (enemyType == "Red1" || enemyType == "20") {
				enemyChild = Instantiate (redZomb, spawnVector, spawnpoint.transform.rotation) as GameObject;
			} else if (enemyType == "Red2" || enemyType == "22") {
				enemyChild = Instantiate (redSpecial, spawnVector, spawnpoint.transform.rotation) as GameObject;
			}  else if (enemyType == "Blue1" || enemyType == "30") {
				enemyChild = Instantiate (blueZomb, spawnVector, spawnpoint.transform.rotation) as GameObject;
			} else if (enemyType == "Blue2" || enemyType == "32") {
				enemyChild = Instantiate (blueSpecial, spawnVector, spawnpoint.transform.rotation) as GameObject;
			} else if (enemyType == "Blue3" || enemyType == "33") {
				enemyChild = Instantiate (blueTurret, spawnVector, spawnpoint.transform.rotation) as GameObject;
			} else {
				spawnVector.y = 0.74f;
				enemyChild = Instantiate (boss, spawnVector, spawnpoint.transform.rotation) as GameObject;
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

	public bool ContainsEnemySpawn() {
		int total = 0;
		foreach (Transform child in transform) {
			if (child.tag == "EnemySpawn") {
				total++;
			}
		}
		return total > 0;
	}

	public bool getPlayersTogether() {
		return playersTogether;
	}

	IEnumerator Wait() {
		yield return new WaitForSeconds (3f);
		SceneManager.LoadScene ("WinScreen");
	}

	IEnumerator OpenDoor(float timeToWait, Animator doorAnimator) {
		doorAnimator.Play("DoorButton-ColorChange");
		yield return new WaitForSeconds (timeToWait);
		doorAnimator.Play("DoorOpening");
	}

}

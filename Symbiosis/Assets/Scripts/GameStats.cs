using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStats : MonoBehaviour {

	private bool playersTogether;
	public bool PlayersTogether {
		get{return playersTogether;}
		set{playersTogether = value;}
	}

	void Start () {
		playersTogether = false;
		GameObject[] roomsList = GameObject.FindGameObjectsWithTag("Room");
		foreach (GameObject room in roomsList) {
			OpenDoors(room);
		}
	}

	void OpenDoors(GameObject room) {
		RoomController roomController = room.GetComponent<RoomController> ();
		bool enemies = roomController.ContainsEnemySpawn();

		if (!enemies && room.name != "Room100") {
			foreach (Transform child in room.transform) {
				GameObject door = child.gameObject;
				if (door.tag == "Door") {
					Animator doorAnimator = door.GetComponent<Animator> ();
					doorAnimator.SetTrigger ("Open");
				}
			}
		}
	}

}
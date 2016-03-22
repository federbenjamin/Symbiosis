using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStats : MonoBehaviour {

	public static bool paused = false;
	private string startButton;
	private bool playersTogether = false;
	public bool PlayersTogether {
		get{return playersTogether;}
		set{playersTogether = value;}
	}
	private AudioPlacement gameAudio;

	void Start () {
		gameAudio = GameObject.Find("AudioListener").GetComponent<AudioPlacement> ();
		GameObject[] roomsList = GameObject.FindGameObjectsWithTag("Room");
		foreach (GameObject room in roomsList) {
			OpenDoors(room);
		}

		if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) {
			startButton = "StartMac";
		} else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) {
			startButton = "StartPC";
		}
	}

	void Update () {
		if (Input.GetButtonDown(startButton)) {
			if (!paused) {
				paused = true;
				Time.timeScale = 0;
				gameAudio.changeMainSongVolume(0.07f);
				gameAudio.changeMainSongPitch(0.9f);
			} else {
				paused = false;
				Time.timeScale = 1;
				gameAudio.changeMainSongVolume(0.264f);
				gameAudio.changeMainSongPitch(1f);
			}
		}
	}

	void OpenDoors(GameObject room) {
		RoomController roomController = room.GetComponent<RoomController> ();
		bool enemies = roomController.ContainsEnemySpawn();

		if (!enemies) {
			foreach (Transform child in room.transform) {
				GameObject door = child.gameObject;
				if (door.tag == "Door" && door.name != "DoorSwitchExit") {
					Animator doorAnimator = door.GetComponent<Animator> ();
					doorAnimator.SetTrigger ("Open");
				}
			}
		}
	}

}
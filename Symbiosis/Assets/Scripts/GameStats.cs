using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStats : MonoBehaviour {

	public static bool paused = false;
	public static bool playersCanMove = false;
	private string startButton;
	private bool playersTogether = false;
	public bool PlayersTogether {
		get{return playersTogether;}
		set{playersTogether = value;}
	}
	private AudioPlacement gameAudio;
	public GameObject P1Slime;
	public GameObject P2Slime;
	public GameObject P1Tank;
	public GameObject P2Tank;

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

		StartCoroutine("WaitForGameLoad");
		StartCoroutine("WaitForAnimationEndToMove");
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

	IEnumerator WaitForAnimationEndToMove() {
		yield return new WaitForSeconds (2.6f);
		playersCanMove = true;
	}

	IEnumerator WaitForGameLoad() {
		yield return new WaitForSeconds (0.75f);
		P1Slime.GetComponent<Animator>().SetTrigger("StartJump");
		P1Tank.GetComponent<Animator>().SetTrigger("StartBreak");
		P2Slime.GetComponent<Animator>().SetTrigger("StartJump");
		P2Tank.GetComponent<Animator>().SetTrigger("StartBreak");
	}

}
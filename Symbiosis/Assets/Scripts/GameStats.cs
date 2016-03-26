using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStats : MonoBehaviour {

	public static GameStats Instance;

	public bool invincible = false;
	public static bool paused = false;
	public static bool gameStarted = false;
	private string startButton;
	private bool playersTogether = false;
	public bool PlayersTogether {
		get{return playersTogether;}
		set{playersTogether = value;}
	}
	private AudioPlacement gameAudio;

	void Awake () {
		Instance = this;
	}

	void Start () {
		// Scale and Translate UI depending on screen size
		foreach (Transform ui in transform) {
			RectTransform uiTransform = (RectTransform) ui;
			float newScale;
			if (ui.name == "Health") {
				newScale = (Screen.height / (256f * 8f)) * 2f;
				Camera healthDivCam = GameObject.Find("DividerHealth").GetComponent<Camera>();
				healthDivCam.rect = new Rect(healthDivCam.rect.x, healthDivCam.rect.y, healthDivCam.rect.width, newScale * 100f / Screen.height);
			} else {
				newScale = Screen.height / (256f * 8f);

				// Translate Augment UI
				float newXPos = (Screen.width * 0.99f / 4f) - (512f / 2f * newScale);
				if (ui.name == "P2Hud") {
					newXPos = -newXPos;
				}
				uiTransform.anchoredPosition = new Vector2(newXPos, 0f);
			}
			uiTransform.localScale = new Vector3(newScale, newScale, 1.0f);
		}

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
		if (!gameStarted) {
			StartCoroutine("StartIntroAnimation");
		}
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

	IEnumerator StartIntroAnimation() {
		yield return new WaitForSeconds (2.0f);
		GameObject.Find("Player_blue_slime").GetComponent<Animator>().SetTrigger("StartJump");
		GameObject.Find("P1InitialSlimeTank").GetComponent<Animator>().SetTrigger("StartBreak");
		GameObject.Find("Player_yellow_slime").GetComponent<Animator>().SetTrigger("StartJump");
		GameObject.Find("P2InitialSlimeTank").GetComponent<Animator>().SetTrigger("StartBreak");
		yield return new WaitForSeconds (2.4f);
		gameStarted = true;
		CameraController.followSlime = false;
	}

}
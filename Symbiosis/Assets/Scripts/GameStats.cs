using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameStats : MonoBehaviour {

	public static GameStats Instance;

	public bool invincible = false;
	public bool skipIntro;


	public static bool paused = false;
	private GameObject pauseUI;
	public static bool gameStarted = false;
	private bool animationStarted = false;
	private string startButton;
	private bool playersTogether = false;
	public bool PlayersTogether {
		get{return playersTogether;}
		set{playersTogether = value;}
	}
	private AudioPlacement gameAudio;
	public bool isGeneratedLevel;

	void Awake () {
		Instance = this;
	}

	void Start () {
		// Scale and Translate UI depending on screen size
		float inverseHudHeight = 7.32f;
		foreach (Transform ui in transform) {
			RectTransform uiTransform = (RectTransform) ui;
			float newScaleX, newScaleY = 0f;

			// Health UI Element
			if (ui.name == "Health") {
				newScaleX = (Screen.height / (256f * inverseHudHeight)) * 1.83f;
				// Camera healthDivCam = GameObject.Find("DividerHealth").GetComponent<Camera>();
				// healthDivCam.rect = new Rect(healthDivCam.rect.x, healthDivCam.rect.y, healthDivCam.rect.width, newScaleX * 100f / Screen.height);
			}
			// Pause Screen
			else if (ui.name == "Pause") {
				// Make pause screen invisible when game starts
				// pauseUI = ui.gameObject;
				// pauseUI.GetComponent<Image>().color = new Color(0, 0, 0, 0);
				// pauseUI.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0);
				// Get scale for pause background and keep pause text in a locked ratio
				 newScaleY = (Screen.height / 1080f);
				 newScaleX = (Screen.width / 1920f);
				// if (Mathf.Min(newScaleY, newScaleX) == newScaleX) {
				// 	pauseUI.transform.GetChild(0).localScale = new Vector3(newScaleY / newScaleX, 1.0f, 1.0f);
				// } else {
				// 	pauseUI.transform.GetChild(0).localScale = new Vector3(1.0f, newScaleX / newScaleY, 1.0f);
				// }
				// Get new anchor position based on screen height
				float newYPos = Screen.height / -2f;
				uiTransform.anchoredPosition = new Vector2(0f, newYPos);
			}
			// Augment UI Element
			else {
				newScaleX = Screen.height / (256f * inverseHudHeight);

				// Translate Augment UI
				float newXPos = (Screen.width * 0.99f / 4f) - (512f / 2f * newScaleX);
				if (ui.name == "P2Hud") {
					newXPos = -newXPos;
				}
				uiTransform.anchoredPosition = new Vector2(newXPos, 0f);
			}
			// Rescale UI element
			newScaleY = (newScaleY == 0f ? newScaleX : newScaleY);
			uiTransform.localScale = new Vector3(newScaleX, newScaleY, 1.0f);
		}

		gameAudio = GameObject.Find("AudioListener").GetComponent<AudioPlacement> ();

		OpenDoorsExceptFirst();

		if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) {
			startButton = "StartMac";
		} else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) {
			startButton = "StartPC";
		}

		if (!gameStarted && !animationStarted && !skipIntro) {
			animationStarted = true;
			StartCoroutine("StartIntroAnimation");
		} else if (skipIntro) {
			StartCoroutine("SkipIntroAnimation");
		}
	}

	void Update () {
		
	}

	void OpenDoorsExceptFirst() {
		List<GameObject> roomsList = new List<GameObject>();
		foreach (GameObject normalRoom in GameObject.FindGameObjectsWithTag("Room")) {
			roomsList.Add(normalRoom);
		}
		if (skipIntro) {
			foreach (GameObject tutRoom in GameObject.FindGameObjectsWithTag("TutorialRoom")) {
				roomsList.Add(tutRoom);
			}
		}

		foreach (GameObject room in roomsList) {
			OpenDoor(room);
		}
	}

	void OpenDoor(GameObject room) {
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
		GameObject blueSlime = GameObject.Find("Player_blue_slime");
		GameObject yellowSlime = GameObject.Find("Player_yellow_slime");
		blueSlime.GetComponent<Animation>().Play("SlimeJumping");//.SetTrigger("StartJump");
		GameObject.Find("P1InitialSlimeTank").GetComponent<Animator>().SetTrigger("StartBreak");
		yellowSlime.GetComponent<Animation>().Play("SlimeJumping");//.SetTrigger("StartJump");
		GameObject.Find("P2InitialSlimeTank").GetComponent<Animator>().SetTrigger("StartBreak");
		yield return new WaitForSeconds (2.2f);

		CameraController.followSlime = false;
		yield return new WaitForSeconds (1.8f);
		gameStarted = true;

		//yield return new WaitForSeconds (6.0f);
		animationStarted = false;
	}

	IEnumerator SkipIntroAnimation() {
		GameObject blueSlime = GameObject.Find("Player_blue_slime");
		GameObject yellowSlime = GameObject.Find("Player_yellow_slime");
		blueSlime.GetComponent<Animation>().Play("SlimeJumping");
		yellowSlime.GetComponent<Animation>().Play("SlimeJumping");

		CameraController.followSlime = false;
		gameStarted = true;
		animationStarted = false;
		yield return new WaitForSeconds (0f);
	}

}
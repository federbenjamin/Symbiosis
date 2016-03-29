using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

public class GamePause : MonoBehaviour {

	public static bool isPaused;
	public bool showControls;
	private string startButton;
	public GameObject pausePanel;
	public GameObject firstSelected;
	public GameObject controlsPanel;
	public GameObject controlsImage;
	public GameObject controlsPageCount;
	private GameObject myEventSystem;
	private Sprite[] tutorialSprites;
	private int controlImageIndex = 0;
	private AudioPlacement gameAudio;


	// Use this for initialization
	void Start () {

		if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) {
			startButton = "StartMac";
		} else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) {
			startButton = "StartPC";
		}

		isPaused = false;
		showControls = false;
		gameAudio = GameObject.Find("AudioListener").GetComponent<AudioPlacement> ();	
		tutorialSprites = Resources.LoadAll("Interface/TutorialFloors", typeof(Sprite)).Cast<Sprite>().ToArray();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown(startButton)) {
			if (showControls) {
				ToggleControls();
			}
			TogglePause ();
		}

		if (Input.GetButtonDown("UICancel")) {
			if (showControls) {
				ToggleControls();
			} else {
				TogglePause ();
			}
		}

		if (showControls) {
			if (Input.GetAxisRaw("UIHorizontal") > 0) {
				changeControlImage("next");
			} else if (Input.GetAxisRaw("UIHorizontal") < 0) {
				changeControlImage("previous");
			}
		}
	}

	public void TogglePause () {
		isPaused = !isPaused;

		if (isPaused) {
			pausePanel.SetActive(true);
			EventSystem.current.SetSelectedGameObject(firstSelected, null);
			Time.timeScale = 0.0f;
			gameAudio.changeMainSongVolume(0.07f);
			gameAudio.changeMainSongPitch(0.9f);
		} else {
			pausePanel.SetActive(false);
			Time.timeScale = 1.0f;
			gameAudio.changeMainSongVolume(0.264f);
			gameAudio.changeMainSongPitch(1f);
		}
	}

	public void ToggleControls () {
		showControls = !showControls;

		if (showControls) {
			controlImageIndex = 0;
			controlsImage.GetComponent<Image>().sprite = tutorialSprites[controlImageIndex];
			controlsPageCount.GetComponent<Text>().text = (controlImageIndex + 1) + "/" + (tutorialSprites.Length);
			controlsPanel.SetActive(true);
		} else {
			controlImageIndex = 0;
			controlsPanel.SetActive(false);
		}
	}

	public void GameQuit() {
		TogglePause();
		Application.Quit();
	}

	public void ReturnToMain () {
		TogglePause();
		SceneManager.LoadScene("StartScreen");
	}

	public void changeControlImage(string targetImage) {
		if (targetImage == "next") {
			if (controlImageIndex != tutorialSprites.Length - 1) {
				controlImageIndex++;
				controlsImage.GetComponent<Image>().sprite = tutorialSprites[controlImageIndex];
				controlsPageCount.GetComponent<Text>().text = (controlImageIndex + 1) + "/" + (tutorialSprites.Length);

			}
		} else if (targetImage == "previous") {
			if (controlImageIndex != 0) {
				controlImageIndex--;
				controlsImage.GetComponent<Image>().sprite = tutorialSprites[controlImageIndex];
				controlsPageCount.GetComponent<Text>().text = (controlImageIndex + 1) + "/" + (tutorialSprites.Length);

			}
		}
	}
}

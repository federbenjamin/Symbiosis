﻿using UnityEngine;
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
	private bool nextInput = true;
	private float unpausedSpeed = 1.0f;


	// Use this for initialization
	void Start () {
		pausePanel.SetActive(false);

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

		if (isPaused) {
			if (Input.GetButtonDown("UICancel")) {
				if (showControls) {
					ToggleControls();
				} else {
					TogglePause ();
				}
			}
		}

		if (showControls) {
			if (Input.GetAxisRaw("UIHorizontal") > 0 && nextInput) {
					changeControlImage ("next");
			} else if (Input.GetAxisRaw("UIHorizontal") < 0 && nextInput) {
					changeControlImage ("previous");
			}
		}
	}

	public void TogglePause () {
		isPaused = !isPaused;

		if (isPaused) {
			unpausedSpeed = Time.timeScale;
			Time.timeScale = 0.0f;
			pausePanel.SetActive(true);
			EventSystem.current.SetSelectedGameObject(firstSelected, null);
			gameAudio.changeMainSongVolume(0.07f);
			gameAudio.changeMainSongPitch(0.9f);
		} else {
			Time.timeScale = unpausedSpeed;
			pausePanel.SetActive(false);
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
		nextInput = false;
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
		StartCoroutine("WaitForNewInput");
	}

	IEnumerator WaitForNewInput() {
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + 0.75f) {
			yield return null;
		}
		Debug.Log (nextInput);
		nextInput = true;
	}
}
